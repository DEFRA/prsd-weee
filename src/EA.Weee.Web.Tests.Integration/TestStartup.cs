namespace EA.Weee.Web.Tests.Integration
{
    using System.Web;
    using Autofac;
    using Web.Services;

    public static class TestStartup
    {
        public static IContainer Initialize()
        {
            var configuration = new ConfigurationService();

            var builder = new ContainerBuilder();
            builder.Register(c => configuration).As<ConfigurationService>().SingleInstance();
            builder.Register(c => configuration.CurrentConfiguration).As<IAppConfiguration>().SingleInstance();
            builder.Register(c => HttpContext.Current.GetOwinContext().Authentication).InstancePerRequest();

            return AutofacBootstrapper.Initialize(new ContainerBuilder());
        }
    }
}
