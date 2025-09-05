namespace ProgressReportSystem.API.Models;
public class ProgressReport
{
    public int Id { get; set; }
    public string FileName { get; set; } = null!;
    public string FilePath { get; set; } = null!;
    public string Department { get; set; } = null!;
    public string CourseProgram { get; set; } = null!;
    public string StudentName { get; set; } = null!;
    public string MatricId { get; set; } = null!;
    public DateTime UploadedAt { get; set; }
    public int UploadedById { get; set; }
    public User UploadedBy { get; set; } = null!;
}