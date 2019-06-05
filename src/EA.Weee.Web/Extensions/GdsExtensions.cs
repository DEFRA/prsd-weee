namespace EA.Weee.Web.Extensions
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Web.Mvc;
    using EA.Prsd.Core.Web.Mvc.RazorHelpers;

    public static class GdsExtensions
    {
        public static MvcHtmlString LabelForOverridden<TModel, TValue>(this Gds<TModel> gds, Expression<Func<TModel, TValue>> expression, Type type)
        {
            var baseType = typeof(TModel);
            if (type.BaseType != baseType)
            {
                throw new ArgumentException($"The Gds model type ({baseType}) must be the base type of the provided type ({type})");
            }

            var labelFor = gds.LabelFor(expression, false);

            var propertyName = ExpressionHelper.GetExpressionText(expression);
            return ReplaceDisplayString(labelFor, type, propertyName);
        }

        private static MvcHtmlString ReplaceDisplayString(MvcHtmlString current, Type type, string propertyName)
        {
            var displayAttributes = type.GetProperty(propertyName).GetCustomAttributes(typeof(DisplayAttribute), false) as DisplayAttribute[];
            if (displayAttributes == null || !displayAttributes.Any() || string.IsNullOrWhiteSpace(displayAttributes[0].Name))
            {
                return current;
            }

            var currentString = current.ToString();

            var end = currentString.IndexOf("</label>");
            var afterDisplayString = currentString.Substring(end);

            var beforeDisplayString = currentString.Substring(0, end);
            var start = beforeDisplayString.LastIndexOf(">");
            beforeDisplayString = beforeDisplayString.Substring(0, start + 1);

            return MvcHtmlString.Create(beforeDisplayString + displayAttributes[0].Name + afterDisplayString);
        }
    }
}