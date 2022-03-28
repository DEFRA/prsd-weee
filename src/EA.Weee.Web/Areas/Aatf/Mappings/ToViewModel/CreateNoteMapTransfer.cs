namespace EA.Weee.Web.Areas.Aatf.Mappings.ToViewModel
{
    using System;
    using System.Collections.Generic;
    using Core.Scheme;
    using Prsd.Core;
    using ViewModels;

    public class CreateNoteMapTransfer
    {
        public List<SchemeData> Schemes { get; private set; }

        public EvidenceNoteViewModel ExistingModel { get; set; }

        public Guid OrganisationId { get; set; }

        public Guid AatfId { get; set; }

        public CreateNoteMapTransfer(List<SchemeData> schemes, 
            EvidenceNoteViewModel existingModel,
            Guid organisationId,
            Guid aatfId)
        {
            Guard.ArgumentNotNull(() => schemes, schemes);
            Guard.ArgumentNotDefaultValue(() => organisationId, organisationId);
            Guard.ArgumentNotDefaultValue(() => aatfId, aatfId);

            ExistingModel = existingModel;
            Schemes = schemes;
            OrganisationId = organisationId;
            AatfId = aatfId;
        }
    }
}