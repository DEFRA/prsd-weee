namespace EA.Weee.Web.Services
{
    using System.Net.Mail;

    /// <summary>
    /// Identical to SmtpClient, but mockable through its ISmtpClient interface.
    /// </summary>
    public class SmtpClientProxy : SmtpClient, ISmtpClient 
    {
    }
}