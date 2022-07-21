namespace EA.Weee.Web.Services
{
    using System.Web;

    public class SessionService : ISessionService
    {
        public void SetTransferSessionObject(HttpSessionStateBase session, object request, string sessionKey)
        {
            session[sessionKey] = request;
        }

        public T GetTransferSessionObject<T>(HttpSessionStateBase session, string sessionKey) where T : class
        {
            if (session[sessionKey] != null)
            {
                return session[sessionKey] as T;
            }

            return null;
        }

        public void ClearTransferSessionObject(HttpSessionStateBase session, string sessionKey)
        {
            if (session[sessionKey] != null)
            {
                session[sessionKey] = null;
            }
        }
    }
}