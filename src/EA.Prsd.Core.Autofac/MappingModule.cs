namespace EA.Prsd.Core.Autofac
{
    using global::Autofac;
    using Mapper;

    public class MappingModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AutofacMappingEngine>().As<IMapper>();
        }
    }
}