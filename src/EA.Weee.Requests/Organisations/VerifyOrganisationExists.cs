namespace EA.Weee.Requests.Organisations
{
    using System;
    using EA.Prsd.Core.Mediator;

    public class VerifyOrganisationExists : IRequest<bool>
    {
        public Guid OrganisationId { get; private set; }

        public VerifyOrganisationExists(Guid organisationId)
        {
            OrganisationId = organisationId;
        }
    }
}
