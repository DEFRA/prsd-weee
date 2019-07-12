namespace EA.Weee.Web.Filters
{
    using System;
    using System.Web.Mvc;
    using Infrastructure;

    public class RenderActionErrorAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            filterContext.ExceptionHandled = false;
            filterContext.Controller.TempData[Constants.ErrorOccurred] = true;
        }
    }
}