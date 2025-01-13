using ELearningPlatform.Model.Order.Entities;
using ELearningPlatform.Service.Constants;
using ELearningPlatform.Service.Payment.Abstracts;

namespace ELearningPlatform.Service.Payment.Concretes;

//FAKE PAYMENT

public class PaymentService : IPaymentService
{
    private readonly Random _random = new Random();

    public async Task<PaymentResult> ProcessPayment(Order order)
    {
        
        var isPaymentSuccessful = _random.Next(1, 101) <= 80;

        await Task.Delay(3000); // simülasyon için 3 saniye bekleme

        return new PaymentResult
        {
            IsSuccessful = isPaymentSuccessful,
            Message = isPaymentSuccessful ? "Ödeme başarılı. Sipariş oluşturuldu." : "Ödeme başarısız. Lütfen tekrar deneyin."
        };
    }
}
