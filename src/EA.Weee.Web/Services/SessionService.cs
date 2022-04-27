namespace EA.Weee.Web.Services
{
    using System.Web;
    using Weee.Requests.Scheme;

    public class SessionService : ISessionService
    {
        public void SetTransferNoteSessionObject(HttpSessionStateBase session, TransferEvidenceNoteRequest request)
        {
            session["TransferEvidenceNoteData"] = request;
        }
    }
}