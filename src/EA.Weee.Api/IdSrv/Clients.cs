namespace EA.Weee.Api.IdSrv
{
    using System.Collections.Generic;
    using System.Configuration;
    using IdentityServer3.Core.Models;
    using Services;
    using Client = IdentityServer3.Core.Models.Client;
    internal static class Clients
    {
        public static List<Client> Get(AppConfiguration config)
        {
            string clientId = ConfigurationManager.AppSettings["Weee.ApiClientID"];
            string clientSecret = ConfigurationManager.AppSettings["Weee.ApiSecret"];

            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
            {
                throw new ConfigurationErrorsException("API client configuration must be provided.");
            }

            return new List<Client>
            {
                new IdentityServer3.Core.Models.Client
                {
                    ClientName = "WEEE Web",
                    ClientId = clientId,
                    Enabled = true,
                    AccessTokenType = AccessTokenType.Reference,
                    Flow = Flows.ResourceOwner,
                    ClientSecrets = new List<Secret>
                    {
                        new Secret(config.ApiSecret.Sha256())
                    },
                    AccessTokenLifetime = 3600, // 1 hour
                    AllowAccessToAllScopes = true
                }
            };
        }
    }
}