using TimescaleDataProcessor.Core.Entities;

namespace TimescaleDataProcessor.Core.Interfaces;

public interface ICsvProcessorService
{
    Task<ResultRecord> ProcessCsvAsync(Stream csvStream, string fileName);
}