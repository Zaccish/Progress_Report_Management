namespace ProgressReportSystem.API.Models
{
    public class OneTimePassword
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public DateTime Expiry { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
