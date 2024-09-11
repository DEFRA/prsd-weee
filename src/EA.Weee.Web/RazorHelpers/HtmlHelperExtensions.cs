namespace EA.Weee.Web.RazorHelpers
{
    using EA.Weee.Core.Organisations;
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Web.Mvc;
    using System.Web.Mvc.Html;
    using EA.Weee.Core.Organisations.Base;

    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString EditorForAs<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            string htmlFieldName = null,
            object additionalViewData = null)
            where TProperty : OrganisationViewModel
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            var model = metadata.Model as OrganisationViewModel;
            if (model == null)
            {
                return MvcHtmlString.Empty;
            }

            // Use the CastToSpecificViewModel method
            var specificModel = model.CastToSpecificViewModel(model);
            var asType = specificModel.GetType();
            string templateName = GetTemplateNameFromType(asType);

            // Get the original property name
            var propertyName = ExpressionHelper.GetExpressionText(expression);

            // Create a new ViewData with the specific model
            var newViewData = new ViewDataDictionary(htmlHelper.ViewData) { Model = specificModel };

            // Create a new HtmlHelper with the original ViewData
            var newHtmlHelper = new HtmlHelper<object>(
                htmlHelper.ViewContext,
                new ViewDataContainer(newViewData),
                htmlHelper.RouteCollection);

            // Store the original HtmlFieldPrefix
            var originalPrefix = newHtmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix;

            try
            {
                // Temporarily set the HtmlFieldPrefix to include the property name
                newHtmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix =
                    string.IsNullOrEmpty(originalPrefix)
                        ? propertyName
                        : originalPrefix + "." + propertyName;

                // Render the editor
                return newHtmlHelper.EditorFor(x => x, templateName, htmlFieldName, additionalViewData);
            }
            finally
            {
                // Restore the original HtmlFieldPrefix
                newHtmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix = originalPrefix;
            }
        }

        private static string GetTemplateNameFromType(Type type)
        {
            if (type == typeof(RegisteredCompanyDetailsViewModel))
            {
                return "RegisteredCompanyDetailsViewModel";
            }

            if (type == typeof(PartnershipDetailsViewModel))
            {
                return "PartnershipDetailsViewModel";
            }

            if (type == typeof(SoleTraderDetailsViewModel))
            {
                return "SoleTraderDetailsViewModel";
            }

            return "RegisteredCompanyDetailsViewModel";
        }

        private class ViewDataContainer : IViewDataContainer
        {
            public ViewDataDictionary ViewData { get; set; }
            public ViewDataContainer(ViewDataDictionary viewData)
            {
                ViewData = viewData;
            }
        }
    }
}