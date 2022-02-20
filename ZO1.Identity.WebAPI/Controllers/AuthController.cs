using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ZO1.Identity.WebAPI.Models;

namespace ZO1.Identity.WebAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public IActionResult Authenticate([FromBody] Credential credential)
        {
            if (credential.Username == "admin" && credential.Password == "123478@Kid")
            {
                // Creating the security context
                var claims = new List<Claim>
                {
                    new(ClaimTypes.Name, credential.Username),
                    new("Password", credential.Password),
                    new("Department", "HR"),
                    new("Admin", "true"),
                    new("Manager", "true"),
                    new("EmploymentDate", DateTime.Now.AddMonths(-5).ToString("yyyy-MM-dd"))
                };

                var expiresAt = DateTime.UtcNow.AddMinutes(10);
                return Ok(new
                {
                    access_token = CreateToken(claims, expiresAt),
                    expires_at = expiresAt
                });
            }

            ModelState.AddModelError("Unauthorized", "You don't have permission to access this endpoint.");
            return Unauthorized(ModelState);
        }

        private string CreateToken(IEnumerable<Claim> claims, DateTime expireAt)
        {
            var secretKey = Encoding.ASCII.GetBytes(_configuration.GetValue<string>("SecretKey"));

            var jwt = new JwtSecurityToken(
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: expireAt, 
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(secretKey), 
                    SecurityAlgorithms.HmacSha256Signature));

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }
}
