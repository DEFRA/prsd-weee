namespace EA.Weee.Web.Areas.Aatf.Mappings.ToViewModel
{
    using System;
    using System.Collections.Generic;
    using EA.Weee.Core.Shared;
    using Prsd.Core;
    using ViewModels;

    public abstract class ModifyNoteTransfer
    {
        public List<EntityIdDisplayNameData> Schemes { get; private set; }

        public EditEvidenceNoteViewModel ExistingModel { get; set; }

        public Guid OrganisationId { get; set; }

        public Guid AatfId { get; set; }

        public int ComplianceYear { get; protected set; }

        protected ModifyNoteTransfer(List<EntityIdDisplayNameData> schemes,
            EditEvidenceNoteViewModel existingModel,
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