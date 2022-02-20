using Microsoft.AspNetCore.Identity;

namespace ZO1.Identity.WebApp.Cores.Models.Models
{
    public class User : IdentityUser
    {
        public string Image { get; set; }
        public string Position { get; set; }
        public string Department { get; set; }
    }
}