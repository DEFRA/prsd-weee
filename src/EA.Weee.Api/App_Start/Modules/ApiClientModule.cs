namespace EA.Weee.Api.Modules
{
    using Api.Client;
    using Autofac;
    using EA.Weee.Api.Client.Serlializer;
    using Prsd.Core.Web.OAuth;
    using Prsd.Core.Web.OpenId;
    using Serilog;
    using Services;
    using System;
    using System.IO;
    using System.Security.Cryptography.X509Certificates;

    public class ApiClientModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var configurationService = new ConfigurationService();
            if (configurationService.CurrentConfiguration.GovUkPayMopUpJobEnabled)
            {
                builder.Register(c =>
                {
                    var cc = c.Resolve<IComponentContext>();
                    var config = cc.Resolve<AppConfiguration>();
                    var httpClient = cc.Resolve<IHttpClientWrapperFactory>();
                    var retryPolicy = cc.Resolve<IRetryPolicyWrapper>();
                    var jsonSerializer = cc.Resolve<IJsonSerializer>();
                    var logger = cc.Resolve<ILogger>();

                    var httpClientHandlerConfig = new HttpClientHandlerConfig
                    {
                        ProxyEnabled = config.ProxyEnabled,
                        ProxyUseDefaultCredentials = config.ProxyUseDefaultCredentials,
                        ProxyWebAddress = config.ProxyWebAddress,
                        ByPassProxyOnLocal = config.ByPassProxyOnLocal
                    };

                    return new PayClient(config.GovUkPayBaseUrl, config.GovUkPayApiKey, httpClient, retryPolicy,
                        jsonSerializer, httpClientHandlerConfig, logger);
                }).As<IPayClient>();
            }
        }
    }
}