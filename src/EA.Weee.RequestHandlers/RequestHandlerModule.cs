namespace EA.Weee.RequestHandlers
{
    using Autofac;
    using Charges.IssuePendingCharges;
    using Prsd.Core.Autofac;
    using Prsd.Core.Decorators;
    using Prsd.Core.Mediator;
    using Scheme.MemberUploadTesting;

    public class RequestHandlerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(this.GetType().Assembly)
                .AsNamedClosedTypesOf(typeof(IRequestHandler<,>), t => "request_handler");

            // Order matters here
            builder.RegisterGenericDecorators(this.GetType().Assembly, typeof(IRequestHandler<,>), "request_handler",
                typeof(EventDispatcherRequestHandlerDecorator<,>), // <-- inner most decorator
                typeof(AuthorizationRequestHandlerDecorator<,>),
                typeof(AuthenticationRequestHandlerDecorator<,>)); // <-- outer most decorator

            builder.RegisterAssemblyTypes()
                .AsClosedTypesOf(typeof(IRequest<>))
                .AsImplementedInterfaces();

            // Register data access types
            builder.RegisterAssemblyTypes(this.GetType().Assembly)
                .Where(t => t.Name.Contains("DataAccess"))
                .AsImplementedInterfaces();

            // Register the map classes
            builder.RegisterAssemblyTypes(this.GetType().Assembly)
                .Where(t => t.Namespace.Contains("Mappings"))
                .AsImplementedInterfaces();

            // Member registration Upload
            builder.RegisterAssemblyTypes(GetType().Assembly)
                .Where(t => t.Namespace.Contains("MemberRegistration"))
                .AsImplementedInterfaces();

            // data returns Upload
            builder.RegisterAssemblyTypes(GetType().Assembly)
                .Where(t => t.Namespace.Contains("DataReturns"))
                .AsImplementedInterfaces();

            // Register singleton types relating to PCS member upload testing.
            builder.RegisterType<ProducerListFactory>().As<IProducerListFactory>();
            builder.RegisterType<XmlGenerator>().As<IXmlGenerator>();

            // Register the types that will generate 1B1S files from member uploads.
            builder.RegisterType<IbisFileDataGenerator>().As<IIbisFileDataGenerator>();
            builder.RegisterType<BySchemeCustomerFileGenerator>().As<IIbisCustomerFileGenerator>();
            builder.RegisterType<BySchemeTransactionFileGenerator>().As<IIbisTransactionFileGenerator>();
            builder.RegisterType<TransactionReferenceGenerator>().As<ITransactionReferenceGenerator>();
        }
    }
}