namespace EA.Weee.Api.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class HttpClientHandlerConfig
    {
        public bool ProxyEnabled { get; set; }

        public bool ByPassProxyOnLocal { get; set; }

        public string ProxyWebAddress { get; set; }

        public bool ProxyUseDefaultCredentials { get; set; }
    }
}
