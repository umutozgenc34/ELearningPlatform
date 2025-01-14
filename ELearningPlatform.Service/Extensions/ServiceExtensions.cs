using ELearningPlatform.Service.Baskets.Abstracts;
using ELearningPlatform.Service.Baskets.Concretes;
using ELearningPlatform.Service.Categories.Abstracts;
using ELearningPlatform.Service.Categories.Concretes;
using ELearningPlatform.Service.Courses.Abstracts;
using ELearningPlatform.Service.Courses.Concretes;
using ELearningPlatform.Service.Discounts.Abstracts;
using ELearningPlatform.Service.Discounts.Concretes;
using ELearningPlatform.Service.Orders.Abstracts;
using ELearningPlatform.Service.Orders.Concretes;
using ELearningPlatform.Service.Payment.Abstracts;
using ELearningPlatform.Service.Payment.Concretes;
using Microsoft.Extensions.Configuration;
using ELearningPlatform.Service.Lessons;
using ELearningPlatform.Service.Lessons.Abstracts;

using Microsoft.Extensions.DependencyInjection;
using ELearningPlatform.Service.Users.Abstracts;
using ELearningPlatform.Service.Users.Concretes;
using ELearningPlatform.Service.Token.Abstracts;
using ELearningPlatform.Service.Token.Concretes;
using ELearningPlatform.Service.Auth.Abstracts;
using ELearningPlatform.Service.Auth.Concretes;

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
        services.AddScoped<IDiscountService,DiscountService>();
        services.AddScoped<IUserService,UserService>();
        services.AddScoped<ITokenService,TokenService>();
        services.AddScoped<IAuthService,AuthService>();

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration =configuration.GetConnectionString("Redis");
        });

        
        services.AddScoped<ILessonService, LessonService>();
        return services;

    }
}
