namespace EA.Weee.Core.Account.PasswordReset
{
    public class PasswordResetResult
    {
        public bool HasResetSuccessfully { get; private set; }

        public string ErrorMessage { get; private set; }

        public static PasswordResetResult Success()
        {
            return new PasswordResetResult(true);
        }

        public static PasswordResetResult Fail(string errorMessage)
        {
            return new PasswordResetResult(false, errorMessage);
        }

        private PasswordResetResult(bool hasResetSuccessfully, string errorMessage = null)
        {
            HasResetSuccessfully = hasResetSuccessfully;
            ErrorMessage = errorMessage;
        }
    }
}
