namespace EA.Prsd.Core
{
    using System;
    using System.Linq.Expressions;

    public static class ExpressionHelper
    {
        /// <summary>
        /// Creates a selector Expression to a private or protected property.
        /// </summary>
        /// <typeparam name="TSource">The type of the source object.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="propertyName">The name of the private or protected property.</param>
        /// <returns>a selector Expression for the private or protected property.</returns>
        /// <remarks>
        /// For a public property you can use the lambda syntax, for example x => x.Property.
        /// If the property is private or protected then this syntax is not available. This
        /// method provides a way to create a selector to the property by using
        /// GetPrivatePropertyExpression<MyObject, string>("PrivateProperty"). This is useful
        /// for mapping protected properties in Entity Framework.
        /// </remarks>
        public static Expression<Func<TSource, TProperty>> GetPrivatePropertyExpression<TSource, TProperty>(string propertyName)
        {
            var param =
                Expression.Parameter(typeof(TSource), "arg");

            var member =
                Expression.Property(param, propertyName);

            var lambda =
                Expression.Lambda(typeof(Func<TSource, TProperty>), member, param);

            return (Expression<Func<TSource, TProperty>>)lambda;
        }
    }
}