namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels
{
    using Core.Shared;
    using CuttingEdge.Conditions;
    using EA.Prsd.Core.Mapper;
    using Filters;
    using Services.Caching;
    using ViewModels;

    public class TransferEvidenceNotesViewModelMap : IMap<TransferEvidenceNotesViewModelMapTransfer, TransferEvidenceNotesViewModel>
    {
        private readonly IWeeeCache cache;

        public TransferEvidenceNotesViewModelMap(IMapper mapper, IWeeeCache cache)
        {
            this.cache = cache;
        }

        public TransferEvidenceNotesViewModel Map(TransferEvidenceNotesViewModelMapTransfer source)
        {
            Condition.Requires(source).IsNotNull();

            var model = new TransferEvidenceNotesViewModel()
            {
                PcsId = source.OrganisationId,
                RecipientName = AsyncHelpers.RunSync(async () => await cache.FetchSchemeName(source.Request.SchemeId))
            };

            foreach (var requestCategoryId in source.Request.CategoryIds)
            {
                model.CategoryValues.Add(new CategoryValue((Core.DataReturns.WeeeCategory)requestCategoryId));
            }
            return model;
        }
    }
}