namespace EA.Weee.Web.Areas.Aatf.Mappings.ToViewModel
{
    using System;
    using System.Collections.Generic;
    using Core.AatfReturn;
    using Prsd.Core;

    public class ManageEvidenceNoteTransfer
    {
        public Guid OrganisationId { get; protected set; }
        public Guid AatfId { get; protected set; }

        public AatfData AatfData { get; protected set; }

        public List<AatfData> Aatfs { get; protected set; }

        public ManageEvidenceNoteTransfer(Guid organisationId, Guid aatfId, AatfData aatfData, List<AatfData> aatfs)
        {
            Guard.ArgumentNotDefaultValue(() => organisationId, organisationId);
            Guard.ArgumentNotNull(() => aatfData, aatfData);
            Guard.ArgumentNotNull(() => aatfs, aatfs);
            Guard.ArgumentNotDefaultValue(() => aatfId, aatfId);
            
            OrganisationId = organisationId;
            AatfData = aatfData;
            Aatfs = aatfs;
            AatfId = aatfId;
        }
    }
}