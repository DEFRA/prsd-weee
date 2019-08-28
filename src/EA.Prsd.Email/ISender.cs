namespace EA.Prsd.Email
{
    using System.Net.Mail;
    using System.Threading.Tasks;

    public interface ISender
    {
        Task<bool> SendAsync(MailMessage message);
    }
}