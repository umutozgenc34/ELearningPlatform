using Core.Repositories.Concretes;
using ELearningPlatform.Model.Courses.Entities;
using ELearningPlatform.Repository.Contexts;
using ELearningPlatform.Repository.Courses.Abstracts;
using Microsoft.EntityFrameworkCore;

namespace ELearningPlatform.Repository.Courses.Concretes;

public class CourseRepository(AppDbContext context) : GenericRepository<AppDbContext, Course, Guid>(context), ICourseRepository
{
    public async Task<Course?> GetCourseWithDetailsAsync(Guid courseId)
    {
        return await context.Courses
            .Include(c => c.CourseDetails) 
            .FirstOrDefaultAsync(c => c.Id == courseId);
    }
    public async Task<List<Course>> GetAllCoursesWithDetailsAsync()
    {
        return await context.Courses
            .Include(c => c.CourseDetails) 
            .ToListAsync();
    }
}
