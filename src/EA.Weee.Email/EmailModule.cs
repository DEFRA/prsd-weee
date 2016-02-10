namespace EA.Weee.Email
{
    using System.Configuration;
    using Autofac;
    using EA.Prsd.Core.Domain;
    using EA.Prsd.Email;
    using EA.Prsd.Email.Rules;
    using EA.Weee.Email.EventHandlers;

    public class EmailModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c => ConfigurationManager.GetSection("emailRules")).As<IRuleSet>();
            builder.Register(c => new WeeeConfigReader())
                .As<IWeeeEmailConfiguration>()
                .As<IEmailConfiguration>();

            builder.Register(c => new ResourceTemplateLoader(ThisAssembly, "EA.Weee.Email.Templates"))
                .As<ITemplateLoader>();

            builder.RegisterType<RazorTemplateExecutor>().As<ITemplateExecutor>();
            builder.RegisterType<MessageCreator>().As<IMessageCreator>();
            builder.RegisterType<Sender>().As<ISender>();
            builder.RegisterType<SmtpClientProxy>().As<ISmtpClient>();
            builder.RegisterType<WeeeEmailService>().As<IWeeeEmailService>();
            builder.RegisterType<WeeeNotificationEmailService>().As<IWeeeNotificationEmailService>();
            builder.RegisterType<NotificationSender>().As<INotificationSender>();

            builder.RegisterAssemblyTypes(ThisAssembly).AsClosedTypesOf(typeof(IEventHandler<>));

            builder.RegisterType<OrganisationUserRequestEventHandlerDataAccess>()
                .As<IOrganisationUserRequestEventHandlerDataAccess>();
        }
    }
}
