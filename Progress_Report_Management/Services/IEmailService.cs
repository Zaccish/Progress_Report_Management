using ProgressReportSystem.API.Models;

public interface IEmailService
{
    Task SendEmailAsync(string toEmail, string subject, string body, string? attachmentPath = null);
    Task SendReportAvailableEmailAsync(User studentUser, ProgressReport report);
}
