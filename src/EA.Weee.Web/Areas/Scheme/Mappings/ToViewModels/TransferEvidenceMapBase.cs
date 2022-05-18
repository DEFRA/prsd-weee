namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels
{
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

        protected TransferEvidenceMapBase(IMapper mapper, IWeeeCache cache)
        {
            Mapper = mapper;
            Cache = cache;
        }

        protected T MapBaseProperties(TransferEvidenceNotesViewModelMapTransfer source)
        {
            Condition.Requires(source).IsNotNull();

            var model = new T()
            {
                PcsId = source.OrganisationId,
                RecipientName = AsyncHelpers.RunSync(async () => await Cache.FetchSchemeName(source.Request.SchemeId))
            };

            foreach (var evidenceNoteData in source.Notes)
            {
                model.EvidenceNotesDataList.Add(Mapper.Map<ViewEvidenceNoteViewModel>(
                    new ViewEvidenceNoteMapTransfer(evidenceNoteData, null)
                    {
                        IncludeAllCategories = false
                    }));
            }

            foreach (var requestCategoryId in source.Request.CategoryIds)
            {
                model.CategoryValues.Add(new TotalCategoryValue((Core.DataReturns.WeeeCategory)requestCategoryId));
            }

            return model;
        }
    }
}