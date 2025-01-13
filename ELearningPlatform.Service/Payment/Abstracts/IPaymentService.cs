using ELearningPlatform.Model.Order.Entities;
using ELearningPlatform.Service.Constants;

namespace ELearningPlatform.Service.Payment.Abstracts;

public interface IPaymentService
{
    Task<PaymentResult> ProcessPayment(Order order);
}
