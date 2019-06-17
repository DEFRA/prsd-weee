namespace EA.Weee.Requests.Admin.DeleteAatf
{
    using EA.Prsd.Core.Mediator;
    using System;

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
