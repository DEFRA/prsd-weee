namespace EA.Weee.Web.Areas.Scheme.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Constant;
    using Core.Scheme;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Requests.Scheme;
    using Infrastructure;
    using Services;
    using Services.Caching;
    using ViewModels;
    using Web.Requests.Base;

    public class TransferEvidenceController : SchemeEvidenceBaseController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly IMapper mapper;
        private readonly IRequestCreator<TransferEvidenceNoteDataViewModel, TransferEvidenceNoteRequest> transferNoteRequestCreator;
        private readonly ISessionService sessionService;

        public TransferEvidenceController(Func<IWeeeClient> apiClient, BreadcrumbService breadcrumb, IMapper mapper, IRequestCreator<TransferEvidenceNoteDataViewModel, TransferEvidenceNoteRequest> transferNoteRequestCreator, IWeeeCache cache, ISessionService sessionService) : base(breadcrumb, cache)
        {
            this.apiClient = apiClient;
            this.mapper = mapper;
            this.transferNoteRequestCreator = transferNoteRequestCreator;
            this.sessionService = sessionService;
        }

        [HttpGet]
        public async Task<ActionResult> TransferEvidenceNote(Guid organisationId)
        {
            using (var client = this.apiClient())
            {
                await SetBreadcrumb(organisationId, BreadCrumbConstant.SchemeManageEvidence);

                var model = new TransferEvidenceNoteDataViewModel();
                model.OrganisationId = organisationId;
                model.SchemasToDisplay = await GetApprovedSchemes();
                return this.View("TransferEvidenceNote", model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> TransferEvidenceNote(TransferEvidenceNoteDataViewModel model)
        {
            var ids = model.CategoryValues.Where(c => c.Selected).Select(c => c.CategoryId).ToList();

            using (var client = apiClient())
            {
                if (ModelState.IsValid)
                {
                    var transferRequest = transferNoteRequestCreator.ViewModelToRequest(model);

                    sessionService.SetTransferNoteSessionObject(Session, transferRequest);

                    return RedirectToAction("Index", "Holding", new { Area = "Aatf", OrganisationId = model.OrganisationId });
                }

                await SetBreadcrumb(model.OrganisationId, BreadCrumbConstant.SchemeManageEvidence);

                model.AddCategoryValues();
                CheckedCategoryIds(model, ids);
                model.SchemasToDisplay = await GetApprovedSchemes();

                return View(model);
            }
        }

        private void CheckedCategoryIds(TransferEvidenceNoteDataViewModel model, List<int> ids)
        {
            if (ids.Any())
            {
                foreach (var id in ids)
                {
                    for (int i = 0; i < model.CategoryValues.Count; i++)
                    {
                        if (model.CategoryValues[i].CategoryId == id)
                        {
                            model.CategoryValues[i].Selected = true;
                        }
                    }
                }
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
    }
}