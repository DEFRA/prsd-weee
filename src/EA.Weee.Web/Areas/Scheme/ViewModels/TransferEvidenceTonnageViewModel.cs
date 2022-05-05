namespace EA.Weee.Web.Areas.Scheme.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Core.AatfEvidence;
    using ManageEvidenceNotes;

    public class TransferEvidenceTonnageViewModel : TransferEvidenceViewModelBase
    {
        public enum ActionEnum
        {
            Save = 1,
            Copy = 2,
            Index = 3
        }

        [DisplayName("Transfer all tonnage from all notes that you have selected")]
        public bool TransferAllTonnage { get; set; }

        public ActionEnum Action { get; set; }

        public List<EvidenceCategoryValue> TransferCategoryValues { get; set; }

        public TransferEvidenceTonnageViewModel()
        {
            TransferCategoryValues = new List<EvidenceCategoryValue>();
        }
    }
}