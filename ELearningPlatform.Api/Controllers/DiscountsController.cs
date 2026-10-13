using Core.Constants;
using ELearningPlatform.Model.Discounts.Dtos;
using ELearningPlatform.Service.Discounts.Abstracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ELearningPlatform.Api.Controllers;

public class DiscountsController(IDiscountService discountService) : CustomBaseController
{
    [Authorize(Roles = AppRoles.AdminOrEducator)]
    [HttpGet]
    public async Task<IActionResult> GetAllDiscounts() =>
        CreateActionResult(await discountService.GetAllDiscountsAsync());

    // Sepet ekranında girilen kuponun geçerli olup olmadığını kontrol eder (giriş yapmış her kullanıcı).
    [Authorize]
    [HttpGet("validate/{coupon}")]
    public async Task<IActionResult> ValidateCoupon([FromRoute] string coupon) =>
        CreateActionResult(await discountService.GetDiscountByCouponAsync(coupon));

    [Authorize(Roles = AppRoles.AdminOrEducator)]
    [HttpGet("{coupon}")]
    public async Task<IActionResult> GetDiscountByCoupon([FromRoute] string coupon) =>
        CreateActionResult(await discountService.GetDiscountByCouponAsync(coupon));

    [Authorize(Roles = AppRoles.AdminOrEducator)]
    [HttpPost]
    public async Task<IActionResult> CreateDiscount([FromBody] DiscountDto request) =>
        CreateActionResult(await discountService.CreateDiscountAsync(request));

    [Authorize(Roles = AppRoles.AdminOrEducator)]
    [HttpDelete("{coupon}")]
    public async Task<IActionResult> DeleteDiscount([FromRoute] string coupon) =>
        CreateActionResult(await discountService.DeleteDiscountAsync(coupon));
}
