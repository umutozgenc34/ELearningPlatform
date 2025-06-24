using ELearningPlatform.Model.Order.Entities;
using ELearningPlatform.Model.Order.Enums;
using ELearningPlatform.Model.Payment.Dtos;
using ELearningPlatform.Service.Constants;
using ELearningPlatform.Service.Orders.Abstracts;
using ELearningPlatform.Service.Payment.Abstracts;
using Microsoft.Extensions.Logging;

namespace ELearningPlatform.Service.Payment.Concretes;

//FAKE PAYMENT

public class PaymentService : IPaymentService
{
    private readonly Random _random = new Random();
    private readonly IOrderService _orderService;
    private readonly ILogger<PaymentService> _logger;

    public PaymentService(IOrderService orderService, ILogger<PaymentService> logger)
    {
        _orderService = orderService;
        _logger = logger;
    }

    public async Task<PaymentResult> ProcessPaymentAsync(PaymentRequest paymentRequest)
    {
        _logger.LogInformation("ProcessPaymentAsync called for Order ID: {OrderId}", paymentRequest.OrderId);

        var orderResult = await _orderService.GetOrderByIdAsync(paymentRequest.OrderId);

        if (!orderResult.IsSuccess || orderResult.Data == null)
        {
            _logger.LogWarning("Payment failed. Order not found. Order ID: {OrderId}", paymentRequest.OrderId);
            return new PaymentResult
            {
                IsSuccessful = false,
                Message = "Order not found."
            };
        }

        var order = orderResult.Data;

        if (order.Status == OrderStatus.Pending)
        {
            _logger.LogWarning("Payment cannot be processed. Order is still pending. Order ID: {OrderId}", order.Id);
            return new PaymentResult
            {
                IsSuccessful = false,
                Message = "Payment cannot be processed for this order."
            };
        }

        var isPaymentSuccessful = _random.Next(1, 101) <= 80;

        _logger.LogInformation("Payment simulation started for Order ID: {OrderId}. Waiting 3 seconds...", order.Id);
        await Task.Delay(3000); // 3 sn bekleme simülasyon

        if (isPaymentSuccessful)
        {
            await _orderService.UpdateOrderStatusAsync(order.Id, OrderStatus.Confirmed);
            _logger.LogInformation("Payment succeeded. Order confirmed. Order ID: {OrderId}", order.Id);
            return new PaymentResult
            {
                IsSuccessful = true,
                Message = "Payment successful. Order completed."
            };
        }
        else
        {
            await _orderService.UpdateOrderStatusAsync(order.Id, OrderStatus.Canceled);
            _logger.LogWarning("Payment failed. Order canceled. Order ID: {OrderId}", order.Id);
            return new PaymentResult
            {
                IsSuccessful = false,
                Message = "Payment failed. Please try again."
            };
        }
    }
}