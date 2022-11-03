namespace EA.Weee.Web.ViewModels.Shared.Mapping
{
    using System;
    using System.Collections.Generic;
    using Areas.Aatf.Mappings.ToViewModel;
    using Areas.Aatf.ViewModels;
    using Core.AatfEvidence;
    using EA.Weee.Core.Shared;

    public class EditNoteMapTransfer : ModifyNoteTransfer
    {
        public EvidenceNoteData NoteData { get; protected set; }

        public string QueryString { get; set; }

        public bool ReturnToView { get; protected set; }

        public EditNoteMapTransfer(List<EntityIdDisplayNameData> schemes,
            EditEvidenceNoteViewModel existingModel,
            Guid organisationId, Guid aatfId, EvidenceNoteData noteData, int complianceYear, 
            string queryString, bool returnToView) : base(schemes, existingModel, organisationId, aatfId)
        {
            ComplianceYear = complianceYear;
            NoteData = noteData;
            QueryString = queryString;
            ReturnToView = returnToView;
        }
    }
}