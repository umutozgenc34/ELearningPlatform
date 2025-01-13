using ELearningPlatform.Service.Baskets.Abstracts;
using ELearningPlatform.Service.Baskets.Concretes;
using ELearningPlatform.Service.Categories.Abstracts;
using ELearningPlatform.Service.Categories.Concretes;
using ELearningPlatform.Service.Courses.Abstracts;
using ELearningPlatform.Service.Courses.Concretes;
using ELearningPlatform.Service.Orders.Abstracts;
using ELearningPlatform.Service.Orders.Concretes;
using ELearningPlatform.Service.Payment.Abstracts;
using ELearningPlatform.Service.Payment.Concretes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ELearningPlatform.Service.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddServiceExtension(this IServiceCollection services,IConfiguration configuration)
    {
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<ICourseService, CourseService>();
        services.AddScoped<IBasketService,BasketService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IPaymentService, PaymentService>();

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration =configuration.GetConnectionString("Redis");
        });

        return services;
    }
}
