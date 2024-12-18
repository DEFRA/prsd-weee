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

        public MvcHtmlString CheckBoxFrontEndFor(Expression<Func<TModel, bool>> expression,
    object htmlAttributes)
        {
            var routeValueDictionary = System.Web.Mvc.HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            return CheckBoxFrontEndFor(expression, routeValueDictionary);
        }

        public MvcHtmlString CheckBoxFrontEnd(Expression<Func<TModel, bool>> expression,
            IDictionary<string, object> htmlAttributes)
        {
            var checkbox = htmlHelper.CheckBoxFor(expression, htmlAttributes).ToHtmlString().Trim();
            var pureCheckBox = checkbox.Substring(0, checkbox.IndexOf("<input", 1));

            return new MvcHtmlString(pureCheckBox.ToString());
        }

        public MvcHtmlString CheckBoxFor(Expression<Func<TModel, bool>> expression,
            IDictionary<string, object> htmlAttributes)
        {
            var checkbox = htmlHelper.CheckBoxFor(expression, htmlAttributes);

            var labelTag = new TagBuilder("label");
            var checkboxName = ExpressionHelper.GetExpressionText(expression);

            labelTag.AddCssClass("form-checkbox");
            labelTag.MergeAttributes(System.Web.Mvc.HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
            labelTag.Attributes.Add("for",
                htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(checkboxName));

            labelTag.InnerHtml = checkbox.ToString() +
                                 LabelHelper(ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData),
                                     checkboxName);

            return new MvcHtmlString(labelTag.ToString());
        }

        public MvcHtmlString CheckBoxFrontEndFor(Expression<Func<TModel, bool>> expression,
            IDictionary<string, object> htmlAttributes,
            string hiddenLegendText = null)
        {
            // Generate the checkbox
            var checkbox = htmlHelper.CheckBoxFor(expression, htmlAttributes).ToHtmlString().Trim();

            // Extract the ID from the generated checkbox
            var idMatch = System.Text.RegularExpressions.Regex.Match(checkbox, @"id=['""]([^'""]+)['""]");
            var checkboxId = idMatch.Success ? idMatch.Groups[1].Value : string.Empty;

            var pureCheckBox = checkbox.Substring(0, checkbox.IndexOf("<input", 1));
            var labelTag = new TagBuilder("label");
            var fieldSetTag = new TagBuilder("fieldset");
            var checkboxName = ExpressionHelper.GetExpressionText(expression);

            fieldSetTag.AddCssClass("govuk-fieldset");
            labelTag.AddCssClass("govuk-label govuk-checkboxes__label");
            labelTag.MergeAttributes(System.Web.Mvc.HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));

            // Set the label's "for" attribute to match the checkbox's ID
            if (!string.IsNullOrEmpty(checkboxId))
            {
                labelTag.Attributes.Add("for", checkboxId);
            }

            labelTag.InnerHtml = LabelHelper(ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData), checkboxName).ToString();

            // Create hidden legend
            var legendHtml = string.Empty;
            if (!string.IsNullOrEmpty(hiddenLegendText))
            {
                var legendTag = new TagBuilder("legend");
                legendTag.AddCssClass("govuk-fieldset__legend govuk-visually-hidden");
                legendTag.InnerHtml = hiddenLegendText;
                legendHtml = legendTag.ToString();
            }

            fieldSetTag.InnerHtml = legendHtml +
                                    "<div class='govuk-checkboxes'><div class='govuk-checkboxes__item'>" +
                                    pureCheckBox + labelTag + "</div></div>";

            return new MvcHtmlString(fieldSetTag.ToString());
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