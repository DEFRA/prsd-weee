namespace EA.Weee.Email
{
    using System;
    using System.Net.Mail;
    using System.Threading.Tasks;
    using Core.Logging;
    using Prsd.Email;

    /// <summary>
    /// Uses an instance of <see cref="ISender"/> to send emails but catches and logs any exceptions
    /// that may occur.
    /// </summary>
    public class NotificationSender : INotificationSender
    {
        private readonly ISender sender;
        private readonly ILogger logger;

        public NotificationSender(ISender sender, ILogger logger)
        {
            this.sender = sender;
            this.logger = logger;
        }

        public async Task SendAsync(MailMessage message)
        {
            try
            {
                await sender.SendAsync(message);
            }
            catch (Exception exception)
            {
                string errorMessage = string.Format("Error sending notification email with subject of '{0}'. See inner exception for details", message.Subject);
                logger.Log(new Exception(errorMessage, exception));
            }
        }
    }
}
