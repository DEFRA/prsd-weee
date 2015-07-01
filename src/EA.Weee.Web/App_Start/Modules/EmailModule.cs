namespace EA.Weee.Web.Modules
{
    using Autofac;
    using Services;

    public class EmailModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<EmailTemplateService>().As<IEmailTemplateService>();
            builder.RegisterType<EmailService>().As<IEmailService>();
        }
    }
}