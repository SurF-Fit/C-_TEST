namespace TimescaleDataProcessor.Core.Exceptions
{
    public class CsvValidationException : Exception
    {
        public CsvValidationException (string message) : base(message)
        {
            
        }
    }    
}