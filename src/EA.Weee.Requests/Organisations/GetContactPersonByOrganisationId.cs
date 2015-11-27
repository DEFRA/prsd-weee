namespace EA.Weee.Requests.Organisations
{
    using System;
    using Core.Organisations;
    using Prsd.Core.Mediator;

    public class GetContactPersonByOrganisationId : IRequest<ContactData>
    {
        public Guid OrganisationsId { get; private set; }

        public GetContactPersonByOrganisationId(Guid organisationsId)
        {
            OrganisationsId = organisationsId;
        }
    }
}
