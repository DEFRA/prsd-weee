namespace EA.Weee.RequestHandlers.Tests.DataAccess.AatfReturn.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoFixture;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn.Internal;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class AatfDataAccessTests
    {
        private readonly Fixture fixture;
        private readonly WeeeContext context;
        private readonly AatfDataAccess dataAccess;
        private readonly DbContextHelper dbContextHelper;

        public AatfDataAccessTests()
        {
            fixture = new Fixture();
            context = A.Fake<WeeeContext>();
            dbContextHelper = new DbContextHelper();

            dataAccess = new AatfDataAccess(context);
        }

        [Fact]
        public async Task GetDetails_GivenAatfId_ContactShouldBeReturned()
        {
            var aatfId = Guid.NewGuid();
            var aatf = A.Fake<Aatf>();

            A.CallTo(() => aatf.Id).Returns(aatfId);
            A.CallTo(() => context.Aatfs).Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<Aatf>() { aatf }));

            var result = await dataAccess.GetDetails(aatfId);

            result.Should().BeEquivalentTo(aatf);
        }

        [Fact]
        public void UpdateDetails_GivenNewAddressData_SaveChangesAsyncShouldBeCalled()
        {
            var oldDetails = A.Fake<Aatf>();
            var newDetails = fixture.Create<Aatf>();

            dataAccess.UpdateDetails(oldDetails, newDetails);

            A.CallTo(() => oldDetails.UpdateDetails(
                newDetails.Name,
                newDetails.CompetentAuthorityId,
                newDetails.ApprovalNumber,
                newDetails.AatfStatus,
                newDetails.Operator,
                newDetails.SiteAddress,
                newDetails.Size,
                newDetails.ApprovalDate)).MustHaveHappenedOnceExactly()
            .Then(A.CallTo(() => context.SaveChangesAsync()).MustHaveHappenedOnceExactly());
        }

        [Fact]
        public async Task GetContact_GivenAatfId_ContactShouldBeReturned()
        {
            var aatfId = Guid.NewGuid();

            var aatfContact = A.Fake<AatfContact>();
            var aatf = A.Fake<Aatf>();

            A.CallTo(() => context.AatfContacts).Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<AatfContact>() { aatfContact }));
            A.CallTo(() => aatf.Id).Returns(aatfId);
            A.CallTo(() => aatf.Contact).Returns(aatfContact);
            A.CallTo(() => context.Aatfs).Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<Aatf>() { aatf }));

            var result = await dataAccess.GetContact(aatfId);

            result.Should().BeEquivalentTo(aatfContact);
        }

        [Fact]
        public void UpdateContact_GivenNewAddressData_SaveChangesAsyncShouldBeCalled()
        {
            var oldSite = A.Fake<AatfContact>();
            var newSite = new AatfContactData() { AddressData = new AatfContactAddressData() };
            var country = A.Fake<Country>();

            dataAccess.UpdateContact(oldSite, newSite, country);

            A.CallTo(() => oldSite.UpdateDetails(
                newSite.FirstName,
                newSite.LastName,
                newSite.Position,
                newSite.AddressData.Address1,
                newSite.AddressData.Address2,
                newSite.AddressData.TownOrCity,
                newSite.AddressData.CountyOrRegion,
                newSite.AddressData.Postcode,
                country,
                newSite.Telephone,
                newSite.Email)).MustHaveHappened(Repeated.Exactly.Once)
            .Then(A.CallTo(() => context.SaveChangesAsync()).MustHaveHappened(Repeated.Exactly.Once));
        }
    }
}
