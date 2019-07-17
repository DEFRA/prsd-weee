namespace EA.Weee.Web.RazorHelpers
{
    using System.Web.Mvc;

    public partial class WeeeGds<TModel>
    {
        public MvcHtmlString ActionLinkToNewTab(string linkText, string url)
        {
            var span = string.Format("{0}<span class=\"hidden-for-screen-reader\">This link opens in a new browser window</span>",
                linkText);

            var tagBuilder = new TagBuilder("a");
            tagBuilder.Attributes.Add("href", url);
            tagBuilder.Attributes.Add("class", "govuk-footer__link");
            tagBuilder.Attributes.Add("target", "_blank");
            tagBuilder.InnerHtml = span;

            return new MvcHtmlString(tagBuilder.ToString(TagRenderMode.Normal));
        }
    }
}