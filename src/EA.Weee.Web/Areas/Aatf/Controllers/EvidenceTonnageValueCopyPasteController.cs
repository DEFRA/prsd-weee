namespace EA.Weee.Web.Areas.Aatf.Controllers
{
    using EA.Weee.Core.Aatf;
    using EA.Weee.Core.Shared;
    using EA.Weee.Web.Areas.Aatf.ViewModels;
    using EA.Weee.Web.Areas.AatfEvidence.Controllers;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    public class EvidenceTonnageValueCopyPasteController : AatfEvidenceBaseController
    {
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private readonly ISessionService sessionService;
        private readonly IPasteProcessor pasteProcessor;

        public EvidenceTonnageValueCopyPasteController(BreadcrumbService breadcrumb, IWeeeCache cache, ISessionService sessionService, IPasteProcessor pasteProcessor)
        {
            this.breadcrumb = breadcrumb;
            this.cache = cache;
            this.sessionService = sessionService;
            this.pasteProcessor = pasteProcessor;
        }

        [HttpGet]
        public async Task<ActionResult> Index(Guid organisationId, string returnAction, int complianceYear, bool redirect = false)
        { 
            await SetBreadcrumb(organisationId, BreadCrumbConstant.AatfManageEvidence);

            var evidenceModel = sessionService.GetTransferSessionObject<EditEvidenceNoteViewModel>(Session, SessionKeyConstant.EditEvidenceViewModelKey);

            if (redirect)
            {
                return ReturnLinkCase(returnAction, evidenceModel, complianceYear);
            }

            var model = new EvidenceTonnageValueCopyPasteViewModel() 
                { 
                    OrganisationId = evidenceModel.OrganisationId, 
                    AatfId = evidenceModel.AatfId, 
                    Action = returnAction, 
                    EvidenceId = evidenceModel.Id,
                    ComplianceYear = complianceYear
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Index(EvidenceTonnageValueCopyPasteViewModel model)
        {
            var evidenceModel = sessionService.GetTransferSessionObject<EditEvidenceNoteViewModel>(Session, SessionKeyConstant.EditEvidenceViewModelKey);
            var receivedContent = model.ReceievedPastedValues.First();
            var reusedContent = model.ReusedPastedValues.First();

            if (receivedContent != null || reusedContent != null)
            {
                var receivedPastedValues = pasteProcessor.BuildModel(receivedContent);
                var reusedPastedValues = pasteProcessor.BuildModel(reusedContent);

                var evidencePastedValues = new EvidencePastedValues() { Receieved = receivedPastedValues, Reused = reusedPastedValues };

                evidenceModel.CategoryValues = pasteProcessor.ParseEvidencePastedValues(evidencePastedValues, evidenceModel.CategoryValues);

                sessionService.SetTransferSessionObject(Session, evidenceModel, SessionKeyConstant.EditEvidenceViewModelKey);
            }
            return ReturnLinkCase(model.Action, evidenceModel, evidenceModel.ComplianceYear);
        }

        private ActionResult ReturnLinkCase(string returnAction, EditEvidenceNoteViewModel evidenceModel, int complianceYear)
        {
            switch (returnAction)
            {
                case EvidenceCopyPasteActionConstants.EditEvidenceNoteAction:
                    return RedirectToRoute(AatfEvidenceRedirect.EditEvidenceRouteName, new { organisationId = evidenceModel.OrganisationId, aatfId = evidenceModel.AatfId, evidenceNoteId = evidenceModel.Id, returnFromCopyPaste = true, complianceYear });
                default: 
                    return RedirectToAction("CreateEvidenceNote", "ManageEvidenceNotes", new { evidenceModel.OrganisationId, evidenceModel.AatfId, complianceYear, returnFromCopyPaste = true });
            }
        }

        private async Task SetBreadcrumb(Guid organisationId, string activity)
        {
            breadcrumb.ExternalOrganisation = await cache.FetchOrganisationName(organisationId);
            breadcrumb.ExternalActivity = activity;
            breadcrumb.OrganisationId = organisationId;
        }
    }
}