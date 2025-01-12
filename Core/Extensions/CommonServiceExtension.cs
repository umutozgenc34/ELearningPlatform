using Core.Infrastructures.CloudinaryServices;
using Core.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Extensions;

public static class CommonServiceExtension
{
    public static IServiceCollection AddCommonServiceExtension(this IServiceCollection services,Type assembly,IConfiguration configuration)
    {
        services.AddAutoMapper(assembly);
        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssemblyContaining(assembly);

        services.AddScoped<IIdentityService,IdentityServiceFake>();
        services.AddScoped<ICloudinaryService, CloudinaryService>();
        services.Configure<CloudinarySettings>(configuration.GetSection("CloudinarySettings"));
        return services;
    }
}
