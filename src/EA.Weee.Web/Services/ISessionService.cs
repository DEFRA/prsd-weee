namespace EA.Weee.Web.Services
{
    using System.Web;
    using Weee.Requests.Scheme;

    public interface ISessionService
    {
        void SetTransferNoteSessionObject(HttpSessionStateBase session, TransferEvidenceNoteRequest request);
    }
}