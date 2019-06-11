namespace EA.Weee.Web.Infrastructure
{
    using System;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Core.DataReturns;

    public static class AeRedirect
    {
        public static string ReturnsRouteName = "ae-returns";
        public static string SchemesRouteName = "scheme";

        public static RedirectToRouteResult ReturnsList(Guid organisationId)
        {
            return new RedirectToRouteResult(ReturnsRouteName, new RouteValueDictionary(new { controller = "Returns", action = "Index", organisationId = organisationId }));
        }

        public static RedirectToRouteResult ExportedWholeWeee(Guid organisationId, Guid returnId)
        {
            return new RedirectToRouteResult(ReturnsRouteName, new RouteValueDictionary(new { controller = "Returns", action = "ExportedWholeWeee", organisationId = organisationId, returnId = returnId }));
        }

        public static RedirectToRouteResult NilReturn(Guid organisationId, Guid returnId)
        {
            return new RedirectToRouteResult(ReturnsRouteName, new RouteValueDictionary(new { controller = "Returns", action = "NilReturn", organisationId = organisationId, returnId = returnId }));
        }
        public static RedirectToRouteResult Confirmation(Guid organisationId)
        {
            return new RedirectToRouteResult(ReturnsRouteName, new RouteValueDictionary(new { controller = "Returns", action = "Confirmation", organisationId = organisationId}));
        }
    }
}