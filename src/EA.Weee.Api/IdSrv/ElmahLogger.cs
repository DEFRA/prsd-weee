namespace EA.Weee.Api.IdSrv
{
    using System;
    using Core.Logging;
    using Elmah;
    using IdentityServer3.Core.Logging;

    public class ElmahLogger : ILog, ILogger
    {
        public void Log(Exception exception)
        {
            ErrorSignal.FromCurrentContext().Raise(exception);
        }

        public bool Log(LogLevel logLevel, Func<string> messageFunc, Exception exception = null)
        {
            if (exception != null)
            {
                Log(exception);
                return true;
            }

            return false;
        }

        public bool Log(LogLevel logLevel, Func<string> messageFunc, Exception exception = null, params object[] formatParameters)
        {
            return Log(logLevel, messageFunc, exception);
        }
    }
}