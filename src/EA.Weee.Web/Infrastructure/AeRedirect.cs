﻿namespace EA.Weee.Web.Infrastructure
{
    using System;
    using System.Web.Mvc;
    using System.Web.Routing;

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

        public static RedirectToRouteResult NilReturn(Guid returnId)
        {
            return new RedirectToRouteResult(ReturnsRouteName, new RouteValueDictionary(new { controller = "Returns", action = "NilReturn", returnId = returnId }));
        }
        public static RedirectToRouteResult Confirmation(Guid returnId)
        {
            return new RedirectToRouteResult(ReturnsRouteName, new RouteValueDictionary(new { controller = "Returns", action = "Confirmation", returnId = returnId }));
        }
    }
}