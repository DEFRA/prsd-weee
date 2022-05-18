namespace EA.Weee.Web.RazorHelpers
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Web;
    using System.Web.Mvc;
    using Prsd.Core.Web.Mvc.RazorHelpers;

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

            var html = $@"<input type=""submit"" class=""govuk-button"" value=""{text}"" {attributes.ToString()}/>";

            return new MvcHtmlString(html);
        }

        public MvcHtmlString Submit(string value)
        {
            var html =
                $@"<div class=""left-cleared""><input type=""submit"" class=""govuk-button"" value=""{value}"" {AttributesHelper.AttributesHtml(null)}/>{SpinnerHtml(false)}</div>";

            return new MvcHtmlString(html);
        }

        public MvcHtmlString Submit(string value, IDictionary<string, object> htmlAttributes = null, bool withSpinner = false)
        {
            var html =
                $@"<div class=""left-cleared""><input type=""submit"" class=""govuk-button"" value=""{value}"" {AttributesHelper.AttributesHtml(htmlAttributes)}/>{SpinnerHtml(withSpinner)}</div>";

            return new MvcHtmlString(html);
        }

        public MvcHtmlString Submit(string value, object htmlAttributes = null, bool withSpinner = false)
        {
            var html =
                $@"<div class=""left-cleared""><input type=""submit"" class=""govuk-button"" value=""{value}"" {AttributesHelper.AttributesHtml(htmlAttributes)}/>{SpinnerHtml(withSpinner)}</div>";

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

        public MvcHtmlString Button(string innerHtml,
            object htmlAttributes = null, bool secondaryButton = false, bool filterButton = false)
        {
            return Button(innerHtml, System.Web.Mvc.HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes), secondaryButton, filterButton);
        }

        public MvcHtmlString Button(string innerHtml, IDictionary<string, object> htmlAttributes, bool secondaryButton = false, bool filterButton = false)
        {
            var builder = new TagBuilder("button")
            {
                InnerHtml = innerHtml
            };
            if (secondaryButton)
            {
                builder.AddCssClass("govuk-button--secondary");
            }
            if (filterButton)
            {
                builder.AddCssClass("govuk-button--secondary-filter");
            }
            builder.AddCssClass("govuk-button");
            builder.Attributes.Add("data-module", "govuk-button");
            builder.Attributes.Add("data-prevent-double-click", "true");
            builder.Attributes.Add("type", "submit");
            builder.MergeAttributes(htmlAttributes);
            
            return MvcHtmlString.Create(builder.ToString());
        }
    }
}