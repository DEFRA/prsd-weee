namespace EA.Weee.Api.Modules
{
    using Autofac;
    using Identity;
    using Prsd.Core.Domain;

    public class UserContextModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<UserContext>().As<IUserContext>().SingleInstance();
        }
    }
}