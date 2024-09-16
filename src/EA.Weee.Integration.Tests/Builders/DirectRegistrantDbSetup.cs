namespace EA.Weee.Integration.Tests.Builders
{
    using Base;
    using EA.Prsd.Core;
    using EA.Weee.Domain.Producer;
    using EA.Weee.Tests.Core;

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
    }
}
