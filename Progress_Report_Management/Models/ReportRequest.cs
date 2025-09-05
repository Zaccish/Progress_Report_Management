namespace ProgressReportSystem.API.Models
{
    public class ReportRequest
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string MatricId { get; set; }
        public DateTime RequestedAt { get; set; }
        public string Email { get; internal set; }
        public string FullName { get; internal set; }
    }

}
