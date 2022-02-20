using System.ComponentModel.DataAnnotations;

namespace ZO1.Identity.WebApp.Cores.Models.ViewModels
{
    public class UserProfileViewModel
    {
        public string Email { get; set; }
        
        [Required]
        public string Department { get; set; }

        [Required]
        public string Position { get; set; }
    }
}