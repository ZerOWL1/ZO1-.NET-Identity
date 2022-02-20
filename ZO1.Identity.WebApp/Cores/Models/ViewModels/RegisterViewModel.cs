using System.ComponentModel.DataAnnotations;

namespace ZO1.Identity.WebApp.Cores.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }


        [Required] 
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        public string Department { get; set; }
        
        [Required]
        public string Position { get; set; }

    }
}