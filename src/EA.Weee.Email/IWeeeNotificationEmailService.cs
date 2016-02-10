namespace EA.Weee.Email
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IWeeeNotificationEmailService
    {
        Task SendSchemeMemberSubmitted(string emailAddress, string schemeName, int complianceYear, int numberOfWarnings);
    }
}
