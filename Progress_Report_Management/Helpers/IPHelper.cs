namespace ProgressReportSystem.API.Helpers
{
    public static class IPHelper
    {
        public static string GetClientIp(HttpContext context)
        {
            if (context.Request.Headers.ContainsKey("X-Forwarded-For"))
                return context.Request.Headers["X-Forwarded-For"];


            return context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        }
    }
}