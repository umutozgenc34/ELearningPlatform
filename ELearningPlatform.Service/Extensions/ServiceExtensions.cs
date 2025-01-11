using ELearningPlatform.Service.Categories.Abstracts;
using ELearningPlatform.Service.Categories.Concretes;
using ELearningPlatform.Service.Courses.Abstracts;
using ELearningPlatform.Service.Courses.Concretes;
using Microsoft.Extensions.DependencyInjection;

namespace ELearningPlatform.Service.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddServiceExtension(this IServiceCollection services)
    {
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<ICourseService, CourseService>();
        return services;
    }
}
