﻿namespace EA.Weee.Web.ViewModels.Shared
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using Areas.Aatf.ViewModels;
    using Core.AatfEvidence;
    using Core.Helpers;

    public class EvidenceNoteViewModel
    {
        private readonly ICategoryValueTotalCalculator categoryValueCalculator;

        public Guid Id { get; set; }

        [DisplayName("Reference ID")]
        public string ReferenceDisplay => $"{Type.ToDisplayString()}{Reference}";

        [Display(Name = "Start date")]
        public virtual DateTime? StartDate { get; set; }

        [Display(Name = "End date")]
        public virtual DateTime EndDate { get; set; }

        public virtual IList<EvidenceCategoryValue> CategoryValues { get; set; }

        [Display(Name = "Type of waste")]
        public virtual WasteType? WasteTypeValue { get; set; }

        [Display(Name = "Actual or protocol")]
        public virtual Protocol? ProtocolValue { get; set; }

        [DisplayName("Date submitted")]
        public string SubmittedDate { get; set; }

        [DisplayName("Date returned")]
        public string ReturnedDate { get; set; }

        [DisplayName("Date approved")]
        public string ApprovedDate { get; set; }

        public int Reference { get; set; }

        [DisplayName("Reason for return")]
        public string Reason { get; set; }

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

        public ManageEvidenceOverviewDisplayOption RedirectTab
        {
            get
            {
                if (Status.Equals(NoteStatus.Draft) ||
                    Status.Equals(NoteStatus.Rejected) ||
                    Status.Equals(NoteStatus.Returned))
                {
                    return ManageEvidenceOverviewDisplayOption.EditDraftAndReturnedNotes;
                }

                return ManageEvidenceOverviewDisplayOption.ViewAllOtherEvidenceNotes;
            }
        }
    }
}