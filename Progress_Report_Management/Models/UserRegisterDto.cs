namespace Progress_Report_ManagementAPI.Models
{
    public class UserRegisterDto
    {
        public string Username { get; set; }
        public int Matid { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
    }
}
