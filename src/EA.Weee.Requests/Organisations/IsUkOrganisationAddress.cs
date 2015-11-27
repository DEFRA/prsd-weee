namespace EA.Weee.Requests.Organisations
{
    using EA.Prsd.Core.Mediator;
    using System;

    public class IsUkOrganisationAddress : IRequest<bool>
    {
        public Guid OrganisationId { get; private set; }

        public IsUkOrganisationAddress(Guid organisationId)
        {
            OrganisationId = organisationId;
        }
    }
}
