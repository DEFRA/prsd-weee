namespace EA.Weee.Requests.Organisations
{
    using System;
    using Prsd.Core.Mediator;
    using Shared;

    public class SaveOrganisationPrincipalPlaceOfBusiness : IRequest<Guid>
    {
        public Guid OrganisationId { get; private set; }

        public AddressData PrincipalPlaceOfBusiness { get; private set; }

        public SaveOrganisationPrincipalPlaceOfBusiness(Guid organisationId, AddressData principalPlaceOfBusiness)
        {
            OrganisationId = organisationId;
            PrincipalPlaceOfBusiness = principalPlaceOfBusiness;
        }
    }
}
