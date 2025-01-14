using ELearningPlatform.Model.User.Dtos.Login;
using FluentValidation;

namespace ELearningPlatform.Service.Users.Validator;

public class LoginValidator : AbstractValidator<LoginDto>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email)
               .NotEmpty().WithMessage("E-posta adresi boş olamaz.")
               .EmailAddress().WithMessage("Geçersiz e-posta adresi.");
        
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Şifre boş olamaz.");
    }
}