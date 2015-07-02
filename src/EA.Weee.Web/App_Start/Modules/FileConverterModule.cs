namespace EA.Weee.Web.Modules
{
    using Autofac;
    using Services;

    public class FileConverterModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<FileConverterService>().As<IFileConverterService>();
        }
    }
}