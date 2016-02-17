namespace EA.Weee.Email
{
    using System;
    using System.Net.Mail;
    using System.Threading.Tasks;
    using Domain.User;
    using Prsd.Email;

    public class WeeeEmailService : IWeeeEmailService
    {
        private readonly ITemplateExecutor templateExecutor;
        private readonly IMessageCreator messageCreator;
        private readonly IWeeeSender sender;
        private readonly IWeeeEmailConfiguration configuration;

        public WeeeEmailService(
            ITemplateExecutor templateExecutor,
            IMessageCreator messageCreator,
            IWeeeSender sender,
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

            using (MailMessage message = messageCreator.Create(emailAddress,
                 "Activate your WEEE user account", content))
            {
                return await sender.SendAsync(message);
            }
        }

        public async Task<bool> SendPasswordResetRequest(string emailAddress, string passwordResetUrl)
        {
            var model = new
            {
                PasswordResetUrl = passwordResetUrl,
            };

            EmailContent content = new EmailContent()
            {
                HtmlText = templateExecutor.Execute("PasswordResetRequest.cshtml", model),
                PlainText = templateExecutor.Execute("PasswordResetRequest.txt", model)
            };

            using (MailMessage message = messageCreator.Create(emailAddress,
                 "Reset your WEEE password", content))
            {
                return await sender.SendAsync(message);
            }
        }

        public async Task<bool> SendOrganisationUserRequest(string emailAddress, string organisationName)
        {
            var model = new
            {
                OrganisationName = organisationName,
                ExternalUserLoginUrl = configuration.ExternalUserLoginUrl,
            };

            EmailContent content = new EmailContent()
            {
                HtmlText = templateExecutor.Execute("OrganisationUserRequest.cshtml", model),
                PlainText = templateExecutor.Execute("OrganisationUserRequest.txt", model)
            };

            using (MailMessage message = messageCreator.Create(emailAddress,
                 "New request to access your organisation", content))
            {
                return await sender.SendAsync(message);
            }
        }

        public async Task<bool> SendOrganisationUserRequestCompleted(Domain.Organisation.OrganisationUser organisationUser)
        {
            var model = new
            {
                OrganisationName = organisationUser.Organisation.OrganisationName,
                Approved = organisationUser.UserStatus == UserStatus.Active,
                ExternalUserLoginUrl = configuration.ExternalUserLoginUrl
            };

            EmailContent content = new EmailContent()
            {
                HtmlText = templateExecutor.Execute("OrganisationUserRequestCompleted.cshtml", model),
                PlainText = templateExecutor.Execute("OrganisationUserRequestCompleted.txt", model)
            };

            using (MailMessage message = messageCreator.Create(organisationUser.User.Email,
                "Your request to access a WEEE organisation", content))
            {
                return await sender.SendAsync(message);
            }
        }

        public async Task<bool> SendSchemeMemberSubmitted(string emailAddress, string schemeName, int complianceYear, int numberOfWarnings)
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
                string.Format("Member registration submission for {0}", schemeName), content))
            {
                return await sender.SendAsync(message, true);
            }
        }

        public async Task<bool> SendSchemeDataReturnSubmitted(string emailAddress, string schemeName, int complianceYear, int quarter, bool isResubmission)
        {
            var model = new
            {
                SchemeName = schemeName,
                ComplianceYear = complianceYear,
                Quarter = quarter,
                IsResubmission = isResubmission
            };

            EmailContent content = new EmailContent
            {
                HtmlText = templateExecutor.Execute("SchemeDataReturnSubmitted.cshtml", model),
                PlainText = templateExecutor.Execute("SchemeDataReturnSubmitted.txt", model)
            };

            using (MailMessage message = messageCreator.Create(emailAddress,
                string.Format("Data return submission for {0}", schemeName), content))
            {
                return await sender.SendAsync(message, true);
            }
        }

        public async Task<bool> SendInternalUserAccountActivated(string emailAddress, string userFullName, string userEmailAddress, string viewUserLink)
        {
            var model = new
            {
                EmailAddress = userEmailAddress,
                FullName = userFullName,
                ViewUserLink = viewUserLink
            };

            EmailContent content = new EmailContent
            {
                HtmlText = templateExecutor.Execute("InternalUserAccountActivated.cshtml", model),
                PlainText = templateExecutor.Execute("InternalUserAccountActivated.txt", model)
            };

            using (MailMessage message = messageCreator.Create(emailAddress, "New internal user request", content))
            {
                return await sender.SendAsync(message, true);
            }
        }
    }
}
