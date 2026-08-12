namespace TimescaleDataProcessor.Core.Entities;

public class ValueRecord
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public double ExecutionTime { get; set; }
    public double Value { get; set; }
    public string FileName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}