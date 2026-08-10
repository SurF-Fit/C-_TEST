using Microsoft.EntityFrameworkCore;
using TimescaleDataProcessor.Core.Interfaces;
using TimescaleDataProcessor.Core.Services;
using TimescaleDataProcessor.Infrastructure.Data;
using TimescaleDataProcessor.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IValueRepository, ValueRepository>();
builder.Services.AddScoped<IResultRepository, ResultRepository>();
builder.Services.AddScoped<ICsvProcessorService, CsvProcessorService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSPA",
        builder =>
        {
            builder.WithOrigins("http://localhost:4200", "http://spa:4200")
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowSPA");
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.EnsureCreated();
}

app.Run();