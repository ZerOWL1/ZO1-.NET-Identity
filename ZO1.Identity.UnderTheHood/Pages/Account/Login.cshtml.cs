using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using ZO1.Identity.UnderTheHood.Models;

namespace ZO1.Identity.UnderTheHood.Pages.Account
{
    public class LoginModel : PageModel
    {
        //TODO: Learn about Bind Property
        [BindProperty]
        public Credential Credential { get; set; }


        public void OnGet()
        {
        }


        public async Task<IActionResult> OnPostAsync()
        {
            if(!ModelState.IsValid)
                return Page();

            // Verify the credential
            if (Credential.UserName == "admin" && Credential.Password == "123478@Kid")
            {
                // Creating the security context
                var claims = new List<Claim>
                {
                    new(ClaimTypes.Name, Credential.UserName),
                    new("Password", Credential.Password),
                    new("Department", "HR"),
                    new("Admin", "true"),
                    new("Manager", "true"),
                    new("EmploymentDate", DateTime.Now.AddMonths(-5).ToString("yyyy-MM-dd"))
                };

                var identity = new ClaimsIdentity(claims, "MyCookieAuth");
                var claimsPrincipal = new ClaimsPrincipal(identity);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = Credential.RememberMe
                };

                await HttpContext.SignInAsync("MyCookieAuth",
                    claimsPrincipal, authProperties);
                return RedirectToPage("/Index");
            }

            return Page();
        }
    }
}
