namespace EA.Weee.Web.Areas.Shared.ToViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using EA.Weee.Core.Admin.Obligation;
    using EA.Weee.Core.DataReturns;
    using EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels;
    using EA.Weee.Web.Areas.Scheme.ViewModels.ManageEvidenceNotes;
    using EA.Weee.Web.ViewModels.Returns.Mappings.ToViewModel;

    public abstract class ObligationEvidenceSummaryDataToViewModel<T, K> where T : IObligationSummaryBase where K : IObligationEvidenceSummaryBase
    {
        private readonly ITonnageUtilities tonnageUtilities;

        public ObligationEvidenceSummaryDataToViewModel(ITonnageUtilities tonnageUtilities)
        {
            this.tonnageUtilities = tonnageUtilities;
        }

        public void MapToViewModel(T model, K source)
        {
            if (model == null)
            {
                throw new ArgumentNullException(typeof(T).Name);
            }

            if (source == null)
            {
                throw new ArgumentNullException(typeof(K).Name);
            }

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
                        Difference = tonnageUtilities.CheckIfTonnageIsNull(summaryCategory.Difference)
                    });
                }

                model.ObligationTotal = tonnageUtilities.CheckIfTonnageIsNull(source.ObligationEvidenceSummaryData.ObligationEvidenceValues.Sum(x => x.Obligation));
                model.Obligation210Total = tonnageUtilities.CheckIfTonnageIsNull(source.ObligationEvidenceSummaryData.ObligationEvidenceValues
                    .Where(x => !excludedCategories.Contains(x.CategoryId))
                    .Sum(x => x.Obligation));
                model.EvidenceTotal = tonnageUtilities.CheckIfTonnageIsNull(source.ObligationEvidenceSummaryData.ObligationEvidenceValues.Sum(x => x.Evidence));
                model.Evidence210Total = tonnageUtilities.CheckIfTonnageIsNull(source.ObligationEvidenceSummaryData.ObligationEvidenceValues
                    .Where(x => !excludedCategories.Contains(x.CategoryId))
                    .Sum(x => x.Evidence));
                model.ReuseTotal = tonnageUtilities.CheckIfTonnageIsNull(source.ObligationEvidenceSummaryData.ObligationEvidenceValues.Sum(x => x.Reuse));
                model.Reuse210Total = tonnageUtilities.CheckIfTonnageIsNull(source.ObligationEvidenceSummaryData.ObligationEvidenceValues
                    .Where(x => !excludedCategories.Contains(x.CategoryId))
                    .Sum(x => x.Reuse));
                model.TransferredOutTotal = tonnageUtilities.CheckIfTonnageIsNull(source.ObligationEvidenceSummaryData.ObligationEvidenceValues.Sum(x => x.TransferredOut));
                model.TransferredOut210Total = tonnageUtilities.CheckIfTonnageIsNull(source.ObligationEvidenceSummaryData.ObligationEvidenceValues
                    .Where(x => !excludedCategories.Contains(x.CategoryId))
                    .Sum(x => x.TransferredOut));
                model.TransferredInTotal = tonnageUtilities.CheckIfTonnageIsNull(source.ObligationEvidenceSummaryData.ObligationEvidenceValues.Sum(x => x.TransferredIn));
                model.TransferredIn210Total = tonnageUtilities.CheckIfTonnageIsNull(source.ObligationEvidenceSummaryData.ObligationEvidenceValues
                    .Where(x => !excludedCategories.Contains(x.CategoryId))
                    .Sum(x => x.TransferredIn));
                model.DifferenceTotal = tonnageUtilities.CheckIfTonnageIsNull(source.ObligationEvidenceSummaryData.ObligationEvidenceValues.Sum(x => x.Difference));
                model.Difference210Total = tonnageUtilities.CheckIfTonnageIsNull(source.ObligationEvidenceSummaryData.ObligationEvidenceValues
                    .Where(x => !excludedCategories.Contains(x.CategoryId))
                    .Sum(x => x.Difference));
            }
        }
    }
}
