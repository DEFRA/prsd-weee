namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels
{
    using System.Collections.Generic;
    using System.Linq;
    using CuttingEdge.Conditions;
    using EA.Weee.Core.Scheme;
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

            var schemeData = source.RecipientData.ToList();
            schemeData.RemoveAll(s => s.Id == source.OrganisationId);
            model.SchemasToDisplay = schemeData;

            if (source.ExistingTransferEvidenceNoteCategoriesViewModel != null)
            {
                model.SelectedSchema = source.ExistingTransferEvidenceNoteCategoriesViewModel.SelectedSchema;

                SetCategoryValues(source.ExistingTransferEvidenceNoteCategoriesViewModel.SelectedCategoryValues, model);
            }
            else
            {
                model.OrganisationId = source.OrganisationId;
                model.SelectedSchema = source.TransferEvidenceNoteData.RecipientOrganisationData.Id;

                SetCategoryValues(source.TransferEvidenceNoteData.CategoryIds, model);
            }
            
            return model;
        }

        private static void SetCategoryValues(List<int> categoryValues,
            TransferEvidenceNoteCategoriesViewModel model)
        {
            foreach (var categoryBooleanViewModel in model.CategoryBooleanViewModels)
            {
                if (categoryValues.Contains(categoryBooleanViewModel.CategoryId))
                {
                    categoryBooleanViewModel.Selected = true;
                }
            }
        }
    }
}