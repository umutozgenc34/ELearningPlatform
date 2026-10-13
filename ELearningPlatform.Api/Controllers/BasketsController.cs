using ELearningPlatform.Model.Baskets.Dtos.Request;
using ELearningPlatform.Service.Baskets.Abstracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ELearningPlatform.Api.Controllers;

// Sepet kullanıcıya özel olduğu için giriş yapmış olmak gerekiyor.
[Authorize]
public class BasketsController(IBasketService basketService) : CustomBaseController
{
    [HttpPost("item")]
    public async Task<IActionResult> AddBasketItem([FromBody] CreateBasketItemRequest request) => CreateActionResult(await basketService
        .AddBasketItemAsync(request));

    [HttpDelete("item/{id:guid}")]
    public async Task<IActionResult> DeleteBasketItem([FromRoute] Guid id) => CreateActionResult(await basketService.DeleteBasketItemAsync(id));

    [HttpGet("user")]
    public async Task<IActionResult> GetBasket() => CreateActionResult(await basketService.GetBasketAsync());

    [HttpDelete("user")]
    public async Task<IActionResult> ClearBasket() => CreateActionResult(await basketService.DeleteBasketAsync());
}
