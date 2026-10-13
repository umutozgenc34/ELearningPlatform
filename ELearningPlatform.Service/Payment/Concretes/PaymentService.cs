using Core.Response;
using ELearningPlatform.Model.Order.Enums;
using ELearningPlatform.Model.Payment.Dtos;
using ELearningPlatform.Service.Orders.Abstracts;
using ELearningPlatform.Service.Payment.Abstracts;
using Microsoft.Extensions.Logging;
using System.Net;

namespace ELearningPlatform.Service.Payment.Concretes;

public class PaymentService : IPaymentService
{
    public const string DeclinedTestCardNumber = "4000000000000002";
    private static readonly TimeSpan SimulatedDelay = TimeSpan.FromSeconds(2);

    private readonly IOrderService _orderService;
    private readonly ILogger<PaymentService> _logger;

    public PaymentService(IOrderService orderService, ILogger<PaymentService> logger)
    {
        _orderService = orderService;
        _logger = logger;
    }

    public async Task<ServiceResult> ProcessPaymentAsync(PaymentRequest paymentRequest)
    {
        _logger.LogInformation("ProcessPaymentAsync called for Order ID: {OrderId}", paymentRequest.OrderId);

        var orderResult = await _orderService.GetOrderByIdAsync(paymentRequest.OrderId);
        if (!orderResult.IsSuccess || orderResult.Data == null)
        {
            _logger.LogWarning("Payment failed. Order not found. Order ID: {OrderId}", paymentRequest.OrderId);
            return ServiceResult.Fail("Sipariş bulunamadı.", HttpStatusCode.NotFound);
        }

        var order = orderResult.Data;

        if (order.Status != OrderStatus.Pending)
        {
            _logger.LogWarning("Payment rejected. Order is not pending. Order ID: {OrderId}, Status: {Status}", order.Id, order.Status);
            return ServiceResult.Fail("Bu sipariş için ödeme alınamaz (zaten ödenmiş veya iptal edilmiş).");
        }

        _logger.LogInformation("Payment simulation started for Order ID: {OrderId}.", order.Id);
        await Task.Delay(SimulatedDelay);

        if (paymentRequest.CardNumber == DeclinedTestCardNumber)
        {
            _logger.LogWarning("Payment declined (test card). Order ID: {OrderId}", order.Id);
            return ServiceResult.Fail("Ödeme reddedildi. Lütfen başka bir kartla tekrar deneyin.", HttpStatusCode.PaymentRequired);
        }

        await _orderService.UpdateOrderStatusAsync(order.Id, OrderStatus.Confirmed);
        _logger.LogInformation("Payment succeeded. Order confirmed. Order ID: {OrderId}", order.Id);
        return ServiceResult.Success("Ödeme başarılı, siparişiniz tamamlandı.");
    }
}
