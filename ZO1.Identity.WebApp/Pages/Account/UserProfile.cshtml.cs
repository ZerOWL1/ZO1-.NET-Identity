using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ZO1.Identity.WebApp.Cores.Models.Models;
using ZO1.Identity.WebApp.Cores.Models.ViewModels;

namespace ZO1.Identity.WebApp.Pages.Account
{
    // This class can access by any user
    [Authorize]
    public class UserProfileModel : PageModel
    {
        private readonly UserManager<User> _userManager;

        [BindProperty]
        public UserProfileViewModel UserProfileViewModel { get; set; }

        [BindProperty]
        public string SuccessMessage { get; set; }

        public UserProfileModel(UserManager<User> userManager)
        {
            _userManager = userManager;
            UserProfileViewModel = new UserProfileViewModel();
            SuccessMessage = string.Empty;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            if (User.Identity == null) return RedirectToPage("/Account/Login");
            var (user, department, position) = await GetUserInformationAsync();
           

            UserProfileViewModel.Email = user.Email;
            UserProfileViewModel.Department = department?.Value;
            UserProfileViewModel.Position = position?.Value;


            return Page();
        }


        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var (user, department, position) = await GetUserInformationAsync();

            // change user claims
            try
            {
                await _userManager.ReplaceClaimAsync(user, department,
                    new Claim(department.Type, UserProfileViewModel.Department));
                await _userManager.ReplaceClaimAsync(user, position,
                    new Claim(position.Type, UserProfileViewModel.Position));
            }
            catch (Exception e)
            {
               ModelState.AddModelError("UserProfile", 
                   $"Error occurred when saving user profile [${e.Message}]");
            }

            // Every time this page get save success!
            SuccessMessage = "The user profile is saved successfully";
            return Page();
        }

        private async Task<(User, Claim, Claim)> GetUserInformationAsync()
        {
            var user = await _userManager.FindByNameAsync(User.Identity?.Name);
            var claims = await _userManager.GetClaimsAsync(user);
            var departmentClaim = claims.FirstOrDefault(c => c.Type == "Department");
            var positionClaim = claims.FirstOrDefault(c => c.Type == "Position");

            return (user, departmentClaim, positionClaim);
        }
    }
}
