namespace EA.Prsd.Core
{
    using System;
    using System.Linq.Expressions;

    public static class Guard
    {
        public static void ArgumentNotNull<T>(T value) where T : class 
        {
            if (value == null)
            {
                throw new ArgumentNullException(typeof(T).Name);
            }
        }

        public static void ArgumentNotZeroOrNegative(Expression<Func<int>> reference, int value)
        {
            if (value <= 0)
            {
                throw new ArgumentOutOfRangeException(GetParameterName(reference), "Parameter cannot be less than or equal to zero.");
            }
        }

        public static void ArgumentNotDefaultValue<T>(Expression<Func<T>> reference, T value)
        {
            if (!IsValueDefined<T>(value))
            {
                throw new ArgumentException("Argument cannot be default value.", GetParameterName(reference));
            }
        }

        private static bool IsValueDefined<T>(T value)
        {
            if (typeof(T).IsValueType)
            {
                return !value.Equals(default(T));
            }
            else
            {
                return value != null;
            }
        }

        private static string GetParameterName(Expression reference)
        {
            var lambda = reference as LambdaExpression;
            var member = lambda.Body as MemberExpression;

            return member.Member.Name;
        }
    }
}