using Microsoft.AspNetCore.Mvc;
using TimescaleDataProcessor.Core.Exceptions;
using TimescaleDataProcessor.Core.Interfaces;
using Microsoft.AspNetCore.Http;

namespace TimescaleDataProcessor.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DataController : ControllerBase
{
    private readonly ICsvProcessorService _csvProcessor;
    private readonly IResultRepository _resultRepository;
    private readonly IValueRepository _valueRepository;

    public DataController(
        ICsvProcessorService csvProcessor,
        IResultRepository resultRepository,
        IValueRepository valueRepository)
    {
        _csvProcessor = csvProcessor;
        _resultRepository = resultRepository;
        _valueRepository = valueRepository;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadCsv(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { error = "File is required" });

        if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            return BadRequest(new { error = "Only CSV files are allowed" });

        try
        {
            using var stream = file.OpenReadStream();
            var result = await _csvProcessor.ProcessCsvAsync(stream, file.FileName);
            return Ok(new { message = "File processed successfully", data = result });
        }
        catch (CsvValidationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("results")]
    public async Task<IActionResult> GetResults(
        [FromQuery] string? fileName,
        [FromQuery] DateTime? minDate,
        [FromQuery] DateTime? maxDate,
        [FromQuery] double? minAvgValue,
        [FromQuery] double? maxAvgValue,
        [FromQuery] double? minAvgExecTime,
        [FromQuery] double? maxAvgExecTime)
    {
        var results = await _resultRepository.GetFilteredAsync(
            fileName,
            minDate,
            maxDate,
            minAvgValue,
            maxAvgValue,
            minAvgExecTime,
            maxAvgExecTime);

        return Ok(results);
    }

    [HttpGet("values/{fileName}/last10")]
    public async Task<IActionResult> GetLast10Values(string fileName)
    {
        var values = await _valueRepository.GetLast10ByFileNameAsync(fileName);
        
        if (!values.Any())
            return NotFound(new { error = $"No values found for file: {fileName}" });

        return Ok(values);
    }
}