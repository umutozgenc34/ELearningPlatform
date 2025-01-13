using ELearningPlatform.Model.Categories.Entity;
using ELearningPlatform.Model.Lessons.Entity;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace ELearningPlatform.Repository.Contexts;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Category> Categories { get; set; }
    public DbSet<Lesson> Lessons { get; set; }  

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}
