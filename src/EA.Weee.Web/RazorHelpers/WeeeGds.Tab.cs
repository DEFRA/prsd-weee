namespace EA.Weee.Web.RazorHelpers
{
    using System.Web.Mvc;

    public partial class WeeeGds<TModel>
    {
        public MvcHtmlString Tab(string displayText, string url, bool isActive, string id = null)   
        {
            var linkBuilder = new TagBuilder("a");
            linkBuilder.Attributes.Add("href", url);
            if (!string.IsNullOrWhiteSpace(id))
            {
                linkBuilder.Attributes.Add("id", id);
            }
            linkBuilder.AddCssClass("govuk-tabs__tab");
            linkBuilder.Attributes.Add("role", "tab");

            if (isActive)
            {
                linkBuilder.Attributes.Add("tabindex", "0");
                linkBuilder.Attributes.Add("aria-selected", "true");
            }
            else
            {
                linkBuilder.Attributes.Add("tabindex", "-1");
                linkBuilder.Attributes.Add("aria-selected", "false");
            }

            linkBuilder.SetInnerText(displayText);

            var tagBuilder = new TagBuilder("li") { InnerHtml = linkBuilder.ToString() };

            if (isActive)
            {
                tagBuilder.AddCssClass("govuk-tabs__list-item--selected");
            }
            tagBuilder.AddCssClass("govuk-tabs__list-item");
            tagBuilder.Attributes.Add("role", "presentation");

            return new MvcHtmlString(tagBuilder.ToString());
        }
    }
}