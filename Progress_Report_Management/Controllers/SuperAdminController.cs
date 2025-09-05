using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProgressReportSystem.API.Data;
using ProgressReportSystem.API.Services;

namespace ProgressReportSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "SuperAdmin")]
public class SuperAdminController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IReportService _reportService;

    public SuperAdminController(AppDbContext db, IReportService reportService)
    {
        _db = db;
        _reportService = reportService;
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _db.Users
            .Select(u => new
            {
                u.Id,
                u.FullName,
                u.Email,
                u.MatricId,
                u.Role,
                u.IsApprovedAdmin
            })
            .ToListAsync();

        return Ok(users);
    }

    [HttpGet("activities")]
    public async Task<IActionResult> GetAllActivities()
    {
        var activities = await _db.ActivityLogs
            .Include(a => a.User)
            .OrderByDescending(a => a.Timestamp)
            .Select(a => new
            {
                a.Id,
                a.Action,
                a.Description,
                a.Timestamp,
                a.IpAddress,
                UserName = a.User.FullName,
                UserEmail = a.User.Email,
                UserRole = a.User.Role
            })
            .ToListAsync();

        return Ok(activities);
    }

    [HttpGet("pending-admin-requests")]
    public async Task<IActionResult> GetPendingAdminRequests()
    {
        var pending = await _db.Users
            .Where(u => u.Role == "Admin" && !u.IsApprovedAdmin)
            .Select(u => new
            {
                u.Id,
                u.FullName,
                u.Email,
                u.MatricId
            })
            .ToListAsync();

        return Ok(pending);
    }

    [HttpPost("approve-admin/{userId}")]
    public async Task<IActionResult> ApproveAdmin(int userId, [FromServices] IEmailService emailService)
    {
        var user = await _db.Users.FindAsync(userId);
        if (user == null) return NotFound("User not found");
        if (user.Role != "Admin") return BadRequest("User is not an admin");
        if (user.IsApprovedAdmin) return BadRequest("This admin has already been approved.");


        user.IsApprovedAdmin = true;
        await _db.SaveChangesAsync();

        // ✅ Send approval email
        await emailService.SendEmailAsync(
            user.Email,
            "✅ Admin Access Approved",
            $@"
    <p>Hello <b>{user.FullName}</b>,</p>
    <p>🎉 Your request to become an Admin has been <b>approved</b>!</p>
    <p>You now have full access to the administrative dashboard, including uploading reports and managing student data.</p>
    <p>Kindly log in and explore your new privileges.</p>
    <hr />
    <p style='font-size: 12px; color: #888;'>Progress Report Management System</p>
    "
        );


        return Ok($"User {user.FullName} is now an approved Admin and has been notified.");
        // await _activityService.LogActivityAsync(user.Id, "Admin Approved", $"Sent admin approval email to {user.FullName} ({user.Email})");
    }

    [HttpGet("report-requests")]
    public async Task<IActionResult> GetPendingReportRequests()
    {
        var pendingRequests = await _reportService.GetPendingReportRequestsAsync();
        return Ok(pendingRequests);
    }
}
