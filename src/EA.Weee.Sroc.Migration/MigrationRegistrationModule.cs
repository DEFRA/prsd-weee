namespace EA.Weee.Sroc.Migration
{
    using Autofac;
    using Prsd.Core.Autofac;
    using Prsd.Core.Decorators;
    using Prsd.Core.Mediator;
    using RequestHandlers.Admin;
    using RequestHandlers.Charges.IssuePendingCharges;
    using RequestHandlers.Charges.IssuePendingCharges.Errors;
    using RequestHandlers.Scheme.MemberUploadTesting;
    using RequestHandlers.Shared.DomainUser;

    public class MigrationRegistrationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(this.GetType().Assembly)
                .AsNamedClosedTypesOf(typeof(IRequestHandler<,>), t => "request_handler");

            builder.RegisterAssemblyTypes()
                .AsClosedTypesOf(typeof(IRequest<>))
                .AsImplementedInterfaces();

            // Register data access types
            builder.RegisterAssemblyTypes(this.GetType().Assembly)
                .AsImplementedInterfaces();

            builder.RegisterType<WeeeMigrationContext>().AsSelf().SingleInstance();
        }
    }
}