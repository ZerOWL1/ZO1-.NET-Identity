using System.ComponentModel.DataAnnotations;

namespace ZO1.Identity.WebApp.Cores.Models.ViewModels
{
    public class SetupMfaViewModel
    {
        public string Key { get; set; }

        [Required]
        [Display(Name = "Security Code")]
        public string SecurityCode { get; set; }
        public byte[] QrCodeBytes { get; set; }
    }
}