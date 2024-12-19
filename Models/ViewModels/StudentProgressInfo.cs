namespace AppDev2Project.Models.ViewModels
{
    public class StudentProgressInfo
    {
        public double TimeRemaining { get; set; }  // Store as minutes
        public DateTime LastSaved { get; set; }
        public int CompletedQuestions { get; set; }
        public DateTime StartedAt { get; set; }
        public int Duration { get; set; }

        public string GetFormattedTimeRemaining()
        {
            var timeRemaining = Math.Max(0, TimeRemaining);
            var hours = Math.Floor(timeRemaining / 60);
            var minutes = Math.Floor(timeRemaining % 60);
            
            if (hours > 0)
                return $"{hours:0}h {minutes:0}m";
            return $"{minutes:0}m";
        }
    }
}
