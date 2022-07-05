namespace EA.Weee.Web.RazorHelpers
{
    using System.Web.Mvc;

    public partial class WeeeGds<TModel>
    {
        public MvcHtmlString Warning(string displayText)
        {
            var outerDiv = new TagBuilder("div");
            outerDiv.AddCssClass("govuk-warning-text");

            var innerSpan = new TagBuilder("span");
            innerSpan.AddCssClass("govuk-warning-text__icon");
            innerSpan.Attributes.Add("aria-hidden", "true");
            innerSpan.SetInnerText("!");

            var innerStrong = new TagBuilder("strong");
            innerStrong.AddCssClass("govuk-warning-text__text");

            var innerStrongSpan = new TagBuilder("span");
            innerStrongSpan.AddCssClass("govuk-warning-text__assistive");
            innerStrongSpan.SetInnerText("Warning");

            innerStrong.InnerHtml += innerStrongSpan.ToString();
            innerStrong.InnerHtml += displayText;

            outerDiv.InnerHtml += innerSpan.ToString();
            outerDiv.InnerHtml += innerStrong.ToString();

            return new MvcHtmlString(outerDiv.ToString());
        }
    }
}