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
    public IQueryable<Course> GetCoursesByDescriptionKeyword(string keyword) => Context.Courses
        .Where(c => !string.IsNullOrEmpty(c.Description) && EF.Functions.Like(c.Description, $"%{keyword}%"));


    public async Task<Course> GetCourseWithLessonsAsync(Guid courseId)
    {
        return await Context.Courses.Include(c => c.Lessons)
                             .FirstOrDefaultAsync(c => c.Id == courseId);

    }
}
