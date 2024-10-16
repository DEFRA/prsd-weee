﻿namespace EA.Prsd.Core.Web.Mvc.RazorHelpers
{
    using System;
    using System.Linq.Expressions;
    using System.Web.Mvc;

    public partial class Gds<TModel>
    {
        public MvcHtmlString DisplayFor<TValue>(Expression<Func<TModel, TValue>> expression, string value, string widthCss = "govuk-grid-column-one-third")
        {
            var modelMetadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            var labelSpan = new TagBuilder("span");
            var displaySpan = new TagBuilder("span");
            var spanName = ExpressionHelper.GetExpressionText(expression);
            var labelText = modelMetadata.DisplayName
                            ?? modelMetadata.PropertyName;

            labelSpan.AddCssClass("govuk-label--s");
            labelSpan.AddCssClass(widthCss);

            labelSpan.Attributes.Add("for",
                htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(spanName));

            var htmlFieldName = ExpressionHelper.GetExpressionText(expression);
            labelSpan.Attributes.Add("id", $"{htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(htmlFieldName)}-label");
            labelSpan.SetInnerText(labelText);

            displaySpan.AddCssClass("govuk-body");

            var labeledBy = $"{htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(htmlFieldName)}-label";

            displaySpan.Attributes.Add("aria-labelledby", labeledBy);
            displaySpan.Attributes.Add("id", spanName);
            displaySpan.SetInnerText(value);

            return new MvcHtmlString($"{labelSpan}{displaySpan}");
        }
    }
}