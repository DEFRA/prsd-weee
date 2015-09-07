using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace EA.Prsd.Email
{
    public interface IMessageCreator
    {
        MailMessage Create(string to, string subject, EmailContent content);
    }
}
