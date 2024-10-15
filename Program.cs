using BlogApi.Configuration;
using BlogApi.Lib.Logger;



var builder = WebApplication.CreateBuilder(args);

builder.AddInitialSetup();
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
