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

public class BasketService(IDistributedCache distributedCache, ICourseService courseService, IIdentityService identityService,
                            ILogger<BasketService> logger) : IBasketService
{
    public async Task<ServiceResult> AddBasketItemAsync(CreateBasketItemRequest request)
    {
        logger.LogInformation("AddBasketItemAsync called for CourseId: {CourseId}", request.CourseId);

        if (!TryGetCacheKey(out var userId, out var cacheKey))
        {
            return ServiceResult.Fail("Kullanıcı bulunamadı.", HttpStatusCode.Unauthorized);
        }

        var courseResult = await courseService.GetByIdAsync(request.CourseId);
        if (!courseResult.IsSuccess || courseResult.Data == null)
        {
            logger.LogWarning("AddBasketItemAsync failed. Course not found. CourseId: {CourseId}", request.CourseId);
            return ServiceResult.Fail("Kurs bulunamadı.", HttpStatusCode.NotFound);
        }

        var course = courseResult.Data;
        var currentBasket = await ReadBasketAsync(cacheKey) ?? new BasketDto(userId, new List<BasketItemDto>());

        // Bir kurs sepete yalnızca bir kez eklenebilir; zaten varsa güncel bilgisiyle değiştiriyoruz.
        currentBasket.Items.RemoveAll(x => x.Id == request.CourseId);
        currentBasket.Items.Add(new BasketItemDto(course.Id, course.Name, course.ImageUrl, course.Price, null));

        await WriteBasketAsync(currentBasket, cacheKey);

        logger.LogInformation("Basket updated successfully for UserId: {UserId}", userId);
        return ServiceResult.Success();
    }

    public async Task<ServiceResult> DeleteBasketItemAsync(Guid id)
    {
        logger.LogInformation("DeleteBasketItemAsync called for ItemId: {ItemId}", id);

        if (!TryGetCacheKey(out var userId, out var cacheKey))
        {
            return ServiceResult.Fail("Kullanıcı bulunamadı.", HttpStatusCode.Unauthorized);
        }

        var currentBasket = await ReadBasketAsync(cacheKey);
        var basketItemToDelete = currentBasket?.Items.FirstOrDefault(x => x.Id == id);

        if (currentBasket is null || basketItemToDelete is null)
        {
            logger.LogWarning("DeleteBasketItemAsync failed. Item not found in basket. ItemId: {ItemId}", id);
            return ServiceResult.Fail("Sepette bu kurs bulunamadı.", HttpStatusCode.NotFound);
        }

        currentBasket.Items.Remove(basketItemToDelete);
        await WriteBasketAsync(currentBasket, cacheKey);

        logger.LogInformation("Basket item deleted successfully. ItemId: {ItemId}, UserId: {UserId}", id, userId);
        return ServiceResult.Success(HttpStatusCode.NoContent);
    }

    public async Task<ServiceResult<BasketDto>> GetBasketAsync()
    {
        logger.LogInformation("GetBasketAsync called.");

        if (!TryGetCacheKey(out var userId, out var cacheKey))
        {
            return ServiceResult<BasketDto>.Fail("Kullanıcı bulunamadı.", HttpStatusCode.Unauthorized);
        }

        // Sepet hiç oluşturulmadıysa hata yerine boş sepet dönüyoruz.
        var basket = await ReadBasketAsync(cacheKey) ?? new BasketDto(userId, new List<BasketItemDto>());

        logger.LogInformation("GetBasketAsync succeeded. Basket contains {Count} items.", basket.Items.Count);
        return ServiceResult<BasketDto>.Success(basket);
    }

    public async Task<ServiceResult> DeleteBasketAsync()
    {
        logger.LogInformation("DeleteBasketAsync called.");

        if (!TryGetCacheKey(out var userId, out var cacheKey))
        {
            return ServiceResult.Fail("Kullanıcı bulunamadı.", HttpStatusCode.Unauthorized);
        }

        await distributedCache.RemoveAsync(cacheKey);

        logger.LogInformation("Basket deleted successfully for UserId: {UserId}", userId);
        return ServiceResult.Success(HttpStatusCode.NoContent);
    }

    // Kullanıcı Id'si boşsa (token yok/okunamadı) tüm anonim istekler aynı "basket:" anahtarını paylaşıyordu.
    // Bu yüzden Id yoksa işlemi reddediyoruz.
    private bool TryGetCacheKey(out string userId, out string cacheKey)
    {
        userId = identityService.GetUserId;
        cacheKey = string.Format(BasketConsts.BacketCacheKey, userId);
        return !string.IsNullOrWhiteSpace(userId);
    }

    private async Task<BasketDto?> ReadBasketAsync(string cacheKey)
    {
        var basketAsString = await distributedCache.GetStringAsync(cacheKey);
        return string.IsNullOrEmpty(basketAsString) ? null : JsonSerializer.Deserialize<BasketDto>(basketAsString);
    }

    private async Task WriteBasketAsync(BasketDto basketDto, string cacheKey)
    {
        var basketAsString = JsonSerializer.Serialize(basketDto);
        await distributedCache.SetStringAsync(cacheKey, basketAsString);
    }
}
