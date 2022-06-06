namespace EA.Weee.Web.RazorHelpers
{
    using System.Web.Mvc;

    public partial class WeeeGds<TModel>
    {
        public MvcHtmlString BackLink(string url, string alternativeDescription = null)
        {
            var tagBuilder = new TagBuilder("a");
            tagBuilder.Attributes.Add("href", url);
            tagBuilder.Attributes.Add("class", "govuk-back-link weee-back-link");
            if (!string.IsNullOrWhiteSpace(alternativeDescription))
            {
                tagBuilder.SetInnerText(alternativeDescription);
            }
            else
            {
                tagBuilder.SetInnerText("Back");
            }

            return new MvcHtmlString(tagBuilder.ToString(TagRenderMode.Normal));
        }
    }
}