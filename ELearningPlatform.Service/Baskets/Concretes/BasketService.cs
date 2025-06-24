using Core.Response;
using Core.Services;
using ELearningPlatform.Model.Baskets.Dtos.Request;
using ELearningPlatform.Model.Baskets.Dtos.Responses;
using ELearningPlatform.Service.Baskets.Abstracts;
using ELearningPlatform.Service.Constants;
using ELearningPlatform.Service.Courses.Abstracts;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace ELearningPlatform.Service.Baskets.Concretes;

public class BasketService(IDistributedCache distributedCache,ICourseService courseService,IIdentityService identityService,
                            ILogger<BasketService> logger) : IBasketService
{
    public async Task<ServiceResult> AddBasketItemAsync(CreateBasketItemRequest request)
    {
        logger.LogInformation("AddBasketItemAsync called for CourseId: {CourseId}", request.CourseId);

        string userId = identityService.GetUserId;
        var cacheKey = string.Format(BasketConsts.BacketCacheKey, userId);

        var basketAsString = await distributedCache.GetStringAsync(cacheKey);
        BasketDto? currentBasket;

        var courseResult = await courseService.GetByIdAsync(request.CourseId);
        if (!courseResult.IsSuccess || courseResult.Data == null)
        {
            logger.LogWarning("AddBasketItemAsync failed. Course not found. CourseId: {CourseId}", request.CourseId);
            return ServiceResult.Fail("Course not found.");
        }

        var course = courseResult.Data;

        var newBasketItem = new BasketItemDto(
            course.Id,
            course.Name,
            course.ImageUrl,
            course.Price,
            1
        );

        if (string.IsNullOrEmpty(basketAsString))
        {
            currentBasket = new BasketDto(userId, new List<BasketItemDto> { newBasketItem });
            await CreateCacheAsync(currentBasket, cacheKey);

            logger.LogInformation("Basket created and item added for UserId: {UserId}, CourseId: {CourseId}", userId, course.Id);
            return ServiceResult.Success();
        }

        currentBasket = JsonSerializer.Deserialize<BasketDto>(basketAsString);

        var existingBasketItem = currentBasket!.Items.FirstOrDefault(x => x.Id == request.CourseId);
        if (existingBasketItem != null)
        {
            currentBasket.Items.Remove(existingBasketItem);
            logger.LogInformation("Existing item removed from basket. CourseId: {CourseId}", request.CourseId);
        }

        currentBasket.Items.Add(newBasketItem);

        await CreateCacheAsync(currentBasket, cacheKey);
        logger.LogInformation("Basket updated successfully for UserId: {UserId}", userId);
        return ServiceResult.Success();
    }

    public async Task<ServiceResult> DeleteBasketItemAsync(Guid id)
    {
        logger.LogInformation("DeleteBasketItemAsync called for ItemId: {ItemId}", id);

        string userID = identityService.GetUserId;
        var cacheKey = string.Format(BasketConsts.BacketCacheKey, userID);
        var basketAsString = await distributedCache.GetStringAsync(cacheKey);

        if (string.IsNullOrEmpty(basketAsString))
        {
            logger.LogWarning("DeleteBasketItemAsync failed. Basket not found for UserId: {UserId}", userID);
            return ServiceResult.Fail("Basket not found", HttpStatusCode.NotFound);
        }

        var currentBasket = JsonSerializer.Deserialize<BasketDto>(basketAsString);
        var basketItemToDelete = currentBasket!.Items.FirstOrDefault(x => x.Id == id);

        if (basketItemToDelete is null)
        {
            logger.LogWarning("DeleteBasketItemAsync failed. Item not found in basket. ItemId: {ItemId}", id);
            return ServiceResult.Fail("Basket item not found", HttpStatusCode.NotFound);
        }

        currentBasket.Items.Remove(basketItemToDelete);
        basketAsString = JsonSerializer.Serialize(currentBasket);
        await distributedCache.SetStringAsync(cacheKey, basketAsString);

        logger.LogInformation("Basket item deleted successfully. ItemId: {ItemId}", id);
        return ServiceResult.Success(HttpStatusCode.NoContent);
    }

    public async Task<ServiceResult<BasketDto>> GetBasketAsync()
    {
        logger.LogInformation("GetBasketAsync called.");

        var cacheKey = string.Format(BasketConsts.BacketCacheKey, identityService.GetUserId);
        var basketAsString = await distributedCache.GetStringAsync(cacheKey);

        if (string.IsNullOrEmpty(basketAsString))
        {
            logger.LogWarning("GetBasketAsync failed. Basket not found.");
            return ServiceResult<BasketDto>.Fail("Basket not found.", HttpStatusCode.NotFound);
        }

        var basket = JsonSerializer.Deserialize<BasketDto>(basketAsString);
        logger.LogInformation("GetBasketAsync succeeded. Basket contains {Count} items.", basket?.Items.Count ?? 0);
        return ServiceResult<BasketDto>.Success(basket);
    }

    public async Task<ServiceResult> DeleteBasketAsync()
    {
        logger.LogInformation("DeleteBasketAsync called.");

        string userId = identityService.GetUserId;
        var cacheKey = string.Format(BasketConsts.BacketCacheKey, userId);

        await distributedCache.RemoveAsync(cacheKey);

        logger.LogInformation("Basket deleted successfully for UserId: {UserId}", userId);
        return ServiceResult.Success();
    }

    private async Task CreateCacheAsync(BasketDto basketDto, string cacheKey)
    {
        var basketAsString = JsonSerializer.Serialize(basketDto);
        await distributedCache.SetStringAsync(cacheKey, basketAsString);
    }

}

