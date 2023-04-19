namespace EA.Weee.Api.IdSrv
{
    using IdentityServer3.Core.Models;
    using Services;
    using System.Collections.Generic;
    using System.Configuration;
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
                new Client
                {
                    ClientName = "WEEE Web",
                    ClientId = clientId,
                    Enabled = true,
                    AccessTokenType = AccessTokenType.Reference,
                    AllowAccessToAllCustomGrantTypes = true,
                    Flow = Flows.ResourceOwner,
                    ClientSecrets = new List<Secret>
                    {
                        new Secret(config.ApiSecret.Sha256())
                    },
                    AccessTokenLifetime = 10805, // 3 hour,
                    AllowAccessToAllScopes = true,
                    //AllowClientCredentialsOnly = true,
                    AllowedCustomGrantTypes = new List<string> { "refresh_token" },
                    //RefreshTokenUsage = TokenUsage.ReUse,
                    RefreshTokenExpiration = TokenExpiration.Sliding,
                },
                new Client
                {
                    ClientName = "WEEE Web Unauthenticated",
                    ClientId = ConfigurationManager.AppSettings["Weee.ApiClientCredentialId"],
                    Enabled = true,
                    AccessTokenType = AccessTokenType.Reference,
                    Flow = Flows.ClientCredentials,
                    ClientSecrets = new List<Secret>
                    {
                        new Secret(ConfigurationManager.AppSettings["Weee.ApiClientCredentialSecret"].Sha256())
                    },
                    AccessTokenLifetime = 10805, // 3 hour,
                    AllowAccessToAllScopes = false,
                    AllowedScopes =
                    {
                        "api2",
                        "api1"
                    }
                }
            };
        }
    }
}