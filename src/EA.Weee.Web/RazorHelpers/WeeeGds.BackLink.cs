namespace EA.Weee.Web.RazorHelpers
{
    using System.Web.Mvc;

    public partial class WeeeGds<TModel>
    {
        public MvcHtmlString BackLink(string url)
        {
            var tagBuilder = new TagBuilder("a");
            tagBuilder.Attributes.Add("href", url);
            tagBuilder.Attributes.Add("class", "govuk-back-link");
            tagBuilder.SetInnerText("Back");

            return new MvcHtmlString(tagBuilder.ToString(TagRenderMode.Normal));
        }
    }
}