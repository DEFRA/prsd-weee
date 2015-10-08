namespace EA.Weee.Web.RazorHelpers
{
    using System.Web.Mvc;
    using System.Web.Mvc.Html;

    public partial class WeeeGds<TModel>
    {
        public MvcHtmlString ActionLinkToNewTab(string linkText, string actionName, string controllerName, object routeValues = null)
        {
            return new MvcHtmlString(string.Format("<label><span class=\"hidden-for-screen-reader\">This link opens in a new browser window</span>{0}</label>",
                HtmlHelper.ActionLink(linkText, actionName, controllerName, routeValues, new { @target = "_blank" })));
        }
    }
}