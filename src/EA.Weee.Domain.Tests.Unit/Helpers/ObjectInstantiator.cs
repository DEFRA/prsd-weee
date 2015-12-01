namespace EA.Weee.Domain.Tests.Unit.Helpers
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.Serialization;

    public class ObjectInstantiator<T>
    {
        public static readonly Func<T> CreateNew = Creator();

        private static Func<T> Creator()
        {
            var type = typeof(T);

            if (type.IsValueType || type.GetConstructor(Type.EmptyTypes) != null)
            {
                return Expression.Lambda<Func<T>>(Expression.New(type)).Compile();
            }
            else if (type == typeof(string))
            {
                throw new NotImplementedException();
            }
            else
            {
                return () => (T)FormatterServices.GetUninitializedObject(type);
            }
        }

        public static void SetProperty<TProperty>(Expression<Func<T, TProperty>> expression, TProperty value,
            T instance)
        {
            var memberExpression = (MemberExpression)expression.Body;
            var property = (PropertyInfo)memberExpression.Member;
            var setMethod = property.GetSetMethod(true);

            var parameterT = Expression.Parameter(typeof(T), "x");
            var parameterTProperty = Expression.Parameter(typeof(TProperty), "y");

            var newExpression =
                Expression.Lambda<Action<T, TProperty>>(
                    Expression.Call(parameterT, setMethod, parameterTProperty),
                    parameterT,
                    parameterTProperty);

            var setter = newExpression.Compile();
            setter.Invoke(instance, value);
        }
    }
}