namespace EA.Weee.Web.ViewModels.Shared.Mapping
{
    using System;
    using System.Collections.Generic;
    using Areas.Aatf.Mappings.ToViewModel;
    using Areas.Aatf.ViewModels;
    using Core.AatfEvidence;
    using Core.Scheme;
    using CuttingEdge.Conditions;

    public class EditNoteMapTransfer : ModifyNoteTransfer
    {
        public EvidenceNoteData NoteData { get; protected set; }

        public EditNoteMapTransfer(List<OrganisationSchemeData> schemes,
            EditEvidenceNoteViewModel existingModel,
            Guid organisationId, Guid aatfId, EvidenceNoteData noteData, int complianceYear) : base(schemes, existingModel, organisationId, aatfId)
        {
            ComplianceYear = complianceYear;
            NoteData = noteData;
        }
    }
}