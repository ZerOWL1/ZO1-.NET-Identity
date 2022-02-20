using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ZO1.Identity.WebApp.Cores.Models.Models;
using ZO1.Identity.WebApp.Cores.Models.ViewModels;

namespace ZO1.Identity.WebApp.Pages.Account
{

    public class LoginTwoFactorModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;

        [BindProperty]
        public AuthenticatorAppMfaViewModel AuthenticatorAppMfaViewModel { get; set; }

        public LoginTwoFactorModel(SignInManager<User> signInManager)
        {
            _signInManager = signInManager;
            AuthenticatorAppMfaViewModel = new AuthenticatorAppMfaViewModel();
        }

        public void OnGet(bool rememberMe)
        {
            AuthenticatorAppMfaViewModel.SecurityCode = string.Empty;
            AuthenticatorAppMfaViewModel.RememberMe = rememberMe;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(
                AuthenticatorAppMfaViewModel.SecurityCode,
                AuthenticatorAppMfaViewModel.RememberMe,
                false);

            if (result.Succeeded)
            {
                return RedirectToPage("/Index");
            }

            ModelState.AddModelError("Authenticator2FA",
                result.IsLockedOut ? "You are locked out." : "Failed to login");

            return Page();
        }
    }
}
