namespace EA.Weee.Web.Areas.Scheme.ViewModels
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.AatfEvidence;

    public class ViewTransferEvidenceAatfDataViewModel
    {
        public virtual string AatfName { get; set; }
        public virtual string AatfApprovalNumber { get; set; }
        public virtual IList<ViewTransferEvidenceNoteTonnageDataViewModel> Notes { get; set; }

        public bool DisplayAatf => true;
    }
}