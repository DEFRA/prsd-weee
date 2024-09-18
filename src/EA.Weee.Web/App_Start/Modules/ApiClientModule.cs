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

            builder.Register(c =>
            {
                var cc = c.Resolve<IComponentContext>();
                var config = cc.Resolve<IAppConfiguration>();
                var httpClient = cc.Resolve<IHttpClientWrapperFactory>();
                var retryPolicy = cc.Resolve<IRetryPolicyWrapper>();
                var jsonSerializer = cc.Resolve<IJsonSerializer>();
                var logger = cc.Resolve<ILogger>();

                string filePath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory) + @"\Cert\" + config.CompaniesCertificateName;
                X509Certificate2 certificate = new X509Certificate2(filePath, config.CompaniesHousePassword);
                HttpClientHandlerConfig httpClientHandlerConfig = new HttpClientHandlerConfig
                {
                    ProxyEnabled = config.ProxyEnabled,
                    ProxyUseDefaultCredentials = config.ProxyUseDefaultCredentials,
                    ProxyWebAddress = config.ProxyWebAddress,
                    ByPassProxyOnLocal = config.ByPassProxyOnLocal
                };

                    return new CompaniesHouseClient(config.CompaniesHouseBaseUrl, httpClient, retryPolicy, 
                        jsonSerializer, httpClientHandlerConfig, certificate, logger);
                }).As<ICompaniesHouseClient>();

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

                return new PayClient(config.GovUkPayBaseUrl, config.GovUkPayApiKey, httpClient, retryPolicy,
                    jsonSerializer, httpClientHandlerConfig, logger);
            }).As<IPayClient>();
        }
    }
}