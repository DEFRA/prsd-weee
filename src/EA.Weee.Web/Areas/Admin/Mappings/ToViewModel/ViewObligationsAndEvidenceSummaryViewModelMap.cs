namespace EA.Weee.Web.Areas.Admin.Mappings.ToViewModel
{
    using CuttingEdge.Conditions;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.Admin.Obligation;
    using EA.Weee.Domain.Lookup;
    using EA.Weee.Web.Areas.Admin.ViewModels.Obligations;
    using EA.Weee.Web.ViewModels.Returns.Mappings.ToViewModel;
    using System.Collections.Generic;
    using System.Linq;

    public class ViewObligationsAndEvidenceSummaryViewModelMap : IMap<ViewObligationsAndEvidenceSummaryViewModelMapTransfer, ViewObligationsAndEvidenceSummaryViewModel>
    {
        private readonly ITonnageUtilities tonnageUtilities;

        public ViewObligationsAndEvidenceSummaryViewModelMap(ITonnageUtilities tonnageUtilities)
        {
            this.tonnageUtilities = tonnageUtilities;
        }

        public ViewObligationsAndEvidenceSummaryViewModel Map(ViewObligationsAndEvidenceSummaryViewModelMapTransfer source)
        {
            Condition.Requires(source).IsNotNull();

            var schemeList = new List<Core.Scheme.OrganisationSchemeData>();
            if (source.SchemeData != null)
            {
                schemeList = source.SchemeData.Select(s => new Core.Scheme.OrganisationSchemeData()
                {
                    DisplayName = s.SchemeName,
                    Id = s.Id
                }).GroupBy(s => s.Id).Select(s => s.First()).ToList();
            }

            var model = new ViewObligationsAndEvidenceSummaryViewModel
            {
                ObligationEvidenceValues = new List<ObligationEvidenceValue>(),
                DisplayNoDataMessage = !source.ComplianceYears.Any(),
                ComplianceYearList = source.ComplianceYears,
                SchemeList = schemeList
            };

            var excludedCategories = new List<WeeeCategory>()
            {
                WeeeCategory.LargeHouseholdAppliances,
                WeeeCategory.DisplayEquipment,
                WeeeCategory.CoolingApplicancesContainingRefrigerants,
                WeeeCategory.GasDischargeLampsAndLedLightSources,
                WeeeCategory.PhotovoltaicPanels
            };

            if (source.ObligationEvidenceSummaryData != null)
            {
                foreach (var summaryCategory in source.ObligationEvidenceSummaryData.ObligationEvidenceValues)
                {
                    model.ObligationEvidenceValues.Add(new ObligationEvidenceValue(summaryCategory.CategoryId)
                    {
                        Obligation = tonnageUtilities.CheckIfTonnageIsNull(summaryCategory.Obligation),
                        Evidence = tonnageUtilities.CheckIfTonnageIsNull(summaryCategory.Evidence),
                        Reused = tonnageUtilities.CheckIfTonnageIsNull(summaryCategory.Reuse),
                        TransferredOut = tonnageUtilities.CheckIfTonnageIsNull(summaryCategory.TransferredOut),
                        TransferredIn = tonnageUtilities.CheckIfTonnageIsNull(summaryCategory.TransferredIn),
                        Difference = tonnageUtilities.CheckIfTonnageIsNull(summaryCategory.Difference < 0 ? 0 : summaryCategory.Difference)
                    });
                }

                model.ObligationTotal = tonnageUtilities.CheckIfTonnageIsNull(source.ObligationEvidenceSummaryData.ObligationEvidenceValues.Sum(x => x.Obligation));
                model.Obligation210Total = tonnageUtilities.CheckIfTonnageIsNull(source.ObligationEvidenceSummaryData.ObligationEvidenceValues.Where(x => !excludedCategories.Contains((WeeeCategory)x.CategoryId)).Sum(x => x.Obligation));
                model.EvidenceTotal = tonnageUtilities.CheckIfTonnageIsNull(source.ObligationEvidenceSummaryData.ObligationEvidenceValues.Sum(x => x.Evidence));
                model.Evidence210Total = tonnageUtilities.CheckIfTonnageIsNull(source.ObligationEvidenceSummaryData.ObligationEvidenceValues.Where(x => !excludedCategories.Contains((WeeeCategory)x.CategoryId)).Sum(x => x.Evidence));
                model.ReuseTotal = tonnageUtilities.CheckIfTonnageIsNull(source.ObligationEvidenceSummaryData.ObligationEvidenceValues.Sum(x => x.Reuse));
                model.Reuse210Total = tonnageUtilities.CheckIfTonnageIsNull(source.ObligationEvidenceSummaryData.ObligationEvidenceValues.Where(x => !excludedCategories.Contains((WeeeCategory)x.CategoryId)).Sum(x => x.Reuse));
                model.TransferredOutTotal = tonnageUtilities.CheckIfTonnageIsNull(source.ObligationEvidenceSummaryData.ObligationEvidenceValues.Sum(x => x.TransferredOut));
                model.TransferredOut210Total = tonnageUtilities.CheckIfTonnageIsNull(source.ObligationEvidenceSummaryData.ObligationEvidenceValues.Where(x => !excludedCategories.Contains((WeeeCategory)x.CategoryId)).Sum(x => x.TransferredOut));
                model.TransferredInTotal = tonnageUtilities.CheckIfTonnageIsNull(source.ObligationEvidenceSummaryData.ObligationEvidenceValues.Sum(x => x.TransferredIn));
                model.TransferredIn210Total = tonnageUtilities.CheckIfTonnageIsNull(source.ObligationEvidenceSummaryData.ObligationEvidenceValues.Where(x => !excludedCategories.Contains((WeeeCategory)x.CategoryId)).Sum(x => x.TransferredIn));
                model.DifferenceTotal = tonnageUtilities.CheckIfTonnageIsNull(source.ObligationEvidenceSummaryData.ObligationEvidenceValues.Sum(x => x.Difference));
                model.Difference210Total = tonnageUtilities.CheckIfTonnageIsNull(source.ObligationEvidenceSummaryData.ObligationEvidenceValues.Where(x => !excludedCategories.Contains((WeeeCategory)x.CategoryId)).Sum(x => x.Difference));
            }

            return model;
        }
    }
}