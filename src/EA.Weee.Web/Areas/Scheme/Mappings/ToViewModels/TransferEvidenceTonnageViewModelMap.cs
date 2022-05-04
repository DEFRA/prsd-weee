namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels
{
    using CuttingEdge.Conditions;
    using EA.Prsd.Core.Mapper;
    using Services.Caching;
    using ViewModels;

    public class TransferEvidenceTonnageViewModelMap : TransferEvidenceMapBase<TransferEvidenceTonnageViewModel>, IMap<TransferEvidenceNotesViewModelMapTransfer, TransferEvidenceTonnageViewModel>
    {
        public TransferEvidenceTonnageViewModelMap(IMapper mapper, IWeeeCache cache) : base(mapper, cache)
        {
        }

        public TransferEvidenceTonnageViewModel Map(TransferEvidenceNotesViewModelMapTransfer source)
        {
            Condition.Requires(source).IsNotNull();
            Condition.Requires(source.Request).IsNotNull();

            var model = MapBaseProperties(source);
            
            return model;
        }
    }
}