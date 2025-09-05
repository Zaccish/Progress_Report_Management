namespace ProgressReportSystem.API.Models;
public class ActivityLog
{
    public int Id { get; set; }
    public string Action { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string Description { get; set; } = string.Empty;
    public int? UserId { get; set; }
    public  User? User { get; set; } 
    public string? IpAddress { get; set; } // ✅ New field
   
}