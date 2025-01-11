using ELearningPlatform.Model.Categories.Dtos.Response;

namespace ELearningPlatform.Model.Courses.Dtos.Response;

public record CreateCourseResponse(
    Guid Id,                           
    string Name,                    
    string Description,                 
    decimal Price,                     
    string? ImageUrl, 
    DateTime Created,
    CategoryDto Category,               
    CourseDetailsDto CourseDetails     
);