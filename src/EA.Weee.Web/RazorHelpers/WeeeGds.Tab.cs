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
            if (isActive)
            {
                linkBuilder.AddCssClass("govuk-tabs__tab--selected");
            }
            linkBuilder.SetInnerText(displayText);

            var tagBuilder = new TagBuilder("li") { InnerHtml = linkBuilder.ToString() };
            tagBuilder.AddCssClass("govuk-tabs__list-item");

            return new MvcHtmlString(tagBuilder.ToString());
        }
    }
}