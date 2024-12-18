namespace AppDev2Project.Helpers
{
    public static class PfpHelper
    {
        // Generate default profile picture URL with initials
        public static string GenerateDefaultPfp(string name)
        {
            if (string.IsNullOrEmpty(name))
                return "https://ui-avatars.com/api/?name=User&background=random&color=fff";

            var initials = string.Join("", name.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                                               .Select(word => word[0]))
                                  .ToUpper();

            return $"https://ui-avatars.com/api/?name={initials}&background=random&color=fff";
        }
    }
}
