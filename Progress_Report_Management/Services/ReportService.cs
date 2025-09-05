using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ProgressReportSystem.API.Data;
using ProgressReportSystem.API.DTOs;
using ProgressReportSystem.API.Helpers;
using ProgressReportSystem.API.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ProgressReportSystem.API.Services;

public class ReportService : IReportService
{
    private readonly AppDbContext _db;
    private readonly IWebHostEnvironment _env;
    private readonly IActivityService _activityService;
    private readonly IEmailService _emailService;

    public ReportService(AppDbContext db, IWebHostEnvironment env, IActivityService activityService, IEmailService emailService)
    {
        _db = db;
        _env = env;
        _activityService = activityService;
        _emailService = emailService;
    }

    public async Task<(bool Success, string Message)> UploadReportAsync(UploadReportDTO dto, int adminId)
    {
        var adminUser = await _db.Users.FindAsync(adminId);
        var isApprovedAdmin = adminUser?.Role == "Admin" && adminUser.IsApprovedAdmin;
        var isSuperAdmin = adminUser?.Role == "SuperAdmin";

        if (adminUser == null || (!isApprovedAdmin && !isSuperAdmin))
            return (false, "You are not authorized or not approved to upload reports.");

        if (!FileHelper.IsValidReportFile(dto.File))
            return (false, "Invalid file type.");

        var filePath = FileHelper.SaveFile(dto.File, _env.ContentRootPath);

        var report = new ProgressReport
        {
            FileName = dto.File.FileName,
            FilePath = filePath,
            Department = dto.Department,
            CourseProgram = dto.CourseProgram,
            StudentName = dto.StudentName,
            MatricId = dto.MatricId,
            UploadedAt = DateTime.UtcNow,
            UploadedById = adminId
        };

        _db.ProgressReports.Add(report);
        await _db.SaveChangesAsync();

        await _activityService.LogActivityAsync(adminId, "Upload", $"Uploaded report for {dto.StudentName}");

        // ✅ Notify student if exists and delete any matching report request
        var studentUser = await _db.Users.FirstOrDefaultAsync(u => u.MatricId == dto.MatricId);
        if (studentUser != null)
        {
            await _emailService.SendReportAvailableEmailAsync(studentUser, report);
            await _activityService.LogActivityAsync(adminId, "Email Notification",
                $"Sent report upload notification with attachment to {studentUser.FullName} ({studentUser.Email})");

            // ✅ Step 3: Remove pending report request after uploading the report
            var existingRequest = await _db.ReportRequests.FirstOrDefaultAsync(r => r.MatricId == dto.MatricId);
            if (existingRequest != null)
            {
                _db.ReportRequests.Remove(existingRequest);
                await _db.SaveChangesAsync();
            }
        }

        return (true, "Report uploaded successfully.");
    }

    public async Task<int> GetVAsync()
    {
        return await _db.SaveChangesAsync();
    }

    // ✅ Step 2: Enhanced SearchReportsAsync to log report request if report is missing
    public async Task<IEnumerable<ProgressReport>> SearchReportsAsync(string? name, string? matricId)
    {
        var query = _db.ProgressReports.AsQueryable();

        if (!string.IsNullOrEmpty(name))
            query = query.Where(r => r.StudentName.Contains(name));

        if (!string.IsNullOrEmpty(matricId))
            query = query.Where(r => r.MatricId == matricId);

        var results = await query.AsNoTracking().ToListAsync();

        if (!results.Any() && !string.IsNullOrEmpty(matricId))
        {
            var student = await _db.Users.FirstOrDefaultAsync(u => u.MatricId == matricId && u.Role == "Student");
            if (student != null)
            {
                var alreadyRequested = await _db.ReportRequests.AnyAsync(r => r.MatricId == matricId);
                if (!alreadyRequested)
                {
                    _db.ReportRequests.Add(new ReportRequest
                    {
                        MatricId = student.MatricId,
                        Email = student.Email,
                        FullName = student.FullName,
                        RequestedAt = DateTime.UtcNow
                    });

                    await _db.SaveChangesAsync();
                }
            }
        }

        return results;
    }

    public async Task<IEnumerable<ActivityLog>> GetActivityLogsByMatricIdAsync(string matricId)
    {
        var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.MatricId == matricId);
        if (user == null) return Enumerable.Empty<ActivityLog>();

        return await _db.ActivityLogs
            .AsNoTracking()
            .Where(a => a.UserId == user.Id)
            .Select(a => new ActivityLog
            {
                Id = a.Id,
                Action = a.Action,
                Description = a.Description,
                Timestamp = a.Timestamp,
                UserId = a.UserId
            })
            .ToListAsync();
    }

    public async Task<ProgressReport?> DownloadReportAsync(DownloadRequest request)
    {
        var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u =>
            u.Email == request.Email &&
            u.FullName == request.FullName &&
            u.MatricId == request.MatricId);

        if (user == null) return null;

        var report = await _db.ProgressReports.AsNoTracking().FirstOrDefaultAsync(r => r.MatricId == request.MatricId);
        if (report == null) return null;

        await _activityService.LogActivityAsync(user.Id, "Download", $"Downloaded report for {report.StudentName}");
        return report;
    }

    public async Task<IEnumerable<ReportRequest>> GetPendingReportRequestsAsync()
    {
        return await _db.ReportRequests
            .AsNoTracking()
            .OrderByDescending(r => r.RequestedAt)
            .ToListAsync();
    }

   
}
