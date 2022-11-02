﻿namespace EA.Weee.Web.ViewModels.Shared
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using Core.AatfEvidence;
    using Extensions;

    [Serializable]
    public class ViewEvidenceNoteViewModel : EvidenceNoteViewModel
    {
        public string SuccessMessage { get; set; }

        public bool DisplayMessage => !string.IsNullOrWhiteSpace(SuccessMessage);

        [DisplayName("Actual or protocol")]
        public string ProtocolDisplay => ProtocolValue.HasValue ? ProtocolValue.Value.ToDisplayString() : string.Empty;

        [DisplayName("Obligation type")]
        public string WasteDisplay => WasteTypeValue.HasValue ? WasteTypeValue.Value.ToDisplayString() : string.Empty;

        [DisplayName("Compliance year")]
        public string ComplianceYearDisplay => ComplianceYear.ToString();

        public string SiteAddress { get; set; }

        public string OperatorAddress { get; set; }

        public string RecipientAddress { get; set; }

        public bool DisplayEditButton { get; set; }

        public bool HasSubmittedDate => !string.IsNullOrWhiteSpace(SubmittedDate);

        public bool HasApprovedDate => !string.IsNullOrWhiteSpace(ApprovedDate);

        public bool HasRejectedDate => Status.Equals(NoteStatus.Rejected);

        public bool HasBeenReturned => Status.Equals(NoteStatus.Returned);

        public bool HasBeenVoided => Status.Equals(NoteStatus.Void);

        public bool HasVoidReason => !string.IsNullOrEmpty(VoidedReason);

        public Guid SchemeId { get; set; }

        public string SubmittedBy { get; set; }

        public string TotalAvailable { get; set; }

        public string AatfApprovalNumber { get; set; }

        public bool DisplayAatfName { get; set; }

        public bool DisplayH2Title { get; set; }

        public IList<EvidenceNoteRowViewModel> EvidenceNoteHistoryData { get; set; }

        public bool DisplayEvidenceNoteHistoryData => EvidenceNoteHistoryData != null && EvidenceNoteHistoryData.Count > 0;

        public virtual IList<EvidenceCategoryValue> RemainingTransferCategoryValues { get; set; }

        public bool DisplayTransferEvidenceColumns { get; set; }

        public string TransferReceivedRemainingTotalDisplay => CategoryValueCalculator.Total(RemainingTransferCategoryValues.Select(x => x.Received).ToList());

        public string TransferReusedRemainingTotalDisplay => CategoryValueCalculator.Total(RemainingTransferCategoryValues.Select(x => x.Reused).ToList());

        public virtual string TabName
        {
            get
            {
                switch (Status)
                {
                    case NoteStatus.Draft:
                        return "Draft evidence note";
                    case NoteStatus.Rejected:
                        return "Rejected evidence note";
                    case NoteStatus.Approved:
                        return "Approved evidence note";
                    case NoteStatus.Returned:
                        return "Returned evidence note";
                    case NoteStatus.Submitted:
                        return "Submitted evidence note";
                    case NoteStatus.Void:
                        return "Voided evidence note";
                    default:
                        return string.Empty;
                }
            }
        }

        public string RedirectTab { get; set; }

        public bool CanDisplayNotesMessage { get; set; }

        public bool IsPrintable { get; set; }

        public bool OpenedInNewTab { get; set; }

        public int Page { get; set; }

        public bool IsInternalUser { get; set; }

        public bool CanDisplayPdfLink => (Status == NoteStatus.Approved || Status == NoteStatus.Submitted || Status == NoteStatus.Rejected || Status == NoteStatus.Returned);

        public ViewEvidenceNoteViewModel()
        {
            AddTransferCategoryValues(new EvidenceCategoryValues());
        }

        private void AddTransferCategoryValues(EvidenceCategoryValues evidenceCategoryValues)
        {
            RemainingTransferCategoryValues = new List<EvidenceCategoryValue>();

            foreach (var categoryValue in evidenceCategoryValues)
            {
                RemainingTransferCategoryValues.Add(categoryValue);
            }
        }
    }
}