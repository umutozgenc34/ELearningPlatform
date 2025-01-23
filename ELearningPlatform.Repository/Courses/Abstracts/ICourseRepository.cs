using Core.Repositories.Abstracts;
using ELearningPlatform.Model.Courses.Entities;

namespace ELearningPlatform.Repository.Courses.Abstracts;

public interface ICourseRepository : IGenericRepository<Course, Guid>
{
    Task<Course?> GetCourseWithDetailsAsync(Guid courseId);
    Task<List<Course>> GetAllCoursesWithDetailsAsync();
    IQueryable<Course> GetCoursesByDescriptionKeyword(string keyword);
    Task<Course> GetCourseWithLessonsAsync(Guid courseId);
}
