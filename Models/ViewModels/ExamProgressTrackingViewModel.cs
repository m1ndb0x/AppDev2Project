using AppDev2Project.Models;

namespace AppDev2Project.Models.ViewModels
{
    public class ExamProgressTrackingViewModel
    {
        public Exam Exam { get; set; }
        public Dictionary<int, StudentProgressInfo> StudentProgress { get; set; }
    }

    public class StudentProgressInfo
    {
        public double TimeRemaining { get; set; }
        public DateTime LastSaved { get; set; }
        public int CompletedQuestions { get; set; }
    }
}
