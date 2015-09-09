namespace EA.Weee.Web.Authorization
{
    using Core.Helpers;

    public class LoginResult
    {
        public bool Successful { get; private set; }

        public string ErrorMessage { get; private set; }

        public string AccessToken { get; private set; }

        private LoginResult(bool successful, string errorMessage, string accessToken)
        {
            Successful = successful;
            ErrorMessage = errorMessage;
            AccessToken = accessToken;
        }

        public static LoginResult Success(string accessToken)
        {
            return new LoginResult(true, null, accessToken);
        }

        public static LoginResult Fail(string errorMessage)
        {
            return new LoginResult(false, errorMessage, null);
        }
    }
}