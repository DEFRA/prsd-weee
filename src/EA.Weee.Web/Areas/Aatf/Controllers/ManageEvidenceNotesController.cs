﻿namespace EA.Weee.Web.Areas.Aatf.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using AatfEvidence.Controllers;
    using Api.Client;
    using Core.AatfEvidence;
    using Core.AatfReturn;
    using EA.Weee.Requests.Aatf;
    using EA.Weee.Web.Constant;
    using Infrastructure;
    using Mappings.ToViewModel;
    using Prsd.Core;
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
        private readonly IRequestCreator<EvidenceNoteViewModel, EvidenceNoteBaseRequest> createRequestCreator;
    
        public ManageEvidenceNotesController(IMapper mapper, 
            BreadcrumbService breadcrumb, 
            IWeeeCache cache, 
            Func<IWeeeClient> apiClient, 
            IRequestCreator<EvidenceNoteViewModel, EvidenceNoteBaseRequest> createRequestCreator)
        {
            this.mapper = mapper;
            this.breadcrumb = breadcrumb;
            this.cache = cache;
            this.apiClient = apiClient;
            this.createRequestCreator = createRequestCreator;
        }

        [HttpGet]
        public async Task<ActionResult> Index(Guid organisationId, Guid aatfId)
        {
            using (var client = this.apiClient())
            {
                var aatf = await client.SendAsync(this.User.GetAccessToken(), new GetAatfByIdExternal(aatfId));
                var allAatfsAndAes = await client.SendAsync(User.GetAccessToken(), new GetAatfByOrganisation(organisationId));

                var models = mapper.Map<SelectYourAatfViewModel>(new AatfDataToSelectYourAatfViewModelMapTransfer() { AatfList = allAatfsAndAes, OrganisationId = organisationId, FacilityType = FacilityType.Aatf });

                var model = mapper.Map<ManageEvidenceNoteViewModel>(new ManageEvidenceNoteTransfer(organisationId, aatfId, aatf, models.AatfList.ToList()));

                await this.SetBreadcrumb(organisationId, BreadCrumbConstant.AatfManageEvidence);

                return this.View(model);
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

                    var routeName = request.Status == NoteStatus.Draft ? AatfEvidenceRedirect.ViewDraftEvidenceRouteName : AatfEvidenceRedirect.ViewSubmittedEvidenceRouteName;
                    return RedirectToRoute(routeName, new
                    {
                        organisationId,
                        aatfId,
                        evidenceNoteId = result
                    });
                }

                var schemes = await client.SendAsync(User.GetAccessToken(), new GetSchemesExternal(false));
                
                var model = mapper.Map<EvidenceNoteViewModel>(new CreateNoteMapTransfer(schemes, viewModel, organisationId, aatfId));

                await SetBreadcrumb(organisationId, BreadCrumbConstant.AatfManageEvidence);

                return View(model);
            }
        }

        [HttpGet]
        public async Task<ActionResult> ViewDraftEvidenceNote(Guid organisationId, Guid aatfId, Guid evidenceNoteId)
        {
            using (var client = apiClient())
            {
                await SetBreadcrumb(organisationId, BreadCrumbConstant.AatfManageEvidence);

                var request = new GetEvidenceNoteRequest(evidenceNoteId, organisationId);

                // retrieve the evidence note
                var result = await client.SendAsync(User.GetAccessToken(), request);

                //TODO: create view model mapper, to map EvidenceNote to ViewModel and to map if success message should be displayed
                var model = mapper.Map<ViewEvidenceNoteViewModel>(new ViewEvidenceNoteMapTransfer(result));
                model.OrganisationId = organisationId;
                model.AatfId = aatfId;

                SetSuccessMessage(result, model);

                //TODO: Remove the Viewbag items below as they should be based off the view model

                //TODO: update the view to only show the success based on a view model property
                //ViewBag.EvidenceNoteId = evidenceNoteId;
                //ViewBag.aatfId = aatfId;
                //ViewBag.organisationId = organisationId;

                return View(model);
            }
        }

        [HttpGet]
        public ActionResult Overview(Guid organisationId, Guid aatfId, ManageEvidenceOverviewDisplayOption? overviewDisplayOption = null, string clicked = null)
        {
            using (var client = apiClient())
            {
                var result = Task.Run(() => client.SendAsync(User.GetAccessToken(), new GetAatfNotesRequest(organisationId, aatfId))).Result;

                var modelToViewList = new EditDraftReturnedNotesViewModel();

                if (result != null && result.Any())
                {
                    foreach (var res in result)
                    {
                        var model = mapper.Map<EditDraftReturnedNote>
                            (new EditDraftReturnedNotesModel(res.Reference, res.SchemeData.SchemeName, res.Status, res.WasteType));

                        modelToViewList.ListOfNotes.Add(model);
                    }
                }
                modelToViewList.AatfId = aatfId;
                modelToViewList.OrganisationId = organisationId;

                return PartialView("Overview/EditDraftReturnedNotesOverview", modelToViewList);
            }
        }

        private void SetSuccessMessage(EvidenceNoteData note, ViewEvidenceNoteViewModel model)
        {
            if (TempData[ViewDataConstant.EvidenceNoteStatus] != null)
            {
                if (TempData[ViewDataConstant.EvidenceNoteStatus] is NoteStatus status)
                {
                    model.SuccessMessage = (status == NoteStatus.Submitted ?
                        $"You have successfully submitted the evidence note with reference ID E{note.Reference}" : $"You have successfully saved the evidence note with reference ID E{note.Reference} as a draft");

                    //TODO: Move this to the mapper
                    model.Status = status;
                }
                else
                {
                    model.Status = NoteStatus.Draft;
                }
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