using TimescaleDataProcessor.Core.Entites;

namespace TimescaleDataProcessor.Core.Interfaces;

public interface IValueRepotory
{
    Task AddRangeAsync(IEnumerable<ValueRecord> records);
    Task DeleteByFileNameAsync(string fileName);
    Task<IEnumerable<ValueRecord>> GetList10ByFileNameAsync(string fileName);
    Task SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}