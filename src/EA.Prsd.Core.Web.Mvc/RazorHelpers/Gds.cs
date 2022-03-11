namespace EA.Prsd.Core.Web.Mvc.RazorHelpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Web.Mvc;

    public partial class Gds<TModel>
    {
        private readonly HtmlHelper<TModel> htmlHelper;

        public Gds(HtmlHelper<TModel> htmlHelper)
        {
            this.htmlHelper = htmlHelper;
        }

        /// <summary>
        /// Should return a property name which will be contained in the View Data dictionary for a property access expression.
        /// e.g. for:
        /// m => m.Name -> Name
        /// m => m.Name.Other -> Name.Other
        /// m => m.Name.Other.Last -> Name.Other.Last
        /// etc.
        /// </summary>
        protected static string GetPropertyName<TProperty>(HtmlHelper htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            var memberExpression = expression.Body as MemberExpression;
            string nameToCheck;

            if (memberExpression == null)
            {
                // This might be a MethodCallExpression such as list index access.
                var memberName = GetIndexedPropertyName(expression.Body);

                return memberName;
            }

            var memberExpressionAsString = memberExpression.ToString();
            nameToCheck = memberExpressionAsString.Substring(memberExpressionAsString.IndexOf('.') + 1);

            var propertyName = htmlHelper.ViewData.ModelMetadata.PropertyName;
            if (!string.IsNullOrWhiteSpace(propertyName))
            {
                nameToCheck = string.Format("{0}.{1}", propertyName, nameToCheck);
            }

            return nameToCheck;
        }

        /// <summary>
        /// Used to get the property name where the expression is a lambda including indexed access.
        /// For example:
        ///     m => m.PropertyList[i]
        /// Works by finding the value for i and then finding the correct name for PropertyList.
        /// </summary>
        /// <param name="expression">The body of the lambda expression.</param>
        /// <returns>A string representing the name of the property being accessed.</returns>
        protected static string GetIndexedPropertyName(Expression expression)
        {
            var methodCallExpression = expression as MethodCallExpression;

            // Return empty if this is not a method call or is not calling an indexed property.
            if (methodCallExpression == null || !methodCallExpression.ToString().ToUpperInvariant().Contains("GET_ITEM"))
            {
                return string.Empty;
            }

            var index = GetIndexFromExpression(methodCallExpression);

            var methodCallExpressionAsString = methodCallExpression.ToString();

            var substring = methodCallExpressionAsString.Substring(methodCallExpressionAsString.IndexOf('.') + 1);

            substring = substring.Substring(0, substring.IndexOf('.'));

            substring += string.Format("[{0}]", index);

            return substring;
        }

        /// <summary>
        /// Returns the index of the IList requested in the expression.
        /// Where this is constant it returns the index simply.
        /// If the index is in a modified closure it must be compiled in order to return a value.
        /// </summary>
        /// <param name="methodCallExpression">The property access as an expression.</param>
        /// <returns>The integer index as a string.</returns>
        private static string GetIndexFromExpression(MethodCallExpression methodCallExpression)
        {
            var indexerArgument = methodCallExpression.Arguments.FirstOrDefault();

            var constExpression = indexerArgument as ConstantExpression;

            if (constExpression != null)
            {
                return constExpression.Value.ToString();
            }

            var memberExpression = indexerArgument as MemberExpression;

            if (memberExpression == null)
            {
                return string.Empty;
            }

            var memberExpressionAsObjectMember = Expression.Convert(memberExpression, typeof(object));
            var getIndex = Expression.Lambda<Func<object>>(memberExpressionAsObjectMember).Compile();

            return getIndex().ToString();
        }
    }
}