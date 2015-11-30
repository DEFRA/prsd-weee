namespace EA.Weee.Api.IdSrv
{
    using EA.Weee.Api.Identity;
    using Thinktecture.IdentityServer.Core.Configuration;
    using Thinktecture.IdentityServer.Core.Services;
    using Thinktecture.IdentityServer.Core.Services.InMemory;
    using Thinktecture.IdentityServer.EntityFramework;

    internal static class Factory
    {
        public static IdentityServerServiceFactory Configure()
        {
            var factory = new IdentityServerServiceFactory();

            var scopeStore = new InMemoryScopeStore(Scopes.Get());
            factory.ScopeStore = new Registration<IScopeStore>(scopeStore);
            var clientStore = new InMemoryClientStore(Clients.Get());
            factory.ClientStore = new Registration<IClientStore>(clientStore);

            var efConfig = new EntityFrameworkServiceOptions
            {
                ConnectionString = "Weee.DefaultConnection",
                Schema = "Identity"
            };

            factory.RegisterOperationalServices(efConfig);

            var cleanup = new TokenCleanup(efConfig);
            cleanup.Start();

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Weee.DefaultConnection"].ConnectionString;
            var auditSecurityEventService = new SecurityEventDatabaseAuditor(connectionString);
            SecurityEventService eventService = new SecurityEventService(auditSecurityEventService);

            factory.Register<ISecurityEventAuditor>(new Registration<ISecurityEventAuditor>(auditSecurityEventService));
            factory.EventService = new Registration<IEventService>(eventService);

            return factory;
        }
    }
}