using Core.Response;
using Core.Services;
using ELearningPlatform.Model.Baskets.Dtos.Request;
using ELearningPlatform.Model.Baskets.Dtos.Responses;
using ELearningPlatform.Service.Baskets.Abstracts;
using ELearningPlatform.Service.Constants;
using ELearningPlatform.Service.Courses.Abstracts;
using Microsoft.Extensions.Caching.Distributed;
using System.Net;
using System.Text.Json;

namespace ELearningPlatform.Service.Baskets.Concretes;

public class BasketService(IDistributedCache distributedCache,ICourseService courseService,IIdentityService identityService) : IBasketService
{
    public async Task<ServiceResult> AddBasketItemAsync(CreateBasketItemRequest request)
    {
        string userId = identityService.GetUserId;
        var cacheKey = string.Format(BasketConsts.BacketCacheKey, userId);

        var basketAsString = await distributedCache.GetStringAsync(cacheKey);
        BasketDto? currentBasket;

        
        var courseResult = await courseService.GetByIdAsync(request.CourseId);

        if (!courseResult.IsSuccess || courseResult.Data == null)
        {
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
            return ServiceResult.Success();
        }

        currentBasket = JsonSerializer.Deserialize<BasketDto>(basketAsString);

        var existingBasketItem = currentBasket!.Items.FirstOrDefault(x => x.Id == request.CourseId);
        if (existingBasketItem != null)
        {
            currentBasket.Items.Remove(existingBasketItem);
        }
        currentBasket.Items.Add(newBasketItem);

        await CreateCacheAsync(currentBasket, cacheKey);
        return ServiceResult.Success();
    }

    public async Task<ServiceResult> DeleteBasketItemAsync(Guid id)
    {
        string userID = identityService.GetUserId;
        var cacheKey = string.Format(BasketConsts.BacketCacheKey, userID);
        var basketAsString = await distributedCache.GetStringAsync(cacheKey);
        if (string.IsNullOrEmpty(basketAsString))
        {
            return ServiceResult.Fail("Basket not found",HttpStatusCode.NotFound);
        }
        var currentBasket = JsonSerializer.Deserialize<BasketDto>(basketAsString);
        var basketItemToDelete = currentBasket!.Items.FirstOrDefault(x => x.Id == id);
        if (basketItemToDelete is null)
        {
            return ServiceResult.Fail("Basket item not found", HttpStatusCode.NotFound);
        }
        currentBasket.Items.Remove(basketItemToDelete);
        basketAsString = JsonSerializer.Serialize(currentBasket);
        await distributedCache.SetStringAsync(cacheKey, basketAsString);
        return ServiceResult.Success(HttpStatusCode.NoContent);
    }

    public async Task<ServiceResult<BasketDto>> GetBasketAsync()
    {
        var cacheKey = string.Format(BasketConsts.BacketCacheKey, identityService.GetUserId);
        var basketAsString = await distributedCache.GetStringAsync(cacheKey);
        if (string.IsNullOrEmpty(basketAsString))
        {
            return ServiceResult<BasketDto>.Fail("Basket not found.", HttpStatusCode.NotFound);
        }

        var basket = JsonSerializer.Deserialize<BasketDto>(basketAsString);
        return ServiceResult<BasketDto>.Success(basket);
    }

    public async Task<ServiceResult> DeleteBasketAsync()
    {
        string userId = identityService.GetUserId;
        var cacheKey = string.Format(BasketConsts.BacketCacheKey, userId);

        await distributedCache.RemoveAsync(cacheKey);

        return ServiceResult.Success();  
    }



    private async Task CreateCacheAsync(BasketDto basketDto, string cacheKey)
    {
        var basketAsString = JsonSerializer.Serialize(basketDto);
        await distributedCache.SetStringAsync(cacheKey, basketAsString);
    }

}

