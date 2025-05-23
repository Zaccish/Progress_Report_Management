namespace ProgressReportAPI.Models
{

    public class User
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public int MATID { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; } // Admin, Student
    }
}
