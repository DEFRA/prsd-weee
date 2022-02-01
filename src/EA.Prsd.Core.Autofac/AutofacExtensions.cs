namespace EA.Prsd.Core.Autofac
{
    using System.ComponentModel;
    using global::Autofac;
    using IContainer = global::Autofac.IContainer;

    public static class AutofacExtensions
    {
        public static ContainerBuilder As(this ContainerBuilder container, EnvironmentResolver environment)
        {
            container.RegisterInstance<IEnvironmentResolver>(environment);
            return container;
        }
    }
}
