using ELearningPlatform.Model.Lessons.Dtos.Request;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningPlatform.Service.Lessons.Validator
{
    public class CreateLessonRequestValidator : AbstractValidator<CreateLessonRequest>
    {
        public CreateLessonRequestValidator()
        {
            // Video URL'nin boş olmaması gerektiğini doğrula
            RuleFor(x => x.VideoFile)
                .NotEmpty()
                .WithMessage("Video URL is required.");

            // Başlık (Title) alanının boş olmaması gerektiğini doğrula
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Title is required.")
                .MaximumLength(200)
                .WithMessage("Title cannot be longer than 200 characters.");

            // LessonOrder alanının sıfırdan büyük olmasını doğrula
            RuleFor(x => x.LessonOrder)
                .GreaterThan(0)
                .WithMessage("Lesson Order must be greater than 0.");

            // CourseId'nin geçerli bir GUID olması gerektiğini doğrula
            RuleFor(x => x.CourseId)
                .NotEmpty()
                .WithMessage("Course Id is required.");
        }
    }
}
