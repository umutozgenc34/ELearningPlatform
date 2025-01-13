using ELearningPlatform.Model.Order.Entities;
using ELearningPlatform.Model.Order.Enums;
using ELearningPlatform.Model.Payment.Dtos;
using ELearningPlatform.Service.Constants;
using ELearningPlatform.Service.Orders.Abstracts;
using ELearningPlatform.Service.Payment.Abstracts;

namespace ELearningPlatform.Service.Payment.Concretes;

//FAKE PAYMENT

public class PaymentService : IPaymentService
{
    private readonly Random _random = new Random();
    private readonly IOrderService _orderService;

    public PaymentService(IOrderService orderService)
    {
        _orderService = orderService;
    }

    public async Task<PaymentResult> ProcessPaymentAsync(PaymentRequest paymentRequest)
    {
        
        var orderResult = await _orderService.GetOrderByIdAsync(paymentRequest.OrderId);

        if (!orderResult.IsSuccess || orderResult.Data == null)
        {
            return new PaymentResult
            {
                IsSuccessful = false,
                Message = "Order not found."
            };
        }

        var order = orderResult.Data;

        if (order.Status == OrderStatus.Pending)
        {
            return new PaymentResult
            {
                IsSuccessful = false,
                Message = "Payment cannot be processed for this order."
            };
        }

        var isPaymentSuccessful = _random.Next(1, 101) <= 80;

        await Task.Delay(3000); // 3 sn bekleme simülasyon

        if (isPaymentSuccessful)
        {
            await _orderService.UpdateOrderStatusAsync(order.Id, OrderStatus.Confirmed);
            return new PaymentResult
            {
                IsSuccessful = true,
                Message = "Payment successful. Order completed."
            };
        }
        else
        {
            await _orderService.UpdateOrderStatusAsync(order.Id, OrderStatus.Canceled);
            return new PaymentResult
            {
                IsSuccessful = false,
                Message = "Payment failed. Please try again."
            };
        }
    }
}