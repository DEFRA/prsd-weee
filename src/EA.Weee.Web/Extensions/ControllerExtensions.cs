namespace EA.Weee.Web.Extensions
{
    using System.Web.Mvc;

    public static class ControllerExtensions
    {
        public static ActionResult LoginRedirect(this Controller controller, ActionResult defaultLoginAction, string returnUrl = null)
        {
            if (returnUrl != null && controller.Url.IsLocalUrl(returnUrl))
            {
                return new RedirectResult(returnUrl);
            }

            return defaultLoginAction;
        }
    }
}