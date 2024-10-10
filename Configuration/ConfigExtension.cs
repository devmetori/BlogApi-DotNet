using Microsoft.EntityFrameworkCore;
using BlogApi.Repository.Interfaces;
using BlogApi.Services.Interfaces;
using BlogApi.Repository;
using BlogApi.Services;
using BlogApi.Models;
using Microsoft.OpenApi.Models;

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
    public static void AddDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<BlogDbContext>(options =>
        {
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection"));
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
}