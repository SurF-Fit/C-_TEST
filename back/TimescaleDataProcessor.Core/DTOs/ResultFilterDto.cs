namespace TimescaleDataProcessor.Core.DTOs
{

    public class ResultFilterDto
    {
        public string? FileName {get; set;}

        public DateTime? MinDate {get; set;}

        public DateTime? MaxDate {get; set;}

        public double? MinAverageValue { get; set; }

        public double? MaxAverageValue { get; set; }

        public double? MinAverageExecutionTime { get; set; }
        
        public double? MaxAverageExecutionTime { get; set; }
    }
    
}