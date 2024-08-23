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

        public MvcHtmlString TextBoxFor<TValue>(Expression<Func<TModel, TValue>> expression, bool useHalfWidth = true, string displayFormat = null)
        {
            return TextBoxFor(expression, new RouteValueDictionary(), useHalfWidth, displayFormat);
        }

        public MvcHtmlString TextAreaFor<TValue>(Expression<Func<TModel, TValue>> expression, bool useHalfWidth = true, string displayFormat = null)
        {
            return TextAreaFor(expression, new RouteValueDictionary(), useHalfWidth, displayFormat);
        }

        public MvcHtmlString TextBoxFor<TValue>(Expression<Func<TModel, TValue>> expression, object htmlAttributes, bool useHalfWidth = true, string displayFormat = null)
        {
            var routeValueDictionary = System.Web.Mvc.HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            return TextBoxFor(expression, routeValueDictionary, useHalfWidth, displayFormat);
        }

        public MvcHtmlString TextBoxFor<TValue>(Expression<Func<TModel, TValue>> expression,
            IDictionary<string, object> htmlAttributes, bool useHalfWidth, string displayFormat)
        {
            GdsExtensions.AddFormControlCssClass(htmlAttributes, useHalfWidth);
            /* SG */
            GdsExtensions.AddClass(htmlAttributes, CssTextClass);

            if (!string.IsNullOrWhiteSpace(displayFormat))
            {
                return htmlHelper.TextBoxFor(expression, displayFormat, htmlAttributes);
            }
            return htmlHelper.TextBoxFor(expression, htmlAttributes);
        }

        public MvcHtmlString TextAreaFor<TValue>(Expression<Func<TModel, TValue>> expression,
         IDictionary<string, object> htmlAttributes, bool useHalfWidth, string displayFormat)
        {
            GdsExtensions.AddFormControlCssClass(htmlAttributes, useHalfWidth);
            /* SG */
            GdsExtensions.AddClass(htmlAttributes, CssTextClass);

            if (!string.IsNullOrWhiteSpace(displayFormat))
            {
                return htmlHelper.TextAreaFor(expression, htmlAttributes);
            }

            return htmlHelper.TextAreaFor(expression, htmlAttributes);
        }
    }
}