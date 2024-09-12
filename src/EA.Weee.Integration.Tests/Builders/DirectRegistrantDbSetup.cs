namespace EA.Weee.Integration.Tests.Builders
{
    using Base;
    using EA.Prsd.Core;
    using EA.Weee.Domain.Producer;

    public class DirectRegistrantDbSetup : DbTestDataBuilder<DirectRegistrant, DirectRegistrantDbSetup>
    {
        protected override DirectRegistrant Instantiate()
        {
            var organisation = OrganisationDbSetup.Init().Create();

            instance = new DirectRegistrant(organisation);

            return instance;
        }
    }
}
