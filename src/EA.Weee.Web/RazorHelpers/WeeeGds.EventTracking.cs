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
            if (!string.IsNullOrWhiteSpace(eventCategory))
            {
                return new MvcHtmlString(
                    $@"<details class=""govuk-details"" data-module=""govuk-details"" aria-live=""polite"" onclick=""{EventTrackingFunction(eventCategory, eventAction, eventLabel)}""><summary class=""govuk-details__summary""><span class=""govuk-details__summary-text"">{linkText}<span class=""govuk-visually-hidden"">This is an expandable link that will reveal more content upon activation</span></span></summary><div class=""govuk-body govuk-details__text"">{hiddenContent}</div></details>");
            }

            return new MvcHtmlString(
                $@"<details class=""govuk-details"" data-module=""govuk-details"" aria-live=""polite""><summary class=""govuk-details__summary""><span class=""govuk-details__summary-text"">{linkText}<span class=""govuk-visually-hidden"">This is an expandable link that will reveal more content upon activation</span></span></summary><div class=""govuk-body govuk-details__text"">{hiddenContent}</div></details>");
        }

        public MvcHtmlString CreateButtonWithEventTracking(string buttonText, string eventCategory, string eventAction, string eventLabel)
        {
            return new MvcHtmlString(
                $@"<button class=""govuk-button"" data-module=""govuk-button"" onclick=""{EventTrackingFunction(eventCategory, eventAction, eventLabel)}"" type=""submit"" data-prevent-double-click=""true"" >{buttonText}</button>");
        }

        public MvcHtmlString CreateLinkButtonWithEventTracking(string buttonText, string eventCategory, string eventAction, string eventLabel)
        {
            return new MvcHtmlString(
                $@"<button class=""link-submit"" data-module=""govuk-button"" onclick=""{EventTrackingFunction(eventCategory, eventAction, eventLabel)}"" type=""submit"" data-prevent-double-click=""true"" >{buttonText}</button>");
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

            const string newBrowserText = "This link opens in a new browser window";

            if (explanationText == null)
            {
                explanationText = "This link opens in a new browser window";
            }
            else
            {
                if (newTab)
                {
                    explanationText = $"{explanationText}. {newBrowserText}";
                }
            }
            string link =
                $@"<a href=""{url}"" {attributes.ToString()}><span class=""govuk-visually-hidden"">{explanationText}</span>{linkText}</a>";

            return new MvcHtmlString(link);
        }
    }
}