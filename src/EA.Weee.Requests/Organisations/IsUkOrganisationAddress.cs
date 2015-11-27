namespace EA.Weee.Requests.Organisations
{
    using System;
    using EA.Prsd.Core.Mediator;

    public class IsUkOrganisationAddress : IRequest<bool>
    {
        public Guid OrganisationId { get; private set; }

        public IsUkOrganisationAddress(Guid organisationId)
        {
            OrganisationId = organisationId;
        }
    }
}
