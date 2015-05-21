namespace EA.Prsd.Core.Web.Mvc.RazorHelpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Web.Mvc;
    using System.Web.Mvc.Html;
    using System.Web.Routing;

    public partial class Gds<TModel>
    {
        public MvcHtmlString CheckBoxFor(Expression<Func<TModel, bool>> expression)
        {
            return CheckBoxFor(expression, new RouteValueDictionary());
        }

        public MvcHtmlString CheckBoxFor(Expression<Func<TModel, bool>> expression,
            object htmlAttributes)
        {
            var routeValueDictionary = System.Web.Mvc.HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            return CheckBoxFor(expression, routeValueDictionary);
        }

        public MvcHtmlString CheckBoxFor(Expression<Func<TModel, bool>> expression,
            IDictionary<string, object> htmlAttributes)
        {
            var checkbox = HtmlHelper.CheckBoxFor(expression, htmlAttributes);

            var labelTag = new TagBuilder("label");
            var checkboxName = ExpressionHelper.GetExpressionText(expression);

            labelTag.AddCssClass("form-checkbox");
            labelTag.MergeAttributes(System.Web.Mvc.HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
            labelTag.Attributes.Add("for",
                HtmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(checkboxName));

            labelTag.InnerHtml = checkbox.ToString() +
                                 LabelHelper(ModelMetadata.FromLambdaExpression(expression, HtmlHelper.ViewData),
                                     checkboxName);

            return new MvcHtmlString(labelTag.ToString());
        }

        private static MvcHtmlString LabelHelper(ModelMetadata metadata, string fieldName)
        {
            string labelText;
            var displayName = metadata.DisplayName;

            if (displayName == null)
            {
                var propertyName = metadata.PropertyName;

                labelText = propertyName ?? fieldName.Split('.').Last();
            }
            else
            {
                labelText = displayName;
            }

            if (string.IsNullOrEmpty(labelText))
            {
                return MvcHtmlString.Empty;
            }

            return new MvcHtmlString(labelText);
        }
    }
}