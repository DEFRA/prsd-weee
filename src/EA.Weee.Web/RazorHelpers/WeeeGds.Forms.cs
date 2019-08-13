﻿namespace EA.Weee.Web.RazorHelpers
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Web;
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

            string html = string.Format(@"<input type=""submit"" class=""govuk-button"" value=""{0}"" {1}/>", text, attributes.ToString());

            return new MvcHtmlString(html);
        }

        public MvcHtmlString Submit(string value)
        {
            var html = string.Format(@"<div class=""left-cleared""><input type=""submit"" class=""govuk-button"" value=""{0}"" {1}/>{2}</div>",
                value, AttributesHelper.AttributesHtml(null), SpinnerHtml(false));

            return new MvcHtmlString(html);
        }

        public MvcHtmlString Submit(string value, IDictionary<string, object> htmlAttributes = null, bool withSpinner = false)
        {
            var html = string.Format(@"<div class=""left-cleared""><input type=""submit"" class=""govuk-button"" value=""{0}"" {1}/>{2}</div>",
                value, AttributesHelper.AttributesHtml(htmlAttributes), SpinnerHtml(withSpinner));

            return new MvcHtmlString(html);
        }

        public MvcHtmlString Submit(string value, object htmlAttributes = null, bool withSpinner = false)
        {
            var html = string.Format(@"<div class=""left-cleared""><input type=""submit"" class=""govuk-button"" value=""{0}"" {1}/>{2}</div>",
                value, AttributesHelper.AttributesHtml(htmlAttributes), SpinnerHtml(withSpinner));

            return new MvcHtmlString(html);
        }

        private string SpinnerHtml(bool withSpinner)
        {
            if (!withSpinner)
            {
                return string.Empty;
            }

            var img = new TagBuilder("img");
            img.Attributes.Add("id", "spinner");
            img.Attributes.Add("src", VirtualPathUtility.ToAbsolute(@"~/Content/weee/images/spinner.gif"));
            img.Attributes.Add("alt", "spinning wheel");
            img.Attributes.Add("class", "spinner-image");

            return img.ToString(TagRenderMode.SelfClosing);
        }
    }
}