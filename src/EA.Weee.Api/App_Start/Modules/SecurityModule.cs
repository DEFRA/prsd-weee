namespace EA.Weee.Api.Modules
{
    using Autofac;
    using Prsd.Core.Autofac;
    using RequestHandlers.Security;

    public class SecurityModule : Module
    {
        private readonly EnvironmentResolver environment;

        public SecurityModule(EnvironmentResolver environment)
        {
            this.environment = environment;
        }

        public SecurityModule()
        {
            this.environment = new EnvironmentResolver()
            {
                HostEnvironment = HostEnvironmentType.Owin,
                IocApplication = IocApplication.RequestHandler,
                IsTestRun = false
            };
        }

        protected override void Load(ContainerBuilder builder)
        {
            if (environment.HostEnvironment == HostEnvironmentType.Console)
            {
                builder.RegisterType<WeeeAuthorization>()
                    .As<IWeeeAuthorization>()
                    .SingleInstance();
            }
            else
            {
                builder.RegisterType<WeeeAuthorization>()
                .As<IWeeeAuthorization>()
                .InstancePerRequest();
            }
        }
    }
}