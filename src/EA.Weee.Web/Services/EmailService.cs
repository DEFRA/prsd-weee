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

        public MailMessage GenerateEmailVerificationMessage(string verificationBaseUrl, string verificationToken, string userId, string mailTo)
        {
            var email = templateService.TemplateWithDynamicModel("VerifyEmailAddress", new { VerifyLink = GetEmailVerificationUrl(verificationBaseUrl, verificationToken, userId) });

            var message = GenerateMailMessageWithHtmlAndPlainTextParts(configurationService.CurrentConfiguration.MailFrom, mailTo, "Verify your email address", email);

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
        /// Generates the correct verification URL for a user to verify their email.
        /// </summary>
        private string GetEmailVerificationUrl(string baseUrl, string verificationToken, string userId)
        {
            var uriBuilder = new UriBuilder(baseUrl);
            uriBuilder.Path += "/" + userId;
            var parameters = HttpUtility.ParseQueryString(string.Empty);
            parameters["code"] = verificationToken;
            uriBuilder.Query = parameters.ToString();
            return uriBuilder.Uri.ToString();
        }
    }
}