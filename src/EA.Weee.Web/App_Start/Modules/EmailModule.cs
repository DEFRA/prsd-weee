namespace EA.Weee.Web.Modules
{
    using Autofac;
    using Core.Configuration.EmailRules;
    using Services;

    public class EmailModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<EmailTemplateService>().As<IEmailTemplateService>();
            builder.RegisterType<EmailService>().As<IEmailService>();
            builder.RegisterType<SmtpClientProxy>().As<ISmtpClient>();
            builder.RegisterType<RuleSectionChecker>().As<IRuleSectionChecker>();
            builder.RegisterType<RuleChecker>().As<IRuleChecker>();
        }
    }
}