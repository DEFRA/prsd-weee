namespace EA.Prsd.Core.Autofac
{
    using System.Diagnostics;
    using global::Autofac;
    using System.Web;

    public static class ServiceLocator
    {
        private static IContainer httpContainer;
        private static IContainer consoleContainer;

        public static IContainer Container
        {
            get => GetRelevantContainer();
            set
            {
                var environment = value.Resolve<IEnvironmentResolver>();
                switch (environment.HostEnvironment)
                {
                    case HostEnvironmentType.Console:
                        consoleContainer = value;
                        break;
                    default:
                        httpContainer = value;
                        break;
                }
            }
        }

        /// <summary>
        /// Get by magic: based on ServiceSecurityContext / HttpContext
        /// </summary>
        private static IContainer GetRelevantContainer()
        {
            switch (GuessEnvironment())
            {
              case HostEnvironmentType.Console:
                    return consoleContainer ?? httpContainer;
                default:
                    return httpContainer ?? httpContainer;
            }
        }

        private static HostEnvironmentType GuessEnvironment()
        {
            var environmentType = HostEnvironmentType.Console;
            try
            {
                //if (ServiceSecurityContext.Current != null)
                    //environmentType = HostEnvironmentType.Wcf;
                if (HttpContext.Current != null)
                {
                    environmentType = HostEnvironmentType.WebApi; // this is a guess
                }
            }
            catch
            {
                // if context cannot be accessed do not try and use it 
                if (!EventLog.SourceExists("EA.Weee"))
                {
                    EventLog.CreateEventSource(new EventSourceCreationData("EA.Weee", string.Empty));
                }
                EventLog.WriteEntry("EA.Weee", "Unable to determine if web / wcf request");
            }

            return environmentType;
        }

        /// <summary>
        /// Shortcut to logger: ONLY USE WHEN ABSOLUTELY NECESSARY (e.g. callbacks etc)
        /// </summary>
        //public static ILogger Logger => Container.Resolve<ILogger>("ILoggerStatic"); // todo: rename LoggerStatic
        //public static ILogger LoggerTransient => Container.Resolve<ILogger>("ILoggerTransient");
        //public static ILog Log => Container.Resolve<ILog>();
    }
}
