namespace EA.Prsd.Core.Web.Mvc.RazorHelpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Web.Mvc;
    using System.Web.Mvc.Html;
    using System.Web.Routing;

    public partial class Gds<TModel>
    {
        private static readonly string CssTextClass = "govuk-input";

        public MvcHtmlString TextBoxFor<TValue>(Expression<Func<TModel, TValue>> expression)
        {
            return TextBoxFor(expression, new RouteValueDictionary());
        }

        public MvcHtmlString TextBoxFor<TValue>(Expression<Func<TModel, TValue>> expression, object htmlAttributes)
        {
            var routeValueDictionary = System.Web.Mvc.HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            return TextBoxFor(expression, routeValueDictionary);
        }

        public MvcHtmlString TextBoxFor<TValue>(Expression<Func<TModel, TValue>> expression,
            IDictionary<string, object> htmlAttributes)
        {
            GdsExtensions.AddFormControlCssClass(htmlAttributes);
            GdsExtensions.AddClass(htmlAttributes, CssTextClass);

            return htmlHelper.TextBoxFor(expression, htmlAttributes);
        }
    }
}