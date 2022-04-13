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
        public MvcHtmlString DropDownListFor<TValue>(Expression<Func<TModel, TValue>> expression,
            IEnumerable<SelectListItem> selectList, bool withLookAhead = false, bool useHalfWidth = true)
        {
            return DropDownListFor(expression, selectList, new RouteValueDictionary(), withLookAhead, useHalfWidth);
        }

        public MvcHtmlString DropDownListFor<TValue>(Expression<Func<TModel, TValue>> expression,
            IEnumerable<SelectListItem> selectList,
            string optionLabel)
        {
            var routeValues = new RouteValueDictionary();
            GdsExtensions.AddFormControlCssClass(routeValues);
            GdsExtensions.AddClass(routeValues, "govuk-select");
            return htmlHelper.DropDownListFor(expression, selectList, optionLabel: optionLabel, htmlAttributes: routeValues);
        }

        public MvcHtmlString DropDownListFor<TValue>(Expression<Func<TModel, TValue>> expression,
            IEnumerable<SelectListItem> selectList, object htmlAttributes, bool withLookAhead = false, bool useHalfWidth = true)
        {
            var routeValueDictionary = System.Web.Mvc.HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            return DropDownListFor(expression, selectList, routeValueDictionary, withLookAhead, useHalfWidth);
        }

        public MvcHtmlString DropDownListFor<TValue>(Expression<Func<TModel, TValue>> expression,
            IEnumerable<SelectListItem> selectList,
            string optionLabel,
            object htmlAttributes,
            bool withLookAhead = false,
            bool useHalfWidth = true)
        {
            var routeValueDictionary = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            GdsExtensions.AddFormControlCssClass(routeValueDictionary, useHalfWidth);
            GdsExtensions.AddClass(routeValueDictionary, "govuk-select");
            AddLookAhead(expression, routeValueDictionary, withLookAhead);
            return htmlHelper.DropDownListFor(expression, selectList, optionLabel: optionLabel, htmlAttributes: routeValueDictionary);
        }

        public MvcHtmlString DropDownListFor<TValue>(Expression<Func<TModel, TValue>> expression,
            IEnumerable<SelectListItem> selectList,
            IDictionary<string, object> htmlAttributes,
            bool withLookAhead = false,
            bool useHalfWidth = true)
        {
            GdsExtensions.AddFormControlCssClass(htmlAttributes, useHalfWidth);
            GdsExtensions.AddClass(htmlAttributes, "govuk-select");

            AddLookAhead(expression, htmlAttributes, withLookAhead);
            return htmlHelper.DropDownListFor(expression, selectList, htmlAttributes);
        }

        private void AddLookAhead<TValue>(Expression<Func<TModel, TValue>> expression, IDictionary<string, object> htmlAttributes, bool withLookAhead)
        {
            if (withLookAhead)
            {
                GdsExtensions.AddClass(htmlAttributes, "gds-auto-complete");

                var htmlFieldName = ExpressionHelper.GetExpressionText(expression);
                var labeledBy = $"{htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(htmlFieldName)}-label";
                htmlAttributes.Add("aria-labelledby", labeledBy);
            }
        }
    }
}