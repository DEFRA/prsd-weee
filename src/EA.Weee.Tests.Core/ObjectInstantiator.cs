namespace EA.Weee.Tests.Core
{
    using System;
    using System.Linq.Expressions;
    using System.Runtime.Serialization;

    internal class ObjectInstantiator<T>
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
    }
}
