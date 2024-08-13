namespace EA.Weee.Web.Modules
{
    using Api.Client;
    using Autofac;
    using Prsd.Core.Web.OAuth;
    using Prsd.Core.Web.OpenId;
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

                string filePath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory) + @"\Cert\Boomi-IWS-TST.pfx";

                X509Certificate2 certificate = new X509Certificate2(filePath, "kN2S6!p6F*LH");

                HttpClientHandlerConfig httpClientHandlerConfig = new HttpClientHandlerConfig
                {
                    ProxyEnabled = config.ProxyEnabled,
                    ProxyUseDefaultCredentials = config.ProxyUseDefaultCredentials,
                    ProxyWebAddress = config.ProxyWebAddress,
                    ByPassProxyOnLocal = config.ByPassProxyOnLocal
                };
                return new CompaniesHouseClient(config.CompaniesHouseBaseUrl, httpClientHandlerConfig, certificate);
            }).As<ICompaniesHouseClient>();
        }
    }
}