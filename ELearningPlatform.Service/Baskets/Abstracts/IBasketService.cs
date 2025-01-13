using Core.Response;
using ELearningPlatform.Model.Baskets.Dtos.Request;
using ELearningPlatform.Model.Baskets.Dtos.Responses;
using ELearningPlatform.Model.Order.Entities;

namespace ELearningPlatform.Service.Baskets.Abstracts;

public interface IBasketService
{
    Task<ServiceResult> AddBasketItemAsync(CreateBasketItemRequest request);
    Task<ServiceResult> DeleteBasketItemAsync(Guid id);
    Task<ServiceResult<BasketDto>> GetBasketAsync();
    Task<ServiceResult> DeleteBasketAsync();
}
