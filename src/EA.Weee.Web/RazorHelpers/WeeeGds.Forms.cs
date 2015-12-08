namespace EA.Weee.Web.RazorHelpers
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
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

        public MvcHtmlString Submit(string value, IDictionary<string, object> htmlAttributes = null)
        {
            var html = string.Format(@"<div class=""form-submit""><input type=""submit"" value=""{0}"" {1}/></div>",
                value, AttributesHtml(htmlAttributes));

            return new MvcHtmlString(html);
        }

        public MvcHtmlString Submit(string value, object htmlAttributes = null)
        {
            var html = string.Format(@"<div class=""form-submit""><input type=""submit"" value=""{0}"" {1}/></div>",
                value, AttributesHtml(htmlAttributes));

            return new MvcHtmlString(html);
        }

        private string AttributesHtml(object htmlAttributes)
        {
            if (htmlAttributes == null)
            {
                return string.Empty;
            }

            var attributeBuilder = new StringBuilder();
            foreach (var prop in htmlAttributes.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                attributeBuilder.Append(string.Format(@"{0}=""{1}"" ", 
                    HtmlHelper.Encode(prop.Name),
                    HtmlHelper.Encode(prop.GetValue(htmlAttributes, null))));
            }

            return attributeBuilder.ToString();
        }

        private string AttributesHtml(IDictionary<string, object> htmlAttributes)
        {
            if (htmlAttributes == null)
            {
                return string.Empty;
            }

            var attributeBuilder = new StringBuilder();
            foreach (var attribute in htmlAttributes)
            {
                attributeBuilder.AppendFormat(@"{0}=""{1}"" ", 
                    HtmlHelper.Encode(attribute.Key),
                    HtmlHelper.Encode(attribute.Value));
            }

            return attributeBuilder.ToString();
        }
    }
}