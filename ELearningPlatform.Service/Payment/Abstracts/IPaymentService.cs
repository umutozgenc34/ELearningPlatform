using ELearningPlatform.Model.Order.Entities;
using ELearningPlatform.Model.Payment.Dtos;
using ELearningPlatform.Service.Constants;

namespace ELearningPlatform.Service.Payment.Abstracts;

public interface IPaymentService
{
    Task<PaymentResult> ProcessPaymentAsync(PaymentRequest paymentRequest);
}
