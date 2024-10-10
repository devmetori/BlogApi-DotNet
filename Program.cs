using BlogApi.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwagger();
builder.Services.AddControllers();
builder.Services.AddLocalRepositories();
builder.Services.AddLocalServices();
builder.Services.AddDbContext(builder.Configuration);

var app = builder.Build();

app.MapControllers();
app.UseLocalSwagger();
app.UseHttpsRedirection();

app.MapGet("/", () => Results.Ok(new { success = true, message = "Hello world" }));

app.Run();

