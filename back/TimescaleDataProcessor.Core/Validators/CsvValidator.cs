using TimescaleDataProcessor.Core.DTOs;

namespace TimescaleDataProcessor.Core.Validators;

public static class CsvValidator
{
    private static readonly DateTime MinDate = new DateTime(2000, 1, 1);
    private const int MaxRecords = 10000;
    private const int MinRecords = 1;

    public static List<string> ValidateRecords(List<CsvRecordsDto> records)
    {
        var errors = new List<string>();

        if (records.Count < MinRecords || records.Count > MaxRecords)
        {
            errors.Add($"Файл должен быть в диапазоне {MinRecords} и {MaxRecords}. Найдено: {records.Count}");
            return errors;
        }

        var now = DateTime.UtcNow;

        for (int i = 0; i < records.Count; i++)
        {
            var record = records[i];
            var rowNumber = i + 1;

            if (record.Date > now)
            {
                errors.Add($"Строка {rowNumber}: Дата не может быть назначена в будущем");
            }
            if (record.Date < MinDate)
            {
                errors.Add($"Строка {rowNumber}: Дата не может быть ранне, чем {MinDate:yyyy-MM-dd}");
            }
            if (record.ExecutionTime < 0)
            {
                errors.Add($"Строка {rowNumber}: Время выполнения не может быть отрицательным");
            }
            if (record.Value < 0)
            {
                errors.Add($"Строка {rowNumber}: Значение не может быть отрицательным");
            }
        }

        return errors;
    }
}