namespace EA.Weee.Api.IdSrv
{
    using System;
    using System.Diagnostics;
    using IdentityServer3.Core.Logging;

    internal class DebugLogProvider : ILogProvider
    {
        public ILog GetLogger(string name)
        {
            return new DebugLog();
        }

        public class DebugLog : ILog
        {
            public bool Log(LogLevel logLevel, System.Func<string> messageFunc, System.Exception exception = null)
            {
                if (messageFunc != null)
                {
                    Debug.WriteLine(messageFunc());
                }
                if (exception != null)
                {
                    Debug.WriteLine(exception.ToString());
                }
                return true;
            }

            public bool Log(LogLevel logLevel, Func<string> messageFunc, Exception exception = null, params object[] formatParameters)
            {
                return Log(logLevel, messageFunc, exception);
            }
        }

        public IDisposable OpenMappedContext(string key, string value)
        {
            throw new NotImplementedException();
        }

        public IDisposable OpenNestedContext(string message)
        {
            throw new NotImplementedException();
        }

        Logger ILogProvider.GetLogger(string name)
        {
            throw new NotImplementedException();
        }
    }
}