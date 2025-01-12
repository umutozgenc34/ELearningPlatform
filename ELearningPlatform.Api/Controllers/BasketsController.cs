using ELearningPlatform.Model.Baskets.Dtos.Request;
using ELearningPlatform.Service.Baskets.Abstracts;
using Microsoft.AspNetCore.Mvc;

namespace ELearningPlatform.Api.Controllers;

public class BasketsController(IBasketService basketService) : CustomBaseController
{
    [HttpPost("item")]
    public async Task<IActionResult> AddBasketItem([FromBody] CreateBasketItemRequest request) => CreateActionResult(await basketService
        .AddBasketItemAsync(request));

    [HttpDelete("item/{id:guid}")]
    public async Task<IActionResult> DeleteBasketItem([FromRoute]Guid id) => CreateActionResult(await basketService.DeleteBasketItemAsync(id));

    [HttpGet("user")]
    public async Task<IActionResult> GetBasket() => CreateActionResult(await basketService.GetBasketAsync());

}
