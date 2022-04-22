namespace EA.Weee.Web.Areas.Aatf.Mappings.ToViewModel
{
    using System;
    using System.Collections.Generic;
    using Core.Scheme;
    using ViewModels;

    public class CreateNoteMapTransfer : ModifyNoteTransfer
    {
        public CreateNoteMapTransfer(List<SchemeData> schemes, 
            EditEvidenceNoteViewModel existingModel, 
            Guid organisationId, Guid aatfId) : base(schemes, existingModel, organisationId, aatfId)
        {
        }
    }
}