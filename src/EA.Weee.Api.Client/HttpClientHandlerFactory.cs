namespace EA.Weee.Api.Client
{
    using System;
    using System.Net;
    using System.Net.Http;

    public static class HttpClientHandlerFactory
    {
        public static HttpClientHandler Create(HttpClientHandlerConfig config)
        {
            var webProxy = new WebProxy { BypassProxyOnLocal = config.ByPassProxyOnLocal };
            if (config.ProxyEnabled)
            {
                webProxy.Address = new Uri(config.ProxyWebAddress);
                webProxy.UseDefaultCredentials = config.ProxyUseDefaultCredentials;
            }

            var handler = new HttpClientHandler()
            {
                Proxy = webProxy,
            };

            return handler;
        }
    }
}