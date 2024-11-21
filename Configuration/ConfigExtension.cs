using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using BlogApi.Lib.TemplateEngine.Interfaces;
using BlogApi.Data.Repository.Interfaces;
using BlogApi.Api.Services.Interfaces;
using BlogApi.Shared.Models.Settings;
using BlogApi.Lib.TemplateService;
using BlogApi.Shared.Exceptions;
using BlogApi.Data.Repository;
using BlogApi.Api.Services;
using BlogApi.Data.Context;
using BlogApi.Api.Filters;
using BlogApi.Lib.Logger;


namespace BlogApi.Configuration;

public static class ConfigExtension
{
    public static void AddInitialSetup(this WebApplicationBuilder builder)
    {
        // Exception handling setup
        builder.Services.AddProblemDetails().AddExceptionHandler<GlobalExceptionHandler>();

        // Logger setup
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();
        builder.Logging.AddProvider(new FileLoggerProvider("logs/app.log"));
        builder.Logging.SetMinimumLevel(LogLevel.Information);

        // Database setup
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddDbContext<BlogDbContext>(options => { options.UseSqlite(connectionString); });

        //  Environment setup
        builder.Services.Configure<TwoFactorSettings>(builder.Configuration.GetSection("TwoFactorSettings"));
        builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
        builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

        // Cors setup

        builder.Services.AddCors(o => o.AddDefaultPolicy(b => b.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

        // setup controllers
        builder.Services.AddControllersWithViews(options => { options.Filters.Add<ModelValidationFilter>(); });

        // setup cache
        builder.Services.AddMemoryCache();
    }

    public static void InitializeDb(this WebApplicationBuilder builder)
    {
        if (!builder.Environment.IsDevelopment()) return;
        using var scope = builder.Services.BuildServiceProvider().CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BlogDbContext>();
        DbInitializer.Initialize(context);
    }


    public static void AddLocalServices(this IServiceCollection services)
    {
        services.AddScoped<ITwoFactorAuthService, TwoFactorAuthService>();
        services.AddScoped<ITemplateService, TemplateService>();
        services.AddScoped<IArticleService, ArticleService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<ICacheService, CacheService>();
    }

    public static void AddLocalRepositories(this IServiceCollection services)
    {
        services.AddScoped<IArticleRepository, ArticleRepository>();
        services.AddScoped<ISessionRepository, SessionRepository>();
        services.AddScoped<IAuditRepository, AuditRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
    }


    public static void AddSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "BlogApi",
                Version = "v1",
                Description = "BlogApi is a simple API to manage articles and authors",
            });
            options.AddSecurityDefinition("bearer", new()
            {
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "bearer"
            });
            options.AddSecurityRequirement(new()
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Name = "Bearer",
                        In = ParameterLocation.Header,
                        Reference = new OpenApiReference
                        {
                            Id = "Bearer",
                            Type = ReferenceType.SecurityScheme
                        }
                    },
                    new List<string>()
                }
            });
        });
    }

    public static void UseLocalSwagger(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment()) return;
        app.UseSwagger();
        app.UseSwaggerUI();
    }
}