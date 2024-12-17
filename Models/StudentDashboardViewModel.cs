namespace AppDev2Project.Models.ViewModels
{
    public class StudentDashboardViewModel
    {
        public IEnumerable<Exam> AvailableExams { get; set; } = new List<Exam>();
        public IEnumerable<CompletedExam> CompletedExams { get; set; } = new List<CompletedExam>();
        public User Student { get; set; } = null!;
    }
}
