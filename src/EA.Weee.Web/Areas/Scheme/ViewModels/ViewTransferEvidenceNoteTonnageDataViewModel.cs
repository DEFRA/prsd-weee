namespace EA.Weee.Web.Areas.Scheme.ViewModels
{
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Web.Extensions;
    using System.Collections.Generic;

    public class ViewTransferEvidenceNoteTonnageDataViewModel
    {
        public virtual int ReferenceId { get; set; }
        public virtual NoteType Type { get; set; }
        public virtual string ReferenceDisplay => $"{Type.ToDisplayString()}{ReferenceId}";
        public virtual IList<EvidenceCategoryValue> CategoryValues { get; set; }
        public virtual bool DisplayTransferNote => CategoryValues != null && CategoryValues.Count > 0;
    }
}