using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ReferralSystem.Controllers
{
    public class AccountController : Controller
    {
        public async Task Login(string returnUrl = "/")
        {
            await HttpContext.ChallengeAsync("Auth0", new AuthenticationProperties() { RedirectUri = returnUrl });

            var authorization = this.Request.Headers["Authorization"].ToString();
            // var tokenstring = authorization.Substring("Bearer ".Length).Trim();
            var handler = new JwtSecurityTokenHandler();
            // var token = handler.ReadJwtToken(tokenstring);

            //string userId = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            //var abc = new UserProfileViewModel()
            //{
            //    Name = User.Identity.Name,
            //    EmailAddress = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value
            //    //ProfileImage = User.Claims.FirstOrDefault(c => c.Type == "picture")?.Value
            //};

            var ss = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        }

        [Authorize]
        public async Task Logout()
        {
            await HttpContext.SignOutAsync("Auth0", new AuthenticationProperties
            {
                // Indicate here where Auth0 should redirect the user after a logout.
                // Note that the resulting absolute Uri must be whitelisted in the 
                // **Allowed Logout URLs** settings for the client.
                RedirectUri = Url.Action("Index", "Home")
            });
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}