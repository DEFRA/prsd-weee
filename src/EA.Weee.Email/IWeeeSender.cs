namespace EA.Weee.Email
{
    using System.Net.Mail;
    using System.Threading.Tasks;

    public interface IWeeeSender
    {
        Task<bool> SendAsync(MailMessage message, bool continueOnException = false);
    }
}
