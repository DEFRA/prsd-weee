namespace EA.Weee.Api.IdSrv
{
    using System.Collections.Generic;
    using System.Configuration;
    using Thinktecture.IdentityServer.Core.Models;

    internal static class Clients
    {
        public static List<Client> Get()
        {
            string clientId = ConfigurationManager.AppSettings["Weee.ApiClientID"];
            string clientSecret = ConfigurationManager.AppSettings["Weee.ApiSecret"];

            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
            {
                throw new ConfigurationErrorsException("API client configuration must be provided.");
            }

            return new List<Client>
            {
                new Client
                {
                    ClientName = "WEEE Web",
                    ClientId = clientId,
                    Enabled = true,
                    AccessTokenType = AccessTokenType.Reference,
                    Flow = Flows.ResourceOwner,
                    ClientSecrets = new List<ClientSecret>
                    {
                        new ClientSecret(clientSecret.Sha256())
                    }
                }
            };
        }
    }
}