namespace ELearningPlatform.Model.Courses.Dtos.Request;

public record UpdateCourseRequest(
       Guid Id,
       string Name,
       string Description,
       decimal Price,
       string? ImageUrl,
       Guid CategoryId); 

