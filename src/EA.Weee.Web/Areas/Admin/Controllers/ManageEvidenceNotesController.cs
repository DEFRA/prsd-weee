namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Core.AatfEvidence;
    using EA.Prsd.Core.Extensions;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.Helpers;
    using EA.Weee.Requests.Admin;
    using EA.Weee.Web.Areas.Admin.Controllers.Base;
    using EA.Weee.Web.Areas.Admin.Mappings.ToViewModel;
    using EA.Weee.Web.Areas.Admin.ViewModels.ManageEvidenceNotes;
    using EA.Weee.Web.Areas.Admin.ViewModels.Shared;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using EA.Weee.Web.ViewModels.Shared;
    using EA.Weee.Web.ViewModels.Shared.Mapping;

    public class ManageEvidenceNotesController : AdminBreadcrumbBaseController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly IMapper mapper;

        public ManageEvidenceNotesController(IMapper mapper,
         BreadcrumbService breadcrumb,
         IWeeeCache cache,
         Func<IWeeeClient> apiClient) : base(breadcrumb, cache)
        {
            this.mapper = mapper;
            this.apiClient = apiClient;
        }

        [HttpGet]
        public async Task<ActionResult> Index(string tab = null, ManageEvidenceNoteViewModel manageEvidenceNoteViewModel = null)
        {
            SetBreadcrumb(BreadCrumbConstant.ManageEvidenceNotesAdmin);

            if (tab == null)
            {
                tab = Extensions.ToDisplayString(ManageEvidenceNotesTabDisplayOptions.ViewAllEvidenceNotes);
            }

            var value = tab.GetValueFromDisplayName<ManageEvidenceNotesTabDisplayOptions>();

            using (var client = this.apiClient())
            {
                switch (value)
                {
                    case ManageEvidenceNotesTabDisplayOptions.ViewAllEvidenceNotes:
                        return await ViewAllEvidenceNotes(client, manageEvidenceNoteViewModel);
                    default:
                        return await ViewAllEvidenceNotes(client, manageEvidenceNoteViewModel);
                }
            }
        }

        [HttpGet]
        public async Task<ActionResult> ViewEvidenceNoteDetails(Guid evidenceNoteId)
        {
            SetBreadcrumb(BreadCrumbConstant.ManageEvidenceNotesAdmin);

            using (var client = apiClient())
            {
                var request = new GetEvidenceNoteForInternalUserRequest(evidenceNoteId);

                var result = await client.SendAsync(User.GetAccessToken(), request);

                var model = mapper.Map<ViewEvidenceNoteViewModel>(new ViewEvidenceNoteMapTransfer(result, TempData[ViewDataConstant.EvidenceNoteStatus]));

                return View(model);
            }
        }

        private async Task<ActionResult> ViewAllEvidenceNotes(IWeeeClient client, ManageEvidenceNoteViewModel manageEvidenceNoteViewModel)
        {
            var allowedStatuses = new List<NoteStatus> { NoteStatus.Approved, NoteStatus.Rejected, NoteStatus.Submitted, NoteStatus.Returned, NoteStatus.Void };

            var notes = await client.SendAsync(User.GetAccessToken(), new GetAllNotes(new List<NoteType> { NoteType.Evidence }, allowedStatuses));

            var model = mapper.Map<ViewAllEvidenceNotesViewModel>(new ViewAllEvidenceNotesMapModel(notes, manageEvidenceNoteViewModel));

            return View("ViewAllEvidenceNotes", model);
        }
    }
}