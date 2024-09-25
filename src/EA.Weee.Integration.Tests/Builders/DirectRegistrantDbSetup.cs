namespace EA.Weee.Integration.Tests.Builders
{
    using Base;
    using EA.Weee.Domain.Producer;
    using EA.Weee.Tests.Core;
    using System;

    public class DirectRegistrantDbSetup : DbTestDataBuilder<DirectRegistrant, DirectRegistrantDbSetup>
    {
        protected override DirectRegistrant Instantiate()
        {
            var organisation = OrganisationDbSetup.Init().Create();

            instance = new DirectRegistrant(organisation);

            return instance;
        }

        public DirectRegistrantDbSetup WithAuthorisedRep(AuthorisedRepresentative authorisedRep)
        {
            ObjectInstantiator<DirectRegistrant>.SetProperty(o => o.AuthorisedRepresentativeId, authorisedRep.Id, instance);

            return this;
        }

        public DirectRegistrantDbSetup WithOrganisation(Guid organisationId)
        {
            ObjectInstantiator<DirectRegistrant>.SetProperty(o => o.OrganisationId, organisationId, instance);

            return this;
        }

        public DirectRegistrantDbSetup WithContact(Guid contactId)
        {
            ObjectInstantiator<DirectRegistrant>.SetProperty(o => o.ContactId, contactId, instance);

            return this;
        }

        public DirectRegistrantDbSetup WithAddress(Guid addressId)
        {
            ObjectInstantiator<DirectRegistrant>.SetProperty(o => o.AddressId, addressId, instance);

            return this;
        }

        public DirectRegistrantDbSetup WithBrandName(string brandName)
        {
            ObjectInstantiator<DirectRegistrant>.SetProperty(o => o.BrandName, new BrandName(brandName), instance);

            return this;
        }
    }
}
