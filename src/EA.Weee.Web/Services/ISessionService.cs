namespace EA.Weee.Web.Services
{
    using System.Web;

    public interface ISessionService
    {
        void SetTransferSessionObject(object request, string sessionKey);

        T GetTransferSessionObject<T>(string sessionKey) where T : class;

        void ClearTransferSessionObject(string sessionKey);
    }
}