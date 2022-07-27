namespace EA.Weee.Web.Areas.Scheme.ViewModels
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class ViewTransferEvidenceAatfDataViewModel
    {
        public virtual string AatfName { get; set; }
        public virtual string AatfApprovalNumber { get; set; }
        public virtual IList<ViewTransferEvidenceNoteTonnageDataViewModel> Notes { get; set; }

        public bool DisplayAatf => true;
    }
}