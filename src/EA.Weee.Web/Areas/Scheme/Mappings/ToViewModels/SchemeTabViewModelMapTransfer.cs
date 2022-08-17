namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels
{
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Core.Scheme;
    using EA.Weee.Web.ViewModels.Shared;
    using System;
    using CuttingEdge.Conditions;
    using Prsd.Core;

    public class SchemeTabViewModelMapTransfer
    {
        public Guid OrganisationId { get; protected set; }

        public SchemePublicInfo Scheme { get; protected set; }

        public EvidenceNoteSearchDataResult NoteData { get; protected set; }

        public DateTime CurrentDate { get; protected set; }

        public ManageEvidenceNoteViewModel ManageEvidenceNoteViewModel { get; protected set; }

        public int PageNumber { get; protected set; }

        public SchemeTabViewModelMapTransfer(Guid organisationId,
            EvidenceNoteSearchDataResult noteData,
            SchemePublicInfo scheme,
            DateTime currentDate,
            ManageEvidenceNoteViewModel manageEvidenceNoteViewModel,
            int pageNumber)
        {
            Guard.ArgumentNotDefaultValue(() => organisationId, organisationId);
            Guard.ArgumentNotNull(() => noteData, noteData);
            Condition.Requires(pageNumber).IsNotLessOrEqual(0);

            OrganisationId = organisationId;
            NoteData = noteData;
            Scheme = scheme;
            CurrentDate = currentDate;
            ManageEvidenceNoteViewModel = manageEvidenceNoteViewModel;
            PageNumber = pageNumber;
        }
    }
}