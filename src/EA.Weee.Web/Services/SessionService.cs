namespace EA.Weee.Web.Services
{
    using System.Web;
    using Weee.Requests.Scheme;

    public class SessionService : ISessionService
    {
        public void SetTransferSessionObject(HttpSessionStateBase session, object request, string sessionKey)
        {
            session[sessionKey] = request;
        }

        public T GetTransferSessionObject<T>(HttpSessionStateBase session, string sessionKey)
        {
            throw new System.NotImplementedException();
        }
    }
}