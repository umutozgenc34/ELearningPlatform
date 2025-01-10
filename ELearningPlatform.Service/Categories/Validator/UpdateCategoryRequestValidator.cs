using ELearningPlatform.Model.Categories.Dtos.Request;
using FluentValidation;

namespace ELearningPlatform.Service.Categories.Validator;

public class UpdateCategoryRequestValidator : AbstractValidator<UpdateCategoryRequest>
{
    public UpdateCategoryRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .MaximumLength(100)
            .WithMessage("Name can not be longer than 50 characters");

        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id is required")
            .GreaterThan(0).WithMessage("Id must be greater than 0");
            
    }
}
