namespace EA.Weee.Web.Services
{
    using System.Net.Mail;
    using System.Threading.Tasks;

    public interface IEmailService
    {
        Task<bool> SendAsync(MailMessage message);

        MailMessage GenerateEmailVerificationMessage(string verificationBaseUrl, string verificationToken, string userId, string mailTo);

        MailMessage GenerateMailMessageWithHtmlAndPlainTextParts(string from, string to, string subject, EmailTemplate emailTemplate);
    }
}
