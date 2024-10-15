using BlogApi.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using BlogApi.Data.Repository.Interfaces;
using BlogApi.Shared.Exceptions;
using BlogApi.Data.Repository;
using BlogApi.Api.Services;
using BlogApi.Data.Context;
using BlogApi.Lib.Logger;

namespace BlogApi.Configuration;

public static class ConfigExtension
{
    public static void AddInitialSetup(this WebApplicationBuilder builder)
    {
        // Exception handling setup
        builder.Services.AddProblemDetails()
            .AddExceptionHandler<GlobalExceptionHandler>();

        // Logger setup
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();
        builder.Logging.AddProvider(new FileLoggerProvider("logs/app.log"));
        builder.Logging.SetMinimumLevel(LogLevel.Information);

        // Database setup
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=blog.db";
        builder.Services.AddDbContext<BlogDbContext>(options => { options.UseSqlite(connectionString); });
    }

    public static void AddLocalServices(this IServiceCollection services)
    {
        services.AddScoped<IArticleService, ArticleService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IJwtService, JwtService>();
    }

    public static void AddLocalRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IArticleRepository, ArticleRepository>();
        services.AddScoped<ISessionRepository, SessionRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IAuditRepository, AuditRepository>();
    }


    public static void AddSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo { Title = "BlogApi", Version = "v1" }); });
    }

    public static void UseLocalSwagger(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment()) return;
        app.UseSwagger();
        app.UseSwaggerUI();
    }
}