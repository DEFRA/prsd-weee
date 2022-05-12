namespace EA.Weee.Web.Areas.Scheme.ViewModels
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using Core.AatfEvidence;

    public class TransferEvidenceTonnageViewModel : TransferEvidenceViewModelBase
    {
        [DisplayName("Transfer all available tonnage from all notes that you have selected")]
        public bool TransferAllTonnage { get; set; }

        public List<EvidenceCategoryValue> TransferCategoryValues { get; set; }

        public TransferEvidenceTonnageViewModel()
        {
            TransferCategoryValues = new List<EvidenceCategoryValue>();
        }
    }
}