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
                    ClientName = "WEEE Web",
                    ClientId = "weee",
                    Enabled = true,
                    AccessTokenType = AccessTokenType.Reference,
                    Flow = Flows.ResourceOwner,
                    ClientSecrets = new List<ClientSecret>
                    {
                        new ClientSecret("C11A1534-554B-453D-B881-3FEAD3EEFEE9".Sha256())
                    }
                }
            };
        }
    }
}