using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProgressReportSystem.API.Data;
using System;


public class OtpCleanupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(30); // run every 30 mins

        public OtpCleanupService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var expiredOtps = await db.Otps
                    .Where(o => o.Expiry < DateTime.UtcNow)
                    .ToListAsync();

                if (expiredOtps.Any())
                {
                    db.Otps.RemoveRange(expiredOtps);
                    await db.SaveChangesAsync();
                }
            }

            await Task.Delay(_interval, stoppingToken);
        }
    }
}