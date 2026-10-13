using System.Net;
using System.Text.Json.Serialization;
using Core.Configuration;
using Core.Extensions;
using Core.Options;
using Core.Response;
using Core.Services;
using ELearningPlatform.Model.Users.Entity;
using ELearningPlatform.Repository.Contexts;
using ELearningPlatform.Repository.Data;
using ELearningPlatform.Repository.Extensions;
using ELearningPlatform.Service;
using ELearningPlatform.Service.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

DotEnv.Load();

var builder = WebApplication.CreateBuilder(args);

var missingSettings = new[]
{
    "ConnectionStrings:SqlCon",
    "ConnectionStrings:MongoDb",
    "TokenOption:SecurityKey",
    "CloudinarySettings:CloudName",
    "CloudinarySettings:ApiKey",
    "CloudinarySettings:ApiSecret"
}.Where(key => string.IsNullOrWhiteSpace(builder.Configuration[key])).ToList();

if (missingSettings.Count > 0)
{
    throw new InvalidOperationException(
        $"Eksik ayar(lar): {string.Join(", ", missingSettings)}. " +
        "Proje kök dizininde .env dosyası olduğundan emin ol (.env.example dosyasını kopyalayıp doldurabilirsin).");
}

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    })
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage) ? "Geçersiz istek." : e.ErrorMessage)
                .Distinct()
                .ToList();

            return new BadRequestObjectResult(ServiceResult.Fail(errors));
        };
    });

builder.Services.AddRepositoryExtension(builder.Configuration);
builder.Services.AddServiceExtension(builder.Configuration);
builder.Services.AddCommonServiceExtension(typeof(ServiceAssembly), builder.Configuration);

builder.Services.Configure<CustomTokenOption>(builder.Configuration.GetSection("TokenOption"));
builder.Services.AddIdentity<User, IdentityRole>(opt =>
{
    opt.User.RequireUniqueEmail = true;
    opt.Password.RequireNonAlphanumeric = false;
}).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

builder.Services.AddVersioningExtension();

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 50 * 1024 * 1024;  // 50MB
});

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? ["http://localhost:5173"];
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opts =>
{
    var tokenOptions = builder.Configuration.GetSection("TokenOption").Get<CustomTokenOption>()!;
    opts.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
    {
        NameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name",
        ValidIssuer = tokenOptions.Issuer,
        ValidAudience = tokenOptions.Audience[0],
        IssuerSigningKey = SignService.GetSymmetricSecurityKey(tokenOptions.SecurityKey),
        ValidateIssuerSigningKey = true,
        ValidateAudience = true,
        ValidateIssuer = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromMinutes(5)
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Login'den dönen accessToken değerini yapıştır (başına 'Bearer' yazmana gerek yok)."
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
        var logger = context.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("UnhandledException");
        logger.LogError(exception, "Unhandled exception for {Method} {Path}", context.Request.Method, context.Request.Path);

        var message = app.Environment.IsDevelopment() && exception is not null
            ? $"Sunucu hatası: {exception.Message}"
            : "Beklenmeyen bir hata oluştu.";

        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        await context.Response.WriteAsJsonAsync(ServiceResult.Fail(message, HttpStatusCode.InternalServerError));
    });
});

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    await SeedData.Initialize(services, context);
}

var apiVersionSet = app.AddVersionSetExtension();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHttpsRedirection();
}

app.UseCors("Frontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
