namespace EA.Weee.Api.Client
{
    using Autofac;
    using Autofac.Core;
    using EA.Weee.Api.Client.Polly;
    using EA.Weee.Api.Client.Serlializer;
    using Serilog;

    public class ApiClientModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<HttpClientWrapper>()
               .As<IHttpClientWrapper>()
               .InstancePerLifetimeScope();
            builder.RegisterType<HttpClientWrapperFactory>()
               .As<IHttpClientWrapperFactory>()
               .InstancePerLifetimeScope();

            builder.Register(c => 
            {
                var cc = c.Resolve<IComponentContext>();
                var logger = cc.Resolve<ILogger>();
                return new RetryPolicyWrapper(PollyPolicies.GetRetryPolicy(logger));
            }).As<IRetryPolicyWrapper>().SingleInstance();

            builder.RegisterType<JsonSerializer>()
                   .As<IJsonSerializer>()
                   .SingleInstance();
        }
    }
}
