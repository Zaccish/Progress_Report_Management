using ProgressReportSystem.API.Data;
using ProgressReportSystem.API.Models;

namespace ProgressReportSystem.API.Services;

public class ActivityService : IActivityService
{
    private readonly AppDbContext _db;

    public ActivityService(AppDbContext db)
    {
        _db = db;
    }

    public async Task LogActivityAsync(int userId, string action, string description, string? ip = null)
    {
        var log = new ActivityLog
        {
            Action = action,
            Description = description,
            Timestamp = DateTime.UtcNow,
            IpAddress = ip
        };

        // Only assign valid UserId if it's greater than 0
        if (userId > 0)
            log.UserId = userId;

        _db.ActivityLogs.Add(log);
        await _db.SaveChangesAsync();
    }



    public async Task LogFailedAttemptAsync(string email, string reason, string ipAddress)
    {
        var log = new FailedLoginAttempt
        {
            Email = email,
            Reason = reason,
            IpAddress = ipAddress,
            Timestamp = DateTime.UtcNow
        };

        _db.FailedLoginAttempts.Add(log);
        await _db.SaveChangesAsync();
    }

    }
