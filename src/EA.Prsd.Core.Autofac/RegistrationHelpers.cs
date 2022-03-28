namespace EA.Prsd.Core.Autofac
{
    using System;
    using global::Autofac;

    public static class RegistrationHelpers
    {
        public static void RegisterTypeByEnvironment(this ContainerBuilder builder, Type type, EnvironmentResolver environment)
        {
            if (environment.HostEnvironment.Equals(HostEnvironmentType.Console))
            {
                builder.RegisterType(type).AsSelf().SingleInstance();
            }
            else
            {
                builder.RegisterType(type).AsSelf().InstancePerRequest();
            }
        }

        public static void RegisterTypeByEnvironment<T, TI>(this ContainerBuilder builder, EnvironmentResolver environment)
        {
            if (environment.HostEnvironment.Equals(HostEnvironmentType.Console))
            {
                builder.RegisterType<T>().As<TI>().SingleInstance();
            }
            else
            {
                builder.RegisterType<T>().As<TI>().InstancePerRequest();
            }
        }
    }
}
