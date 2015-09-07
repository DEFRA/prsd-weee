namespace EA.Prsd.Email
{
    using System.Net.Mail;
    using System.Threading.Tasks;

    public interface ISmtpClient
    {
        Task SendMailAsync(MailMessage message);
    }
}