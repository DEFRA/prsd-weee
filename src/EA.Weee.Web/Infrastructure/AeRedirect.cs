namespace EA.Weee.Web.Infrastructure
{
    using System;
    using System.Web.Mvc;
    using System.Web.Routing;

    public static class AeRedirect
    {
        public static string ReturnsRouteName = "ae-returns";

        public static RedirectToRouteResult ReturnsList(Guid organisationId)
        {
            return new RedirectToRouteResult(ReturnsRouteName, new RouteValueDictionary(new { controller = "Returns", action = "Index", organisationId = organisationId }));
        }

        public static RedirectToRouteResult ExportedWholeWeee(Guid organisationId)
        {
            return new RedirectToRouteResult(ReturnsRouteName, new RouteValueDictionary(new { controller = "Returns", action = "ExportedWholeWeee", organisationId = organisationId }));
        }

        public static RedirectToRouteResult NilReturn(Guid organisationId)
        {
            return new RedirectToRouteResult(ReturnsRouteName, new RouteValueDictionary(new { controller = "Returns", action = "NilReturn", organisationId = organisationId }));
        }
    }
}