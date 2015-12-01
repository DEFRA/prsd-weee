namespace EA.Weee.Api.IdSrv
{
    using DataAccess.Identity;
    using EA.Prsd.Core.Domain;
    using EA.Weee.DataAccess;
    using Identity;
    using Microsoft.AspNet.Identity;
    using Microsoft.Owin.Security.DataProtection;
    using Owin;
    using Services;
    using Thinktecture.IdentityServer.Core.Configuration;
    using Thinktecture.IdentityServer.Core.Services;

    public static class UserServiceExtensions
    {
        public static void ConfigureUserService(this IdentityServerServiceFactory factory, IAppBuilder app)
        {
            factory.UserService = new Registration<IUserService, UserService>
            {
                Mode = RegistrationMode.InstancePerHttpRequest
            };
            factory.Register(new Registration<ApplicationUserManager> { Mode = RegistrationMode.InstancePerHttpRequest });
            factory.Register(new Registration<IUserStore<ApplicationUser>, ApplicationUserStore>
            {
                Mode = RegistrationMode.InstancePerHttpRequest
            });
            factory.Register(new Registration<WeeeIdentityContext> { Mode = RegistrationMode.InstancePerHttpRequest });
            factory.Register(new Registration<IDataProtectionProvider>(f => app.GetDataProtectionProvider())
            {
                Mode = RegistrationMode.InstancePerHttpRequest
            });
            factory.Register(new Registration<ConfigurationService> { Mode = RegistrationMode.InstancePerHttpRequest });
            factory.Register(new Registration<IUserContext, UserContext> { Mode = RegistrationMode.InstancePerHttpRequest });
            factory.Register(new Registration<WeeeContext> { Mode = RegistrationMode.InstancePerHttpRequest });
            factory.Register(new Registration<IEventDispatcher, NullEventDispatcher>() { Mode = RegistrationMode.Singleton });
        }
    }
}