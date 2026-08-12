using TimescaleDataProcessor.Core.DTOs;

namespace TimescaleDataProcessor.Core.Validators;

public static class CsvValidator
{
    private static readonly DateTime MinDate = new DateTime(2000, 1, 1).ToUniversalTime();
    private const int MaxRecords = 10000;
    private const int MinRecords = 1;

    public static List<string> ValidateRecords(List<CsvRecordDto> records)
    {
        var errors = new List<string>();

        if (records.Count < MinRecords || records.Count > MaxRecords)
        {
            errors.Add($"File must contain between {MinRecords} and {MaxRecords} records. Found: {records.Count}");
            return errors;
        }

        var now = DateTime.UtcNow;

        for (int i = 0; i < records.Count; i++)
        {
            var record = records[i];
            var rowNumber = i + 1;

            if (record.Date > now)
                errors.Add($"Row {rowNumber}: Date cannot be in the future");

            if (record.Date < MinDate)
                errors.Add($"Row {rowNumber}: Date cannot be earlier than {MinDate:yyyy-MM-dd}");

            if (record.ExecutionTime < 0)
                errors.Add($"Row {rowNumber}: Execution time cannot be negative");

            if (record.Value < 0)
                errors.Add($"Row {rowNumber}: Value cannot be negative");
        }

        return errors;
    }
}