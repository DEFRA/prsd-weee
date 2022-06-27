namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels
{
    using System.Linq;
    using Core.Helpers;
    using CuttingEdge.Conditions;
    using Filters;
    using Prsd.Core.Mapper;
    using Services.Caching;
    using ViewModels;
    using Web.ViewModels.Shared;
    using Web.ViewModels.Shared.Mapping;

    public abstract class TransferEvidenceMapBase<T> where T : TransferEvidenceViewModelBase, new()
    {
        protected readonly IWeeeCache Cache;
        protected readonly IMapper Mapper;
        protected readonly IMap<ViewTransferNoteViewModelMapTransfer, ViewTransferNoteViewModel> TransferNoteMapper;

        protected TransferEvidenceMapBase(IMapper mapper, IWeeeCache cache, IMap<ViewTransferNoteViewModelMapTransfer, ViewTransferNoteViewModel> transferNoteMapper)
        {
            Mapper = mapper;
            Cache = cache;
            TransferNoteMapper = transferNoteMapper;
        }

        protected T MapBaseProperties(TransferEvidenceNotesViewModelMapTransfer source)
        {
            Condition.Requires(source).IsNotNull();

            var recipientId = source.Request?.SchemeId ?? source.TransferEvidenceNoteData.RecipientSchemeData.Id;

            var model = new T()
            {
                PcsId = source.OrganisationId,
                RecipientName = AsyncHelpers.RunSync(async () => await Cache.FetchSchemeName(recipientId))
            };

            foreach (var evidenceNoteData in source.Notes)
            {
                model.EvidenceNotesDataList.Add(Mapper.Map<ViewEvidenceNoteViewModel>(
                    new ViewEvidenceNoteMapTransfer(evidenceNoteData, null)
                    {
                        IncludeAllCategories = false
                    }));
            }

            if (source.TransferEvidenceNoteData != null)
            {
                model.ViewTransferNoteViewModel = TransferNoteMapper.Map(
                    new ViewTransferNoteViewModelMapTransfer(source.OrganisationId, source.TransferEvidenceNoteData, null));
            }

            var categoryValues = source.Request != null
                ? source.Request.CategoryIds
                : source.TransferEvidenceNoteData.CategoryIds;

            foreach (var requestCategoryId in categoryValues.OrderBy(c => c.ToInt()))
            {
                model.CategoryValues.Add(new TotalCategoryValue((Core.DataReturns.WeeeCategory)requestCategoryId));
            }

            return model;
        }
    }
}