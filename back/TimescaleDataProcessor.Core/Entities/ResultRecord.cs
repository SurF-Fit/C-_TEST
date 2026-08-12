namespace TimescaleDataProcessor.Core.Entities;

public class ResultRecord
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FileName { get; set; } = string.Empty;
    public double DeltaTimeSeconds { get; set; }
    public DateTime MinDate { get; set; } = DateTime.UtcNow;
    public double AverageExecutionTime { get; set; }
    public double AverageValue { get; set; }
    public double MedianValue { get; set; }
    public double MaxValue { get; set; }
    public double MinValue { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}