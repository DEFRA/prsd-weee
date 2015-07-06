namespace EA.Weee.Web.Infrastructure
{
    using System.Linq;
    using System.Web.Mvc;

    public static class AuthorizationContextExtensions
    {
        private static readonly string[] AllowAnonymousActions =
        {
            "LogOff",
            "Login",
            "UserAccountActivationRequired",
            "ActivateUserAccount",
            "CheckIsPcs",
            "CheckComplianceYear",
            "UserCreation",
            "AccountNotRequired",
            "LandingPage"
        };

        public static bool SkipAuthorisation(this AuthorizationContext authorizationContext)
        {
            return authorizationContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true)
                   ||
                   authorizationContext.ActionDescriptor.ControllerDescriptor.IsDefined(
                       typeof(AllowAnonymousAttribute), true)
                   || authorizationContext.ActionDescriptor.ControllerDescriptor.ControllerName.Equals("Elmah")
                   || AllowAnonymousActions.Contains(authorizationContext.ActionDescriptor.ActionName);
        }
    }
}