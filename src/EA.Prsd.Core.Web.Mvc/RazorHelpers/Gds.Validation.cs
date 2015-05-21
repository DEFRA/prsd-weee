namespace EA.Prsd.Core.Web.Mvc.RazorHelpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;
    using System.Web.Mvc;

    public partial class Gds<TModel>
    {
        /// <summary>
        ///     This extension will provide a GDS compliant validation summary in the waste carriers style.
        /// </summary>
        /// <returns>A div containing the list of validation errors if applicable.</returns>
        public MvcHtmlString ValidationSummary()
        {
            var modelStateDictionary = HtmlHelper.ViewData.ModelState;

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
            var memberExpression = expression.Body as MemberExpression;
            string nameToCheck;

            if (memberExpression == null)
            {
                return string.Empty;
            }

            // We are accessing a sub property, the model state records the fully qualified name.
            // For the expression m.Class.Property:
            // Class.Property rather than Property
            if (memberExpression.ToString().Count(me => me == '.') == 2)
            {
                var memberExpressionAsString = memberExpression.ToString();
                nameToCheck = memberExpressionAsString.Substring(memberExpressionAsString.IndexOf('.') + 1);
            }
            else
            {
                var property = memberExpression.Member as PropertyInfo;

                if (property == null)
                {
                    return string.Empty;
                }

                nameToCheck = property.Name;
            }

            var modelState = HtmlHelper.ViewData.ModelState[nameToCheck];

            if (modelState == null)
            {
                return string.Empty;
            }

            var cssClass = (modelState.Errors.Count > 0) ? "form-group-error" : string.Empty;

            return cssClass;
        }

        private string GetJavascriptEnabledBlankSummary()
        {
            return @"<div class='validation-summary-valid' data-valmsg-summary='true'>
                        <ul>
                            <li style='display:none'></li>
                        </ul>
                    </div>";
        }

        private string GetJavascriptDisabledErrorSummary(IEnumerable<ModelErrorWithFieldId> modelErrors)
        {
            var startErrorRegion = @"<div class='validation-summary' id='error_explanation'>";

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

            errorListBuilder.AppendLine("<ul>");

            foreach (var modelError in modelErrors)
            {
                errorListBuilder.AppendLine("<li>");
                errorListBuilder.AppendFormat("<a class=\"error-text\" href=\"#{0}\">{1}</a>", modelError.FieldId,
                    modelError.ModelError.ErrorMessage);
                errorListBuilder.AppendLine("</li>");
            }

            errorListBuilder.AppendLine("</ul>");

            return errorListBuilder.ToString();
        }

        private string GetErrorSummaryHeading(IEnumerable<ModelErrorWithFieldId> modelErrors)
        {
            var modelErrorsCount = modelErrors.Count();

            var tagBuilder = new TagBuilder("h2");

            tagBuilder.AddCssClass("heading-small");

            var modelErrorsCountString = string.Empty;
            if (modelErrorsCount > 1)
            {
                modelErrorsCountString = modelErrorsCount + " errors";
            }
            else
            {
                modelErrorsCountString = "1 error";
            }

            tagBuilder.SetInnerText(string.Format("You have {0} on this page", modelErrorsCountString));

            return tagBuilder.ToString();
        }

        private TagBuilder GetErrorSummaryDiv()
        {
            var tagBuilder = new TagBuilder("div");

            tagBuilder.AddCssClass("validation-summary");

            tagBuilder.GenerateId("error_explanation");

            return tagBuilder;
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