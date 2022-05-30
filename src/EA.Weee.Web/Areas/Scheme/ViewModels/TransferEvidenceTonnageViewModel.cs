namespace EA.Weee.Web.Areas.Scheme.ViewModels
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using Core.AatfEvidence;
    using EA.Weee.Web.Areas.Aatf.Attributes;
    using EA.Weee.Web.Areas.Aatf.ViewModels;
    using Web.ViewModels.Shared;

    public class TransferEvidenceTonnageViewModel : TransferEvidenceViewModelBase, IActionModel
    {
        [DisplayName("Transfer all available tonnage from all notes that you have selected")]
        public bool TransferAllTonnage { get; set; }

        [RequiredTonnage]
        public List<TransferEvidenceCategoryValue> TransferCategoryValues { get; set; }

        public ActionEnum Action { get; set; }

        public bool Edit
        {
            get
            {
                return false;
            }
        }

        public TransferEvidenceTonnageViewModel()
        {
            TransferCategoryValues = new List<TransferEvidenceCategoryValue>();
        }
    }
}