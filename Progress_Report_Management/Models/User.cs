
namespace ProgressReportSystem.API.Models;

public class User
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string MatricId { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;

    public string Role { get; set; } = "Student"; // "SuperAdmin", "Admin", "Student"
    public bool IsApprovedAdmin { get; set; } = false;
    private ICollection<ActivityLog>? activityLogs;

    public ICollection<ActivityLog>? GetActivityLogs()
    {
        return activityLogs;
    }

    public void SetActivityLogs(ICollection<ActivityLog>? value)
    {
        activityLogs = value;
    }

    public DateTime CreatedAt { get; internal set; }
    public bool IsEmailConfirmed { get; set; } = false;

    // Navigation property
    public ICollection<ActivityLog> ActivityLogs { get; set; }
        = new List<ActivityLog>();
}
