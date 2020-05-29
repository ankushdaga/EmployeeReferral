using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ReferralSystem.Models;
using ReferralSystem.Repository;
using Microsoft.AspNetCore.Mvc;

namespace ReferralSystem.Extensions
{
    public class CustomMiddleware : ControllerBase
    {
        private readonly RequestDelegate _next;
      //  private readonly IMongoRepository<UserModel> _user;

        public CustomMiddleware(RequestDelegate next)
        {
            _next = next;
            //_user = userrRepository;
        }

        public async Task Invoke(HttpContext context, IMongoRepository<UserModel> userrRepository)
        {
            var aa = "ankush.daga@cappita.co.uk";
            string token = context.Request.Headers["Token"];
            var usera = userrRepository.Get().FirstOrDefault(x => x.EmailId==aa)?.RoleName;
            ClaimsIdentity claimsIdentity = new ClaimsIdentity("Custom");
            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, "admin"));
            claims.Add(new Claim(ClaimTypes.NameIdentifier, "admin"));
            claims.Add(new Claim(ClaimTypes.NameIdentifier, "admin"));


            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            context.User = claimsPrincipal;
            await _next(context);
        }
    }
}
