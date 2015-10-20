namespace EA.Weee.Web.Authorization
{
    using System.Web.Mvc;

    public class AuthorizationState
    {
        public bool IsLoggedIn { get; private set; }

        public ActionResult DefaultLoginAction { get; private set; }

        private AuthorizationState(bool isLoggedIn, ActionResult defaultLoginAction = null)
        {
            IsLoggedIn = isLoggedIn;
            DefaultLoginAction = defaultLoginAction;
        }

        public static AuthorizationState LoggedIn(string accessToken, ActionResult defaultLoginAction)
        {
            return new AuthorizationState(true, defaultLoginAction);
        }

        public static AuthorizationState NotLoggedIn()
        {
            return new AuthorizationState(false);
        }
    }
}