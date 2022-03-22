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

        public MvcHtmlString TextBoxFor<TValue>(Expression<Func<TModel, TValue>> expression, bool useHalfWidth = true)
        {
            return TextBoxFor(expression, new RouteValueDictionary(), useHalfWidth);
        }

        public MvcHtmlString TextBoxFor<TValue>(Expression<Func<TModel, TValue>> expression, object htmlAttributes, bool useHalfWidth = true)
        {
            var routeValueDictionary = System.Web.Mvc.HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            return TextBoxFor(expression, routeValueDictionary, useHalfWidth);
        }

        public MvcHtmlString TextBoxFor<TValue>(Expression<Func<TModel, TValue>> expression,
            IDictionary<string, object> htmlAttributes, bool useHalfWidth)
        {
            AddFormControlCssClass(htmlAttributes, useHalfWidth);
    /* SG */
            GdsExtensions.AddClass(htmlAttributes, CssTextClass);

            return htmlHelper.TextBoxFor(expression, htmlAttributes);
        }
    }
}