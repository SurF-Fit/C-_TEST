using Microsoft.EntityFrameworkCore;
using TimescaleDataProcessor.Core.Entities;

namespace TimescaleDataProcessor.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        
    }

    public DbSet<ValueRecord> ValueRecords {get; set;}

    public DbSet<ResultRecord> ResultRecords {get; set;}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ValueRecord>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.FileName, e.Date });
            entity.Property(e => e.FileName).IsRequired().HasMaxLength(255);
            entity.Property(e => e.ExecutionTime).HasPrecision(18, 6);
            entity.Property(e => e.Value).HasPrecision(18, 6);
        });

        modelBuilder.Entity<ResultRecord>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.FileName).IsUnique();
            entity.Property(e => e.FileName).IsRequired().HasMaxLength(255);
            entity.Property(e => e.AverageExecutionTime).HasPrecision(18, 6);
            entity.Property(e => e.AverageValue).HasPrecision(18, 6);
            entity.Property(e => e.MedianValue).HasPrecision(18, 6);
            entity.Property(e => e.MaxValue).HasPrecision(18, 6);
            entity.Property(e => e.MinValue).HasPrecision(18, 6);
        });
    }
}