namespace EA.Weee.Api.Client.Entities
{
    public class PasswordResetResult
    {
        public string EmailAddress { get; private set; }

        public PasswordResetResult(string emailAddress)
        {
            EmailAddress = emailAddress;
        }
    }
}
