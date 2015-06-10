namespace EA.Weee.Domain.Tests.Unit.Organisation
{
    using System;
    using FakeItEasy;
    using Xunit;
    using Organisation = Domain.Organisation;

    public class OrganisationTests
    {
        [Fact]
        public void CreateNewOrganisation_ThrowsArgumentNullExceptionIfTypeIsNotProvided()
        {
            Assert.Throws<ArgumentNullException>(() => new Organisation(null));
        }

        [Fact]
        public void CreateNewOrganisation_SetsStatusToIncomplete()
        {
            var organisationType = OrganisationType.SoleTraderOrIndividual; // cannot be null

            var organisation = new Organisation(organisationType);

            Assert.Equal(OrganisationStatus.Incomplete, organisation.OrganisationStatus);
        }
    }
}
