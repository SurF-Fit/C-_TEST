using TimescaleDataProcessor.Core.Entities;

namespace TimescaleDataProcessor.Core.Interfaces;

public interface IResultRepository
{
    Task AddOrUpdateAsync(ResultRecord record);
    Task<IEnumerable<ResultRecord>> GetFilteredAsync(
        string? fileName,
        DateTime? minDate,
        DateTime? maxDate,
        double? minAvgValue,
        double? maxAvgValue,
        double? minAvgExecTime,
        double? maxAvgExecTime
    );
    Task SaveChangesAsync();
}