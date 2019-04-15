using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EA.Weee.Web.RazorHelpers
{
    using System.Web.Mvc;
    using System.Web.Routing;

    public static class WeeeGds
    {
        public static MvcHtmlString NavigationRouteLink(this HtmlHelper<dynamic> htmlHelper, string linkText, string routeName, RouteValueDictionary routeValues = null, IDictionary<string, object> htmlAttributes = null)
        {
            var dict = htmlAttributes ?? new Dictionary<string, object>();

            if (htmlHelper.ViewContext.RequestContext.HttpContext.Request.Url.AbsolutePath == new UrlHelper(htmlHelper.ViewContext.RequestContext).RouteUrl(routeName))
                dict.Add("class", "current");

            //var generateLInk = GenerateLinkInternal2(htmlHelper.ViewContext.RequestContext, RouteTable.Routes, linkText, routeName, 

                string ur2l = UrlHelper.GenerateUrl(routeName, null, null, null, null, null, routeValues, RouteTable.Routes, htmlHelper.ViewContext.RequestContext, false);
      TagBuilder tagBuilder = new TagBuilder("a");
      tagBuilder.InnerHtml = !string.IsNullOrEmpty(linkText) ? HttpUtility.HtmlEncode(linkText) : string.Empty;
      tagBuilder.MergeAttributes<string, object>(htmlAttributes);
      tagBuilder.MergeAttribute("href", ur2l);
      //return tagBuilder.ToString(TagRenderMode.Normal);

            return new MvcHtmlString(HtmlHelper.GenerateRouteLink(htmlHelper.ViewContext.RequestContext, RouteTable.Routes, linkText, routeName, routeValues, dict));
        }

        private static string GenerateLinkInternal2(
            RequestContext requestContext,
            RouteCollection routeCollection,
            string linkText,
            string routeName,
            string actionName,
            string controllerName,
            string protocol,
            string hostName,
            string fragment,
            RouteValueDictionary routeValues,
            IDictionary<string, object> htmlAttributes,
            bool includeImplicitMvcValues)
        {
            string url = UrlHelper.GenerateUrl(routeName, actionName, controllerName, protocol, hostName, fragment, routeValues, routeCollection, requestContext, includeImplicitMvcValues);
            TagBuilder tagBuilder = new TagBuilder("a");
            tagBuilder.InnerHtml = !string.IsNullOrEmpty(linkText) ? HttpUtility.HtmlEncode(linkText) : string.Empty;
            tagBuilder.MergeAttributes<string, object>(htmlAttributes);
            tagBuilder.MergeAttribute("href", url);
            return tagBuilder.ToString(TagRenderMode.Normal);
        }
    }
}