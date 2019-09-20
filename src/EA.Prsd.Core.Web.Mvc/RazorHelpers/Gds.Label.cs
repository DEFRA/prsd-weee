namespace EA.Prsd.Core.Web.Mvc.RazorHelpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Validation;

    public partial class Gds<TModel>
    {
        private static readonly string CssLabelClass = "govuk-label form-label";

        public MvcHtmlString LabelFor<TValue>(Expression<Func<TModel, TValue>> expression, bool showOptionalLabel)
        {
            return LabelFor(expression, new RouteValueDictionary(), CssLabelClass, string.Empty, showOptionalLabel);
        }

        public MvcHtmlString LabelFor<TValue>(Expression<Func<TModel, TValue>> expression, string optionalMessage = "")
        {
            return LabelFor(expression, new RouteValueDictionary(), optionalMessage);
        }

        public MvcHtmlString LabelFor<TValue>(Expression<Func<TModel, TValue>> expression, object htmlAttributes, string optionalMessage = "")
        {
            return LabelFor(expression, new RouteValueDictionary(htmlAttributes), optionalMessage);
        }

        public MvcHtmlString LabelFor<TValue>(Expression<Func<TModel, TValue>> expression, IDictionary<string, object> htmlAttributes, string optionalMessage = "")
        {
            return LabelFor(expression, htmlAttributes, CssLabelClass, optionalMessage);
        }

        private MvcHtmlString LabelFor<TValue>(Expression<Func<TModel, TValue>> expression, IDictionary<string, object> htmlAttributes, string cssClass, string optionalMessage = "", bool showOptionalLabel = true)
        {
            var modelMetadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);

            var containsRequiredIfAttribute = ContainsAttribute<RequiredIfAttribute>(modelMetadata, modelMetadata.PropertyName);

            string appendOptional;
            if (string.IsNullOrWhiteSpace(optionalMessage))
            {
                appendOptional = modelMetadata.IsRequired || containsRequiredIfAttribute || !showOptionalLabel ? string.Empty : "(optional)";
            }
            else
            {
                appendOptional = optionalMessage;
            }

            var htmlFieldName = ExpressionHelper.GetExpressionText(expression);

            var labelText = modelMetadata.DisplayName
                            ?? modelMetadata.PropertyName
                            ?? htmlFieldName.Split('.').Last();

            if (String.IsNullOrEmpty(labelText))
            {
                return MvcHtmlString.Empty;
            }

            var labelTag = new TagBuilder("label");

            labelTag.MergeAttributes(htmlAttributes);

            labelTag.Attributes.Add("for",
                htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(htmlFieldName));

            labelTag.InnerHtml = labelText + " " + appendOptional;

            labelTag.AddCssClass(cssClass);

            return MvcHtmlString.Create(labelTag.ToString(TagRenderMode.Normal));
        }

        private bool ContainsAttribute<T>(ModelMetadata modelMetadata, string propertyName)
        {
            var property = modelMetadata.ContainerType.GetProperties().Single(p => p.Name.Equals(propertyName));

            var attribute = Attribute.GetCustomAttribute(property, typeof(T));
            return attribute != null;
        }
    }
}