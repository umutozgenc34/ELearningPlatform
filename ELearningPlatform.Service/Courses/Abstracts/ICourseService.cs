using Core.Response;
using ELearningPlatform.Model.Courses.Dtos.Request;
using ELearningPlatform.Model.Courses.Dtos.Response;
using ELearningPlatform.Model.Courses.Entities;

namespace ELearningPlatform.Service.Courses.Abstracts;

public interface ICourseService
{
    Task<ServiceResult<List<CourseDto>>> GetAllAsync();
    Task<ServiceResult<CourseDto>> GetByIdAsync(Guid id);
    Task<ServiceResult<CourseDto>> GetCourseWithDetailsAsync(Guid id);
    Task<ServiceResult<List<CourseDto>>> GetAllCoursesWithDetailsAsync();
    Task<ServiceResult<CreateCourseResponse>> CreateAsync(CreateCourseRequest request);
    Task<ServiceResult> UpdateAsync(UpdateCourseRequest request);
    Task<ServiceResult> DeleteAsync(Guid id);
    Task <ServiceResult<List<Course>>> GetCoursesByDescriptionKeyword(string keyword);
    Task<ServiceResult<List<CourseDto>>> GetCoursesByCategoryIdAsync(int categoryId);

}
