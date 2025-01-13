using ELearningPlatform.Model.Courses.Dtos.Request;
using FluentValidation;

namespace ELearningPlatform.Service.Courses.Validator;

public class CreateCourseRequestValidator : AbstractValidator<CreateCourseRequest>
{
    public CreateCourseRequestValidator()
    {
        
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required");

        RuleFor(x => x.Price)
            .NotEmpty()
            .WithMessage("Price is required")
            .GreaterThan(0)
            .WithMessage("Price must be greater than 0");

        RuleFor(x => x.CategoryId)
            .NotEmpty()
            .WithMessage("Category is required");
    }
}
