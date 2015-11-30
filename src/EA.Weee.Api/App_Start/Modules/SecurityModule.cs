namespace EA.Weee.Api.Modules
{
    using Autofac;
    using RequestHandlers.Security;

    public class SecurityModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<WeeeAuthorization>()
                .As<IWeeeAuthorization>()
                .InstancePerRequest();
        }
    }
}