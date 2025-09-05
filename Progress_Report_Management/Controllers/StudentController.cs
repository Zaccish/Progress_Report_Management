using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProgressReportSystem.API.DTOs;
using ProgressReportSystem.API.Services;
using System.Security.Claims;

namespace ProgressReportSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Student")]
public class StudentController : ControllerBase
{
    private readonly IReportService _reportService;

    public StudentController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchReports([FromQuery] string? name, [FromQuery] string? matricId)
    {
        var loggedInMatricId = User.FindFirst("MatricId")?.Value;

        // Students can only search their own reports
        if (!string.IsNullOrEmpty(matricId) && matricId != loggedInMatricId)
            return Unauthorized("You are not allowed to access other students' reports.");

        var result = await _reportService.SearchReportsAsync(name, loggedInMatricId);
        return Ok(result);
    }

    [HttpPost("download")]
    public async Task<IActionResult> DownloadReport([FromBody] DownloadRequest request)
    {
        var loggedInMatricId = User.FindFirst("MatricId")?.Value;

        if (request.MatricId != loggedInMatricId)
            return Unauthorized("You are not authorized to download this report.");

        var result = await _reportService.DownloadReportAsync(request);
        if (result == null) return NotFound("Report not found or validation failed.");

        var memory = new MemoryStream();
        using (var stream = new FileStream(result.FilePath, FileMode.Open))
        {
            await stream.CopyToAsync(memory);
        }

        memory.Position = 0;
        return File(memory, "application/octet-stream", result.FileName);
    }
}