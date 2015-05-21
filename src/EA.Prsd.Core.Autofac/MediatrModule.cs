namespace EA.Prsd.Core.Autofac
{
    using global::Autofac;
    using Mediator;

    public class MediatrModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AutofacMediator>().As<IMediator>();
        }
    }
}