namespace EA.Weee.Api.IdSrv
{
    using EA.Weee.Api.Identity;
    using IdentityServer3.Core.Configuration;
    using IdentityServer3.Core.Services;
    using IdentityServer3.Core.Services.InMemory;
    using IdentityServer3.EntityFramework;
    using Services;

    internal static class Factory
    {
        public static IdentityServerServiceFactory Configure(AppConfiguration config)
        {
            var factory = new IdentityServerServiceFactory();

            var scopeStore = new InMemoryScopeStore(Scopes.Get());
            factory.ScopeStore = new Registration<IScopeStore>(scopeStore);
            var clientStore = new InMemoryClientStore(Clients.Get(config));
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