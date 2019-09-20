namespace EA.Prsd.Core.Autofac
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using global::Autofac;
    using global::Autofac.Builder;
    using global::Autofac.Core;
    using global::Autofac.Features.Scanning;

    public static class CustomRegistrationExtensions
    {
        // Adapted from https://groups.google.com/forum/#!topic/autofac/lV2X4hR0mak
        // This is the important custom bit: Registering a named service during scanning.
        public static IRegistrationBuilder<TLimit, TScanningActivatorData, TRegistrationStyle>
            AsNamedClosedTypesOf<TLimit, TScanningActivatorData, TRegistrationStyle>(
            this IRegistrationBuilder<TLimit, TScanningActivatorData, TRegistrationStyle> registration,
            Type openGenericServiceType,
            Func<Type, object> keyFactory)
            where TScanningActivatorData : ScanningActivatorData
        {
            if (openGenericServiceType == null)
            {
                throw new ArgumentNullException("openGenericServiceType");
            }

            return registration
                .Where(candidateType => candidateType.IsClosedTypeOf(openGenericServiceType))
                .As(
                    candidateType =>
                        candidateType.GetTypesThatClose(openGenericServiceType)
                            .Select(t => (Service)new KeyedService(keyFactory(t), t)));
        }

        // These next two methods are basically copy/paste of some Autofac internals that
        // are used to determine closed generic types during scanning.
        internal static IEnumerable<Type> GetTypesThatClose(this Type candidateType, Type openGenericServiceType)
        {
            return
                candidateType.GetInterfaces()
                    .Concat(TraverseAcross(candidateType, t => t.BaseType))
                    .Where(t => t.IsClosedTypeOf(openGenericServiceType));
        }

        internal static IEnumerable<T> TraverseAcross<T>(T first, Func<T, T> next) where T : class
        {
            var item = first;

            while (item != null)
            {
                yield return item;
                item = next(item);
            }
        }

        // Adapted from http://stackoverflow.com/a/26018954
        public static void RegisterGenericDecorators(
            this ContainerBuilder builder,
            Assembly assembly,
            Type handlerType,
            string fromKey,
            params Type[] decorators)
        {
            for (var i = 0; i < decorators.Length; i++)
            {
                RegisterGenericDecorator(
                    builder,
                    decorators[i],
                    handlerType,
                    i == 0 ? fromKey : decorators[i - 1].Name,
                    i != decorators.Length - 1);
            }
        }

        private static void RegisterGenericDecorator(
            ContainerBuilder builder,
            Type decoratorType,
            Type decoratedServiceType,
            string fromKey,
            bool hasKey)
        {
            var result = builder.RegisterGenericDecorator(
                decoratorType,
                decoratedServiceType,
                fromKey);

            if (hasKey)
            {
                result.Keyed(decoratorType.Name, decoratedServiceType);
            }
        }
    }
}