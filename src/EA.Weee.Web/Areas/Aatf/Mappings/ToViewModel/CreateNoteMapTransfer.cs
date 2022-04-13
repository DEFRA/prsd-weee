namespace EA.Weee.Web.Areas.Aatf.Mappings.ToViewModel
{
    using System;
    using System.Collections.Generic;
    using Core.Scheme;
    using Prsd.Core;
    using ViewModels;

    public class CreateNoteMapTransfer : ModifyNoteTransfer
    {
        public CreateNoteMapTransfer(List<SchemeData> schemes, 
            EvidenceNoteViewModel existingModel, 
            Guid organisationId, Guid aatfId) : base(schemes, existingModel, organisationId, aatfId)
        {
        }
    }
}