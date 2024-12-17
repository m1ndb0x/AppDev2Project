using AppDev2Project.Models;

namespace AppDev2Project.Models.ViewModels
{
    public class StudentDashboardViewModelV2
    {
        public string Name { get; set; } = string.Empty;
        public string ProfilePictureUrl { get; set; } = string.Empty;
        public List<Exam> Exams { get; set; } = new List<Exam>();
        public List<CompletedExam> CompletedExams { get; set; } = new List<CompletedExam>();
        public IEnumerable<ExamProgress> ExamProgresses { get; set; }
    }
}
