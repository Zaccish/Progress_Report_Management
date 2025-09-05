namespace ProgressReportSystem.API.DTOs;
public class DownloadRequest
{
    public string FullName { get; set; } = null!;
    public string MatricId { get; set; } = null!;
    public string Email { get; set; } = null!;
}