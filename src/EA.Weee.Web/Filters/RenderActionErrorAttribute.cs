namespace EA.Weee.Web.Filters
{
    using Infrastructure;
    using System.Web.Mvc;

    public class RenderActionErrorAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            filterContext.ExceptionHandled = false;
            filterContext.Controller.TempData[Constants.ErrorOccurred] = true;
        }
    }
}