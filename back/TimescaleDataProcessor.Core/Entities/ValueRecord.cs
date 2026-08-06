namespace TimescaleDataProcessor.Core.Entites;

public class ValueRecord
{
    public Guid Id {get; set;}

    public DateTime Date {get; set;}

    public double ExecutionTime {get; set;}

    public double Value {get; set;}

    public string FileName {get; set;}

    public DateTime CreatedAt {get; set;} 
}