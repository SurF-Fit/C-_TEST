using Microsoft.EntityFrameworkCore;
using TimescaleDataProcessor.Core.Entities;
using TimescaleDataProcessor.Core.Interfaces;
using TimescaleDataProcessor.Infrastructure.Data;

namespace TimescaleDataProcessor.Infrastructure.Repositories;

public class ResultRepository : IResultRepository
{
    private readonly ApplicationDbContext _context;

    public ResultRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddOrUpdateAsync(ResultRecord record)
    {
        var existing = await _context.ResultRecords
            .FirstOrDefaultAsync(r => r.FileName == record.FileName);

        if (existing != null)
        {
            existing.DeltaTimeSeconds = record.DeltaTimeSeconds;
            existing.MinDate = record.MinDate;
            existing.AverageExecutionTime = record.AverageExecutionTime;
            existing.AverageValue = record.AverageValue;
            existing.MedianValue = record.MedianValue;
            existing.MaxValue = record.MaxValue;
            existing.MinValue = record.MinValue;
            existing.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            record.CreatedAt = DateTime.UtcNow;
            record.UpdatedAt = DateTime.UtcNow;
            await _context.ResultRecords.AddAsync(record);
        }
    }

    public async Task<IEnumerable<ResultRecord>> GetFilteredAsync(
        string? fileName,
        DateTime? minDate,
        DateTime? maxDate,
        double? minAvgValue,
        double? maxAvgValue,
        double? minAvgExecTime,
        double? maxAvgExecTime)
    {
        var query = _context.ResultRecords.AsQueryable();

        if (!string.IsNullOrEmpty(fileName))
            query = query.Where(r => r.FileName.Contains(fileName));

        if (minDate.HasValue)
            query = query.Where(r => r.MinDate >= minDate.Value);

        if (maxDate.HasValue)
            query = query.Where(r => r.MinDate <= maxDate.Value);

        if (minAvgValue.HasValue)
            query = query.Where(r => r.AverageValue >= minAvgValue.Value);

        if (maxAvgValue.HasValue)
            query = query.Where(r => r.AverageValue <= maxAvgValue.Value);

        if (minAvgExecTime.HasValue)
            query = query.Where(r => r.AverageExecutionTime >= minAvgExecTime.Value);

        if (maxAvgExecTime.HasValue)
            query = query.Where(r => r.AverageExecutionTime <= maxAvgExecTime.Value);

        return await query.ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}