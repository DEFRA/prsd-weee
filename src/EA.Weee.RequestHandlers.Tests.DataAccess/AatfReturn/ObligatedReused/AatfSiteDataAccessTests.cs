namespace EA.Weee.RequestHandlers.Tests.DataAccess.AatfReturn.ObligatedReused
{
    using Domain.AatfReturn;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain;
    using EA.Weee.RequestHandlers.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn.ObligatedReused;
    using EA.Weee.RequestHandlers.AatfReturn.Specification;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Mappings;
    using Weee.DataAccess.DataAccess;
    using Weee.Tests.Core;
    using Xunit;

    public class AatfSiteDataAccessTests
    {
        private readonly WeeeContext context;
        private readonly AatfSiteDataAccess dataAccess;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly DbContextHelper dbContextHelper;
        private readonly Guid aatfId;

        public AatfSiteDataAccessTests()
        {
            context = A.Fake<WeeeContext>();
            genericDataAccess = A.Fake<IGenericDataAccess>();
            dataAccess = new AatfSiteDataAccess(context, genericDataAccess);
            dbContextHelper = new DbContextHelper();
            aatfId = A.Dummy<Guid>();
        }

        [Fact]
        public void Submit_GivenReusedSiteData_ValuesShouldBeAddedToContext()
        {
            var aatfAddress = new AatfAddress(
                "Name",
                "Address",
                A.Dummy<string>(),
                "TownOrCity",
                A.Dummy<string>(),
                A.Dummy<string>(),
                A.Dummy<Country>());
            var weeeReused = new WeeeReused(aatfId, A.Dummy<Guid>());
            var weeeReusedSite = new WeeeReusedSite(weeeReused, aatfAddress);

            var weeeReusedSiteDbSet = dbContextHelper.GetAsyncEnabledDbSet(new List<WeeeReusedSite>());

            A.CallTo(() => context.WeeeReusedSite).Returns(weeeReusedSiteDbSet);

            dataAccess.Submit(weeeReusedSite);

            context.WeeeReusedSite.Should().AllBeEquivalentTo(weeeReusedSite);
        }

        [Fact]
        public void Submit_GivenReusedSiteData_SaveChangesAsyncShouldBeCalled()
        {
            var site = new WeeeReusedSite();

            dataAccess.Submit(site);

            A.CallTo(() => context.WeeeReusedSite.Add(site)).MustHaveHappened(1, Times.Exactly)
                .Then(A.CallTo(() => context.SaveChangesAsync()).MustHaveHappened(1, Times.Exactly));
        }

        [Fact]
        public async Task GetAddresses_GivenAatfAndReturnId_AddressShouldBeReturned()
        {
            var aatfId = Guid.NewGuid();
            var returnId = Guid.NewGuid();
            var reusedId = Guid.NewGuid();
            var addressId = Guid.NewGuid();

            var aatfAddressMatch = A.Fake<AatfAddress>();
            A.CallTo(() => aatfAddressMatch.Id).Returns(addressId);

            var weeeReused = A.Fake<WeeeReused>();
            A.CallTo(() => weeeReused.Id).Returns(reusedId);

            var weeeReusedSite = A.Fake<WeeeReusedSite>();

            A.CallTo(() => context.AatfAddress).Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<AatfAddress>() { aatfAddressMatch, new AatfAddress() }));
            A.CallTo(() => weeeReused.AatfId).Returns(aatfId);
            A.CallTo(() => weeeReused.ReturnId).Returns(returnId);
            A.CallTo(() => context.WeeeReused).Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<WeeeReused>() { weeeReused }));
            A.CallTo(() => genericDataAccess.GetManyByExpression(A<WeeeReusedByAatfIdAndReturnIdSpecification>._)).Returns(new List<WeeeReused>() { weeeReused });
            A.CallTo(() => weeeReusedSite.WeeeReused).Returns(weeeReused);
            A.CallTo(() => weeeReusedSite.Address).Returns(aatfAddressMatch);
            A.CallTo(() => context.WeeeReusedSite).Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<WeeeReusedSite>() { weeeReusedSite }));

            var result = await dataAccess.GetAddresses(aatfId, returnId);

            result.Should().BeEquivalentTo(new List<AatfAddress>() { aatfAddressMatch });
        }

        [Fact]
        public async Task GetObligatedWeeeForReturnAndAatf_GivenAatfAndReturnId_WeeeReceivedAmountsShouldBeReturned()
        {
            var aatfId = Guid.NewGuid();
            var returnId = Guid.NewGuid();

            var aatf = A.Fake<Aatf>();
            var weeeReused = A.Fake<WeeeReused>();
            var weeeReusedSite = A.Fake<WeeeReusedSite>();
            var weeeReusedAmountMatch = A.Fake<WeeeReusedAmount>();
            var weeeReusedAmountNoMatch = A.Fake<WeeeReusedAmount>();

            A.CallTo(() => aatf.Id).Returns(aatfId);
            A.CallTo(() => weeeReused.Aatf).Returns(aatf);
            A.CallTo(() => weeeReused.ReturnId).Returns(returnId);
            A.CallTo(() => context.WeeeReused).Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<WeeeReused>() { weeeReused }));
            A.CallTo(() => weeeReusedAmountMatch.WeeeReused).Returns(weeeReused);
            A.CallTo(() => context.WeeeReusedAmount).Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<WeeeReusedAmount>() { weeeReusedAmountMatch, weeeReusedAmountNoMatch }));

            var result = await dataAccess.GetObligatedWeeeForReturnAndAatf(aatfId, returnId);

            result.Count.Should().Be(1);
            result[0].Should().BeEquivalentTo(weeeReusedAmountMatch);
        }

        [Fact]
        public void Update_GivenNewAddressData_SaveChangesAsyncShouldBeCalled()
        {
            var oldSite = A.Fake<AatfAddress>();
            var newSite = new SiteAddressData();
            var country = A.Fake<Country>();

            dataAccess.Update(oldSite, newSite, country);

            A.CallTo(() => oldSite.UpdateAddress(
                newSite.Name,
                newSite.Address1,
                newSite.Address2,
                newSite.TownOrCity,
                newSite.CountyOrRegion,
                newSite.Postcode,
                country)).MustHaveHappened(1, Times.Exactly)
            .Then(A.CallTo(() => context.SaveChangesAsync()).MustHaveHappened(1, Times.Exactly));
        }
    }
}
