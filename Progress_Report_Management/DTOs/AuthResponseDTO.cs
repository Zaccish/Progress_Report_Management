namespace ProgressReportSystem.API.DTOs;

public class AuthResponseDTO
{
    public string? Token { get; set; }
    public string? Role { get; set; }
    public string? FullName { get; set; }
}
