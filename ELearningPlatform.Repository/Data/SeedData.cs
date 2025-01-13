using ELearningPlatform.Model.Categories.Entity;
using ELearningPlatform.Model.Courses.Entities;
using ELearningPlatform.Repository.Contexts;
using Microsoft.EntityFrameworkCore;

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
    }
}
