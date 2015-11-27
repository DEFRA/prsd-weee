namespace EA.Weee.Requests.Organisations
{
    using Core.Organisations;
    using Prsd.Core.Mediator;
    using System;

    public class GetContactPersonByOrganisationId : IRequest<ContactData>
    {
        public Guid OrganisationsId { get; private set; }

        public GetContactPersonByOrganisationId(Guid organisationsId)
        {
            OrganisationsId = organisationsId;
        }
    }
}
