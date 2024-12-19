// using System.ComponentModel.DataAnnotations;

// namespace AppDev2Project.Models
// {
//     public class SettingsViewModel
//     {
//         public string Name { get; set; }
//         public string Email { get; set; }

//         [DataType(DataType.Password)]
//         public string CurrentPassword { get; set; }

//         [DataType(DataType.Password)]
//         public string NewPassword { get; set; }

//         [DataType(DataType.Password)]
//         [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
//         public string ConfirmPassword { get; set; }
//        public string ProfilePictureUrl { get; set; }

//         [DataType(DataType.Upload)]
//         public IFormFile? ProfilePictureFile { get; set; }
//     }
// }
