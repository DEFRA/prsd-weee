namespace EA.Weee.Requests.AatfEvidence
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mediator;
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class GetDraftReturnedNotesRequest : IRequest<List<EditDraftReturnedNotesRequest>>
    {
        public Guid OrganisationId { get; set; }

        public Guid AatfId { get; set; }

        public GetDraftReturnedNotesRequest(Guid organisationId, Guid aatfId)
        {
            Guard.ArgumentNotDefaultValue(() => organisationId, organisationId);
            Guard.ArgumentNotDefaultValue(() => aatfId, aatfId);

            OrganisationId = organisationId;
            AatfId = aatfId;
        }
    }
}
