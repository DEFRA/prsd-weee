namespace EA.Weee.Web.ViewModels.Shared
{
    using Areas.Aatf.ViewModels;
    using Core.AatfEvidence;
    using Core.Helpers;
    using EA.Weee.Web.Areas.Admin.ViewModels.Shared;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    [Serializable]
    public class EvidenceNoteViewModel
    {
        protected readonly ICategoryValueTotalCalculator CategoryValueCalculator;

        public Guid Id { get; set; }

        [DisplayName("Reference ID")]
        public string ReferenceDisplay => $"{Type.ToDisplayString()}{Reference}";

        [DisplayName("Start date")]
        public virtual DateTime? StartDate { get; set; }

        [DisplayName("End date")]
        public virtual DateTime? EndDate { get; set; }

        public virtual IList<EvidenceCategoryValue> CategoryValues { get; set; }

        [DisplayName("Type of waste")]
        public virtual WasteType? WasteTypeValue { get; set; }

        [DisplayName("Actual or protocol")]
        public virtual Protocol? ProtocolValue { get; set; }

        [DisplayName("Date submitted")]
        public string SubmittedDate { get; set; }

        [DisplayName("Date rejected")]
        public string RejectedDate { get; set; }

        [DisplayName("Date returned")]
        public string ReturnedDate { get; set; }

        [DisplayName("Date approved")]
        public string ApprovedDate { get; set; }

        [DisplayName("Date voided")]
        public string VoidedDate { get; set; }

        public int Reference { get; set; }

        [DisplayName("Reason")]
        public string ReturnedReason { get; set; }

        [DisplayName("Reason")]
        public string RejectedReason { get; set; }

        [DisplayName("Reason")]
        public string VoidedReason { get; set; }

        public bool CanVoid { get; set; }

        public bool DisplayReturnedReason => Status.Equals(NoteStatus.Returned) && !string.IsNullOrWhiteSpace(ReturnedReason);

        public bool DisplayRejectedReason => Status.Equals(NoteStatus.Rejected) && !string.IsNullOrWhiteSpace(RejectedReason);

        public bool DisplayVoidedReason => Status.Equals(NoteStatus.Void) && !string.IsNullOrWhiteSpace(VoidedReason);

        public EvidenceNoteViewModel()
        {
            CategoryValueCalculator = new CategoryValueTotalCalculator();
            AddCategoryValues(new EvidenceCategoryValues());
        }

        public EvidenceNoteViewModel(ICategoryValueTotalCalculator categoryValueCalculator)
        {
            this.CategoryValueCalculator = categoryValueCalculator;
            AddCategoryValues(new EvidenceCategoryValues());
        }

        public Guid OrganisationId { get; set; }

        public Guid AatfId { get; set; }

        public NoteStatus Status { get; set; }

        public NoteType Type { get; set; }

        private void AddCategoryValues(EvidenceCategoryValues evidenceCategoryValues)
        {
            CategoryValues = new List<EvidenceCategoryValue>();

            foreach (var categoryValue in evidenceCategoryValues)
            {
                CategoryValues.Add(categoryValue);
            }
        }

        public string ReceivedTotal => CategoryValueCalculator.Total(CategoryValues.Select(c => c.Received).ToList());

        public string ReusedTotal => CategoryValueCalculator.Total(CategoryValues.Select(c => c.Reused).ToList());

        public string AatfRedirectTab
        {
            get
            {
                if (Status.Equals(NoteStatus.Draft) ||
                    Status.Equals(NoteStatus.Returned))
                {
                    return ManageEvidenceOverviewDisplayOption.EditDraftAndReturnedNotes.ToDisplayString();
                }

                return ManageEvidenceOverviewDisplayOption.ViewAllOtherEvidenceNotes.ToDisplayString();
            }
        }

        public string InternalUserRedirectTab
        {
            get
            {
                if (Type.Equals(NoteType.Transfer))
                {
                    return ManageEvidenceNotesTabDisplayOptions.ViewAllEvidenceTransfers.ToDisplayString();
                }

                return ManageEvidenceNotesTabDisplayOptions.ViewAllEvidenceNotes.ToDisplayString();
            }
        }

        public int ComplianceYear { get; set; }
    }
}