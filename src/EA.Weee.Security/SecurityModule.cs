namespace EA.Weee.Security
{
    using Autofac;

    public class SecurityModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<RoleRequestHandler>().As<IRoleRequestHandler>()
                .InstancePerRequest();
        }
    }
}
