namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels
{
    using System.Linq;
    using CuttingEdge.Conditions;
    using Prsd.Core.Mapper;
    using Services.Caching;
    using ViewModels;
    using Web.ViewModels.Shared;

    public class TransferEvidenceCategoriesViewModelMap : TransferEvidenceMapBase<TransferEvidenceNoteCategoriesViewModel>, IMap<TransferEvidenceNotesViewModelMapTransfer, TransferEvidenceNoteCategoriesViewModel>
    {
        public TransferEvidenceCategoriesViewModelMap(IWeeeCache cache, IMapper mapper, IMap<ViewTransferNoteViewModelMapTransfer, ViewTransferNoteViewModel> transferNoteMapper) : base(mapper, cache, transferNoteMapper)
        {
        }

        public TransferEvidenceNoteCategoriesViewModel Map(TransferEvidenceNotesViewModelMapTransfer source)
        {
            Condition.Requires(source).IsNotNull();

            var model = MapBaseProperties(source);

            var schemeData = source.SchemeData.ToList();
            schemeData.RemoveAll(s => s.OrganisationId == source.OrganisationId);

            model.OrganisationId = source.OrganisationId;
            model.SchemasToDisplay = schemeData;
            model.SelectedSchema = source.TransferEvidenceNoteData.RecipientSchemeData.Id;

            foreach (var categoryBooleanViewModel in model.CategoryValues)
            {
                if (source.TransferEvidenceNoteData.CategoryIds.Contains(categoryBooleanViewModel.CategoryId))
                {
                    categoryBooleanViewModel.Selected = true;
                }
            }

            return model;
        }
    }
}