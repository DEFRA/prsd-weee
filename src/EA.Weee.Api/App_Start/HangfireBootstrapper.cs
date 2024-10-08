namespace EA.Weee.Api.App_Start
{
    using System;
    using Hangfire;

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