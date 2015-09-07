namespace EA.Prsd.Email
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Mail;
    using System.Text;
    using System.Threading.Tasks;

    public interface ISender
    {
        Task<bool> SendAsync(MailMessage message);
    }
}
