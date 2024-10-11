using Microsoft.EntityFrameworkCore;
using BlogApi.Repository.Interfaces;
using BlogApi.Services.Interfaces;
using Microsoft.OpenApi.Models;
using BlogApi.Libs.Logger;
using BlogApi.Repository;
using BlogApi.Services;
using BlogApi.Models;
using BlogApi.Shared.Exceptions;

namespace BlogApi.Configuration;

public static class ConfigExtension
{
    
    public static void AddLocalServices(this IServiceCollection services)
    {
        services.AddScoped<IArticleService, ArticleService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IAuthService, AuthService>();
    }
    
    public static void AddLocalRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IArticleRepository, ArticleRepository>();
        services.AddScoped<ISessionRepository, SessionRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IAuditRepository, AuditRepository>();
        
    }
    // add setup db context
    public static void AddLocalDbContext(this IServiceCollection services, string connectionString)
    {

        services.AddDbContext<BlogDbContext>(options =>
        {
            options.UseSqlite(connectionString);
        });
    }
    
    // Config swagger
    public static void AddSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "BlogApi", Version = "v1" });
        });
    }
    public static void UseLocalSwagger(this  WebApplication app)
    {
        if (!app.Environment.IsDevelopment()) return;
        app.UseSwagger();
        app.UseSwaggerUI();
        
    }

   public static void AddMainSetup(this WebApplicationBuilder builder)
    {
        builder.Services.AddProblemDetails().AddExceptionHandler<GlobalExceptionHandler>();
        builder.Logging.ClearProviders(); 
        builder.Logging.AddConsole();     
        builder.Logging.AddProvider( new FileLoggerProvider("logs/app.log", 10));
        builder.Logging.SetMinimumLevel(LogLevel.Information);
    }
}