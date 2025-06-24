using Core.Response;
using Core.Services;
using ELearningPlatform.Model.Order.Entities;
using ELearningPlatform.Model.Order.Enums;
using ELearningPlatform.Repository.Orders.Abstracts;
using ELearningPlatform.Service.Baskets.Abstracts;
using ELearningPlatform.Service.Discounts.Abstracts;
using ELearningPlatform.Service.Orders.Abstracts;
using ELearningPlatform.Service.Payment.Abstracts;
using Microsoft.Extensions.Logging;
using System.Net;

namespace ELearningPlatform.Service.Orders.Concretes;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IBasketService _basketService;
    private readonly IIdentityService _identityService;
    private readonly IDiscountService _discountService;
    private readonly ILogger<OrderService> _logger;

    public OrderService(
        IOrderRepository orderRepository,
        IBasketService basketService,
        IIdentityService identityService,
        IDiscountService discountService,
        ILogger<OrderService> logger)
    {
        _orderRepository = orderRepository;
        _basketService = basketService;
        _identityService = identityService;
        _discountService = discountService;
        _logger = logger;
    }


    public async Task<ServiceResult> CreateOrderAsync(string coupon)
    {
        _logger.LogInformation("CreateOrderAsync called with coupon: {Coupon}", coupon);

        var basketResult = await _basketService.GetBasketAsync();
        if (!basketResult.IsSuccess || basketResult.Data == null)
        {
            _logger.LogWarning("CreateOrderAsync failed. Basket not found or empty.");
            return ServiceResult.Fail("Basket not found or empty.");
        }

        var basket = basketResult.Data;

        var orderItems = basket.Items.Select(basketItem => new OrderItem
        {
            CourseId = basketItem.Id,
            CourseName = basketItem.Name,
            Price = basketItem.Price
        }).ToList();

        var totalPrice = orderItems.Sum(item => item.Price);

        if (!string.IsNullOrEmpty(coupon))
        {
            var discountResult = await _discountService.GetDiscountByCouponAsync(coupon);
            if (discountResult.IsSuccess)
            {
                var discount = discountResult.Data;
                totalPrice -= totalPrice * (discount.Rate / 100);
                _logger.LogInformation("Discount applied. Coupon: {Coupon}, Rate: {Rate}", discount.Coupon, discount.Rate);
            }
            else
            {
                _logger.LogWarning("Invalid coupon code: {Coupon}", coupon);
            }
        }

        var order = new Order
        {
            UserId = _identityService.GetUserId,
            OrderItems = orderItems,
            TotalPrice = totalPrice,
            Status = OrderStatus.Pending,
            Created = DateTime.UtcNow,
            Updated = DateTime.UtcNow
        };

        await _orderRepository.CreateAsync(order);
        await _basketService.DeleteBasketAsync();

        _logger.LogInformation("Order created successfully for User ID: {UserId}", order.UserId);
        return ServiceResult.Success("Sipariş oluşturuldu.");
    }

    public async Task<ServiceResult<Order>> GetOrderByIdAsync(string orderId)
    {
        _logger.LogInformation("GetOrderByIdAsync called for Order ID: {OrderId}", orderId);

        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null)
        {
            _logger.LogWarning("Order not found. ID: {OrderId}", orderId);
            return ServiceResult<Order>.Fail("Order not found.", HttpStatusCode.NotFound);
        }

        order.Status = OrderStatus.Confirmed;
        _logger.LogInformation("Order retrieved successfully. ID: {OrderId}", orderId);
        return ServiceResult<Order>.Success(order);
    }

    public async Task<ServiceResult<List<Order>>> GetUserOrdersAsync()
    {
        var userId = _identityService.GetUserId;
        _logger.LogInformation("GetUserOrdersAsync called for User ID: {UserId}", userId);

        var orders = await _orderRepository.GetOrdersByUserIdAsync(userId);
        if (orders == null || !orders.Any())
        {
            _logger.LogWarning("No orders found for User ID: {UserId}", userId);
            return ServiceResult<List<Order>>.Fail("No orders found.", HttpStatusCode.NotFound);
        }

        _logger.LogInformation("Returned {Count} orders for User ID: {UserId}", orders.Count, userId);
        return ServiceResult<List<Order>>.Success(orders);
    }

    public async Task<ServiceResult> DeleteOrderAsync(string orderId)
    {
        _logger.LogInformation("DeleteOrderAsync called for Order ID: {OrderId}", orderId);

        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null)
        {
            _logger.LogWarning("DeleteOrderAsync failed. Order not found. ID: {OrderId}", orderId);
            return ServiceResult.Fail("Order not found.", HttpStatusCode.NotFound);
        }

        await _orderRepository.DeleteAsync(orderId);
        _logger.LogInformation("Order deleted successfully. ID: {OrderId}", orderId);
        return ServiceResult.Success();
    }

    public async Task<ServiceResult<List<Order>>> GetAllOrdersAsync()
    {
        _logger.LogInformation("GetAllOrdersAsync called.");

        var orders = await _orderRepository.GetAllAsync();
        if (orders == null || !orders.Any())
        {
            _logger.LogWarning("No orders found.");
            return ServiceResult<List<Order>>.Fail("No orders found.", HttpStatusCode.NotFound);
        }

        _logger.LogInformation("Returned {Count} orders.", orders.Count);
        return ServiceResult<List<Order>>.Success(orders);
    }

    public async Task<ServiceResult> UpdateOrderStatusAsync(string orderId, OrderStatus status = OrderStatus.Pending)
    {
        _logger.LogInformation("UpdateOrderStatusAsync called for Order ID: {OrderId} with Status: {Status}", orderId, status);

        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null)
        {
            _logger.LogWarning("Order not found for update. ID: {OrderId}", orderId);
            return ServiceResult.Fail("Order not found.");
        }

        order.Status = status;
        order.Updated = DateTime.UtcNow;

        await _orderRepository.UpdateAsync(order);
        _logger.LogInformation("Order status updated successfully. ID: {OrderId}, New Status: {Status}", orderId, status);
        return ServiceResult.Success("Order status updated successfully.");
    }

}
