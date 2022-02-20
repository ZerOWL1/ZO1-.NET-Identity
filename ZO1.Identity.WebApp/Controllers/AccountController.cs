using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using ZO1.Identity.WebApp.Cores.Models.Models;

namespace ZO1.Identity.WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly SignInManager<User> _signInManager;

        public AccountController(SignInManager<User> signInManager)
        {
            _signInManager = signInManager;
        }

        public async Task<IActionResult> ExternalLoginCallback()
        {
            var loginInfo = await _signInManager.GetExternalLoginInfoAsync();
            var emailClaim = loginInfo.Principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
            var userNameClaim =  loginInfo.Principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);

            if (emailClaim == null || userNameClaim == null) return RedirectToPage("/Index");

            var user = new User
            {
                Email = emailClaim.Value,
                UserName = userNameClaim.Value
            };

            await _signInManager.SignInAsync(user, false);

            return RedirectToPage("/Index");
        }
    }
}
