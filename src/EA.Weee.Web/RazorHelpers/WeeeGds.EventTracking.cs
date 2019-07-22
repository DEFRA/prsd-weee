namespace EA.Weee.Web.RazorHelpers
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Web.Mvc;

    public partial class WeeeGds<TModel>
    {
        public MvcHtmlString CreateProgressiveDisclosure(string linkText, string hiddenContent, string eventCategory, string eventAction, string eventLabel, object htmlAttributes = null)
        {
            return new MvcHtmlString(string.Format(
                @"<details class=""govuk-details"" aria-live=""polite"" role=""group"" {1}><summary class=""govuk-details__summary""><span class=""govuk-details__summary-text"">{2}<span class=""hidden-for-screen-reader"">This is an expandable link that will reveal more content upon activation</span></span></summary><div class=""govuk-details__text""id=""details - content"" aria-hidden=""true""><div class=""govuk - body govuk - !-font - size - 16"">{3}</div></div></details>",
                EventTrackingFunction(eventCategory, eventAction, eventLabel), AttributesHelper.AttributesHtml(htmlAttributes), linkText, hiddenContent));
        }

        public MvcHtmlString CreateButtonWithEventTracking(string buttonText, string eventCategory, string eventAction, string eventLabel)
        {
            return new MvcHtmlString(string.Format(@"<button class=""govuk-button"" ""onclick=""{0}"" type=""submit"">{1}</button>", EventTrackingFunction(eventCategory, eventAction, eventLabel), buttonText));
        }

        public MvcHtmlString CreateLinkButtonWithEventTracking(string buttonText, string eventCategory, string eventAction, string eventLabel)
        {
            return new MvcHtmlString(string.Format(@"<button class=""link-submit"" ""onclick=""{0}"" type=""submit"">{1}</button>", EventTrackingFunction(eventCategory, eventAction, eventLabel), buttonText));
        }

        public MvcHtmlString ActionLinkWithEventTracking(string linkText,
            string explanationText,
            string url,
            string eventCategory,
            string eventAction,
            string eventLabel = null,
            IDictionary<string, object> htmlAttributes = null,
            bool newTab = false)
        {
            StringBuilder attributes = new StringBuilder();
            string additionalOnclickContent = string.Empty;
            if (htmlAttributes != null)
            {
                foreach (var item in htmlAttributes)
                {
                    if (string.Equals(item.Key, "onclick", StringComparison.InvariantCultureIgnoreCase))
                    {
                        additionalOnclickContent = item.Value.ToString();
                    }
                    else
                    {
                        attributes.AppendFormat(@"{0}=""{1}"" ", HtmlHelper.Encode(item.Key), HtmlHelper.Encode(item.Value));
                    }
                }
            }

            if (newTab)
            {
                if (htmlAttributes != null &&
                    htmlAttributes.ContainsKey("target"))
                {
                    throw new InvalidOperationException("A value for the target attribute has already been specified");
                }

                attributes.Append(@" target=""_blank"" ");
            }

            attributes.AppendFormat(@"onclick=""{0}{1}""", EventTrackingFunction(eventCategory, eventAction, eventLabel), additionalOnclickContent);

            if (explanationText == null)
            {
                explanationText = "This link opens in a new browser window";
            }

            string link = string.Format(@"<a href=""{0}"" {1}><span class=""hidden-for-screen-reader"">{2}</span>{3}</a>", url, attributes.ToString(), explanationText, linkText);

            return new MvcHtmlString(link);
        }
    }
}