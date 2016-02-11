namespace EA.Weee.Email
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Mail;
    using System.Text;
    using System.Threading.Tasks;
    using EA.Prsd.Email;

    /// <summary>
    /// Uses a <see cref="NotificationSender"/> to send emails. This ensures that exceptions
    /// are not thrown when errors occur while sending emails.
    /// </summary>
    public class WeeeNotificationEmailService : IWeeeNotificationEmailService
    {
        private readonly ITemplateExecutor templateExecutor;
        private readonly IMessageCreator messageCreator;
        private readonly INotificationSender notificationSender;

        public WeeeNotificationEmailService(
            ITemplateExecutor templateExecutor,
            IMessageCreator messageCreator,
            INotificationSender notificationSender)
        {
            this.templateExecutor = templateExecutor;
            this.messageCreator = messageCreator;
            this.notificationSender = notificationSender;
        }

        public async Task SendSchemeMemberSubmitted(string emailAddress, string schemeName, int complianceYear, int numberOfWarnings)
        {
            var model = new
            {
                SchemeName = schemeName,
                ComplianceYear = complianceYear,
                NumberOfWarnings = numberOfWarnings
            };

            EmailContent content = new EmailContent
            {
                HtmlText = templateExecutor.Execute("SchemeMemberSubmitted.cshtml", model),
                PlainText = templateExecutor.Execute("SchemeMemberSubmitted.txt", model)
            };

            using (MailMessage message = messageCreator.Create(emailAddress,
                string.Format("New member registration submission for {0}", schemeName), content))
            {
                await notificationSender.SendAsync(message);
            }
        }
    }
}
