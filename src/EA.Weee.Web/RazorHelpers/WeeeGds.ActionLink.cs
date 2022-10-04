namespace EA.Weee.Web.RazorHelpers
{
    using System.Web.Mvc;

    public partial class WeeeGds<TModel>
    {
        public MvcHtmlString ActionLinkToNewTab(string linkText, string url, string additionalMessage = null)
        {
            var span = $"{linkText}<span class=\"govuk-visually-hidden\">{additionalMessage} This link opens in a new tab</span>";

            var tagBuilder = new TagBuilder("a");
            tagBuilder.Attributes.Add("href", url);
            tagBuilder.Attributes.Add("target", "_blank");
            tagBuilder.InnerHtml = span;

            return new MvcHtmlString(tagBuilder.ToString(TagRenderMode.Normal));
        }
    }
}