using ProgressReportSystem.API.DTOs;
using ProgressReportSystem.API.Models;

namespace ProgressReportSystem.API.Services;

public interface IReportService
{
    Task<(bool Success, string Message)> UploadReportAsync(UploadReportDTO dto, int adminId);
    Task<IEnumerable<ActivityLog>> GetActivityLogsByMatricIdAsync(string matricId);
    Task<IEnumerable<ProgressReport>> SearchReportsAsync(string? name, string? matricId);
    Task<ProgressReport?> DownloadReportAsync(DownloadRequest request);
    Task<IEnumerable<ReportRequest>> GetPendingReportRequestsAsync();

}
