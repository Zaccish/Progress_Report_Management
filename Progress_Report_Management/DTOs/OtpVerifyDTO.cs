namespace ProgressReportSystem.API.DTOs
{
    public class OtpVerifyDTO
    {
        public string Email { get; set; } = null!;
        public string Otp { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string MatricId { get; set; } = null!;
    }

}
