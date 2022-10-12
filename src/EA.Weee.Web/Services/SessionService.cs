namespace EA.Weee.Web.Services
{
    using System.Web;

    public class SessionService : ISessionService
    {
        public void SetTransferSessionObject(object request, string sessionKey)
        {
            HttpContext.Current.Session[sessionKey] = request;
        }

        public T GetTransferSessionObject<T>(string sessionKey) where T : class
        {
            if (HttpContext.Current.Session[sessionKey] != null)
            {
                return HttpContext.Current.Session[sessionKey] as T;
            }

            return null;
        }

        public void ClearTransferSessionObject(string sessionKey)
        {
            HttpContext.Current.Session[sessionKey] = null;
        }
    }
}