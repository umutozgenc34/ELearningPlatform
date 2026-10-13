using Core.Response;
using ELearningPlatform.Model.Payment.Dtos;

namespace ELearningPlatform.Service.Payment.Abstracts;

public interface IPaymentService
{
    Task<ServiceResult> ProcessPaymentAsync(PaymentRequest paymentRequest);
}
