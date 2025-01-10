using ELearningPlatform.Model.Categories.Dtos.Request;
using FluentValidation;

namespace ELearningPlatform.Service.Categories.Validator;

public class CreateCategoryRequestValidator : AbstractValidator<CreateCategoryRequest>
{
    public CreateCategoryRequestValidator()
    {
        
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .MaximumLength(100)
            .WithMessage("Name can not be longer than 50 characters");
    }
}
