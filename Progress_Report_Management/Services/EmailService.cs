using System.Net;
using System.Net.Mail;
using ProgressReportSystem.API.Models;
using Microsoft.Extensions.Configuration;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body, string? attachmentPath = null)
    {
        var smtpClient = new SmtpClient(_config["Email:SmtpHost"])
        {
            Port = int.Parse(_config["Email:SmtpPort"]),
            Credentials = new NetworkCredential(
        _config["Email:Username"],
        _config["Email:Password"]
    ),
            EnableSsl = true,
            UseDefaultCredentials = false
        };


        var mail = new MailMessage
        {
            From = new MailAddress(_config["Email:From"]),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        mail.To.Add(toEmail);

        // ✅ Attach file if present
        if (!string.IsNullOrEmpty(attachmentPath) && File.Exists(attachmentPath))
        {
            var attachment = new Attachment(attachmentPath);
            mail.Attachments.Add(attachment);
        }

        await smtpClient.SendMailAsync(mail);
    }

    // ✅ Send progress report email with attachment
    public async Task SendReportAvailableEmailAsync(User user, ProgressReport report)
    {
        var subject = "📄 Your Progress Report is Available";
        var body = $@"
            <p>Hello <b>{user.FullName}</b>,</p>
            <p>Your progress report for <b>{report.CourseProgram}</b> has been uploaded.</p>
            <p>Please find the report attached.</p>";

        await SendEmailAsync(user.Email, subject, body, report.FilePath);
    }
}
