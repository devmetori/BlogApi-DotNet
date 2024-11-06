using BlogApi.Api.Middlewares;
using BlogApi.Configuration;


var builder = WebApplication.CreateBuilder(args);

builder.AddInitialSetup();
builder.InitializeDb();
builder.Services.AddLocalRepositories();
builder.Services.AddLocalServices();
builder.Services.AddControllers();
builder.Services.AddSwagger();

var app = builder.Build();
app.UseMiddleware<InterceptorMiddleware>();
app.UsePathBase("/api/v1");
app.MapControllers();
app.UseLocalSwagger();
app.UseHttpsRedirection();
app.UseStatusCodePages();
app.UseExceptionHandler();
app.MapGet("/", async () => {

    return Results.Ok(new { success = true, message = "Hello world!" });
});
app.MapGet("/health", () => Results.Ok(new { success = true, message = "👍 API en linea" }));
app.Run();