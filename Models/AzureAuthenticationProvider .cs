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
            string clientId = "76b04c1f-2db1-4c60-b7a5-4022369018ab";
            string clientSecret = "ykUxQNDCLWBsk41vv_09-N36o1i~Hpl3F-";

            string Instance = "https://login.microsoftonline.com/{0}";
            string authority = string.Format(CultureInfo.InvariantCulture, Instance, "ReferralSystem.onmicrosoft.com");

            AuthenticationContext authContext = new AuthenticationContext(authority);

            ClientCredential creds = new ClientCredential(clientId, clientSecret);

            AuthenticationResult authResult = await authContext.AcquireTokenAsync("https://graph.microsoft.com/", creds);

            request.Headers.Add("Authorization", "Bearer " + authResult.AccessToken);
        }
    }
}
