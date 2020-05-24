using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Graph;
using Microsoft.IdentityModel.Clients.ActiveDirectory;


namespace ReferralSystem.Models
{
    public class AzureAuthenticationProvider : IAuthenticationProvider
    {

        private string clientId = "";
        private string appKey = "";
        private string aadInstance = "";

        public async Task AuthenticateRequestAsync(HttpRequestMessage request)
        {
            string clientId = "";
            string clientSecret = "";

            string Instance = "https://login.microsoftonline.com/{0}";
            string authority = string.Format(CultureInfo.InvariantCulture, Instance, "");

            AuthenticationContext authContext = new AuthenticationContext(authority);

            ClientCredential creds = new ClientCredential(clientId, clientSecret);

            AuthenticationResult authResult = await authContext.AcquireTokenAsync("https://graph.microsoft.com/", creds);

            request.Headers.Add("Authorization", "Bearer " + authResult.AccessToken);
        }
    }
}
