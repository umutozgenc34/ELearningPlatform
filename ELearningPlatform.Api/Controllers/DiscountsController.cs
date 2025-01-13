using ELearningPlatform.Model.Discounts.Dtos;
using ELearningPlatform.Service.Discounts.Abstracts;
using Microsoft.AspNetCore.Mvc;

namespace ELearningPlatform.Api.Controllers;
public class DiscountsController(IDiscountService discountService) : CustomBaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAllDiscounts() =>
        CreateActionResult(await discountService.GetAllDiscountsAsync());

    [HttpGet("{coupon}")]
    public async Task<IActionResult> GetDiscountByCoupon([FromRoute] string coupon) =>
        CreateActionResult(await discountService.GetDiscountByCouponAsync(coupon));

    [HttpPost]
    public async Task<IActionResult> CreateDiscount([FromBody] DiscountDto request) =>
        CreateActionResult(await discountService.CreateDiscountAsync(request));

    [HttpDelete("{coupon}")]
    public async Task<IActionResult> DeleteDiscount([FromRoute] string coupon) =>
        CreateActionResult(await discountService.DeleteDiscountAsync(coupon));
}
