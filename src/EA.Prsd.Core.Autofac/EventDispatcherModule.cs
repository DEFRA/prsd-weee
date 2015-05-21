namespace EA.Prsd.Core.Autofac
{
    using global::Autofac;
    using Domain;

    public class EventDispatcherModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AutofacDeferredEventDispatcher>().As<IDeferredEventDispatcher>();
        }
    }
}