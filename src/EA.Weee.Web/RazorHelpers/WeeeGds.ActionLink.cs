﻿namespace EA.Weee.Web.RazorHelpers
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Mvc.Html;
    using System.Web.Routing;

    public partial class WeeeGds<TModel>
    {
        public MvcHtmlString ActionLinkToNewTab(string linkText, string actionName, string controllerName, object routeValues = null)
        {
            var id = "external-link-" + Guid.NewGuid();
            var label =
                string.Format(
                    "<label for=\"{0}\"><span class=\"hidden-for-screen-reader\">This link opens in a new browser window</span></label>",
                    id);
            var link = HtmlHelper.ActionLink(linkText, actionName, controllerName, routeValues, new { @target = "_blank", id });

            return new MvcHtmlString(label + link);
        }

        public MvcHtmlString ActionLinkWithEventTracking(string linkText, string actionName, string controllerName,
                 string eventCategory, string eventAction, string eventLabel = null, RouteValueDictionary routeValues = null,
                 IDictionary<string, object> htmlAttributes = null, bool newTab = false)
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

            if (string.IsNullOrEmpty(eventLabel))
            {
                attributes.AppendFormat(@"onclick=""ga('send', 'event', '{0}', '{1}');{2}""", eventCategory, eventAction, additionalOnclickContent);
            }
            else
            {
                attributes.AppendFormat(@"onclick=""ga('send', 'event', '{0}', '{1}', '{2}');{3}""", eventCategory, eventAction, eventLabel, additionalOnclickContent);
            }

            var action = UrlHelper.Action(actionName, controllerName, routeValues);
            string link = string.Format(@"<a href=""{0}"" {1}>{2}</a>", action, attributes.ToString(), linkText);

            return new MvcHtmlString(link);
        }
    }
}