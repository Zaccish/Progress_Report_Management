using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProgressReportSystem.API.DTOs;
using ProgressReportSystem.API.Services;
using System.Security.Claims;

namespace ProgressReportSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin, SuperAdmin")]
public class AdminController : ControllerBase
{
    private readonly IReportService _reportService;

    public AdminController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpPost("upload-report")]
    public async Task<IActionResult> UploadReport([FromForm] UploadReportDTO dto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var result = await _reportService.UploadReportAsync(dto, userId);

        if (!result.Success)
            return BadRequest(result.Message);

        return Ok(result.Message);
    }

    [HttpGet("student-activity/{matricId}")]
    public async Task<IActionResult> GetStudentActivity(string matricId)
    {
        // Optionally: Add logging/audit here
        var logs = await _reportService.GetActivityLogsByMatricIdAsync(matricId);
        return Ok(logs);
    }
}
