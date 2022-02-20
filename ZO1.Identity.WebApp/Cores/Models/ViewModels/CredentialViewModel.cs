using System.ComponentModel.DataAnnotations;

namespace ZO1.Identity.WebApp.Cores.Models.ViewModels
{
    public class CredentialViewModel
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        
        [Display(Name = "Remember Me")] 
        public bool RememberMe { get; set; }
    }
}