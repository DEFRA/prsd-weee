namespace EA.Weee.Web.Areas.Scheme.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Attributes;
    using Core.AatfEvidence;
    using EA.Weee.Web.Areas.Aatf.Attributes;
    using EA.Weee.Web.Areas.Aatf.ViewModels;
    using Web.ViewModels.Shared;

    [Serializable]
    public class TransferEvidenceTonnageViewModel : TransferEvidenceViewModelBase, IActionModel
    {
        [DisplayName("Transfer all available tonnage from all notes that you have selected")]
        public bool TransferAllTonnage { get; set; }

        [RequiredTransferTonnage]
        public List<TransferEvidenceCategoryValue> TransferCategoryValues { get; set; }

        public ActionEnum Action { get; set; }

        public bool? ReturnToEditDraftTransfer { get; set; }

        public bool Edit => ViewTransferNoteViewModel != null && ViewTransferNoteViewModel.EvidenceNoteId != Guid.Empty;

        public TransferEvidenceTonnageViewModel()
        {
            TransferCategoryValues = new List<TransferEvidenceCategoryValue>();
        }
    }
}