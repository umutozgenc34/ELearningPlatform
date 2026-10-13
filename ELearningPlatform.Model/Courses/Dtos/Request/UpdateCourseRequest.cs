using Microsoft.AspNetCore.Http;

namespace ELearningPlatform.Model.Courses.Dtos.Request;

// CategoryId önceden Guid'di ama Course.CategoryId int; bu yüzden güncelleme hiç çalışmıyordu.
public record UpdateCourseRequest(
       Guid Id,
       string Name,
       string Description,
       decimal Price,
       IFormFile? ImageFile,
       int CategoryId);
