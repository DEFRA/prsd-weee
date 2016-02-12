namespace EA.Weee.Email
{
    using System;
    using System.Net.Mail;
    using System.Threading.Tasks;
    using Core.Logging;
    using Prsd.Email;

    /// <summary>
    /// Uses an instance of <see cref="ISender"/> to send emails while providing the option to
    /// catch and log any exceptions that may occur.
    /// </summary>
    public class WeeeSender : IWeeeSender
    {
        private readonly ISender sender;
        private readonly ILogger logger;

        public WeeeSender(ISender sender, ILogger logger)
        {
            this.sender = sender;
            this.logger = logger;
        }

        public async Task<bool> SendAsync(MailMessage message, bool continueOnException = false)
        {
            try
            {
                return await sender.SendAsync(message);
            }
            catch (Exception exception)
            {
                if (continueOnException)
                {
                    string errorMessage = string.Format("Error sending notification email with subject of '{0}'. See inner exception for details", message.Subject);
                    logger.Log(new Exception(errorMessage, exception));

                    return true;
                }
                else
                {
                    throw exception;
                }
            }
        }
    }
}
