namespace AppDev2Project.Models
{
    public class StudentDashboard
    {
        public string WelcomeMessage { get; set; } = string.Empty;
        public List<string> RecentExams { get; set; } = new List<string>();
    }
}
