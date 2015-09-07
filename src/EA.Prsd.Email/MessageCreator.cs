using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EA.Prsd.Email
{
    public class MessageCreator : IMessageCreator
    {
        private readonly IEmailConfiguration configuration;

        public MessageCreator(IEmailConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public MailMessage Create(string to, string subject, EmailContent content)
        {
            if (configuration.TruncateEmailAfterPlus)
            {
                // Remove characters from email address after "+" till '@'.
                // This is done to aid manual testing by allowing multiple user
                // accounts to be set up to receive email at the same email address.
                to = TruncateEmailAfterPlus(to);
            }

            var mail = new MailMessage(configuration.SystemEmailAddress, to)
            {
                Subject = subject,
                IsBodyHtml = true
            };

            // Add the plain text alternative for other email clients.
            var plainText = AlternateView.CreateAlternateViewFromString(
                content.PlainText,
                Encoding.UTF8,
                MediaTypeNames.Text.Plain);

            // Add the HTML view.
            var htmlText = AlternateView.CreateAlternateViewFromString(
                content.HtmlText,
                Encoding.UTF8,
                MediaTypeNames.Text.Html);

            // Create linked resource for embedded image.
            // The stream for the resource is not read utill the message is send, so we
            // cannot wrap this in the traditional "using" statement.
            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream stream = assembly.GetManifestResourceStream("EA.Prsd.Email.Content.govuk_logotype_email.png");

            LinkedResource govuklogo = new LinkedResource(stream)
            {
                ContentId = "logo"
            };
            htmlText.LinkedResources.Add((govuklogo));

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
    }
}
