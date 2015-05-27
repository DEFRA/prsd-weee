namespace EA.Weee.Web.Tests.Unit.TestHelpers.Factories
{
    using Autofac;

    internal class AutofacContainerFactory
    {
        public static IContainer GetBuiltWebProjectContainer()
        {
            var containerBuilder = new ContainerBuilder();

            return AutofacBootstrapper.Initialize(containerBuilder);
        }
    }
}
