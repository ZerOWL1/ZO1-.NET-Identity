using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ZO1.Identity.WebApp.Commons;
using ZO1.Identity.WebApp.Cores.Models.Models;
using ZO1.Identity.WebApp.Cores.Models.ViewModels;

namespace ZO1.Identity.WebApp.Pages.Account
{
    /// <summary>
    /// This class use to Authenticator with MFA setup
    /// </summary>
    [Authorize]
    public class AuthenticatorModel : PageModel
    {
        private readonly UserManager<User> _userManager;

        [BindProperty]
        public SetupMfaViewModel SetupMfaViewModel { get; set; }

        [BindProperty] 
        public bool Succeeded { get; set; }

        [BindProperty]
        public bool Failed { get; set; }

        public AuthenticatorModel(UserManager<User> userManager)
        {
            _userManager = userManager;
            SetupMfaViewModel = new SetupMfaViewModel();
            Succeeded = false;
            Failed = false;
        }


        public async Task<IActionResult> OnGetAsync()
        {
            var user =  await _userManager.GetUserAsync(User);

            if (user == null) return BadRequest();

            var isFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user);

            if (isFactorEnabled)
            {
                Failed = true;
                return Page();
            }

            await _userManager.ResetAuthenticatorKeyAsync(user);

            // Get mobile app authenticator key
            var securityKey = await _userManager.GetAuthenticatorKeyAsync(user);
            SetupMfaViewModel.Key = securityKey;
            SetupMfaViewModel.QrCodeBytes = QrCodeHelpers.GenerateQRCodeBytes(
                "Uta95s Identity", securityKey, user.Email);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var user = await _userManager.GetUserAsync(User);
            var isTwoFactorToken = await _userManager.VerifyTwoFactorTokenAsync(
                user,
                _userManager.Options.Tokens.AuthenticatorTokenProvider,
                SetupMfaViewModel.SecurityCode
            );

            if (isTwoFactorToken)
            {
                await _userManager.SetTwoFactorEnabledAsync(user, true);
                Succeeded = true;
            }
            else
            {
                ModelState.AddModelError("AuthenticatorSetup",
                    "Something went wrong with authenticator setup.");
            }

            return Page();
        }
    }
}
