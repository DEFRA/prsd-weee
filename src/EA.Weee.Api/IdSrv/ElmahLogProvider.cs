namespace EA.Weee.Api.IdSrv
{
    using Thinktecture.IdentityServer.Core.Logging;

    internal class ElmahLogProvider : ILogProvider
    {
        public ILog GetLogger(string name)
        {
            return new ElmahLogger();
        }
    }
}