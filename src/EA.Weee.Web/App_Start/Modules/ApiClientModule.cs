namespace EA.Weee.Web.Modules
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
            builder.Register(c =>
            {
                var cc = c.Resolve<IComponentContext>();
                var config = cc.Resolve<IAppConfiguration>();
                TimeSpan timeout = TimeSpan.FromSeconds(config.ApiTimeoutInSeconds);
                return new WeeeClient(config.ApiUrl, timeout);
            }).As<IWeeeClient>();

            builder.Register(c =>
            {
                var cc = c.Resolve<IComponentContext>();
                var config = cc.Resolve<IAppConfiguration>();
                return new OAuthClient(config.ApiUrl, config.ApiClientId, config.ApiSecret);
            }).As<IOAuthClient>();

            builder.Register(c =>
            {
                var cc = c.Resolve<IComponentContext>();
                var config = cc.Resolve<IAppConfiguration>();
                return new OAuthClientCredentialClient(config.ApiUrl, config.ApiClientCredentialId, config.ApiClientCredentialSecret);
            }).As<IOAuthClientCredentialClient>();

            builder.Register(c =>
            {
                var cc = c.Resolve<IComponentContext>();
                var config = cc.Resolve<IAppConfiguration>();
                return new UserInfoClient(config.ApiUrl);
            }).As<IUserInfoClient>();

            // example registraction of external service
            //builder.Register(c =>
            //{
            //    var cc = c.Resolve<IComponentContext>();
            //    var config = cc.Resolve<IAppConfiguration>();
            //    var httpClient = cc.Resolve<IHttpClientWrapperFactory>();
            //    var retryPolicy = cc.Resolve<IRetryPolicyWrapper>();
            //    var jsonSerializer = cc.Resolve<IJsonSerializer>();
            //    var logger = cc.Resolve<ILogger>();

            //    string filePath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory) + @"\Cert\.pfx";
            //    X509Certificate2 certificate = new X509Certificate2(filePath, "");
            //    HttpClientHandlerConfig httpClientHandlerConfig = new HttpClientHandlerConfig
            //    {
            //        ProxyEnabled = config.ProxyEnabled,
            //        ProxyUseDefaultCredentials = config.ProxyUseDefaultCredentials,
            //        ProxyWebAddress = config.ProxyWebAddress,
            //        ByPassProxyOnLocal = config.ByPassProxyOnLocal
            //    };

            //    return new CompaniesHouseClient("https://integration-tst.azure.defra.cloud/ws/rest/DEFRA/v2.1/", httpClient, retryPolicy, 
            //        jsonSerializer, httpClientHandlerConfig, certificate, logger);
            //}).As<ICompaniesHouseClient>();

            // example registraction of external service
            builder.Register(c =>
            {
                var cc = c.Resolve<IComponentContext>();
                var config = cc.Resolve<IAppConfiguration>();
                var httpClient = cc.Resolve<IHttpClientWrapperFactory>();
                var retryPolicy = cc.Resolve<IRetryPolicyWrapper>();
                var jsonSerializer = cc.Resolve<IJsonSerializer>();
                var logger = cc.Resolve<ILogger>();

                HttpClientHandlerConfig httpClientHandlerConfig = new HttpClientHandlerConfig
                {
                    ProxyEnabled = config.ProxyEnabled,
                    ProxyUseDefaultCredentials = config.ProxyUseDefaultCredentials,
                    ProxyWebAddress = config.ProxyWebAddress,
                    ByPassProxyOnLocal = config.ByPassProxyOnLocal
                };

                return new AddressLookupClient("https://dev-api-gateway.azure.defra.cloud/", httpClient, retryPolicy,
                    jsonSerializer, httpClientHandlerConfig, logger);
            }).As<IAddressLookupClient>();
        }
    }
}