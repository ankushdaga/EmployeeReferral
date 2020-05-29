

using System.Linq;
using Microsoft.AspNetCore.Http;

namespace ReferralSystem.Service
{
   
    public static class AzureAdClaimTypes
    {
        public const string ObjectId = "http://schemas.microsoft.com/identity/claims/objectidentifier";

        public const string Scope = "http://schemas.microsoft.com/identity/claims/scope";
    }

    public class AzureAdIdentityService 
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AzureAdIdentityService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public bool IsAuthenticated()
        {
            return _httpContextAccessor.HttpContext.User.Identity.IsAuthenticated;
        }

        public string GetMail()
        {
            return _httpContextAccessor.HttpContext.User.Identity.Name;
        }

        public string GetId()
        {
            var idClaims = _httpContextAccessor.HttpContext.User.Claims
                .FirstOrDefault(c => c.Type == AzureAdClaimTypes.ObjectId);

            return idClaims?.Value;
        }
    }
}
