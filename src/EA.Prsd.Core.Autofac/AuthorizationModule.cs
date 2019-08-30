namespace EA.Prsd.Core.Autofac
{
    using global::Autofac;
    using Security;

    public class AuthorizationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ResourceAuthorizationManager>().As<IResourceAuthorizationManager>();
        }
    }
}