namespace EA.Weee.Integration.Tests.IoC
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using Api.Identity;
    using Api.Modules;
    using Api.Services;
    using Autofac;
    using Autofac.Core;
    using Core;
    using DataAccess.DataAccess;
    using DataAccess.Identity;
    using EA.Weee.DataAccess;
    using Microsoft.AspNet.Identity;
    using Microsoft.Owin.Security.DataProtection;
    using Prsd.Core.Autofac;
    using Prsd.Core.Domain;
    using RequestHandlers;
    using RequestHandlers.Charges.IssuePendingCharges;
    using IContainer = Autofac.IContainer;

    public class IoCFactory
    {
        private static readonly Dictionary<string, IContainer> ContainerCache = new Dictionary<string, IContainer>();

        public void RemoveContainer(EnvironmentResolver environment)
        {
            ContainerCache.Remove(GetKey(environment));
        }

        private string GetKey(EnvironmentResolver environment)
        {
            if (environment.IocApplication == IocApplication.Unknown || environment.HostEnvironment == HostEnvironmentType.Unknown)
            {
                throw new ArgumentException("Unknown environment or application", nameof(environment));
            }
                
            return $"{(int)environment.IocApplication}-{(int)environment.HostEnvironment}";
        }

        public bool IsContainerExisting(EnvironmentResolver environment)
        {
            var key = GetKey(environment);
            if (ContainerCache.ContainsKey(key))
            {
                var container = ContainerCache[key];

                try
                { // check if already installed
                    if (container.IsRegistered<IIbisFileDataGenerator>() &&
                        container.Resolve<IEnvironmentResolver>()?.IocApplication == environment.IocApplication &&
                        container.Resolve<IEnvironmentResolver>()?.HostEnvironment == environment.HostEnvironment)
                    {
                        return true;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            return false;
        }

        public IContainer OverrideContainerWithNew(EnvironmentResolver environment, TestUserContext testUserContext)
        {
            var key = GetKey(environment);
            if (ContainerCache.ContainsKey(key))
            {
                Console.WriteLine("Disposing old IoC");
                var oldContainer = ContainerCache[key];
                oldContainer?.Dispose();
            }

            Console.WriteLine("New IoC");
            var container = new ContainerBuilder().As(environment);

            Console.WriteLine($"Installing IoC...");
            if (testUserContext == null)
            {
                testUserContext = new TestUserContext(Guid.Empty, false);
            }

            switch (environment.IocApplication)
            {
                case IocApplication.RequestHandler:
                    container.RegisterModule(new CoreModule());
                    container.RegisterModule(new RequestHandlerModule());
                    container.RegisterModule(new AuthorizationModule());
                    container.RegisterModule(new EventDispatcherModule());
                    container.RegisterModule(new SecurityModule(environment));
                    container.RegisterModule(new EntityFrameworkModule(environment));
                    container.Register(context => testUserContext).As<IUserContext>();
                    container.Register<Action<TestUserContext>>(context => newInstance => testUserContext = newInstance);
                    container.RegisterModule(new MappingModule());
                    container.RegisterType<WeeeIdentityContext>().AsSelf().SingleInstance();
                    container.RegisterType<ApplicationUserManager>().AsSelf().SingleInstance();
                    container.RegisterType<ApplicationUserStore>().As<IUserStore<ApplicationUser>>();
                    container.RegisterType<SecurityEventDatabaseAuditor>()
                        .WithParameter(new NamedParameter("connectionString",
                            ConfigurationManager.ConnectionStrings["Weee.DefaultConnection"].ToString()))
                        .As<ISecurityEventAuditor>()
                        .SingleInstance();
                    container.RegisterType<TestDataProtectionProvider>().As<IDataProtectionProvider>();
                    container.RegisterType<ConfigurationService>().AsSelf().SingleInstance();
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(environment.IocApplication), environment.IocApplication, null);
            }

            Console.WriteLine($"Finished Installing IoC...");

            return ContainerCache[key] = container.Build();
        }

        public IContainer GetOrCreateContainer(EnvironmentResolver environment, TestUserContext userContext = null)
        {
            return IsContainerExisting(environment) ? ContainerCache[GetKey(environment)] : OverrideContainerWithNew(environment, userContext);
        }
    }
}
