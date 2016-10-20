namespace EA.Weee.Api.IdSrv
{
    using System;
    using IdentityServer3.Core.Logging;

    internal class ElmahLogProvider : ILogProvider
    {
        public ILog GetLogger(string name)
        {
            return new ElmahLogger();
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