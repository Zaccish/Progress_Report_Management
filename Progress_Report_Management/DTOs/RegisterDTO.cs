namespace ProgressReportSystem.API.DTOs;

public class RegisterDTO
{
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public required string MatricId { get; set; }
    public required string Password { get; set; }
    public required string Role { get; set; }
}
