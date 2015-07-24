namespace EA.Weee.Web.Services
{
    using System;
    using System.Linq;
    using System.Net.Mail;
    using System.Net.Mime;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web;
    using EmailRules;

    public class EmailService : IEmailService
    {
        private readonly ConfigurationService configurationService;
        private readonly IEmailTemplateService templateService;

        public EmailService(ConfigurationService configurationService, IEmailTemplateService templateService)
        {
            this.templateService = templateService;
            this.configurationService = configurationService;
        }

        public async Task<bool> SendAsync(MailMessage message)
        {
            var client = new SmtpClient();
            var emailRuleChecker = new RuleChecker();
            var toAddress = string.Empty;

            if (message.To.Count == 1 && !message.To.Single().Address.Contains(","))
            {
                toAddress = message.To.Single().Address;
            }
            else
            {
                throw new InvalidOperationException("Emails cannot be sent to multiple email addresses. Whitelist is set up to parse a single address.");
            }

            if (!string.IsNullOrWhiteSpace(configurationService.CurrentConfiguration.SendEmail)
                && configurationService.CurrentConfiguration.SendEmail.Equals("true", StringComparison.InvariantCultureIgnoreCase)
                && emailRuleChecker.CheckEmailAddress(toAddress) == RuleAction.Allow)
            {
                try
                {
                    await client.SendMailAsync(message);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(ex.Message);
                }

                return true;
            }

            return false;
        }

        public MailMessage GenerateUserAccountActivationMessage(string activationBaseUrl, string activationToken, string userId, string mailTo)
        {
            var email = templateService.TemplateWithDynamicModel("ActivateUserAccount", new { VerifyLink = GetUserAccountActivationUrl(activationBaseUrl, activationToken, userId) });

            var message = GenerateMailMessageWithHtmlAndPlainTextParts(configurationService.CurrentConfiguration.MailFrom, mailTo, "Activate your WEEE user account", email);

            return message;
        }

        public MailMessage GenerateMailMessageWithHtmlAndPlainTextParts(string from, string to, string subject,
            EmailTemplate emailTemplate)
        {
            if (!string.IsNullOrWhiteSpace(configurationService.CurrentConfiguration.TruncateEmailAfterPlus)
                        && configurationService.CurrentConfiguration.TruncateEmailAfterPlus.Equals("true", StringComparison.InvariantCultureIgnoreCase))
            {
                //Remove characters from email address after "+" till '@'
                //this is done to aid manual testing by allowing multiple user accounts to be set up to receive email at the same email address.
                to = TruncateEmailAfterPlus(to);
            }

            var mail = new MailMessage(from, to) { Subject = subject, IsBodyHtml = true };

            // Add the plain text alternative for other email clients.
            var plainText = AlternateView.CreateAlternateViewFromString(emailTemplate.PlainText, Encoding.UTF8, MediaTypeNames.Text.Plain);

            // Add the HTML view.
            var htmlText = AlternateView.CreateAlternateViewFromString(emailTemplate.HtmlText, Encoding.UTF8,
                MediaTypeNames.Text.Html);

            //create linked resource for embedded image
            string file = HttpContext.Current.Server.MapPath("~/Content/govuk/images/govuk_logotype_email.png");
            if (!String.IsNullOrEmpty(file))
            {
                LinkedResource govuklogo = new LinkedResource(file)
                {
                    ContentId = "logo"
                };
                htmlText.LinkedResources.Add((govuklogo));
            }

            //add the views
            mail.AlternateViews.Add(plainText);
            mail.AlternateViews.Add(htmlText);

            return mail;
        }

        private static string TruncateEmailAfterPlus(string emailaddress)
        {
            int indexOfPlus = emailaddress.LastIndexOf('+');
            if (indexOfPlus != -1)
            {
                int indexOfAt = emailaddress.LastIndexOf('@');
                if (indexOfAt != -1)
                {
                    int numberofCharsToRemove = indexOfAt - indexOfPlus;
                    if (numberofCharsToRemove > 0)
                    {
                        emailaddress = emailaddress.Remove(indexOfPlus, numberofCharsToRemove);
                    }
                }
            }
            return emailaddress;
        }

        /// <summary>
        /// Generates the correct activation URL for a user to activate the account.
        /// </summary>
        private string GetUserAccountActivationUrl(string baseUrl, string activationToken, string userId)
        {
            var uriBuilder = new UriBuilder(baseUrl);
            uriBuilder.Path += "/" + userId;
            var parameters = HttpUtility.ParseQueryString(string.Empty);
            parameters["code"] = activationToken;
            uriBuilder.Query = parameters.ToString();
            return uriBuilder.Uri.ToString();
        }
    }
}