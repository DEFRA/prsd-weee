namespace EA.Weee.Web.Authorization
{
    using System.Web.Mvc;

    public class LoginResult
    {
        public bool Successful { get; private set; }

        public string ErrorMessage { get; private set; }

        public string AccessToken { get; private set; }

        public ActionResult DefaultLoginAction { get; private set; }

        private LoginResult(bool successful, string errorMessage, string accessToken, ActionResult defaultLoginAction)
        {
            Successful = successful;
            ErrorMessage = errorMessage;
            AccessToken = accessToken;
            DefaultLoginAction = defaultLoginAction;
        }

        public static LoginResult Success(string accessToken, ActionResult defaultLoginAction)
        {
            return new LoginResult(true, null, accessToken, defaultLoginAction);
        }

        public static LoginResult Fail(string errorMessage)
        {
            return new LoginResult(false, errorMessage, null, null);
        }
    }
}