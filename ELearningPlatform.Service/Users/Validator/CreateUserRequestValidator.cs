using ELearningPlatform.Model.User.Dtos.Create;
using FluentValidation;

namespace ELearningPlatform.Service.Users.Validator;

public class CreateUserRequestValidator : AbstractValidator<CreateUserDto>
{
    public CreateUserRequestValidator()
    {
        RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Kullanıcı adı boş olamaz.")
                .Length(3, 50).WithMessage("Kullanıcı adı 3 ile 50 karakter arasında olmalıdır.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("E-posta boş olamaz.")
            .EmailAddress().WithMessage("Geçerli bir e-posta adresi girin.");
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Şifre boş olamaz.")
            .Length(6, 100).WithMessage("Şifre 6 ile 100 karakter arasında olmalıdır.");

        RuleFor(x => x.Role)
            .IsInEnum().WithMessage("Geçerli bir rol seçin."); 
    }
}
