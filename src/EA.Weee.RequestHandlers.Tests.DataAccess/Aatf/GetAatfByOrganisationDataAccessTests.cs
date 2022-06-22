namespace EA.Weee.RequestHandlers.Tests.DataAccess.Aatf
{
    using System.Linq;
    using AutoFixture;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.Aatf;
    using EA.Weee.RequestHandlers.Factories;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using Weee.Tests.Core.Model;
    using Xunit;
    using Organisation = Domain.Organisation.Organisation;

    public class GetAatfByOrganisationDataAccessTests
    {
        private readonly Fixture fixture;
        private readonly IQuarterWindowFactory quarterWindowFactory;

        public GetAatfByOrganisationDataAccessTests()
        {
            fixture = new Fixture();
            quarterWindowFactory = A.Fake<IQuarterWindowFactory>();
        }

        [Fact]
        public async void GetAatfsForOrganisation_GivenOrganisationAatfsShouldBeReturned()
        {
            using (var databaseWrapper = new DatabaseWrapper())
            {
                var aatfDataAccess = new AatfDataAccess(databaseWrapper.WeeeContext, GetGenericDataAccess(databaseWrapper), quarterWindowFactory);
                var organisation = Organisation.CreateSoleTrader(fixture.Create<string>());

                var aatf1 = ObligatedWeeeIntegrationCommon.CreateAatf(databaseWrapper, organisation);
                ObjectInstantiator<EA.Weee.Domain.AatfReturn.Aatf>.SetProperty<AatfStatus>(a => a.AatfStatus, AatfStatus.Approved, aatf1);

                var aatf2 = ObligatedWeeeIntegrationCommon.CreateAatf(databaseWrapper, organisation);
                ObjectInstantiator<EA.Weee.Domain.AatfReturn.Aatf>.SetProperty<AatfStatus>(a => a.AatfStatus, AatfStatus.Cancelled, aatf2);

                var aatf3 = ObligatedWeeeIntegrationCommon.CreateAatf(databaseWrapper, organisation);
                ObjectInstantiator<EA.Weee.Domain.AatfReturn.Aatf>.SetProperty<AatfStatus>(a => a.AatfStatus, AatfStatus.Suspended, aatf3);

                var otherOrganisation = Organisation.CreateSoleTrader(fixture.Create<string>());
                var aatf4NotToBeIncluded = ObligatedWeeeIntegrationCommon.CreateAatf(databaseWrapper, otherOrganisation);

                databaseWrapper.WeeeContext.Aatfs.Add(aatf1);
                databaseWrapper.WeeeContext.Aatfs.Add(aatf2);
                databaseWrapper.WeeeContext.Aatfs.Add(aatf3);
                databaseWrapper.WeeeContext.Aatfs.Add(aatf4NotToBeIncluded);

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                var aatfs = await aatfDataAccess.GetAatfsForOrganisation(organisation.Id);

                aatfs.Count.Should().Be(3);
                aatfs.FirstOrDefault(a => a.Id == aatf1.Id).Should().NotBeNull();
                aatfs.FirstOrDefault(a => a.Id == aatf2.Id).Should().NotBeNull();
                aatfs.FirstOrDefault(a => a.Id == aatf3.Id).Should().NotBeNull();
                aatfs.FirstOrDefault(a => a.Id == aatf4NotToBeIncluded.Id).Should().BeNull();
            }
        }

        private GenericDataAccess GetGenericDataAccess(DatabaseWrapper databaseWrapper)
        {
            return new GenericDataAccess(databaseWrapper.WeeeContext);
        }
    }
}
