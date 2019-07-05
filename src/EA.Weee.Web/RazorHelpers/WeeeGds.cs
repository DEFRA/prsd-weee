﻿namespace EA.Weee.Web.RazorHelpers
{
    using System.Collections.Generic;
    using System.Web;
    using System.Web.Mvc;
    using Prsd.Core.Web.Mvc.RazorHelpers;

    public partial class WeeeGds<TModel>
    {
        public readonly HtmlHelper<TModel> HtmlHelper;
        public readonly UrlHelper UrlHelper;

        private readonly Gds<TModel> gdsHelper;
        public WeeeGds(WebViewPage<TModel> webViewPage)
        {
            HtmlHelper = webViewPage.Html;
            UrlHelper = webViewPage.Url;

            gdsHelper = new Gds<TModel>(webViewPage.Html);
        }

        public TableBuilder<TModel, TDataset> TableBuilder<TDataset>(IEnumerable<TDataset> dataset, string caption, object htmlAttributes = null)
        {
            return new TableBuilder<TModel, TDataset>(this, dataset, caption, htmlAttributes);
        }

        public string EventTrackingFunction(string eventCategory, string eventAction, string eventLabel = null)
        {
            string result;

            if (string.IsNullOrEmpty(eventLabel))
            {
                result = string.Format("ga('send', 'event', '{0}', '{1}');", HttpUtility.JavaScriptStringEncode(eventCategory), HttpUtility.JavaScriptStringEncode(eventAction));
            }
            else
            {
                result = string.Format("ga('send', 'event', '{0}', '{1}', '{2}');", HttpUtility.JavaScriptStringEncode(eventCategory), HttpUtility.JavaScriptStringEncode(eventAction), HttpUtility.JavaScriptStringEncode(eventLabel));
            }

            return result;
        }
    }
}