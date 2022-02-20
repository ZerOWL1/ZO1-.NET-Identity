using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using ZO1.Identity.WebApp.Cores.Models.Models;

namespace ZO1.Identity.WebApp.Pages.Account
{
    public class ConfirmEmailModel : PageModel
    {
        private readonly UserManager<User> _userManager;


        [BindProperty]
        public string Message { get; set; }

        public ConfirmEmailModel(UserManager<User> userManager)
        {
            _userManager = userManager;
        }
        public async Task<IActionResult> OnGetAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    Message = "Confirm email address is successfully, now you can login with your account";
                    return Page();
                }
            }

            Message = "Failed to validate email.";
            return Page();
        }
    }
}
