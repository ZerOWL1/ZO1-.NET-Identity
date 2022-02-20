using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using ZO1.Identity.WebApp.Cores.Models.Models;
using ZO1.Identity.WebApp.Cores.Models.ViewModels;
using ZO1.Identity.WebApp.Services;

namespace ZO1.Identity.WebApp.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly IEmailServices _emailServices;

        public RegisterModel(UserManager<User> userManager, IEmailServices emailServices)
        {
            _userManager = userManager;
            _emailServices = emailServices;
        }

        [BindProperty]
        public RegisterViewModel RegisterViewModel { get; set; }

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            // Validating Email address (Optional)

            // Create the user
            var user = new User
            {
                Email = RegisterViewModel.Email,
                UserName = RegisterViewModel.Email.Split('@')[0],
                Position = RegisterViewModel.Position,
                Department = RegisterViewModel.Department
            };


            // Create Claims
            var claims = new List<Claim>{
                new("Department", RegisterViewModel.Department),
                new("Position", RegisterViewModel.Position)
            };


            var createResult = await _userManager.CreateAsync(user,
                RegisterViewModel.Password);

            if (createResult.Succeeded)
            {
                // Add Claims here
                await _userManager.AddClaimsAsync(user, claims);

                // Create token and send to email
                // token
                var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                // link
                var linkConfirmation = Url.PageLink(pageName: "/Account/ConfirmEmail", values: new
                {
                    userId = user.Id,
                    token = emailConfirmationToken
                });

                await _emailServices.SendAsync("uta95s.verify@gmail.com", user.Email,
                     "UTA95S TEAM - Confirmation Email",
                     $"Please click on this link to confirm your email address. {linkConfirmation}");

                return RedirectToPage("/Account/Login");
            }

            foreach (var error in createResult.Errors)
            {
                ModelState.AddModelError("Register", error.Description);
            }

            return Page();
        }

    }
}
