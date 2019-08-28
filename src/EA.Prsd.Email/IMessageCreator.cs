namespace EA.Prsd.Email
{
    using System.Net.Mail;

    public interface IMessageCreator
    {
        MailMessage Create(string to, string subject, EmailContent content);
    }
}