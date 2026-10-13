using Core.Constants;
using ELearningPlatform.Service.Orders.Abstracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ELearningPlatform.Api.Controllers;

[Authorize]
public class OrdersController(IOrderService orderService) : CustomBaseController
{
    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromQuery] string? coupon) => CreateActionResult(await orderService.CreateOrderAsync(coupon));

    [HttpGet("{orderId}")]
    public async Task<IActionResult> GetOrder([FromRoute] string orderId) => CreateActionResult(await orderService.GetOrderByIdAsync(orderId));

    [HttpGet("user")]
    public async Task<IActionResult> GetUserOrders() => CreateActionResult(await orderService.GetUserOrdersAsync());

    [Authorize(Roles = AppRoles.Admin)]
    [HttpDelete("{orderId}")]
    public async Task<IActionResult> DeleteOrder([FromRoute] string orderId) => CreateActionResult(await orderService.DeleteOrderAsync(orderId));

    [Authorize(Roles = AppRoles.Admin)]
    [HttpGet]
    public async Task<IActionResult> GetAllOrders() => CreateActionResult(await orderService.GetAllOrdersAsync());
}
