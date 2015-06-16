namespace EA.Weee.Requests.Organisations
{
    using System;
    using Prsd.Core.Mediator;
    using Shared;

    public class SaveOrganisationPrincipalPlaceOfBusiness : IRequest<Guid>
    {
        public Guid OrganisationId { get; set; }

        public AddressData PrincipalPlaceOfBusiness { get; set; }

        public SaveOrganisationPrincipalPlaceOfBusiness()
        {            
        }

        public SaveOrganisationPrincipalPlaceOfBusiness(Guid organisationId, AddressData principalPlaceOfBusiness)
        {
            OrganisationId = organisationId;
            PrincipalPlaceOfBusiness = principalPlaceOfBusiness;
        }
    }
}
