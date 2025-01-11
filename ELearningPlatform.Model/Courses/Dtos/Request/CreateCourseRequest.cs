namespace ELearningPlatform.Model.Courses.Dtos.Request;

public record CreateCourseRequest(
       string Name,
       string Description,
       decimal Price,
       string? ImageUrl,
       Guid CategoryId);
