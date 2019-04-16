namespace EA.Weee.Web.RazorHelpers
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Reflection;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using System.Web.WebPages;

    public static class ExampleRouteLinkExtensions
    {
        public static MvcHtmlString NavigationRouteLink(this HtmlHelper helper, string linkText, string descriptiveText, string routeName,
            object routeValues, object htmlAttributes = null)
        {
            var dict = htmlAttributes ?? new Dictionary<string, object>();

            var url = UrlHelper.GenerateUrl(routeName, null, null, null, null, null, TypeHelper.ObjectToDictionary(routeValues), RouteTable.Routes,
                helper.ViewContext.RequestContext, false);

            var linkTagBuilder = new TagBuilder("a");

            var spanTagBuilder = new TagBuilder("span");
            spanTagBuilder.AddCssClass("govuk-visually-hidden");
            spanTagBuilder.InnerHtml = !string.IsNullOrEmpty(descriptiveText) ? HttpUtility.HtmlEncode(descriptiveText) : string.Empty;
            var encodedLinkText = !string.IsNullOrEmpty(linkText) ? HttpUtility.HtmlEncode(linkText) : string.Empty;
            linkTagBuilder.InnerHtml = $"{spanTagBuilder.ToString()}{encodedLinkText}";
            linkTagBuilder.AddCssClass("govuk-link");
            linkTagBuilder.AddCssClass("govuk-link--no-visited-state");
            linkTagBuilder.MergeAttributes<string, object>((IDictionary<string, object>)HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
            linkTagBuilder.MergeAttribute("href", url);
            return new MvcHtmlString(linkTagBuilder.ToString(TagRenderMode.Normal));
        }
    }
}