namespace EA.Weee.Web.Areas.Admin.Mappings.ToViewModel
{
    using CuttingEdge.Conditions;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.Admin.Obligation;
    using EA.Weee.Domain.Lookup;
    using EA.Weee.Web.Areas.Admin.ViewModels.Obligations;
    using System.Collections.Generic;
    using System.Linq;
    using Core.Helpers;

    public class ViewObligationsAndEvidenceSummaryViewModelMap : IMap<ViewObligationsAndEvidenceSummaryViewModelMapTransfer, ViewObligationsAndEvidenceSummaryViewModel>
    {
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
                }).GroupBy(s => s.Id)
                    .Select(s => s.First())
                    .OrderBy(s => s.DisplayName)
                    .ToList();
            }

            var model = new ViewObligationsAndEvidenceSummaryViewModel
            {
                ObligationEvidenceValues = new List<ObligationEvidenceValue>(),
                DisplayNoDataMessage = !source.ComplianceYears.Any(),
                ComplianceYearList = source.ComplianceYears,
                SchemeList = schemeList,
                SchemeId = source.SchemeData != null ? schemeList.Any(s => s.Id == source.SchemeId) ? source.SchemeId : null : source.SchemeId
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
                        Obligation = summaryCategory.Obligation.ToTonnageDisplay(),
                        Evidence = summaryCategory.Evidence.ToTonnageDisplay(),
                        Reused = summaryCategory.Reuse.ToTonnageDisplay(),
                        TransferredOut = summaryCategory.TransferredOut.ToTonnageDisplay(),
                        TransferredIn = summaryCategory.TransferredIn.ToTonnageDisplay(),
                        Difference = summaryCategory.Difference.ToTonnageDisplay()
                    });
                }

                model.ObligationTotal = source.ObligationEvidenceSummaryData.ObligationEvidenceValues.Sum(x => x.Obligation).ToTonnageDisplay();
                model.Obligation210Total = source.ObligationEvidenceSummaryData.ObligationEvidenceValues.Where(x => !excludedCategories.Contains((WeeeCategory)x.CategoryId)).Sum(x => x.Obligation).ToTonnageDisplay();
                model.EvidenceTotal = source.ObligationEvidenceSummaryData.ObligationEvidenceValues.Sum(x => x.Evidence).ToTonnageDisplay();
                model.Evidence210Total = source.ObligationEvidenceSummaryData.ObligationEvidenceValues.Where(x => !excludedCategories.Contains((WeeeCategory)x.CategoryId)).Sum(x => x.Evidence).ToTonnageDisplay();
                model.ReuseTotal = source.ObligationEvidenceSummaryData.ObligationEvidenceValues.Sum(x => x.Reuse).ToTonnageDisplay();
                model.Reuse210Total = source.ObligationEvidenceSummaryData.ObligationEvidenceValues.Where(x => !excludedCategories.Contains((WeeeCategory)x.CategoryId)).Sum(x => x.Reuse).ToTonnageDisplay();
                model.TransferredOutTotal = source.ObligationEvidenceSummaryData.ObligationEvidenceValues.Sum(x => x.TransferredOut).ToTonnageDisplay();
                model.TransferredOut210Total = source.ObligationEvidenceSummaryData.ObligationEvidenceValues.Where(x => !excludedCategories.Contains((WeeeCategory)x.CategoryId)).Sum(x => x.TransferredOut).ToTonnageDisplay();
                model.TransferredInTotal = source.ObligationEvidenceSummaryData.ObligationEvidenceValues.Sum(x => x.TransferredIn).ToTonnageDisplay();
                model.TransferredIn210Total = source.ObligationEvidenceSummaryData.ObligationEvidenceValues.Where(x => !excludedCategories.Contains((WeeeCategory)x.CategoryId)).Sum(x => x.TransferredIn).ToTonnageDisplay();
                model.DifferenceTotal = source.ObligationEvidenceSummaryData.ObligationEvidenceValues.Sum(x => x.Difference).ToTonnageDisplay();
                model.Difference210Total = source.ObligationEvidenceSummaryData.ObligationEvidenceValues.Where(x => !excludedCategories.Contains((WeeeCategory)x.CategoryId)).Sum(x => x.Difference).ToTonnageDisplay();
            }

            return model;
        }
    }
}