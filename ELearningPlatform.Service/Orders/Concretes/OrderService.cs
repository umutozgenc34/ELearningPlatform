using Core.Constants;
using Core.Response;
using Core.Services;
using ELearningPlatform.Model.Order.Entities;
using ELearningPlatform.Model.Order.Enums;
using ELearningPlatform.Repository.Orders.Abstracts;
using ELearningPlatform.Service.Baskets.Abstracts;
using ELearningPlatform.Service.Discounts.Abstracts;
using ELearningPlatform.Service.Orders.Abstracts;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
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

    public async Task<ServiceResult<Order>> CreateOrderAsync(string? coupon)
    {
        _logger.LogInformation("CreateOrderAsync called with coupon: {Coupon}", coupon);

        var basketResult = await _basketService.GetBasketAsync();
        if (!basketResult.IsSuccess || basketResult.Data == null || basketResult.Data.Items.Count == 0)
        {
            _logger.LogWarning("CreateOrderAsync failed. Basket is empty.");
            return ServiceResult<Order>.Fail("Sepetiniz boş.");
        }

        var basket = basketResult.Data;

        var orderItems = basket.Items.Select(basketItem => new OrderItem
        {
            CourseId = basketItem.Id,
            CourseName = basketItem.Name,
            Price = basketItem.Price
        }).ToList();

        var totalPrice = orderItems.Sum(item => item.Price);

        if (!string.IsNullOrWhiteSpace(coupon))
        {
            // Geçersiz kupon artık sessizce yok sayılmıyor; kullanıcıya hata dönüyoruz.
            var discountResult = await _discountService.GetDiscountByCouponAsync(coupon);
            if (!discountResult.IsSuccess || discountResult.Data is null)
            {
                _logger.LogWarning("Invalid coupon code: {Coupon}", coupon);
                return ServiceResult<Order>.Fail("Geçersiz kupon kodu.");
            }

            var discount = discountResult.Data;
            totalPrice -= totalPrice * (discount.Rate / 100);
            _logger.LogInformation("Discount applied. Coupon: {Coupon}, Rate: {Rate}", discount.Coupon, discount.Rate);
        }

        var order = new Order
        {
            // Id'yi biz üretiyoruz ki ekleme sonrası frontend'e kesin olarak dönebilelim.
            Id = ObjectId.GenerateNewId().ToString(),
            UserId = _identityService.GetUserId,
            OrderItems = orderItems,
            TotalPrice = Math.Round(totalPrice, 2),
            Status = OrderStatus.Pending,
            Created = DateTime.UtcNow,
            Updated = DateTime.UtcNow
        };

        await _orderRepository.CreateAsync(order);
        await _basketService.DeleteBasketAsync();

        _logger.LogInformation("Order created successfully. Order ID: {OrderId}, User ID: {UserId}", order.Id, order.UserId);
        return ServiceResult<Order>.Success(order, HttpStatusCode.Created);
    }

    public async Task<ServiceResult<Order>> GetOrderByIdAsync(string orderId)
    {
        _logger.LogInformation("GetOrderByIdAsync called for Order ID: {OrderId}", orderId);

        var order = ObjectId.TryParse(orderId, out _) ? await _orderRepository.GetByIdAsync(orderId) : null;

        // Başkasının siparişini görmek isteyen kullanıcıya "yok" diyoruz (Admin hariç).
        if (order == null || (order.UserId != _identityService.GetUserId && !_identityService.IsInRole(AppRoles.Admin)))
        {
            _logger.LogWarning("Order not found. ID: {OrderId}", orderId);
            return ServiceResult<Order>.Fail("Sipariş bulunamadı.", HttpStatusCode.NotFound);
        }

        // Not: Burada önceden order.Status = Confirmed yapılıyordu; bu yüzden ödeme kontrolü hiçbir zaman doğru çalışmıyordu.
        _logger.LogInformation("Order retrieved successfully. ID: {OrderId}", orderId);
        return ServiceResult<Order>.Success(order);
    }

    public async Task<ServiceResult<List<Order>>> GetUserOrdersAsync()
    {
        var userId = _identityService.GetUserId;
        _logger.LogInformation("GetUserOrdersAsync called for User ID: {UserId}", userId);

        var orders = await _orderRepository.GetOrdersByUserIdAsync(userId);
        var sortedOrders = orders.OrderByDescending(o => o.Created).ToList();

        _logger.LogInformation("Returned {Count} orders for User ID: {UserId}", sortedOrders.Count, userId);
        return ServiceResult<List<Order>>.Success(sortedOrders);
    }

    public async Task<ServiceResult> DeleteOrderAsync(string orderId)
    {
        _logger.LogInformation("DeleteOrderAsync called for Order ID: {OrderId}", orderId);

        var order = ObjectId.TryParse(orderId, out _) ? await _orderRepository.GetByIdAsync(orderId) : null;
        if (order == null)
        {
            _logger.LogWarning("DeleteOrderAsync failed. Order not found. ID: {OrderId}", orderId);
            return ServiceResult.Fail("Sipariş bulunamadı.", HttpStatusCode.NotFound);
        }

        await _orderRepository.DeleteAsync(orderId);
        _logger.LogInformation("Order deleted successfully. ID: {OrderId}", orderId);
        return ServiceResult.Success(HttpStatusCode.NoContent);
    }

    public async Task<ServiceResult<List<Order>>> GetAllOrdersAsync()
    {
        _logger.LogInformation("GetAllOrdersAsync called.");

        var orders = await _orderRepository.GetAllAsync();
        var sortedOrders = orders.OrderByDescending(o => o.Created).ToList();

        _logger.LogInformation("Returned {Count} orders.", sortedOrders.Count);
        return ServiceResult<List<Order>>.Success(sortedOrders);
    }

    public async Task<ServiceResult> UpdateOrderStatusAsync(string orderId, OrderStatus status)
    {
        _logger.LogInformation("UpdateOrderStatusAsync called for Order ID: {OrderId} with Status: {Status}", orderId, status);

        var order = ObjectId.TryParse(orderId, out _) ? await _orderRepository.GetByIdAsync(orderId) : null;
        if (order == null)
        {
            _logger.LogWarning("Order not found for update. ID: {OrderId}", orderId);
            return ServiceResult.Fail("Sipariş bulunamadı.", HttpStatusCode.NotFound);
        }

        order.Status = status;
        order.Updated = DateTime.UtcNow;

        await _orderRepository.UpdateAsync(order);
        _logger.LogInformation("Order status updated successfully. ID: {OrderId}, New Status: {Status}", orderId, status);
        return ServiceResult.Success();
    }
}
