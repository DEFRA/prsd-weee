namespace EA.Weee.Web.RazorHelpers
{
    using System;
    using System.Web.Mvc;
    using System.Web.Mvc.Html;

    public partial class WeeeGds<TModel>
    {
        public MvcHtmlString ActionLinkToNewTab(string linkText, string actionName, string controllerName, object routeValues = null)
        {
            var id = "external-link-" + Guid.NewGuid();
            var label =
                string.Format(
                    "<label for=\"{0}\"><span class=\"hidden-for-screen-reader\">This link opens in a new browser window</span></label>",
                    id);
            var link = HtmlHelper.ActionLink(linkText, actionName, controllerName, routeValues, new { @target = "_blank", id });

            return new MvcHtmlString(label + link);
        }
    }
}