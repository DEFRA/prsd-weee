namespace EA.Weee.DataAccess
{
    using Autofac;
    using EA.Prsd.Core.Domain;

    public class EntityFrameworkModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<WeeeContext>().AsSelf().InstancePerRequest();

            builder.RegisterAssemblyTypes(ThisAssembly).AsClosedTypesOf(typeof(IEventHandler<>));
        }
    }
}