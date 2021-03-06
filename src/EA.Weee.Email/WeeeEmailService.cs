﻿namespace EA.Weee.Email
{
    using Prsd.Email;
    using System.Net.Mail;
    using System.Threading.Tasks;

    using EA.Weee.Core.Shared;

    using UserStatus = EA.Weee.Domain.User.UserStatus;

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

            var content = new EmailContent()
            {
                HtmlText = templateExecutor.Execute("ActivateUserAccount.cshtml", model),
                PlainText = templateExecutor.Execute("ActivateUserAccount.txt", model)
            };

            using (var message = messageCreator.Create(emailAddress,
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

            var content = new EmailContent()
            {
                HtmlText = templateExecutor.Execute("PasswordResetRequest.cshtml", model),
                PlainText = templateExecutor.Execute("PasswordResetRequest.txt", model)
            };

            using (var message = messageCreator.Create(emailAddress,
                 "Reset your WEEE password", content))
            {
                return await sender.SendAsync(message);
            }
        }

        public async Task<bool> SendOrganisationUserRequest(string emailAddress, string organisationName, string userName)
        {
            var model = new
            {
                OrganisationName = organisationName,
                ExternalUserLoginUrl = configuration.ExternalUserLoginUrl,
                FullName = userName
            };

            var content = new EmailContent()
            {
                HtmlText = templateExecutor.Execute("OrganisationUserRequest.cshtml", model),
                PlainText = templateExecutor.Execute("OrganisationUserRequest.txt", model)
            };

            using (var message = messageCreator.Create(emailAddress,
                 "New request to access your organisation", content))
            {
                return await sender.SendAsync(message);
            }
        }

        public async Task<bool> SendOrganisationUserRequestToEA(string emailAddress, string organisationName, string userName)
        {
            var model = new
            {
                OrganisationName = organisationName,
                ExternalUserLoginUrl = configuration.ExternalUserLoginUrl,
                FullName = userName
            };

            var content = new EmailContent()
            {
                HtmlText = templateExecutor.Execute("OrganisationUserRequestToEA.cshtml", model),
                PlainText = templateExecutor.Execute("OrganisationUserRequestToEA.txt", model)
            };

            using (var message = messageCreator.Create(emailAddress,
                 "New request to access an organisation in WEEE Online", content))
            {
                return await sender.SendAsync(message);
            }
        }

        public async Task<bool> SendOrganisationUserRequestCompleted(Domain.Organisation.OrganisationUser organisationUser, bool activeUsers)
        {
            var model = new
            {
                OrganisationName = organisationUser.Organisation.OrganisationName,
                Approved = organisationUser.UserStatus == UserStatus.Active,
                ExternalUserLoginUrl = configuration.ExternalUserLoginUrl,
                FullName = organisationUser.User.FullName,
                ActiveUsers = activeUsers
            };

            var content = new EmailContent()
            {
                HtmlText = templateExecutor.Execute("OrganisationUserRequestCompleted.cshtml", model),
                PlainText = templateExecutor.Execute("OrganisationUserRequestCompleted.txt", model)
            };

            using (var message = messageCreator.Create(organisationUser.User.Email,
                "Your request to access " + organisationUser.Organisation.OrganisationName, content))
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

            var content = new EmailContent
            {
                HtmlText = templateExecutor.Execute("SchemeMemberSubmitted.cshtml", model),
                PlainText = templateExecutor.Execute("SchemeMemberSubmitted.txt", model)
            };

            using (var message = messageCreator.Create(emailAddress,
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

            var content = new EmailContent
            {
                HtmlText = templateExecutor.Execute("SchemeDataReturnSubmitted.cshtml", model),
                PlainText = templateExecutor.Execute("SchemeDataReturnSubmitted.txt", model)
            };

            using (var message = messageCreator.Create(emailAddress,
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

            var content = new EmailContent
            {
                HtmlText = templateExecutor.Execute("InternalUserAccountActivated.cshtml", model),
                PlainText = templateExecutor.Execute("InternalUserAccountActivated.txt", model)
            };

            using (var message = messageCreator.Create(emailAddress, "New internal user request", content))
            {
                return await sender.SendAsync(message, true);
            }
        }

        public async Task<bool> SendTestEmail(string emailAddress)
        {
            var content = new EmailContent
            {
                HtmlText = templateExecutor.Execute("Test.cshtml", null),
                PlainText = templateExecutor.Execute("Test.txt", null)
            };

            using (var message = messageCreator.Create(emailAddress, "Test email from WEEE", content))
            {
                return await sender.SendAsync(message, false);
            }
        }

        public async Task<bool> SendOrganisationContactDetailsChanged(string emailAddress, string name, EntityType entityType)
        {
            var model = new
            {
                Name = name,
                EntityType = entityType
            };

            var content = new EmailContent
            {
                HtmlText = templateExecutor.Execute("OrganisationContactDetailsChanged.cshtml", model),
                PlainText = templateExecutor.Execute("OrganisationContactDetailsChanged.txt", model)
            };

            var subject = $"Change of contact details for {name}";

            using (var message = messageCreator.Create(emailAddress, subject, content))
            {
                return await sender.SendAsync(message, true);
            }
        }
    }
}
