namespace EA.Weee.Web.Areas.Scheme.ViewModels
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using Core.AatfEvidence;

    public class TransferEvidenceTonnageViewModel : TransferEvidenceViewModelBase
    {
        public enum ActionEnum
        {
            Save = 1,
            Copy = 2
        }

        [DisplayName("Transfer all tonnage from all notes that you have selected")]
        public bool TransferAllTonnage { get; set; }

        public ActionEnum Action { get; set; }

        public List<TransferEvidenceTonnageAatfViewModel> AatfViewModels;

        public IList<EvidenceCategoryValue> TransferCategoryValues { get; set; }

        public TransferEvidenceTonnageViewModel()
        {
            AatfViewModels = new List<TransferEvidenceTonnageAatfViewModel>();
        }
    }
}