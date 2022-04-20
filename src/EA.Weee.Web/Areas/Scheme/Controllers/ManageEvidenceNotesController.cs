namespace EA.Weee.Web.Areas.Scheme.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Core.AatfEvidence;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.Scheme;
    using EA.Weee.Requests.AatfEvidence;
    using EA.Weee.Requests.Scheme;
    using EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels;
    using EA.Weee.Web.Areas.Scheme.ViewModels;
    using EA.Weee.Web.Areas.Scheme.ViewModels.ManageEvidenceNotes;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;

    public class ManageEvidenceNotesController : SchemeEvidenceBaseController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly IMapper mapper;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;

        public ManageEvidenceNotesController(IMapper mapper,
            BreadcrumbService breadcrumb,
            IWeeeCache cache,
            Func<IWeeeClient> apiClient)
        {
            this.mapper = mapper;
            this.breadcrumb = breadcrumb;
            this.cache = cache;
            this.apiClient = apiClient;
        }

        [HttpGet]
        public async Task<ActionResult> Index(Guid organisationId, ManageEvidenceNotesDisplayOptions? activeDisplayOption = null)
        {
            using (var client = this.apiClient())
            {
                await SetBreadcrumb(organisationId, BreadCrumbConstant.SchemeManageEvidence);

                var result = await client.SendAsync(User.GetAccessToken(), 
                    new GetEvidenceNotesByOrganisationRequest(organisationId, new List<NoteStatus>() { NoteStatus.Submitted }));

                var scheme = await client.SendAsync(User.GetAccessToken(), new GetSchemeByOrganisationId(organisationId));

                var schemeName = scheme != null ? scheme.SchemeName : string.Empty;

                var model = mapper.Map<ReviewSubmittedEvidenceNotesViewModel>(new ReviewSubmittedEvidenceNotesViewModelMapTransfer(organisationId, result, schemeName));

                return this.View("ReviewSubmittedEvidence", model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(Guid organisationId)
        {
            return RedirectToAction("TransferEvidenceNote", "ManageEvidenceNotes", new { area = "Scheme", organisationId });
        }

        [HttpGet]
        public async Task<ActionResult> TransferEvidenceNote(Guid organisationId)
        {
            using (var client = this.apiClient())
            {
                await SetBreadcrumb(organisationId, BreadCrumbConstant.SchemeManageEvidence);
                
                var model = new TransferEvidenceNoteDataViewModel();
                model.SchemasToDisplay = await GetApprovedSchemes();
                return this.View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> TransferEvidenceNote(TransferEvidenceNoteDataViewModel model)
        {
            using (var client = apiClient())
            {
                var selectedCaterogyIds = model.CategoryValues.Where(c => c.Selected).Select(c => c.CategoryId).ToList();

                if (ModelState.IsValid)
                {
                    var transferSelectedData = new TransferSelectedDataModel(selectedCaterogyIds, model.SelectedSchema.Value);

                    var sessionId = $"TransferEvidenceNoteData_{User.GetUserId()}_{model.SelectedSchema.Value}";
                    Session[sessionId] = transferSelectedData;
                }
                model.AddCategoryValues();
                
                // need to be refactored to look nicer
                if (selectedCaterogyIds.Any())
                {
                    foreach (var cat in selectedCaterogyIds)
                    {
                        for (int i = 0; i < model.CategoryValues.Count; i++)
                        {
                            if (model.CategoryValues[i].CategoryId == cat)
                            {
                                model.CategoryValues[i].Selected = true;
                            }
                        }
                    }
                }
                model.SchemasToDisplay = await GetApprovedSchemes();
                
                return this.View(model);
            }
        }

        private async Task<List<SchemeData>> GetApprovedSchemes()
        {
            using (var client = apiClient())
            {
                var schemes = await client.SendAsync(User.GetAccessToken(), new GetSchemesExternal(false));
                return schemes;
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