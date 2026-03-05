using System.ComponentModel.DataAnnotations;

namespace LinkwellProductionSystem.ViewModels
{
    public class Profile
    {
        [Display(Name = "User name")]
        [Required(ErrorMessage = "User name is required.")]
        [StringLength(50, ErrorMessage = "User name must be 50 characters or fewer.")]
        public string UserName { get; set; }

        [Display(Name = "Email address")]
        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address (example: name@example.com).")]
        public string Email { get; set; }

        public string ProfileImagePath { get; set; }

        [Display(Name = "Role")]
        public string Role { get; set; }
    }
}
