namespace EA.Weee.Email
{
    using EA.Prsd.Email;
    using EA.Weee.Domain;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Mail;
    using System.Text;
    using System.Threading.Tasks;

    public class WeeeEmailService : IWeeeEmailService
    {
        private readonly ITemplateExecutor templateExecutor;
        private readonly IMessageCreator messageCreator;
        private readonly ISender sender;
        private readonly IWeeeEmailConfiguration configuration;

        public WeeeEmailService(
            ITemplateExecutor templateExecutor,
            IMessageCreator messageCreator,
            ISender sender,
            IWeeeEmailConfiguration configuration)
        {
            this.templateExecutor = templateExecutor;
            this.messageCreator = messageCreator;
            this.sender = sender;
            this.configuration = configuration;
        }

        public async Task<bool> SendActivateUserAccount(string emailAddress, string activationUrl)
        {
            var model = new
            {
                ActivationUrl = activationUrl,
            };

            EmailContent content = new EmailContent()
            {
                HtmlText = templateExecutor.Execute("ActivateUserAccount.cshtml", model),
                PlainText = templateExecutor.Execute("ActivateUserAccount.txt", model)
            };

            MailMessage message = messageCreator.Create(
                emailAddress,
                "Activate your WEEE user account",
                content);

            return await sender.SendAsync(message);
        }

        public async Task<bool> SendOrganisationUserRequest(string emailAddress, Domain.Organisation.OrganisationUser organisationUser)
        {
            var model = new
            {
                OrganisationName = organisationUser.Organisation.OrganisationName,
                SiteUrl = configuration.SiteUrl,
            };

            EmailContent content = new EmailContent()
            {
                HtmlText = templateExecutor.Execute("OrganisationUserRequest.cshtml", model),
                PlainText = templateExecutor.Execute("OrganisationUserRequest.txt", model)
            };

            MailMessage message = messageCreator.Create(
                emailAddress,
                string.Format("Request to perform WEEE activities for {0}", model.OrganisationName),
                content);

            return await sender.SendAsync(message);
        }

        public async Task<bool> SendOrganisationUserRequestCompleted(Domain.Organisation.OrganisationUser organisationUser)
        {
            var model = new
            {
                OrganisationName = organisationUser.Organisation.OrganisationName,
                Approved = organisationUser.UserStatus == UserStatus.Active,
            };

            EmailContent content = new EmailContent()
            {
                HtmlText = templateExecutor.Execute("OrganisationUserRequestCompleted.cshtml", model),
                PlainText = templateExecutor.Execute("OrganisationUserRequestCompleted.txt", model)
            };

            MailMessage message = messageCreator.Create(
                organisationUser.User.Email,
                "Decision on your request to perform WEEE activities",
                content);

            return await sender.SendAsync(message);
        }
    }
}
