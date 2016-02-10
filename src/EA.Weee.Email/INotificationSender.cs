namespace EA.Weee.Email
{
    using System.Net.Mail;
    using System.Threading.Tasks;

    public interface INotificationSender
    {
        Task SendAsync(MailMessage message);
    }
}
