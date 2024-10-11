using BlogApi.Configuration;
using BlogApi.Libs.Logger;


var builder = WebApplication.CreateBuilder(args);

builder.AddMainSetup();
builder.Services.AddLocalDbContext(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=blog.db");
builder.Services.AddLocalRepositories();
builder.Services.AddLocalServices();
builder.Services.AddControllers();
builder.Services.AddSwagger();

var app = builder.Build();
app.UseMiddleware<LoggingMiddleware>();
app.MapControllers();
app.UseLocalSwagger();
app.UseHttpsRedirection();
app.UseStatusCodePages();
app.UseExceptionHandler();

app.MapGet("/", () => Results.Ok(new { success = true, message = "Hello world" }));

app.Run();