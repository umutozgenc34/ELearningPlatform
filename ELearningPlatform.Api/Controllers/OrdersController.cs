using ELearningPlatform.Service.Orders.Abstracts;
using Microsoft.AspNetCore.Mvc;

namespace ELearningPlatform.Api.Controllers;

public class OrdersController(IOrderService orderService) : CustomBaseController
{
        
    [HttpPost]
    public async Task<IActionResult> CreateOrder(string coupon) => CreateActionResult(await orderService.CreateOrderAsync(coupon));
    [HttpGet("{orderId}")]
    public async Task<IActionResult> GetOrder([FromRoute] string orderId) => CreateActionResult(await orderService.GetOrderByIdAsync(orderId));
    [HttpGet("user")]
    public async Task<IActionResult> GetUserOrders() => CreateActionResult(await orderService.GetUserOrdersAsync());
    [HttpDelete("{orderId}")]
    public async Task<IActionResult> DeleteOrder([FromRoute] string orderId) => CreateActionResult(await orderService.DeleteOrderAsync(orderId));
    [HttpGet]
    public async Task<IActionResult> GetAllOrders() => CreateActionResult(await orderService.GetAllOrdersAsync());
}
