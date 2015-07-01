namespace EA.Weee.Web.Services
{
    using System;
    using System.Net.Mail;
    using System.Net.Mime;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web;

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

            if (!string.IsNullOrWhiteSpace(configurationService.CurrentConfiguration.SendEmail)
                && configurationService.CurrentConfiguration.SendEmail.Equals("true", StringComparison.InvariantCultureIgnoreCase))
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
            var mail = new MailMessage(from, to) { Subject = subject, IsBodyHtml = true };

            // Add the plain text alternative for other email clients.
            var plainText = AlternateView.CreateAlternateViewFromString(emailTemplate.PlainText, Encoding.UTF8, MediaTypeNames.Text.Plain);
            mail.AlternateViews.Add(plainText);

            // Add the HTML view.
            var htmlText = AlternateView.CreateAlternateViewFromString(emailTemplate.HtmlText, Encoding.UTF8,
                MediaTypeNames.Text.Html);
            mail.AlternateViews.Add(htmlText);

            return mail;
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