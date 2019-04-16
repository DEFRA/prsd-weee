namespace EA.Weee.Web.RazorHelpers
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class PropertyHelper
    {
        private static readonly ConcurrentDictionary<Type, PropertyHelper[]> ReflectionCache = new ConcurrentDictionary<Type, PropertyHelper[]>();
        private static readonly MethodInfo CallPropertyGetterOpenGenericMethod = typeof(PropertyHelper).GetMethod("CallPropertyGetter", BindingFlags.Static | BindingFlags.NonPublic);
        private static readonly MethodInfo CallPropertyGetterByReferenceOpenGenericMethod = typeof(PropertyHelper).GetMethod("CallPropertyGetterByReference", BindingFlags.Static | BindingFlags.NonPublic);
        private static readonly MethodInfo CallPropertySetterOpenGenericMethod = typeof(PropertyHelper).GetMethod("CallPropertySetter", BindingFlags.Static | BindingFlags.NonPublic);
        private readonly Func<object, object> valueGetter;

        public PropertyHelper(PropertyInfo property)
        {
            this.Name = property.Name;
            this.valueGetter = PropertyHelper.MakeFastPropertyGetter(property);
        }

        public static Action<TDeclaringType, object> MakeFastPropertySetter<TDeclaringType>(
          PropertyInfo propertyInfo)
          where TDeclaringType : class
        {
            var setMethod = propertyInfo.GetSetMethod();
            var reflectedType = propertyInfo.ReflectedType;
            var parameterType = setMethod.GetParameters()[0].ParameterType;
            return (Action<TDeclaringType, object>)Delegate.CreateDelegate(typeof(Action<TDeclaringType, object>), (object)setMethod.CreateDelegate(typeof(Action<,>).MakeGenericType(reflectedType, parameterType)), PropertyHelper.CallPropertySetterOpenGenericMethod.MakeGenericMethod(reflectedType, parameterType));
        }

        public virtual string Name { get; protected set; }

        public object GetValue(object instance)
        {
            return this.valueGetter(instance);
        }

        public static PropertyHelper[] GetProperties(object instance)
        {
            return PropertyHelper.GetProperties(instance, new Func<PropertyInfo, PropertyHelper>(PropertyHelper.CreateInstance), PropertyHelper.ReflectionCache);
        }

        public static Func<object, object> MakeFastPropertyGetter(PropertyInfo propertyInfo)
        {
            var getMethod = propertyInfo.GetGetMethod();
            var reflectedType = getMethod.ReflectedType;
            var returnType = getMethod.ReturnType;
            var @delegate = reflectedType.IsValueType ? Delegate.CreateDelegate(typeof(Func<object, object>), (object)getMethod.CreateDelegate(typeof(PropertyHelper.ByRefFunc<,>).MakeGenericType(reflectedType, returnType)), PropertyHelper.CallPropertyGetterByReferenceOpenGenericMethod.MakeGenericMethod(reflectedType, returnType)) : Delegate.CreateDelegate(typeof(Func<object, object>), (object)getMethod.CreateDelegate(typeof(Func<,>).MakeGenericType(reflectedType, returnType)), PropertyHelper.CallPropertyGetterOpenGenericMethod.MakeGenericMethod(reflectedType, returnType));
            return (Func<object, object>)@delegate;
        }

        private static PropertyHelper CreateInstance(PropertyInfo property)
        {
            return new PropertyHelper(property);
        }

        private static object CallPropertyGetter<TDeclaringType, TValue>(
          Func<TDeclaringType, TValue> getter,
          object @this)
        {
            return (object)getter((TDeclaringType)@this);
        }

        private static object CallPropertyGetterByReference<TDeclaringType, TValue>(
          PropertyHelper.ByRefFunc<TDeclaringType, TValue> getter,
          object @this)
        {
            var declaringType = (TDeclaringType)@this;
            return (object)getter(ref declaringType);
        }

        private static void CallPropertySetter<TDeclaringType, TValue>(
          Action<TDeclaringType, TValue> setter,
          object @this,
          object value)
        {
            setter((TDeclaringType)@this, (TValue)value);
        }

        protected static PropertyHelper[] GetProperties(
          object instance,
          Func<PropertyInfo, PropertyHelper> createPropertyHelper,
          ConcurrentDictionary<Type, PropertyHelper[]> cache)
        {
            var type = instance.GetType();
            PropertyHelper[] array;
            if (!cache.TryGetValue(type, out array))
            {
                var propertyInfos = ((IEnumerable<PropertyInfo>)type.GetProperties(BindingFlags.Instance | BindingFlags.Public)).Where<PropertyInfo>((Func<PropertyInfo, bool>)(prop =>
                {
                    if (prop.GetIndexParameters().Length == 0)
                    {
                        return prop.GetMethod != (MethodInfo)null;
                    }
                    return false;
                }));
                var propertyHelperList = new List<PropertyHelper>();
                foreach (var propertyInfo in propertyInfos)
                {
                    var propertyHelper = createPropertyHelper(propertyInfo);
                    propertyHelperList.Add(propertyHelper);
                }
                array = propertyHelperList.ToArray();
                cache.TryAdd(type, array);
            }
            return array;
        }

        private delegate TValue ByRefFunc<TDeclaringType, TValue>(ref TDeclaringType arg);
    }
}
