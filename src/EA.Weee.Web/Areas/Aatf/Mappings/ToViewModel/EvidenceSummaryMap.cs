namespace EA.Weee.Web.Areas.Aatf.Mappings.ToViewModel
{
    using System.Linq;
    using Core.AatfEvidence;
    using Core.Helpers;
    using CuttingEdge.Conditions;
    using EA.Weee.Web.ViewModels.Shared;
    using Prsd.Core.Mapper;
    using ViewModels;
    using Web.ViewModels.Returns.Mappings.ToViewModel;

    public class EvidenceSummaryMap : IMap<EvidenceSummaryMapTransfer, ManageEvidenceSummaryViewModel>
    {
        private readonly ICategoryValueTotalCalculator totalCalculator;
        private readonly ITonnageUtilities tonnageUtilities;

        public EvidenceSummaryMap(ICategoryValueTotalCalculator totalCalculator, 
            ITonnageUtilities tonnageUtilities)
        {
            this.totalCalculator = totalCalculator;
            this.tonnageUtilities = tonnageUtilities;
        }

        public ManageEvidenceSummaryViewModel Map(EvidenceSummaryMapTransfer source)
        {
            Condition.Requires(source).IsNotNull();

            var model = new ManageEvidenceSummaryViewModel()
            {
                CategoryValues = new EvidenceCategoryValues(),
                ManageEvidenceNoteViewModel = new ManageEvidenceNoteViewModel()
                {
                    OrganisationId = source.OrganisationId,
                    AatfId = source.AatfId
                }
            };

            foreach (var categoryDisplayValue in model.CategoryValues)
            {
                var evidenceSummaryData = source.AatfEvidenceSummaryData.EvidenceCategoryTotals.FirstOrDefault(c => c.CategoryId.ToInt().Equals(categoryDisplayValue.CategoryId.ToInt()));

                if (evidenceSummaryData != null)
                {
                    categoryDisplayValue.Received = tonnageUtilities.CheckIfTonnageIsNull(evidenceSummaryData.Received);
                    categoryDisplayValue.Reused = tonnageUtilities.CheckIfTonnageIsNull(evidenceSummaryData.Reused);
                }
            }

            model.TotalReceivedEvidence = totalCalculator.Total(model.CategoryValues.Select(t => t.Received).ToList());
            model.TotalReuseEvidence = totalCalculator.Total(model.CategoryValues.Select(t => t.Reused).ToList());
            model.NumberOfSubmittedNotes = source.AatfEvidenceSummaryData.NumberOfSubmittedNotes.ToString();
            model.NumberOfApprovedNotes = source.AatfEvidenceSummaryData.NumberOfApprovedNotes.ToString();
            model.NumberOfDraftNotes = source.AatfEvidenceSummaryData.NumberOfDraftNotes.ToString();

            return model;
        }
    }
}