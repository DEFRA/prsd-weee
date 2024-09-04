namespace EA.Weee.Web.RazorHelpers
{
    using System;
    using System.Linq.Expressions;
    using System.Web.Mvc;
    using System.Web.Mvc.Html;
    using Prsd.Core.Web.Mvc.RazorHelpers;

    public partial class WeeeGds<TModel>
    {
        private const string CssTextClass = "govuk-input";
        private const string CssWeeeIconClass = "weee-icon";
        private const string CssFontAwesomeClass = "fa";
        private const string CssFontAwesomeSizeClass = "fa-2x";
        private const string CssBackToTopClass = "back-to-top";
        private const string CssFontAwesomeUpIcon = "fa-arrow-up";

        public MvcHtmlString TextBoxWithIconFor<TValue>(Expression<Func<TModel, TValue>> expression,
            object htmlAttributes, string icon)
        {
            var routeValueDictionary = System.Web.Mvc.HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            var divTagBuilder = new TagBuilder("div");
            var iconTagBuilder = new TagBuilder("i");
            iconTagBuilder.AddCssClass(CssWeeeIconClass);
            iconTagBuilder.AddCssClass(CssFontAwesomeSizeClass);
            iconTagBuilder.AddCssClass(icon);
            iconTagBuilder.AddCssClass(CssFontAwesomeClass);
            
            divTagBuilder.InnerHtml += iconTagBuilder.ToString();

            GdsExtensions.AddClass(routeValueDictionary, CssTextClass);
            
            var textBox = HtmlHelper.TextBoxFor(expression, routeValueDictionary);
            divTagBuilder.InnerHtml += textBox.ToHtmlString();
            return new MvcHtmlString(divTagBuilder.InnerHtml);
        }

        public MvcHtmlString BackToTopLink()
        {
            var linkTagBuilder = new TagBuilder("a")
            {
                Attributes =
                {
                    ["href"] = "#",
                    ["onclick"] = "window.scrollTo(0, 0); return false;"
                }
            };
            linkTagBuilder.AddCssClass(CssBackToTopClass);

            var iconTagBuilder = new TagBuilder("i");
            iconTagBuilder.AddCssClass(CssFontAwesomeUpIcon);
            iconTagBuilder.AddCssClass(CssFontAwesomeClass);

            linkTagBuilder.InnerHtml += iconTagBuilder.ToString(TagRenderMode.Normal);
            linkTagBuilder.InnerHtml += "<span>Back to Top</span>";

            return new MvcHtmlString(linkTagBuilder.ToString(TagRenderMode.Normal));
        }
    }
}