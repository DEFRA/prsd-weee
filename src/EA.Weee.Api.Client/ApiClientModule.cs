namespace EA.Weee.Api.Client
{
    using Autofac;
    using Autofac.Core;
    using EA.Weee.Api.Client.Polly;
    using EA.Weee.Api.Client.Serlializer;

    public class ApiClientModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<HttpClientWrapper>()
               .As<IHttpClientWrapper>()
               .InstancePerLifetimeScope();

            builder.Register(c => new RetryPolicyWrapper(PollyPolicies.GetRetryPolicy()))
                   .As<IRetryPolicyWrapper>()
                   .SingleInstance();

            builder.RegisterType<JsonSerializer>()
                   .As<IJsonSerializer>()
                   .SingleInstance();

            //builder.RegisterType<CompaniesHouseClient>()
            //       .As<ICompaniesHouseClient>()
            //       .InstancePerLifetimeScope();

            //builder.Register(c =>
            //{
            //    var cc = c.Resolve<IComponentContext>();
            //    var config = cc.Resolve<IAppConfiguration>();
            //    string filePath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory) + @"\Cert\.pfx";
            //    X509Certificate2 certificate = new X509Certificate2(filePath, "passcode");
            //    HttpClientHandlerConfig httpClientHandlerConfig = new HttpClientHandlerConfig
            //    {
            //        ProxyEnabled = config.ProxyEnabled,
            //        ProxyUseDefaultCredentials = config.ProxyUseDefaultCredentials,
            //        ProxyWebAddress = config.ProxyWebAddress,
            //        ByPassProxyOnLocal = config.ByPassProxyOnLocal
            //    };
            //    return new CompaniesHouseClient(config.CompaniesHouseBaseUrl, httpClientHandlerConfig, certificate);
            //}).As<ICompaniesHouseClient>();
        }
    }
}
