using ELearningPlatform.Model.Categories.Dtos.Response;
using ELearningPlatform.Model.Courses.Dtos.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ELearningPlatform.Model.Courses.Dtos.Request;

public class CreateCourseRequest
{
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public decimal Price { get; set; }
    public int CategoryId { get; set; }

    [FromForm]
    public IFormFile? ImageFile { get; set; }
    public CourseDetailsDto CourseDetails { get; set; }
}