namespace EA.Weee.Web.Modules
{
    using Autofac;
    using Services;
    using Services.EmailRules;

    public class EmailModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<EmailTemplateService>().As<IEmailTemplateService>();
            builder.RegisterType<EmailService>().As<IEmailService>();
            builder.RegisterType<SmtpClientProxy>().As<ISmtpClient>();
            builder.RegisterType<RuleChecker>().As<IRuleChecker>();
        }
    }
}