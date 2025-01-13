


using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ELearningPlatform.Model.Lessons.Dtos.Request;

public class CreateLessonRequest
{
    public Guid CourseId { get; set; }
    public string Title { get; set; }
    [FromForm]
    public IFormFile VideoFile { get; set; }
    public int LessonOrder { get; set; }
}

   // (Guid CourseId, string Title, IFormFile VideoFile, int LessonOrder);