namespace EA.Weee.Web.Areas.Aatf.Mappings.ToViewModel
{
    using System;
    using System.Collections.Generic;
    using EA.Weee.Core.Shared;
    using ViewModels;

    public class CreateNoteMapTransfer : ModifyNoteTransfer
    {
        public CreateNoteMapTransfer(List<EntityIdDisplayNameData> schemes, 
            EditEvidenceNoteViewModel existingModel, 
            Guid organisationId, Guid aatfId, int complianceYear) : base(schemes, existingModel, organisationId, aatfId)
        {
            ComplianceYear = complianceYear;
        }
    }
}