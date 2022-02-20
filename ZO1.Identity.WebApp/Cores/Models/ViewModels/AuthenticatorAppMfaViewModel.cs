using System.ComponentModel.DataAnnotations;

namespace ZO1.Identity.WebApp.Cores.Models.ViewModels
{
    public class AuthenticatorAppMfaViewModel
    {
        [Required]
        [Display(Name = "Security Code")]
        public string SecurityCode { get; set; }

        public bool RememberMe { get; set; }

    }
}