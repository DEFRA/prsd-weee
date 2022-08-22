namespace EA.Weee.Web.Areas.Shared.ToViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using EA.Weee.Core.Admin.Obligation;
    using EA.Weee.Core.DataReturns;
    using EA.Weee.Core.Helpers;
    using EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels;
    using EA.Weee.Web.Areas.Scheme.ViewModels.ManageEvidenceNotes;

    public abstract class ObligationEvidenceSummaryDataToViewModel<T, K> where T : IObligationSummaryBase where K : IObligationEvidenceSummaryBase
    {
        public static void MapToViewModel(T model, K source)
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
        }
    }
}
