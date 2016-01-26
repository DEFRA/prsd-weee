namespace EA.Weee.Web.RazorHelpers
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Web.Mvc;
    using System.Web.Mvc.Html;

    public partial class WeeeGds<TModel>
    {
        public MvcHtmlString ActionLinkToNewTab(string linkText, string actionName, string controllerName, object routeValues = null)
        {
            string span = "<span class=\"hidden-for-screen-reader\">This link opens in a new browser window</span>";
            string link = HtmlHelper.ActionLink(linkText, actionName, controllerName, routeValues, new { @target = "_blank" }).ToString();

            return new MvcHtmlString(span + link);
        }

        public MvcHtmlString ActionLinkWithEventTracking(string linkText,
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

            string link = string.Format(@"<a href=""{0}"" {1}>{2}</a>", url, attributes.ToString(), linkText);

            return new MvcHtmlString(link);
        }
    }
}