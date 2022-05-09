﻿namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.AatfEvidence;
    using Core.Helpers;
    using CuttingEdge.Conditions;
    using EA.Prsd.Core.Mapper;
    using Services.Caching;
    using ViewModels;
    using Web.ViewModels.Shared;

    public class TransferEvidenceTonnageViewModelMap : TransferEvidenceMapBase<TransferEvidenceTonnageViewModel>, IMap<TransferEvidenceNotesViewModelMapTransfer, TransferEvidenceTonnageViewModel>
    {
        public TransferEvidenceTonnageViewModelMap(IMapper mapper, IWeeeCache cache) : base(mapper, cache)
        {
        }

        public TransferEvidenceTonnageViewModel Map(TransferEvidenceNotesViewModelMapTransfer source)
        {
            Condition.Requires(source).IsNotNull();
            
            var model = MapBaseProperties(source);
            model.TransferAllTonnage = source.TransferAllTonnage;

            model.EvidenceNotesDataList =
                model.EvidenceNotesDataList.OrderBy(a => a.SubmittedBy).ThenBy(ab => ab.Id).ToList();

            // Multiple evidence notes can come from the same AATF, where the AATF is the same for sequential evidence notes the aatf name isn't displayed
            for (var i = 0; i < model.EvidenceNotesDataList.Count; i++)
            {
                DisplayAatf(i, model);
            }

            SetupTonnages(source, model, model.EvidenceNotesDataList);

            SetupTotals(source, model);

            return model;
        }

        private static void SetupTotals(TransferEvidenceNotesViewModelMapTransfer source,
            TransferEvidenceTonnageViewModel model)
        {
            foreach (var category in model.CategoryValues)
            {
                if (source.TransferAllTonnage)
                {
                    category.TotalReceived = source.Notes.SelectMany(n => n.EvidenceTonnageData)
                        .Where(nt => nt.CategoryId.ToInt().Equals(category.CategoryId)).Sum(r => r.Received)
                        .ToTonnageDisplay();
                    category.TotalReused = source.Notes.SelectMany(n => n.EvidenceTonnageData)
                        .Where(nt => nt.CategoryId.ToInt().Equals(category.CategoryId)).Sum(r => r.Reused)
                        .ToTonnageDisplay();
                }
                else
                {
                    category.TotalReceived = 0m.ToTonnageDisplay();
                    category.TotalReused = 0m.ToTonnageDisplay();
                }
            }
        }

        private void SetupTonnages(TransferEvidenceNotesViewModelMapTransfer source,
            TransferEvidenceTonnageViewModel model, List<ViewEvidenceNoteViewModel> models)
        {
            foreach (var viewModel in models)
            {
                var evidenceNoteData = source.Notes.First(n => n.Id.Equals(viewModel.Id));

                foreach (var evidenceTonnageData in evidenceNoteData.EvidenceTonnageData)
                {
                    var tonnage = new EvidenceCategoryValue(evidenceTonnageData.CategoryId)
                    {
                        Id = evidenceTonnageData.Id,
                    };

                    if (source.TransferAllTonnage)
                    {
                        tonnage.Received = evidenceTonnageData.Received.HasValue
                            ? evidenceTonnageData.Received.ToTonnageDisplay()
                            : null;
                        tonnage.Reused = evidenceTonnageData.Reused.HasValue
                            ? evidenceTonnageData.Reused.ToTonnageDisplay()
                            : null;
                    }

                    model.TransferCategoryValues.Add(tonnage);
                }
            }
        }

        private void DisplayAatf(int i, TransferEvidenceViewModelBase model)
        {
            if (i > 0 && model.EvidenceNotesDataList.Count > 1 && model.EvidenceNotesDataList.ElementAt(i).SubmittedBy
                    .Equals(model.EvidenceNotesDataList.ElementAt(i - 1).SubmittedBy))
            {
                model.EvidenceNotesDataList.ElementAt(i).DisplayAatfName = false;
            }
            else
            {
                model.EvidenceNotesDataList.ElementAt(i).DisplayAatfName = true;
            }
        }
    }
}