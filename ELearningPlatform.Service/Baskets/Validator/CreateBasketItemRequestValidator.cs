using ELearningPlatform.Model.Baskets.Dtos.Request;
using FluentValidation;

namespace ELearningPlatform.Service.Baskets.Validator;

public class CreateBasketItemRequestValidator : AbstractValidator<CreateBasketItemRequest>
{
    public CreateBasketItemRequestValidator()
    {
          
        RuleFor(x => x.CourseId)
            .NotEmpty()
            .WithMessage("CourseId is required");

       
    }
}
