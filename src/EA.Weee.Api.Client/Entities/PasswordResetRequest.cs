namespace EA.Weee.Api.Client.Entities
{
    using EA.Weee.Core.Routing;

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
