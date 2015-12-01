namespace EA.Weee.Api.Client.Entities
{
    using EA.Weee.Core.Routing;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class PasswordResetRequest
    {
        public string EmailAddress { get; private set; }

        /// <summary>
        /// The route to the reset password page.
        /// </summary>
        public ResetPasswordRoute Route { get; set; }

        public PasswordResetRequest(string emailAddress, ResetPasswordRoute route)
        {
            EmailAddress = emailAddress;
            Route = route;
        }
    }
}
