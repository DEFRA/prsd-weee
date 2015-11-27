namespace EA.Weee.Requests.Organisations
{
    using System;
    using EA.Prsd.Core.Mediator;

    public class VerifyOrganisationExistsAndIncomplete : IRequest<bool>
    {
        public Guid OrganisationId { get; private set; }

        public VerifyOrganisationExistsAndIncomplete(Guid organisationId)
        {
            OrganisationId = organisationId;
        }
    }
}
