namespace EA.Weee.Web.Services
{
    using System;
    using System.Web;

    public interface ISessionService
    {
        void SetTransferSessionObject(HttpSessionStateBase session, object request, string sessionKey);

        T GetTransferSessionObject<T>(HttpSessionStateBase session, string sessionKey) where T : class;
    }
}