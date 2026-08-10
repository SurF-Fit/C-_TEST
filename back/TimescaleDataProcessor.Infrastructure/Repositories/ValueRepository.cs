using Microsoft.EntityFrameworkCore;
using TimescaleDataProcessor.Core.Entities;
using TimescaleDataProcessor.Core.Interfaces;
using TimescaleDataProcessor.Infrastructure.Data;

namespace TimescaleDataProcessor.Infrastructure.Repositories;

public class ValueRepository : IValueRepository
{
    private readonly ApplicationDbContext _context;

    public ValueRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddRangeAsync(IEnumerable<ValueRecord> records)
    {
        await _context.ValueRecords.AddRangeAsync(records);
    }

    public async Task DeleteByFileNameAsync(string fileName)
    {
        var records = await _context.ValueRecords
            .Where(v => v.FileName == fileName)
            .ToListAsync();
        
        if (records.Any())
        {
            _context.ValueRecords.RemoveRange(records);
        }
    }

    public async Task<IEnumerable<ValueRecord>> GetLast10ByFileNameAsync(string fileName)
    {
        return await _context.ValueRecords
            .Where(v => v.FileName == fileName)
            .OrderByDescending(v => v.Date)
            .Take(10)
            .ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        await _context.Database.CommitTransactionAsync();
    }

    public async Task RollbackTransactionAsync()
    {
        await _context.Database.RollbackTransactionAsync();
    }
}