namespace EA.Weee.Web.Areas.Scheme.Controllers
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Requests.AatfEvidence;
    using EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using ViewModels;

    public class OutgoingTransfersController : SchemeEvidenceBaseController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly IMapper mapper;

        public OutgoingTransfersController(IMapper mapper,
            BreadcrumbService breadcrumb,
            IWeeeCache cache,
            Func<IWeeeClient> apiClient) : base(breadcrumb, cache)
        {
            this.mapper = mapper;
            this.apiClient = apiClient;
        }

        [HttpGet]
        public async Task<ActionResult> EditTonnages(Guid pcsId, Guid evidenceNoteId)
        {
            await SetBreadcrumb(pcsId, BreadCrumbConstant.SchemeManageEvidence);

            using (var client = apiClient())
            {
                var noteData = await client.SendAsync(User.GetAccessToken(), new GetTransferEvidenceNoteForSchemeRequest(evidenceNoteId));

                var existingEvidenceNoteIds = noteData.TransferEvidenceNoteTonnageData.Select(t => t.OriginalNoteId).ToList();

                var result = await client.SendAsync(User.GetAccessToken(),
                    new GetEvidenceNotesForTransferRequest(pcsId, noteData.CategoryIds, existingEvidenceNoteIds));

                var mapperObject = new TransferEvidenceNotesViewModelMapTransfer(result, noteData, pcsId);

                var model = mapper.Map<TransferEvidenceNotesViewModelMapTransfer, TransferEvidenceTonnageViewModel>(mapperObject);

                return this.View("EditTonnages", model);
            }
        }

        [HttpGet]
        public async Task<ActionResult> EditDraftTransfer(Guid pcsId, Guid evidenceNoteId)
        {
            await SetBreadcrumb(pcsId, BreadCrumbConstant.SchemeManageEvidence);

            using (var client = apiClient())
            {
               
                return this.View("EditTonnages", model);
            }
        }
    }
}