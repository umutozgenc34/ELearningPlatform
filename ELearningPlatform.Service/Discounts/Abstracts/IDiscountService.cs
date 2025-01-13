using Core.Response;
using ELearningPlatform.Model.Discounts.Dtos;

namespace ELearningPlatform.Service.Discounts.Abstracts;

public interface IDiscountService
{
    Task<ServiceResult<List<DiscountDto>>> GetAllDiscountsAsync();
    Task<ServiceResult<DiscountDto>> GetDiscountByCouponAsync(string coupon);
    Task<ServiceResult<DiscountDto>> CreateDiscountAsync(DiscountDto request);
    Task<ServiceResult> DeleteDiscountAsync(string coupon);

}
