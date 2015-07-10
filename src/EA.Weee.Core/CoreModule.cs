namespace EA.Weee.Core
{
    using Autofac;

    public class CoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Register the helper classes
            builder.RegisterAssemblyTypes(this.GetType().Assembly)
                .Where(t => t.Namespace.Contains("Helpers"))
                .AsImplementedInterfaces();
        }
    }
}
