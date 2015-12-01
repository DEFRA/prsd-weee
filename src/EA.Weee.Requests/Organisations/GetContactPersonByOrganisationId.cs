namespace EA.Weee.Requests.Organisations
{
    using Prsd.Core.Mediator;
    using System;
    using Core.Organisations;

    public class GetContactPersonByOrganisationId : IRequest<ContactData>
    {
        public Guid OrganisationsId { get; private set; }

        public GetContactPersonByOrganisationId(Guid organisationsId)
        {
            OrganisationsId = organisationsId;
        }
    }
}
