namespace EA.Weee.Web.Areas.Scheme.Controllers
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using Constant;
    using Services;
    using Services.Caching;
    using ViewModels;
    using Weee.Requests.Scheme;

    public abstract class TransferEvidenceBaseController : SchemeEvidenceBaseController
    {
        protected readonly ISessionService SessionService;

        protected TransferEvidenceBaseController(BreadcrumbService breadcrumb, 
            IWeeeCache cache, ISessionService sessionService) : base(breadcrumb, cache)
        {
            SessionService = sessionService;
        }

        public abstract Task<ActionResult> SelectEvidenceNote(TransferSelectEvidenceNoteModel model);

        public abstract ActionResult DeselectEvidenceNote(TransferDeselectEvidenceNoteModel model);

        protected void DeselectEvidenceNote(Guid evidenceNoteId, string sessionKey)
        {
            var transferRequest = SessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(Session,
                sessionKey);

            transferRequest.EvidenceNoteIds.Remove(evidenceNoteId);
            transferRequest.DeselectedEvidenceNoteIds.Add(evidenceNoteId);
        }

        public TransferEvidenceNoteRequest SelectEvidenceNote(Guid evidenceNoteId, string sessionKey)
        {
            var transferRequest = SessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(Session,
                sessionKey);

            if (ModelState.IsValid)
            {
                transferRequest.EvidenceNoteIds.Add(evidenceNoteId);
                transferRequest.DeselectedEvidenceNoteIds.Remove(evidenceNoteId);
            }

            return transferRequest;
        }
    }
}