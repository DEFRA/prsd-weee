namespace EA.Prsd.Core.Web.Mvc.RazorHelpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;
    using System.Web.Mvc;
    using System.Web.Mvc.Html;

    public partial class Gds<TModel>
    {
        /// <summary>
        ///     This extension will provide a GDS compliant validation summary in the waste carriers style.
        /// </summary>
        /// <returns>A div containing the list of validation errors if applicable.</returns>
        public MvcHtmlString ValidationSummary()
        {
            var modelStateDictionary = htmlHelper.ViewData.ModelState;

            var modelErrors = GetErrorsForModel(modelStateDictionary);

            if (modelStateDictionary == null || modelStateDictionary.Count == 0 || !modelErrors.Any())
            {
                return new MvcHtmlString(GetJavascriptEnabledBlankSummary());
            }

            // If the ModelState has errors it has been through an HTML post and javascript is disabled.
            return new MvcHtmlString(GetJavascriptDisabledErrorSummary(modelErrors));
        }

        /// <summary>
        ///     This extension will highlight any validation error input boxes.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public string FormGroupClass<TProperty>(Expression<Func<TModel, TProperty>> expression)
        {
            var nameToCheck = GetPropertyName(htmlHelper, expression);

            if (string.IsNullOrWhiteSpace(nameToCheck))
            {
                return null;
            }

            var modelState = htmlHelper.ViewData.ModelState[nameToCheck];

            if (modelState == null)
            {
                return null;
            }

            var cssClass = (modelState.Errors.Count > 0) ? "govuk-form-group govuk-form-group--error error" : null;

            return cssClass;
        }

        public string FormGroupClass<TProperty, TList>(Expression<Func<TModel, TList>> methodExpression, Expression<Func<TModel, TProperty>> expression)
        {
            var indexProperty = GetIndexedPropertyName(methodExpression.Body);

            if (!(expression.Body is MemberExpression expressionBody))
            {
                return null;
            }

            // CHECK THIS
            var nameToCheck = $"{indexProperty}.{expressionBody.Member.Name}";

            if (string.IsNullOrWhiteSpace(nameToCheck))
            {
                return null;
            }

            var modelState = htmlHelper.ViewData.ModelState[nameToCheck];

            if (modelState == null)
            {
                return null;
            }

            var cssClass = (modelState.Errors.Count > 0) ? "govuk-form-group--error error" : null;

            return cssClass;
        }

        public MvcHtmlString ValidationMessageFor<TValue>(Expression<Func<TModel, TValue>> expression)
        {
            var generatedHtml = htmlHelper.ValidationMessageFor(expression, null,
                new { @class = "govuk-error-message error-message" }, "p");

            if (generatedHtml != null)
            {
                return AppendHiddenErrorText(generatedHtml);
            }
            
            return null;
        }

        private static MvcHtmlString AppendHiddenErrorText(MvcHtmlString html2)
        {
            var node = HtmlAgilityPack.HtmlNode.CreateNode(html2.ToHtmlString());

            var spanBuilder = new TagBuilder("span");
            spanBuilder.AddCssClass("govuk-visually-hidden");
            spanBuilder.SetInnerText("Error:");

            node.InnerHtml = spanBuilder + node.InnerHtml;
            return new MvcHtmlString(node.OuterHtml);
        }

        public MvcHtmlString ValidationMessageFor<TValue>(Expression<Func<TModel, TValue>> expression,
            string validationMessage)
        {
            var generatedHtml = htmlHelper.ValidationMessageFor(expression, validationMessage, new { @class = "govuk-error-message error-message" },
                "p");

            if (generatedHtml != null)
            {
                return AppendHiddenErrorText(generatedHtml);
            }

            return null;
        }

        private string GetJavascriptEnabledBlankSummary()
        {
            return @"<div class='error-summary-valid govuk-error-summary__body' data-valmsg-summary='true' role='alert' aria-atomic='true'></div>";
        }

        private string GetJavascriptDisabledErrorSummary(IEnumerable<ModelErrorWithFieldId> modelErrors)
        {
            var startErrorRegion = @"<div class='error-summary govuk-error-summary' id='error_explanation' data-module='govuk-error-summary' aria-labelledby='error-summary-title'  role='alert' aria-atomic='true'>";

            var errorTitle = GetErrorSummaryHeading(modelErrors);

            var errorList = GetErrorSummaryList(modelErrors);

            var endErrorRegion = "</div>";

            var summaryBuilder = new StringBuilder();

            summaryBuilder.Append(startErrorRegion)
                .Append(errorTitle)
                .Append(errorList)
                .Append(endErrorRegion);

            return summaryBuilder.ToString();
        }

        private string GetErrorSummaryList(IEnumerable<ModelErrorWithFieldId> modelErrors)
        {
            var errorListBuilder = new StringBuilder();
            errorListBuilder.AppendLine("<ul class='govuk-list govuk-error-summary__list error-summary-list'>");

            foreach (var modelError in modelErrors)
            {
                string fieldName = modelError.FieldId;
                if (!string.IsNullOrEmpty(System.Web.Mvc.HtmlHelper.IdAttributeDotReplacement))
                {
                    fieldName = fieldName.Replace(".", System.Web.Mvc.HtmlHelper.IdAttributeDotReplacement);
                    fieldName = fieldName.Replace("[", System.Web.Mvc.HtmlHelper.IdAttributeDotReplacement);
                    fieldName = fieldName.Replace("]", System.Web.Mvc.HtmlHelper.IdAttributeDotReplacement);
                }
                
                errorListBuilder.AppendLine("<li>");
                errorListBuilder.AppendFormat("<a href=\"#{0}\">{1}</a>", fieldName,
                    modelError.ModelError.ErrorMessage);
                errorListBuilder.AppendLine("</li>");
            }

            errorListBuilder.AppendLine("</ul>");

            return errorListBuilder.ToString();
        }

        private string GetErrorSummaryHeading(IEnumerable<ModelErrorWithFieldId> modelErrors)
        {
            var tagBuilder = new TagBuilder("h2");

            tagBuilder.AddCssClass("govuk-error-summary__title");
            tagBuilder.AddCssClass("heading-medium");
            tagBuilder.AddCssClass("error-summary-heading");
            tagBuilder.Attributes.Add("id", "error-summary-title");

            tagBuilder.SetInnerText("There is a problem");

            return tagBuilder.ToString();
        }

        private IEnumerable<ModelErrorWithFieldId> GetErrorsForModel(ModelStateDictionary modelStateDictionary)
        {
            // TODO: Our current method of finding the field ID is unlikely to work for arrays or complex types.
            foreach (var key in modelStateDictionary.Keys)
            {
                // Required for closure.
                var keyName = key;

                foreach (var error in modelStateDictionary[key].Errors)
                {
                    yield return new ModelErrorWithFieldId(error, key);
                }
            }
        }

        private class ModelErrorWithFieldId
        {
            public ModelErrorWithFieldId(ModelError modelError, string fieldId)
            {
                ModelError = modelError;
                FieldId = fieldId;
            }

            public ModelError ModelError { get; private set; }

            public string FieldId { get; private set; }
        }
    }
}