using Microsoft.EntityFrameworkCore;
using ProgressReportSystem.API.Models;

namespace ProgressReportSystem.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<ActivityLog> ActivityLogs => Set<ActivityLog>();
    public DbSet<ProgressReport> ProgressReports => Set<ProgressReport>();
    public DbSet<ReportRequest> ReportRequests { get; set; }
    public DbSet<OneTimePassword> Otps { get; set; }
    public DbSet<FailedLoginAttempt> FailedLoginAttempts { get; set; }
}

