namespace EA.Weee.Api.IdSrv
{
    using System.Collections.Generic;
    using Thinktecture.IdentityServer.Core.Models;

    internal static class Clients
    {
        public static List<Client> Get()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientName = "IWS Web",
                    ClientId = "iws",
                    Enabled = true,
                    AccessTokenType = AccessTokenType.Reference,
                    Flow = Flows.ResourceOwner,
                    ClientSecrets = new List<ClientSecret>
                    {
                        new ClientSecret("4945361C-A026-4B28-B077-9DA715238C23".Sha256())
                    }
                }
            };
        }
    }
}