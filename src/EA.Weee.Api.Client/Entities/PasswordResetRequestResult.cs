namespace EA.Weee.Api.Client.Entities
{
    public class PasswordResetRequestResult
    {
        public bool ValidEmail { get; set; }

        public string PasswordResetToken { get; set; }
    }
}
