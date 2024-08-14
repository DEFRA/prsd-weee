namespace EA.Weee.Web.App_Start
{
    using global::Hangfire;
    using Hangfire;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    public class HangfireBootstrapper
    {
        private static readonly Lazy<HangfireBootstrapper> instance =
            new Lazy<HangfireBootstrapper>(() => new HangfireBootstrapper());

        private BackgroundJobServer backgroundJobServer;

        public static HangfireBootstrapper Instance => instance.Value;

        public void Start()
        {
            backgroundJobServer = new BackgroundJobServer();
        }

        public void Stop()
        {
            backgroundJobServer?.Dispose();
        }
    }
}