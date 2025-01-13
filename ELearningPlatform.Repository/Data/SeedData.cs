using ELearningPlatform.Model.Categories.Entity;
using ELearningPlatform.Model.Lessons.Entity;
using ELearningPlatform.Repository.Contexts;

namespace ELearningPlatform.Repository.Data;

public static class SeedData
{
    public static async Task Initialize(IServiceProvider serviceProvider, AppDbContext context)
    {
        
        if (!context.Categories.Any())
        {
            var categories = new List<Category>
            {
                new() { Name = "Game Development" },
                new() { Name = "Web" },
                new() { Name = "UI/UX" },
                new() { Name = "Data Science" },
                new() { Name = "Cyber Security" }
            };
            
            await context.Categories.AddRangeAsync(categories);
            await context.SaveChangesAsync();
        }
        if (!context.Lessons.Any())
        {
            var lessons = new List<Lesson>
    {
        new() { CourseId = Guid.NewGuid(), Title = "Introduction to Game Development", VideoUrl = "https://example.com/game-development-intro", LessonOrder = 1, Created = DateTime.UtcNow, Updated = DateTime.UtcNow },
        new() { CourseId = Guid.NewGuid(), Title = "HTML & CSS Basics", VideoUrl = "https://example.com/html-css-basics", LessonOrder = 1, Created = DateTime.UtcNow, Updated = DateTime.UtcNow },
        new() { CourseId = Guid.NewGuid(), Title = "Understanding User Experience Design", VideoUrl = "https://example.com/ux-design", LessonOrder = 1, Created = DateTime.UtcNow, Updated = DateTime.UtcNow },
        new() { CourseId = Guid.NewGuid(), Title = "Data Analysis with Python", VideoUrl = "https://example.com/data-analysis-python", LessonOrder = 1, Created = DateTime.UtcNow, Updated = DateTime.UtcNow },
        new() { CourseId = Guid.NewGuid(), Title = "Introduction to Cyber Security", VideoUrl = "https://example.com/cyber-security-intro", LessonOrder = 1, Created = DateTime.UtcNow, Updated = DateTime.UtcNow }
    };

            await context.Lessons.AddRangeAsync(lessons);
            await context.SaveChangesAsync();
        }
    }
}
