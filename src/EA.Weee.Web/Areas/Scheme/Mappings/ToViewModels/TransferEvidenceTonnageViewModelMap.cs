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
        private readonly IMap<ViewTransferNoteViewModelMapTransfer, ViewTransferNoteViewModel> transferNoteMapper;

        public TransferEvidenceTonnageViewModelMap(IMapper mapper, IWeeeCache cache, IMap<ViewTransferNoteViewModelMapTransfer, ViewTransferNoteViewModel> transferNoteMapper) : base(mapper, cache)
        {
            this.transferNoteMapper = transferNoteMapper;
        }

        public TransferEvidenceTonnageViewModel Map(TransferEvidenceNotesViewModelMapTransfer source)
        {
            Condition.Requires(source).IsNotNull();
            
            var model = MapBaseProperties(source);

            if (source.TransferEvidenceNoteData != null)
            {
                model.ViewTransferNoteViewModel = transferNoteMapper.Map(
                    new ViewTransferNoteViewModelMapTransfer(source.OrganisationId, source.TransferEvidenceNoteData, null));
            }

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

        private void SetupTotals(TransferEvidenceNotesViewModelMapTransfer source,
            TransferEvidenceTonnageViewModel model)
        {
            foreach (var category in model.CategoryValues)
            {
                if (source.TransferAllTonnage)
                {
                    category.TotalReceived = source.Notes.SelectMany(n => n.EvidenceTonnageData)
                        .Where(nt => nt.CategoryId.ToInt().Equals(category.CategoryId)).Sum(r => r.AvailableReceived)
                        .ToTonnageDisplay();
                    category.TotalReused = source.Notes.SelectMany(n => n.EvidenceTonnageData)
                        .Where(nt => nt.CategoryId.ToInt().Equals(category.CategoryId)).Sum(r => r.AvailableReused)
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
                var evidenceNoteData = source.Notes.FirstOrDefault(n => n.Id.Equals(viewModel.Id));

                if (evidenceNoteData != null)
                {
                    foreach (var evidenceTonnageData in evidenceNoteData.EvidenceTonnageData.OrderBy(c => c.CategoryId.ToInt()))
                    {
                        string receivedTonnage = null;
                        string reusedTonnage = null;
                        var id = evidenceTonnageData.Id;

                        if (source.TransferAllTonnage)
                        {
                            receivedTonnage = evidenceTonnageData.AvailableReceived.HasValue
                                ? evidenceTonnageData.AvailableReceived.ToTonnageDisplay()
                                : null;
                            reusedTonnage = evidenceTonnageData.AvailableReused.HasValue
                                ? evidenceTonnageData.AvailableReused.ToTonnageDisplay()
                                : null;
                        }

                        var transferTonnageData  = source.TransferEvidenceNoteData?.TransferEvidenceNoteTonnageData.FirstOrDefault(t1 => t1.EvidenceTonnageData.OriginatingNoteTonnageId == evidenceTonnageData.Id);

                        if (transferTonnageData != null)
                        {
                            receivedTonnage = transferTonnageData.EvidenceTonnageData.TransferredReceived.HasValue
                                ? transferTonnageData.EvidenceTonnageData.TransferredReceived.ToTonnageDisplay()
                                : string.Empty;

                            reusedTonnage = transferTonnageData.EvidenceTonnageData.TransferredReused.HasValue
                                ? transferTonnageData.EvidenceTonnageData.TransferredReused.ToTonnageDisplay()
                                : string.Empty;

                            id = transferTonnageData.EvidenceTonnageData.Id;
                        }
                        
                        var tonnage = new TransferEvidenceCategoryValue(evidenceTonnageData.CategoryId,
                            id,
                            evidenceTonnageData.AvailableReceived,
                            evidenceTonnageData.AvailableReused,
                            receivedTonnage,
                            reusedTonnage);

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

        private TransferEvidenceMapBaseTransfer CreateTransferEvidenceBaseTransferObject()
        {
            return null; // TO DO - Create Object and return and switch over TransferEvidenceMapBase to use this type
        }
    }
}