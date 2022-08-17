namespace EA.Weee.Web.Areas.Admin.Mappings.ToViewModel
{
    using System;
    using System.Collections.Generic;
    using CuttingEdge.Conditions;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Web.ViewModels.Shared;

    public class ViewEvidenceNotesMapTransfer
    {
        public EvidenceNoteSearchDataResult NoteData { get; set; }

        public ManageEvidenceNoteViewModel ManageEvidenceNoteViewModel { get; set; }

        public DateTime CurrentDate { get; protected set; }

        public IEnumerable<int> ComplianceYearList { get; set; }

        public int PageNumber { get; private set; }

        public ViewEvidenceNotesMapTransfer(EvidenceNoteSearchDataResult noteData,
            ManageEvidenceNoteViewModel manageEvidenceNoteViewModel, DateTime currentDate, int pageNumber, IEnumerable<int> complianceYearList)
        {
            Condition.Requires(noteData).IsNotNull();
            Condition.Requires(pageNumber).IsNotLessOrEqual(0);
            
            NoteData = noteData;
            CurrentDate = currentDate;
            ManageEvidenceNoteViewModel = manageEvidenceNoteViewModel;
            ComplianceYearList = complianceYearList;
            PageNumber = pageNumber;
        }
    }
}
