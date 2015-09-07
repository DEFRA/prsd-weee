namespace EA.Weee.Api.Client.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class PasswordResetRequest
    {
        public string EmailAddress { get; private set; }

        public PasswordResetRequest(string emailAddress)
        {
            EmailAddress = emailAddress;
        }
    }
}
