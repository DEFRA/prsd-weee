namespace EA.Weee.Api.Client.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class PasswordResetRequestResult
    {
        public bool ValidEmail { get; set; }

        public string PasswordResetToken { get; set; }
    }
}
