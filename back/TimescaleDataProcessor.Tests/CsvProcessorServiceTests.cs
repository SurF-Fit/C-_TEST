using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using TimescaleDataProcessor.Core.DTOs;
using TimescaleDataProcessor.Core.Entities;
using TimescaleDataProcessor.Core.Exceptions;
using TimescaleDataProcessor.Core.Interfaces;
using TimescaleDataProcessor.Core.Validators;

namespace TimescaleDataProcessor.Core.Services;

public class CsvProcessorService : ICsvProcessorService
{
    private readonly IValueRepository _valueRepository;
    private readonly IResultRepository _resultRepository;

    public CsvProcessorService(
        IValueRepository valueRepository,
        IResultRepository resultRepository)
    {
        _valueRepository = valueRepository;
        _resultRepository = resultRepository;
    }

    public async Task<ResultRecord> ProcessCsvAsync(Stream csvStream, string fileName)
    {
        var records = await ParseCsvAsync(csvStream);
        
        var errors = CsvValidator.ValidateRecords(records);
        if (errors.Any())
        {
            throw new CsvValidationException(string.Join("; ", errors));
        }

        await _valueRepository.BeginTransactionAsync();

        try
        {
            await _valueRepository.DeleteByFileNameAsync(fileName);

            var valueRecords = records.Select(dto => new ValueRecord
            {
                Id = Guid.NewGuid(),
                Date = dto.Date,
                ExecutionTime = dto.ExecutionTime,
                Value = dto.Value,
                FileName = fileName,
                CreatedAt = DateTime.UtcNow
            });

            await _valueRepository.AddRangeAsync(valueRecords);
            await _valueRepository.SaveChangesAsync();

            var result = CalculateResults(records, fileName);
            await _resultRepository.AddOrUpdateAsync(result);
            await _resultRepository.SaveChangesAsync();

            await _valueRepository.CommitTransactionAsync();

            return result;
        }
        catch
        {
            await _valueRepository.RollbackTransactionAsync();
            throw;
        }
    }

    private async Task<List<CsvRecordDto>> ParseCsvAsync(Stream csvStream)
    {
        var records = new List<CsvRecordDto>();

        using var reader = new StreamReader(csvStream);
        
        // Настраиваем конфигурацию для CSV с разделителем ;
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ";",
            HasHeaderRecord = true,
            MissingFieldFound = null,
            HeaderValidated = null,
            TrimOptions = TrimOptions.Trim,
            IgnoreBlankLines = true,
            AllowComments = true,
            Comment = '#'
        };

        using var csv = new CsvReader(reader, config);
        
        await csv.ReadAsync();
        csv.ReadHeader();

        var headers = csv.HeaderRecord;
        if (headers == null || headers.Length == 0)
        {
            throw new CsvValidationException("CSV файл не содержит заголовков");
        }

        while (await csv.ReadAsync())
        {
            try
            {
                var record = new CsvRecordDto
                {
                    Date = csv.GetField<DateTime>("Date"),
                    ExecutionTime = csv.GetField<double>("ExecutionTime"),
                    Value = csv.GetField<double>("Value")
                };
                records.Add(record);
            }
            catch (Exception ex)
            {
                throw new CsvValidationException($"Ошибка парсинга строки {csv.Parser.Row}: {ex.Message}");
            }
        }

        return records;
    }

    private ResultRecord CalculateResults(List<CsvRecordDto> records, string fileName)
    {
        var sortedRecords = records.OrderBy(r => r.Date).ToList();
        
        var minDate = sortedRecords.First().Date;
        var maxDate = sortedRecords.Last().Date;
        var deltaTime = (maxDate - minDate).TotalSeconds;

        var executionTimes = records.Select(r => r.ExecutionTime).ToList();
        var values = records.Select(r => r.Value).ToList();

        var averageExecutionTime = executionTimes.Average();
        var averageValue = values.Average();
        var medianValue = CalculateMedian(values);
        var maxValue = values.Max();
        var minValue = values.Min();

        return new ResultRecord
        {
            Id = Guid.NewGuid(),
            FileName = fileName,
            DeltaTimeSeconds = deltaTime,
            MinDate = minDate,
            AverageExecutionTime = averageExecutionTime,
            AverageValue = averageValue,
            MedianValue = medianValue,
            MaxValue = maxValue,
            MinValue = minValue
        };
    }

    private double CalculateMedian(List<double> values)
    {
        var sorted = values.OrderBy(v => v).ToList();
        var count = sorted.Count;
        
        if (count % 2 == 0)
        {
            return (sorted[count / 2 - 1] + sorted[count / 2]) / 2.0;
        }
        else
        {
            return sorted[count / 2];
        }
    }
}