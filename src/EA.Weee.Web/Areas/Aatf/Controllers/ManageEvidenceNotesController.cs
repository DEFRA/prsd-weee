namespace EA.Weee.Web.Areas.Aatf.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using AatfEvidence.Controllers;
    using Api.Client;
    using Attributes;
    using Core.AatfEvidence;
    using Core.AatfReturn;
    using EA.Weee.Requests.Aatf;
    using EA.Weee.Web.Constant;
    using Infrastructure;
    using Mappings.ToViewModel;
    using Prsd.Core.Mapper;
    using Services;
    using Services.Caching;
    using ViewModels;
    using Web.Requests.Base;
    using Weee.Requests.AatfEvidence;
    using Weee.Requests.AatfReturn;
    using Weee.Requests.Scheme;

    public class ManageEvidenceNotesController : AatfEvidenceBaseController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly IMapper mapper;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private readonly IRequestCreator<EvidenceNoteViewModel, CreateEvidenceNoteRequest> createRequestCreator;
        private readonly IRequestCreator<EvidenceNoteViewModel, EditEvidenceNoteRequest> editRequestCreator;

        public ManageEvidenceNotesController(IMapper mapper, 
            BreadcrumbService breadcrumb, 
            IWeeeCache cache, 
            Func<IWeeeClient> apiClient, 
            IRequestCreator<EvidenceNoteViewModel, CreateEvidenceNoteRequest> createRequestCreator, 
            IRequestCreator<EvidenceNoteViewModel, EditEvidenceNoteRequest> editRequestCreator)
        {
            this.mapper = mapper;
            this.breadcrumb = breadcrumb;
            this.cache = cache;
            this.apiClient = apiClient;
            this.createRequestCreator = createRequestCreator;
            this.editRequestCreator = editRequestCreator;
        }

        [HttpGet]
        public async Task<ActionResult> Index(Guid organisationId, Guid aatfId, 
            ManageEvidenceOverviewDisplayOption? activeOverviewDisplayOption = null,
            ManageEvidenceNoteViewModel viewModel = null)
        {
            await this.SetBreadcrumb(organisationId, BreadCrumbConstant.AatfManageEvidence);

            if (activeOverviewDisplayOption == null)
            {
                activeOverviewDisplayOption = ManageEvidenceOverviewDisplayOption.EditDraftAndReturnedNotes;
            }

            using (var client = this.apiClient())
            {
                var aatf = await client.SendAsync(this.User.GetAccessToken(), new GetAatfByIdExternal(aatfId));
                var allAatfsAndAes = await client.SendAsync(User.GetAccessToken(), new GetAatfByOrganisation(organisationId));

                var models = mapper.Map<SelectYourAatfViewModel>(new AatfDataToSelectYourAatfViewModelMapTransfer() { AatfList = allAatfsAndAes, OrganisationId = organisationId, FacilityType = FacilityType.Aatf });

                switch (activeOverviewDisplayOption.Value)
                {
                    case ManageEvidenceOverviewDisplayOption.EditDraftAndReturnedNotes:

                        return await EditDraftReturnNoteCase(client, 
                            organisationId, 
                            aatfId, 
                            aatf, 
                            models,
                            viewModel);

                    case ManageEvidenceOverviewDisplayOption.ViewAllOtherEvidenceNotes:

                        return await ViewAllOtherEvidenceNotesCase(organisationId, aatfId, client, aatf, models, viewModel);
                        
                    default:
                        return await EditDraftReturnNoteCase(client, organisationId, aatfId, aatf, models, viewModel);
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(ManageEvidenceNoteViewModel model)
        {
              return RedirectToAction("CreateEvidenceNote", "ManageEvidenceNotes", new { area = "Aatf", model.OrganisationId, model.AatfId });
        }

        [HttpGet]
        public async Task<ActionResult> CreateEvidenceNote(Guid organisationId, Guid aatfId)
        {
            using (var client = apiClient())
            {
                var schemes = await client.SendAsync(User.GetAccessToken(), new GetSchemesExternal(false));

                var model = mapper.Map<EvidenceNoteViewModel>(new CreateNoteMapTransfer(schemes, null, organisationId, aatfId));

                await SetBreadcrumb(organisationId, BreadCrumbConstant.AatfManageEvidence);

                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateEvidenceNote(EvidenceNoteViewModel viewModel, Guid organisationId, Guid aatfId)
        {
            using (var client = apiClient())
            {
                if (ModelState.IsValid)
                {
                    var request = createRequestCreator.ViewModelToRequest(viewModel);

                    TempData[ViewDataConstant.EvidenceNoteStatus] = request.Status;

                    var result = await client.SendAsync(User.GetAccessToken(), request);

                    return RedirectAfterNoteAction(organisationId, aatfId, request.Status, result);
                }

                var schemes = await client.SendAsync(User.GetAccessToken(), new GetSchemesExternal(false));
                
                var model = mapper.Map<EvidenceNoteViewModel>(new CreateNoteMapTransfer(schemes, viewModel, organisationId, aatfId));

                await SetBreadcrumb(organisationId, BreadCrumbConstant.AatfManageEvidence);

                return View(model);
            }
        }

        [HttpGet]
        public async Task<ActionResult> ViewDraftEvidenceNote(Guid organisationId, Guid evidenceNoteId)
        {
            using (var client = apiClient())
            {
                await SetBreadcrumb(organisationId, BreadCrumbConstant.AatfManageEvidence);

                var request = new GetEvidenceNoteRequest(evidenceNoteId);

                var result = await client.SendAsync(User.GetAccessToken(), request);

                var model = mapper.Map<ViewEvidenceNoteViewModel>(new ViewEvidenceNoteMapTransfer(result, TempData[ViewDataConstant.EvidenceNoteStatus]));

                return View(model);
            }
        }

        [HttpGet]
        [CheckEditEvidenceNoteStatus]
        public async Task<ActionResult> EditEvidenceNote(Guid organisationId, Guid evidenceNoteId)
        {
            using (var client = apiClient())
            {
                var schemes = await client.SendAsync(User.GetAccessToken(), new GetSchemesExternal(false));
                
                var request = new GetEvidenceNoteRequest(evidenceNoteId);
                var result = await client.SendAsync(User.GetAccessToken(), request);

                var model = mapper.Map<EvidenceNoteViewModel>(new EditNoteMapTransfer(schemes, null, organisationId, result.AatfData.Id, result));
                
                await SetBreadcrumb(organisationId, BreadCrumbConstant.AatfManageEvidence);

                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckEditEvidenceNoteStatus]
        public async Task<ActionResult> EditEvidenceNote(EvidenceNoteViewModel viewModel, Guid organisationId, Guid aatfId)
        {
            using (var client = apiClient())
            {
                if (ModelState.IsValid)
                {
                    var request = editRequestCreator.ViewModelToRequest(viewModel);

                    TempData[ViewDataConstant.EvidenceNoteStatus] = request.Status;

                    var result = await client.SendAsync(User.GetAccessToken(), request);

                    return RedirectAfterNoteAction(organisationId, aatfId, request.Status, result);
                }

                var schemes = await client.SendAsync(User.GetAccessToken(), new GetSchemesExternal(false));

                var model = mapper.Map<EvidenceNoteViewModel>(new EditNoteMapTransfer(schemes, viewModel, organisationId, aatfId, null));

                await SetBreadcrumb(organisationId, BreadCrumbConstant.AatfManageEvidence);

                return View(model);
            }
        }

        private async Task SetBreadcrumb(Guid organisationId, string activity)
        {
            breadcrumb.ExternalOrganisation = await cache.FetchOrganisationName(organisationId);
            breadcrumb.ExternalActivity = activity;
            breadcrumb.OrganisationId = organisationId;
        }

        private ActionResult RedirectAfterNoteAction(Guid organisationId, Guid aatfId, NoteStatus status,
            Guid result)
        {
            var routeName = status == NoteStatus.Draft
                ? AatfEvidenceRedirect.ViewDraftEvidenceRouteName
                : AatfEvidenceRedirect.ViewSubmittedEvidenceRouteName;

            return RedirectToRoute(routeName, new
            {
                organisationId,
                aatfId,
                evidenceNoteId = result
            });
        }
        private async Task<ActionResult> ViewAllOtherEvidenceNotesCase(Guid organisationId, Guid aatfId, IWeeeClient client, AatfData aatf,
            SelectYourAatfViewModel models, ManageEvidenceNoteViewModel manageEvidenceViewModel)
        {
            var resultAllNotes = await client.SendAsync(User.GetAccessToken(),
                new GetAatfNotesRequest(organisationId, aatfId,
                    new List<NoteStatus> { NoteStatus.Approved, NoteStatus.Rejected, NoteStatus.Submitted, NoteStatus.Void },
                    manageEvidenceViewModel?.FilterViewModel.SearchRef));

            var modelAllNotes =
                mapper.Map<AllOtherEvidenceNotesViewModel>(
                    new EditDraftReturnNotesViewModelTransfer(organisationId, aatfId, resultAllNotes));

            modelAllNotes.ManageEvidenceNoteViewModel =
                mapper.Map<ManageEvidenceNoteViewModel>(new ManageEvidenceNoteTransfer(organisationId, aatfId, aatf,
                    models.AatfList.ToList(), manageEvidenceViewModel?.FilterViewModel));

            return this.View("Overview/ViewAllOtherEvidenceOverview", modelAllNotes);
        }

        private async Task<ActionResult> EditDraftReturnNoteCase(IWeeeClient client, 
            Guid organisationId, 
            Guid aatfId, 
            AatfData aatf, 
            SelectYourAatfViewModel models,
            ManageEvidenceNoteViewModel manageEvidenceViewModel)
        {
            var result = await client.SendAsync(User.GetAccessToken(), 
                new GetAatfNotesRequest(organisationId, aatfId, new List<NoteStatus> { NoteStatus.Draft }, manageEvidenceViewModel?.FilterViewModel.SearchRef));
            
            var model = mapper.Map<EditDraftReturnedNotesViewModel>(new EditDraftReturnNotesViewModelTransfer(organisationId, aatfId, result));
            
            model.ManageEvidenceNoteViewModel = mapper.Map<ManageEvidenceNoteViewModel>(new ManageEvidenceNoteTransfer(organisationId, aatfId, aatf, models.AatfList.ToList(), manageEvidenceViewModel?.FilterViewModel));
            
            return this.View("Overview/EditDraftReturnedNotesOverview", model);
        }
    }
}