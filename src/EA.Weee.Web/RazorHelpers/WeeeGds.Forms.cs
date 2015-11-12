namespace EA.Weee.Web.RazorHelpers
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Web.Mvc;

    public partial class WeeeGds<TModel>
    {
        public MvcHtmlString SubmitWithEventTracking(string text, string eventCategory, string eventAction, string eventLabel = null,
            IDictionary<string, object> htmlAttributes = null)
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

            attributes.AppendFormat(@"onclick=""{0}{1}""", EventTrackingFunction(eventCategory, eventAction, eventLabel), additionalOnclickContent);

            string html = string.Format(@"<input type=""submit"" value=""{0}"" {1}/>", text, attributes.ToString());

            return new MvcHtmlString(html);
        }
    }
}