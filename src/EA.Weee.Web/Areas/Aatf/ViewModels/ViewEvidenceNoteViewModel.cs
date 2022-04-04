namespace EA.Weee.Web.Areas.Aatf.ViewModels
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using Core.AatfEvidence;
    using Extensions;

    public class ViewEvidenceNoteViewModel : EvidenceNoteViewModel
    {
        public string SuccessMessage { get; set; }

        public bool DisplayMessage => !string.IsNullOrWhiteSpace(SuccessMessage);

        [DisplayName("Reference ID")]
        public string ReferenceDisplay
        {
            get
            {
                return $"{Type.ToDisplayString()}{Reference}";
            }
        }

        [DisplayName("Protocol")]
        public string ProtocolDisplay => ProtocolValue.HasValue ? ProtocolValue.ToString() : "-";

        [DisplayName("Type of waste")]
        public string WasteDisplay => WasteTypeValue.HasValue ? WasteTypeValue.ToString() : "-";

        public NoteStatus Status { get; set; }

        public NoteType Type { get; set; }

        [DisplayName("Compliance year")]
        public string ComplianceYearDisplay => StartDate.Year.ToString();

        public string SiteAddress { get; set; }

        public string OperatorAddress { get; set; }

        public string RecipientAddress { get; set; }
    }
}