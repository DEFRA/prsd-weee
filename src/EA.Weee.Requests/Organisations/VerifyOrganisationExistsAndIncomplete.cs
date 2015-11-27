namespace EA.Weee.Requests.Organisations
{
    using EA.Prsd.Core.Mediator;
    using System;

    public class VerifyOrganisationExistsAndIncomplete : IRequest<bool>
    {
        public Guid OrganisationId { get; private set; }

        public VerifyOrganisationExistsAndIncomplete(Guid organisationId)
        {
            OrganisationId = organisationId;
        }
    }
}
