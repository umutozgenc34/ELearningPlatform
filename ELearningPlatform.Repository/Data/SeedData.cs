using ELearningPlatform.Model.Categories.Entity;
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
    }
}
