namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels
{
    using Filters;
    using Prsd.Core.Mapper;
    using Services.Caching;
    using ViewModels;
    using Web.ViewModels.Shared;
    using Web.ViewModels.Shared.Mapping;

    public class OutgoingTransferEditTonnageViewModelMap : TransferEvidenceTonnageViewModel, IMap<OutgoingTransferEditTonnageViewModelTransfer, TransferEvidenceTonnageViewModel>
    {
        private readonly IWeeeCache cache;
        private readonly IMap<ViewTransferNoteViewModelMapTransfer, ViewTransferNoteViewModel> transferNoteMapper;

        public OutgoingTransferEditTonnageViewModelMap(IWeeeCache cache, IMap<ViewTransferNoteViewModelMapTransfer, ViewTransferNoteViewModel> transferNoteMapper)
        {
            this.cache = cache;
            this.transferNoteMapper = transferNoteMapper;
        }

        public TransferEvidenceTonnageViewModel Map(OutgoingTransferEditTonnageViewModelTransfer source)
        {
            var model = new TransferEvidenceTonnageViewModel
            {
                PcsId = source.SchemeId,
                RecipientName = AsyncHelpers.RunSync(async () => await cache.FetchSchemeName(source.TransferEvidenceNoteData.RecipientSchemeData.Id)),
                ViewTransferNoteViewModel = transferNoteMapper.Map(new ViewTransferNoteViewModelMapTransfer(source.SchemeId, source.TransferEvidenceNoteData, null))
            };
            
            return model;
        }
    }
}