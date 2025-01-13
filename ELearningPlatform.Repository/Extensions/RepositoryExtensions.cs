using Core.Options;
using ELearningPlatform.Repository.Categories.Abstracts;
using ELearningPlatform.Repository.Categories.Concretes;
using ELearningPlatform.Repository.Contexts;
using ELearningPlatform.Repository.Courses.Abstracts;
using ELearningPlatform.Repository.Courses.Concretes;
using ELearningPlatform.Repository.Discounts.Abstracts;
using ELearningPlatform.Repository.Discounts.Concretes;
using ELearningPlatform.Repository.Interceptors;
using ELearningPlatform.Repository.Lessons.Abstracts;
using ELearningPlatform.Repository.Lessons.Concretes;
using ELearningPlatform.Repository.Orders.Abstracts;
using ELearningPlatform.Repository.Orders.Concretes;
using ELearningPlatform.Repository.UnitOfWorks.Abstracts;
using ELearningPlatform.Repository.UnitOfWorks.Concretes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ELearningPlatform.Repository.Extensions;

public static class RepositoryExtensions
{
    public static IServiceCollection AddRepositoryExtension(this IServiceCollection services,IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            var connectionStrings = configuration.GetSection
            (ConnectionStringOption.Key).Get<ConnectionStringOption>();

            options.UseSqlServer(connectionStrings!.SqlCon, sqlServerOptionsAction =>
            {
                sqlServerOptionsAction.MigrationsAssembly(typeof(RepositoryAssembly).Assembly.FullName);
            });

            options.AddInterceptors(new AuditDbContextInterceptor());
        });

        services.AddScoped<IUnitOfWork,UnitOfWork>();
        services.AddScoped<ICategoryRepository,CategoryRepository>();
        services.AddScoped<ICourseRepository,CourseRepository>();
        services.AddScoped<IOrderRepository, OrderRepositoryFromMongoDb>();
        services.AddScoped<IDiscountRepository, DiscountRepository>();
        services.AddScoped<ILessonRepository, LessonRepository>();  

        return services;
    }
}
