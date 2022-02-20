using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using ZO1.Identity.WebApp.Cores.Models.Models;
using ZO1.Identity.WebApp.Cores.Models.ViewModels;
using ZO1.Identity.WebApp.Services;

namespace ZO1.Identity.WebApp.Pages.Account
{
    public class LoginWithEmailAuthenticatorModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly IEmailServices _emailServices;
        private readonly SignInManager<User> _signInManager;

        [BindProperty]
        public EmailMfaViewModel EmailMfaViewModel { get; set; }


        public LoginWithEmailAuthenticatorModel(
            UserManager<User> userManager,
            IEmailServices emailService,
            SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _emailServices = emailService;
            _signInManager = signInManager;
            EmailMfaViewModel = new EmailMfaViewModel();
        }
        public async Task<IActionResult> OnGetAsync(string email, bool rememberMe)
        {
            // Get user
            var user = await _userManager.FindByEmailAsync(email);

            EmailMfaViewModel.SecurityCode = string.Empty;
            EmailMfaViewModel.RememberMe = rememberMe;

            // Generate Security Code
            var securityCode = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");

            // Send token to the user
            await _emailServices.SendAsync("uta95s.verify@gmail.com", email,
                "UTA95S TEAM - OTP Security Code",
                $"Please use this code as the OTP to confirm MFA: {securityCode}"
            );

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var result = await _signInManager.TwoFactorSignInAsync("Email",
                EmailMfaViewModel.SecurityCode,
                EmailMfaViewModel.RememberMe,
                false);

            if (result.Succeeded)
            {
                return RedirectToPage("/Index");
            }

            ModelState.AddModelError("Login2FA", result.IsLockedOut ?
                "You're locked out." : "Failed to login.");
            return Page();
        }
    }
}
