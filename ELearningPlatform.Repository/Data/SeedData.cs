using ELearningPlatform.Model.Categories.Entity;
using ELearningPlatform.Model.Lessons.Entity;
using ELearningPlatform.Model.Courses.Entities;
using ELearningPlatform.Model.Discounts.Entity;
using ELearningPlatform.Repository.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace ELearningPlatform.Repository.Data;

public static class SeedData
{
    public static async Task Initialize(IServiceProvider serviceProvider, AppDbContext context)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        var roles = new List<string> { "Admin", "User", "Educator" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

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

        if (!context.Courses.Any())
        {
            var gameDevCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Game Development");
            var webCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Web");
            var uiUxCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "UI/UX");

            var courses = new List<Course>
            {
                new Course
                {
                    Name = "Introduction to Unity",
                    Description = "Learn the basics of Unity, the most popular game development engine.",
                    Price = 99.99m,
                    ImageUrl = "https://example.com/unity-course-image.jpg",
                    UserId = Guid.NewGuid().ToString(),
                    Category = gameDevCategory,
                    CourseDetails = new CourseDetails
                    {
                        Duration = 30, // örnek süre: 30 saat
                        Rating = 4.5f,
                        EducatorFullName = "John Doe"
                    }
                },
                new Course
                {
                    Name = "Full Stack Web Development",
                    Description = "Comprehensive guide to becoming a full stack developer using modern tools.",
                    Price = 199.99m,
                    ImageUrl = "https://example.com/web-course-image.jpg",
                    UserId = Guid.NewGuid().ToString(),
                    Category = webCategory,
                    CourseDetails = new CourseDetails
                    {
                        Duration = 40,
                        Rating = 4.8f,
                        EducatorFullName = "Jane Smith"
                    }
                },
                new Course
                {
                    Name = "UI/UX Design Basics",
                    Description = "Learn the fundamental principles of user interface and user experience design.",
                    Price = 79.99m,
                    ImageUrl = "https://example.com/uiux-course-image.jpg",
                    UserId = Guid.NewGuid().ToString(),
                    Category = uiUxCategory,
                    CourseDetails = new CourseDetails
                    {
                        Duration = 25,
                        Rating = 4.2f,
                        EducatorFullName = "Alice Johnson"
                    }
                }
            };

            await context.Courses.AddRangeAsync(courses);
            await context.SaveChangesAsync();
        }

        if (!context.Discounts.Any())
        {
            var discounts = new List<Discount>() 
            {
                new Discount { Coupon = "BABA10", Rate = 10 },
                new Discount { Coupon = "BABA20", Rate = 20 },
            };

            await context.Discounts.AddRangeAsync(discounts);
            await context.SaveChangesAsync();
        }

        
    }
}
