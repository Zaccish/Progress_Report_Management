using ProgressReportSystem.API.Models;

namespace ProgressReportSystem.API.Services;

public interface IActivityService
{
    Task LogActivityAsync(int userId, string action, string description, string? ip = null);
    Task LogFailedAttemptAsync(string email, string reason, string? ip = null);
}
