namespace EA.Weee.Web.RazorHelpers
{
    using System.Web.Mvc;

    public partial class WeeeGds<TModel>
    {
        public MvcHtmlString CreateProgressiveDisclosure(string linkText, string hiddenContent, string eventCategory, string eventAction, string eventLabel)
        {
            return new MvcHtmlString(string.Format(
                @"<details class=""govuk-details"" aria-live=""polite"" role=""group""><summary class=""govuk-details__summary""><span class=""govuk-details__summary-text"">{1}<span class=""hidden-for-screen-reader"">This is an expandable link that will reveal more content upon activation</span></span></summary><div class=""govuk-details__text""id=""details - content"" aria-hidden=""true""><div class=""govuk - body govuk - !-font - size - 16"">{2}</div></div></details>",
                EventTrackingFunction(eventCategory, eventAction, eventLabel), linkText, hiddenContent));
        }
    }
}