namespace EA.Weee.Web.Areas.Aatf.Mappings.ToViewModel
{
    using System;
    using Core.AatfEvidence;
    using CuttingEdge.Conditions;
    using EA.Weee.Web.ViewModels.Shared;
    using Prsd.Core;

    public class EvidenceNotesViewModelTransfer
    {
        public Guid OrganisationId { get; protected set; }

        public Guid AatfId { get; protected set; }

        public EvidenceNoteSearchDataResult NoteData { get; protected set; }

        public DateTime CurrentDate { get; protected set; }

        public ManageEvidenceNoteViewModel ManageEvidenceNoteViewModel { get; protected set; }

        public int PageNumber { get; protected set; }

        public int PageSize { get; protected set; }

        public EvidenceNotesViewModelTransfer(Guid organisationId, 
            Guid aatfId, EvidenceNoteSearchDataResult noteData,
            DateTime currentDate,
            ManageEvidenceNoteViewModel manageEvidenceNoteViewModel, int pageNumber, int pageSize)
        {
            Guard.ArgumentNotDefaultValue(() => organisationId, organisationId);
            Guard.ArgumentNotNull(() => noteData, noteData);
            Guard.ArgumentNotDefaultValue(() => aatfId, aatfId);
            Condition.Requires(pageNumber).IsNotLessOrEqual(0);
            Condition.Requires(pageSize).IsNotLessOrEqual(0);

            OrganisationId = organisationId;
            AatfId = aatfId;
            NoteData = noteData;
            CurrentDate = currentDate;
            ManageEvidenceNoteViewModel = manageEvidenceNoteViewModel;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }
}