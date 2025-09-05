namespace ProgressReportSystem.API.DTOs;
public class UploadReportDTO
{
    public string StudentName { get; set; } = string.Empty;
    public string MatricId { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string CourseProgram { get; set; } = string.Empty;
    public string Semester { get; set; } = string.Empty;  // ✅ Add this line

    public IFormFile File { get; set; } = null!;
}
