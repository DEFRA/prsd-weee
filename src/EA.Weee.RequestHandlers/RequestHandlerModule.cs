﻿namespace EA.Weee.RequestHandlers
{
    using Autofac;
    using EA.Weee.RequestHandlers.PCS.MemberRegistration;
    using Prsd.Core.Autofac;
    using Prsd.Core.Decorators;
    using Prsd.Core.Mediator;

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

            // Register the map classes
            builder.RegisterAssemblyTypes(this.GetType().Assembly)
                .Where(t => t.Namespace.Contains("Mappings"))
                .AsImplementedInterfaces();

            builder.RegisterType<XmlErrorTranslator>().AsImplementedInterfaces();
        }
    }
}