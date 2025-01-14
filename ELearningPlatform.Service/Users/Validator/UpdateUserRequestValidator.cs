﻿using ELearningPlatform.Model.User.Dtos.Update;
using FluentValidation;

namespace ELearningPlatform.Service.Users.Validator;

public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator()
    {
        RuleFor(x => x.UserName)
               .NotEmpty().WithMessage("Kullanıcı adı boş olamaz.")
               .Length(3, 50).WithMessage("Kullanıcı adı 3 ile 50 karakter arasında olmalıdır.");
    }
}
