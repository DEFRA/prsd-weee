namespace EA.Weee.Api.IdSrv
{
    using System.Diagnostics;
    using Thinktecture.IdentityServer.Core.Logging;

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
        }
    }
}