namespace EA.Weee.Api.IdSrv
{
    using Elmah;
    using System;
    using Thinktecture.IdentityServer.Core.Logging;

    internal class ElmahLogger : ILogProvider
    {
        public ILog GetLogger(string name)
        {
            return new ElmahLog();
        }

        public class ElmahLog : ILog
        {
            public bool Log(LogLevel logLevel, Func<string> messageFunc, Exception exception = null)
            {
                if (exception != null)
                {
                    ErrorSignal.FromCurrentContext().Raise(exception);
                    return true;
                }

                return false;
            }
        }
    }
}