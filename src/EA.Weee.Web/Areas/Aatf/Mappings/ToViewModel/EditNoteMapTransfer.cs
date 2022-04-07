namespace EA.Weee.Web.Areas.Aatf.Mappings.ToViewModel
{
    using System;
    using System.Collections.Generic;
    using Core.AatfEvidence;
    using Core.Scheme;
    using Prsd.Core;
    using ViewModels;

    public class EditNoteMapTransfer : ModifyNoteTransfer
    {
        public EvidenceNoteData NoteData { get; protected set; }

        public EditNoteMapTransfer(List<SchemeData> schemes,
            EvidenceNoteViewModel existingModel,
            Guid organisationId, Guid aatfId, EvidenceNoteData noteData) : base(schemes, existingModel, organisationId, aatfId)
        {
            Guard.ArgumentNotNull(() => noteData, noteData);

            NoteData = noteData;
        }
    }
}