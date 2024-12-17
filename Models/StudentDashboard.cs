namespace AppDev2Project.Models
{
    public class StudentDashboard
    {
        public string WelcomeMessage { get; set; } = string.Empty;
        public List<string> RecentExams { get; set; } = new List<string>();
        public string Name { get; set; } = string.Empty;

        public string ProfilePictureUrl
        {
            get => string.IsNullOrEmpty(_profilePictureUrl)
                ? GenerateDefaultPfp(Name) // Generate initials-based image URL
                : _profilePictureUrl;
            set => _profilePictureUrl = value;
        }

        private string? _profilePictureUrl;

        // Default Profile Picture Generator
        private string GenerateDefaultPfp(string name)
        {
            var initials = string.Join("", name.Split(' ').Select(w => w[0])).ToUpper();
            return $"https://ui-avatars.com/api/?name={initials}&background=random&color=fff";
        }
    }
}
