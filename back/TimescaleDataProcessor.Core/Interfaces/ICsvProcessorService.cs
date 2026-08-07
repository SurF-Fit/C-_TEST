using TimescaleDataProcessor.Core.Emtites;

namespace TimescaleDataProcessor.Core.Interfaces;

public interface ICsvProcessorService
{
    Task<ResultRecord> ProcessCsvAsync(Stream csvStream, string fileName);
}