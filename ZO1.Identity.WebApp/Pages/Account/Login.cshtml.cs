using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using ZO1.Identity.WebApp.Cores.Models.Models;
using ZO1.Identity.WebApp.Cores.Models.ViewModels;

namespace ZO1.Identity.WebApp.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;

        public LoginModel(SignInManager<User> signInManager)
        {
            _signInManager = signInManager;
        }


        //TODO: Learn about Bind Property
        [BindProperty]
        public CredentialViewModel CredentialViewModel { get; set; }

        [BindProperty]
        public IEnumerable<AuthenticationScheme> ExternalLoginProviders { get; set; }

        public async Task OnGet()
        {
            ExternalLoginProviders = await _signInManager.GetExternalAuthenticationSchemesAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();


            var result = await _signInManager.PasswordSignInAsync(
                CredentialViewModel.Email.Split('@')[0],
                CredentialViewModel.Password,
                CredentialViewModel.RememberMe, false);

            if (result.Succeeded)
            {
                #region Create Authen with Claims

                // Creating the security context
                var claims = new List<Claim>
                {
                    new(ClaimTypes.Email, CredentialViewModel.Email),
                    new("Password", CredentialViewModel.Password),
                };

                var identity = new ClaimsIdentity(claims, "MyCookieAuth");
                var claimsPrincipal = new ClaimsPrincipal(identity);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = CredentialViewModel.RememberMe
                };

                await HttpContext.SignInAsync("MyCookieAuth",
                    claimsPrincipal, authProperties);

                #endregion

                return RedirectToPage("/Index");
            }

            // Xac thuc buoc 2
            if (result.RequiresTwoFactor)
            {
                return RedirectToPage("/Account/LoginTwoFactor",
                    new
                    {
                        RememberMe = CredentialViewModel.RememberMe
                    });
            }


            ModelState.AddModelError("Login", result.IsLockedOut ? "You're locked out." : "Failed to login.");
            return Page();
        }

        public IActionResult OnPostLoginExternally(string provider)
        {
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, null);
            properties.RedirectUri = Url.Action("ExternalLoginCallback", "Account");

            return Challenge(properties, provider);
        }
    }
}
