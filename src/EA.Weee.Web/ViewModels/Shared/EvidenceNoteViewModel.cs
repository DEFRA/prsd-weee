namespace EA.Weee.Web.ViewModels.Shared
{
    using Areas.Aatf.ViewModels;
    using Core.AatfEvidence;
    using Core.Helpers;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    [Serializable]
    public class EvidenceNoteViewModel
    {
        private readonly ICategoryValueTotalCalculator categoryValueCalculator;

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

        public int Reference { get; set; }

        [DisplayName("Reason")]
        public string ReturnedReason { get; set; }

        [DisplayName("Reason")]
        public string RejectedReason { get; set; }

        public bool DisplayReturnedReason => Status.Equals(NoteStatus.Returned) && !string.IsNullOrWhiteSpace(ReturnedReason);

        public bool DisplayRejectedReason => Status.Equals(NoteStatus.Rejected) && !string.IsNullOrWhiteSpace(RejectedReason);

        public EvidenceNoteViewModel()
        {
            categoryValueCalculator = new CategoryValueTotalCalculator();
            AddCategoryValues(new EvidenceCategoryValues());
        }

        public EvidenceNoteViewModel(ICategoryValueTotalCalculator categoryValueCalculator)
        {
            this.categoryValueCalculator = categoryValueCalculator;
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

        public string ReceivedTotal => categoryValueCalculator.Total(CategoryValues.Select(c => c.Received).ToList());

        public string ReusedTotal => categoryValueCalculator.Total(CategoryValues.Select(c => c.Reused).ToList());

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

        public string TabName
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
                    default:
                        return string.Empty;
                }
            }
        }
    }
}