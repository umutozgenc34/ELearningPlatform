using Core.Extensions;
using Core.Infrastructures.CloudinaryServices;
using ELearningPlatform.Repository.Contexts;
using ELearningPlatform.Repository.Data;
using ELearningPlatform.Repository.Extensions;
using ELearningPlatform.Service;
using ELearningPlatform.Service.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddRepositoryExtension(builder.Configuration);
builder.Services.AddServiceExtension(builder.Configuration);
builder.Services.AddCommonServiceExtension(typeof(ServiceAssembly),builder.Configuration);

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 50 * 1024 * 1024;  // 50MB
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyOrigin() 
              .AllowAnyMethod() 
              .AllowAnyHeader(); 
    });
});



builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors("AllowAllOrigins");

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    await SeedData.Initialize(services, context);
}



if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
