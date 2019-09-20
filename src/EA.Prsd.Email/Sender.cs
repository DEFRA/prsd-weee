namespace EA.Prsd.Email
{
    using System;
    using System.Diagnostics;
    using System.Net.Mail;
    using System.Threading.Tasks;
    using Rules;

    public class Sender : ISender
    {
        private readonly IEmailConfiguration configuration;
        private readonly IRuleSet ruleSet;
        private readonly ISmtpClient smtpClient;

        public Sender(IEmailConfiguration configuration, IRuleSet ruleSet, ISmtpClient smtpClient)
        {
            this.configuration = configuration;
            this.ruleSet = ruleSet;
            this.smtpClient = smtpClient;
        }

        public async Task<bool> SendAsync(MailMessage message)
        {
            if (message.To.Count == 0)
            {
                throw new InvalidOperationException(
                    "At least one \"To\" recipient must be defined for an email to be sent.");
            }

            if (!configuration.SendEmail)
            {
                Trace.TraceInformation("Email sending has been disabled.");
                return false;
            }

            CleanMailAddressCollection(message.To);
            CleanMailAddressCollection(message.CC);
            CleanMailAddressCollection(message.Bcc);

            if (message.To.Count == 0)
            {
                Trace.TraceInformation("All \"To\" recipients have been removed; this email will not be sent.");
                return false;
            }

            await smtpClient.SendMailAsync(message);

            return true;
        }

        /// <summary>
        ///     Checks each mail asdress in the collection against the sending rules,
        ///     removing any addresses to which sending is not allowed.
        /// </summary>
        /// <param name="addresses"></param>
        private void CleanMailAddressCollection(MailAddressCollection addresses)
        {
            for (var index = addresses.Count - 1; index >= 0; index--)
            {
                var address = addresses[index];

                if (ruleSet.Check(address.Address) != RuleAction.Allow)
                {
                    Trace.TraceInformation(
                        "The email address \"{0}\" has been removed from this message " +
                        "as sending to this address is not allowed.", address.Address);
                    addresses.RemoveAt(index);
                }
            }
        }
    }
}