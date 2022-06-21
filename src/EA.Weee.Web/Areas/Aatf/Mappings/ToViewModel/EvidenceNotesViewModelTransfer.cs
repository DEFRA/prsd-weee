namespace EA.Weee.Web.Areas.Aatf.Mappings.ToViewModel
{
    using System;
    using System.Collections.Generic;
    using Core.AatfEvidence;
    using Prsd.Core;
    using ViewModels;

    public class EvidenceNotesViewModelTransfer
    {
        public Guid OrganisationId { get; protected set; }

        public Guid AatfId { get; protected set; }

        public List<EvidenceNoteData> Notes { get; protected set; }

        public DateTime CurrentDate { get; protected set; }

        public ManageEvidenceNoteViewModel ManageEvidenceNoteViewModel { get; private set; }

        public EvidenceNotesViewModelTransfer(Guid organisationId, 
            Guid aatfId, List<EvidenceNoteData> notes, 
            DateTime currentDate,
            ManageEvidenceNoteViewModel manageEvidenceNoteViewModel)
        {
            Guard.ArgumentNotDefaultValue(() => organisationId, organisationId);
            Guard.ArgumentNotNull(() => notes, notes);
            Guard.ArgumentNotDefaultValue(() => aatfId, aatfId);

            OrganisationId = organisationId;
            AatfId = aatfId;
            Notes = notes;
            CurrentDate = currentDate;
            ManageEvidenceNoteViewModel = manageEvidenceNoteViewModel;
        }
    }
}