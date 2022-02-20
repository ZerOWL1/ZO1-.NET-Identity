using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using ZO1.Identity.WebApp.Cores.Models.Models;

namespace ZO1.Identity.WebApp.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;

        public LogoutModel(SignInManager<User> signInManager)
        {
            _signInManager = signInManager;
        }

        public void OnGet()
        {
        }


        public async Task<IActionResult> OnPostAsync()
        {
            await _signInManager.SignOutAsync();

            await HttpContext.SignOutAsync("MyCookieAuth");

            return RedirectToPage("/Account/Login");
        }
    }
}
