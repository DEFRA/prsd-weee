namespace EA.Weee.Requests.Organisations
{
    using EA.Prsd.Core.Mediator;
    using System;

    public class VerifyOrganisationExists : IRequest<bool>
    {
        public Guid OrganisationId { get; private set; }

        public VerifyOrganisationExists(Guid organisationId)
        {
            OrganisationId = organisationId;
        }
    }
}
