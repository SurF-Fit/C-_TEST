using Microsoft.EntityFrameworkCore;
using TimescaleDataProcessor.Core.Interfaces;
using TimescaleDataProcessor.Core.Services;
using TimescaleDataProcessor.Infrastructure.Data;
using TimescaleDataProcessor.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "TimescaleDataProcessor API",
        Version = "v1",
        Description = "API для работы с timescale данными"
    });
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Host=localhost;Database=timescaledb;Username=postgres;Password=postgres";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<IValueRepository, ValueRepository>();
builder.Services.AddScoped<IResultRepository, ResultRepository>();
builder.Services.AddScoped<ICsvProcessorService, CsvProcessorService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "TimescaleDataProcessor API V1");
    c.RoutePrefix = "swagger";
});

app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.MapGet("/health", () => Results.Ok(new { status = "Healthy", timestamp = DateTime.UtcNow }));

try
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        Console.WriteLine("Attempting to connect to database...");
        dbContext.Database.EnsureCreated();
        Console.WriteLine("Database connection successful!");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Database connection failed: {ex.Message}");
}

app.Run();