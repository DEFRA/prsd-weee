namespace EA.Weee.Web.RazorHelpers
{
    using System.Web.Mvc;

    public partial class WeeeGds<TModel>
    {
        public MvcHtmlString CreateButtonWithEventTracking(string buttonText, string eventCategory, string eventAction, string eventLabel)
        {
            return new MvcHtmlString(string.Format(@"<button class=""govuk-button"" ""onclick=""{0}"" type=""submit"">{1}</button>", EventTrackingFunction(eventCategory, eventAction, eventLabel), buttonText));
        }

        public MvcHtmlString CreateLinkButtonWithEventTracking(string buttonText, string eventCategory, string eventAction, string eventLabel)
        {
            return new MvcHtmlString(string.Format(@"<button class=""link-submit"" ""onclick=""{0}"" type=""submit"">{1}</button>", EventTrackingFunction(eventCategory, eventAction, eventLabel), buttonText));
        }
    }
}