using TimescaleDataProcessor.Core.Entities;

namespace TimescaleDataProcessor.Core.Interfaces;

public interface IValueRepository
{
    Task AddRangeAsync(IEnumerable<ValueRecord> records);
    Task DeleteByFileNameAsync(string fileName);
    Task<IEnumerable<ValueRecord>> GetLast10ByFileNameAsync(string fileName);
    Task SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}