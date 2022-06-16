namespace EA.Weee.Web.Areas.Scheme.ViewModels
{
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Web.Extensions;

    public class ViewTransferEvidenceNoteTonnageDataViewModel
    {
        public virtual int ReferenceId { get; set; }
        public virtual NoteType Type { get; set; }
        public virtual string ReferenceDisplay => $"{Type.ToDisplayString()}{ReferenceId}";
        public virtual bool DisplayReferenceCell { get; set; }
        public virtual string AatfName { get; set; }
        public virtual string AatfApprovalNumber { get; set; }
        public virtual bool DisplayAatfName { get; set; }
        public virtual EvidenceCategoryValue CategoryValue { get; set; }
    }
}