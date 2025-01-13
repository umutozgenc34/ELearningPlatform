using Core.Response;
using Core.Services;
using ELearningPlatform.Model.Order.Entities;
using ELearningPlatform.Model.Order.Enums;
using ELearningPlatform.Repository.Orders.Abstracts;
using ELearningPlatform.Service.Baskets.Abstracts;
using ELearningPlatform.Service.Discounts.Abstracts;
using ELearningPlatform.Service.Orders.Abstracts;
using ELearningPlatform.Service.Payment.Abstracts;
using System.Net;

namespace ELearningPlatform.Service.Orders.Concretes;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IBasketService _basketService;
    private readonly IIdentityService _identityService;
    private readonly IPaymentService _paymentService;
    private readonly IDiscountService _discountService;

    public OrderService(
        IOrderRepository orderRepository,
        IBasketService basketService,
        IIdentityService identityService,
        IPaymentService paymentService,
        IDiscountService discountService)
    {
        _orderRepository = orderRepository;
        _basketService = basketService;
        _identityService = identityService;
        _paymentService = paymentService;
        _discountService = discountService;
    }

    public async Task<ServiceResult> CreateOrderAsync(string coupon)
    {
        var basketResult = await _basketService.GetBasketAsync();

        if (!basketResult.IsSuccess || basketResult.Data == null)
        {
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

        var paymentResult = await _paymentService.ProcessPayment(order);

        if (!paymentResult.IsSuccessful)
        {
            order.Status = OrderStatus.Canceled;
            return ServiceResult.Fail(paymentResult.Message); 
        }


        await _orderRepository.CreateAsync(order);

        await _basketService.DeleteBasketAsync();

        return ServiceResult.Success(paymentResult.Message);
    }

    public async Task<ServiceResult<Order>> GetOrderByIdAsync(string orderId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);

        if (order == null)
        {
            return ServiceResult<Order>.Fail("Order not found.", HttpStatusCode.NotFound);
        }
        order.Status = OrderStatus.Confirmed;
        return ServiceResult<Order>.Success(order);
    }

    public async Task<ServiceResult<List<Order>>> GetUserOrdersAsync()
    {
        var userId = _identityService.GetUserId;
        var orders = await _orderRepository.GetOrdersByUserIdAsync(userId);

        if (orders == null || !orders.Any())
        {
            return ServiceResult<List<Order>>.Fail("No orders found.", HttpStatusCode.NotFound);
        }

        return ServiceResult<List<Order>>.Success(orders);
    }

    public async Task<ServiceResult> DeleteOrderAsync(string orderId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);

        if (order == null)
        {
            return ServiceResult.Fail("Order not found.", HttpStatusCode.NotFound);
        }

        await _orderRepository.DeleteAsync(orderId);
        return ServiceResult.Success();
    }

    public async Task<ServiceResult<List<Order>>> GetAllOrdersAsync()
    {
        var orders = await _orderRepository.GetAllAsync();

        if (orders == null || !orders.Any())
        {
            return ServiceResult<List<Order>>.Fail("No orders found.", HttpStatusCode.NotFound);
        }

        return ServiceResult<List<Order>>.Success(orders);
    }

}
