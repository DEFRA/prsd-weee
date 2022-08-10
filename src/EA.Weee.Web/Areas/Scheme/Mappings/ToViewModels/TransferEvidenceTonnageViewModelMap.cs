namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels
{
    using System;
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
        public TransferEvidenceTonnageViewModelMap(IMapper mapper, IWeeeCache cache, IMap<ViewTransferNoteViewModelMapTransfer, ViewTransferNoteViewModel> transferNoteMapper) : base(mapper, cache, transferNoteMapper)
        {
        }

        public TransferEvidenceTonnageViewModel Map(TransferEvidenceNotesViewModelMapTransfer source)
        {
            Condition.Requires(source).IsNotNull();
            
            var model = MapBaseProperties(source);

            model.TransferAllTonnage = source.TransferAllTonnage;

            model.EvidenceNotesDataList =
                model.EvidenceNotesDataList.OrderBy(a => a.SubmittedBy).ThenByDescending(ab => ab.Reference).ToList();

            // Multiple evidence notes can come from the same AATF, where the AATF is the same for sequential evidence notes the aatf name isn't displayed
            for (var i = 0; i < model.EvidenceNotesDataList.Count; i++)
            {
                DisplayAatf(i, model);
            }

            SetupTonnages(source, model, model.EvidenceNotesDataList);

            SetupExistingValues(source, model);

            SetupTotals(source, model);

            return model;
        }

        private void SetupExistingValues(TransferEvidenceNotesViewModelMapTransfer source,
            TransferEvidenceTonnageViewModel model)
        {
            if (source.ExistingTransferTonnageViewModel != null)
            {
                foreach (var transferEvidenceCategoryValue in model.TransferCategoryValues)
                {
                    var existingValue =
                        source.ExistingTransferTonnageViewModel.TransferCategoryValues.FirstOrDefault(t =>
                            t.Id == transferEvidenceCategoryValue.Id);

                    if (existingValue != null)
                    {
                        transferEvidenceCategoryValue.Received = existingValue.Received;
                        transferEvidenceCategoryValue.Reused = existingValue.Reused;
                    }
                }
            }
        }

        private void SetupTotals(TransferEvidenceNotesViewModelMapTransfer source,
            TransferEvidenceTonnageViewModel model)
        {
            foreach (var category in model.CategoryValues)
            {
                if (source.Request != null && source.TransferAllTonnage)
                {
                    category.TotalReceived = source.Notes.Results.SelectMany(n => n.EvidenceTonnageData)
                        .Where(nt => nt.CategoryId.ToInt().Equals(category.CategoryId)).Sum(r => r.AvailableReceived)
                        .ToTonnageDisplay();
                    category.TotalReused = source.Notes.Results.SelectMany(n => n.EvidenceTonnageData)
                        .Where(nt => nt.CategoryId.ToInt().Equals(category.CategoryId)).Sum(r => r.AvailableReused)
                        .ToTonnageDisplay();
                }
                else if (source.TransferEvidenceNoteData != null)     
                {
                    category.TotalReceived = source.TransferEvidenceNoteData.TransferEvidenceNoteTonnageData.Select(n => n.EvidenceTonnageData)
                        .Where(nt => nt.CategoryId.ToInt().Equals(category.CategoryId)).Sum(r => r.TransferredReceived)
                        .ToTonnageDisplay();
                    category.TotalReused = source.TransferEvidenceNoteData.TransferEvidenceNoteTonnageData.Select(n => n.EvidenceTonnageData)
                        .Where(nt => nt.CategoryId.ToInt().Equals(category.CategoryId)).Sum(r => r.TransferredReused)
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
                var evidenceNoteData = source.Notes.Results.FirstOrDefault(n => n.Id.Equals(viewModel.Id));

                if (evidenceNoteData != null)
                {
                    foreach (var evidenceTonnageData in evidenceNoteData.EvidenceTonnageData.OrderBy(c => c.CategoryId.ToInt()))
                    {
                        string receivedTonnage = null;
                        string reusedTonnage = null;
                        var availableReceived = evidenceTonnageData.AvailableReceived;
                        var availableReused = evidenceTonnageData.AvailableReused;

                        var existingTonnageId = evidenceTonnageData.Id;
                        var transferNoteTonnageId = Guid.Empty;

                        if (source.TransferAllTonnage)
                        {
                            receivedTonnage = availableReceived.HasValue
                                ? availableReceived.ToTonnageDisplay()
                                : null;
                            reusedTonnage = availableReused.HasValue
                                ? availableReused.ToTonnageDisplay()
                                : null;
                        }
                        else
                        {
                            var transferTonnageData = source.TransferEvidenceNoteData?.TransferEvidenceNoteTonnageData.FirstOrDefault(t1 => t1.EvidenceTonnageData.OriginatingNoteTonnageId == evidenceTonnageData.Id);

                            if (transferTonnageData != null)
                            {
                                if (transferTonnageData.EvidenceTonnageData.TransferredReceived.HasValue)
                                {
                                    availableReceived += transferTonnageData.EvidenceTonnageData.TransferredReceived
                                        .Value;
                                }
                                if (transferTonnageData.EvidenceTonnageData.TransferredReused.HasValue)
                                {
                                    availableReused += transferTonnageData.EvidenceTonnageData.TransferredReused
                                        .Value;
                                }

                                receivedTonnage = transferTonnageData.EvidenceTonnageData.TransferredReceived.HasValue
                                    ? transferTonnageData.EvidenceTonnageData.TransferredReceived.ToTonnageDisplay()
                                    : string.Empty;

                                reusedTonnage = transferTonnageData.EvidenceTonnageData.TransferredReused.HasValue
                                    ? transferTonnageData.EvidenceTonnageData.TransferredReused.ToTonnageDisplay()
                                    : string.Empty;

                                transferNoteTonnageId = transferTonnageData.EvidenceTonnageData.Id;
                            }
                        }

                        var tonnage = new TransferEvidenceCategoryValue(evidenceTonnageData.CategoryId,
                            transferNoteTonnageId,
                            availableReceived,
                            availableReused,
                            receivedTonnage,
                            reusedTonnage)
                        {
                            Id = existingTonnageId
                        };

                        model.TransferCategoryValues.Add(tonnage);
                    }
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