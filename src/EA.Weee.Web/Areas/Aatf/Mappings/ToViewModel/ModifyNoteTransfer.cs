namespace EA.Weee.Web.Areas.Aatf.Mappings.ToViewModel
{
    using System;
    using System.Collections.Generic;
    using Core.Scheme;
    using Prsd.Core;
    using ViewModels;
    using Web.ViewModels.Shared;

    public abstract class ModifyNoteTransfer
    {
        public List<OrganisationSchemeData> Schemes { get; private set; }

        public EditEvidenceNoteViewModel ExistingModel { get; set; }

        public Guid OrganisationId { get; set; }

        public Guid AatfId { get; set; }

        public int ComplianceYear { get; protected set; }

        protected ModifyNoteTransfer(List<OrganisationSchemeData> schemes,
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