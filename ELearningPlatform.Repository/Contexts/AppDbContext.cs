using ELearningPlatform.Model.Categories.Entity;
using ELearningPlatform.Model.Lessons.Entity;
using ELearningPlatform.Model.Courses.Entities;
using ELearningPlatform.Model.Discounts.Entity;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using ELearningPlatform.Model.Users.Entity;

namespace ELearningPlatform.Repository.Contexts;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<User,IdentityRole,string>(options)
{
    public DbSet<Category> Categories { get; set; }
    public DbSet<Lesson> Lessons { get; set; }  
    public DbSet<Course> Courses { get; set; }
    public DbSet<Discount> Discounts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}
