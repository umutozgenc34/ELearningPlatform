using ELearningPlatform.Model.Payment.Dtos;
using FluentValidation;

namespace ELearningPlatform.Service.Payment.Validator;

public class PaymentRequestValidator : AbstractValidator<PaymentRequest>    
{
    public PaymentRequestValidator()
    {
        

        RuleFor(x=> x.OrderId).NotEmpty().WithMessage("sipariş ıd boş bırakılamaz");  
        RuleFor(x=> x.CardHolderName).NotEmpty().WithMessage("kart sahibinin ismi boş bırakılamaz")
            .MaximumLength(30).WithMessage("kart ismi maximum 30 hane olabilir");  
        RuleFor(x => x.CardNumber).NotEmpty().WithMessage("kart numarası boş bırakılamaz")
            .Matches(@"^\d{16}$").WithMessage("kart numarası 16 haneli olmalıdır");
        RuleFor(x => x.ExpiryDate).NotEmpty().WithMessage("son kullanma tarihi boş bırakılamaz")
            .Matches(@"^\d{2}\/\d{2}$").WithMessage("son kullanma tarihi xx/yy formatında olmalıdır");
        RuleFor(x => x.Cvv).NotEmpty().WithMessage("cvv boş bırakılamaz").
            Matches(@"^\d{3}$").WithMessage("cvv 3 haneli olmalıdır");


    }
}
