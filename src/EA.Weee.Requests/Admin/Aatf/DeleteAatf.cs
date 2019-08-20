namespace EA.Weee.Requests.Admin.Aatf
{
    using System;
    using Prsd.Core.Mediator;

    public class DeleteAnAatf : IRequest<bool>
    {
        public Guid AatfId { get; private set; }
        public Guid OrganisationId { get; private set; }

        public DeleteAnAatf(Guid aatfId, Guid organisationId)
        {
            AatfId = aatfId;
            OrganisationId = organisationId;
        }
    }
}
