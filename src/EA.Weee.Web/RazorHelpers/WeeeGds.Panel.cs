namespace EA.Weee.Web.RazorHelpers
{
    using System.Web.Mvc;

    public partial class WeeeGds<TModel>
    {
        public MvcHtmlString Panel(string displayText, string headerText = null)
        {
            var outerDiv = new TagBuilder("div");
            outerDiv.AddCssClass("govuk-panel--confirmation");
            outerDiv.AddCssClass("govuk-panel");

            if (!string.IsNullOrWhiteSpace(headerText))
            {
                var header = new TagBuilder("h1");
                header.AddCssClass("govuk-panel__title");
                header.SetInnerText(headerText);
                outerDiv.InnerHtml += header.ToString();
            }

            var bodyDiv = new TagBuilder("div");
            bodyDiv.AddCssClass("govuk-panel__body");
            bodyDiv.SetInnerText(displayText);

            outerDiv.InnerHtml += bodyDiv.ToString();

            return new MvcHtmlString(outerDiv.ToString());
        }
    }
}