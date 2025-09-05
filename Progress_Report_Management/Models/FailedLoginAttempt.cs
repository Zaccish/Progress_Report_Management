namespace ProgressReportSystem.API.Models
{
    public class FailedLoginAttempt
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }
}
