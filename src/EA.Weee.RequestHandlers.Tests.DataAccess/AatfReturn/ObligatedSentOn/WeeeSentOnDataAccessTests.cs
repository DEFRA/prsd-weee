namespace EA.Weee.RequestHandlers.Tests.DataAccess.AatfReturn.ObligatedSentOn
{
    using EA.Weee.DataAccess;
    using EA.Weee.Domain;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn.ObligatedSentOn;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Xunit;

    public class WeeeSentOnDataAccessTests
    {
        private readonly WeeeContext context;
        private readonly WeeeSentOnDataAccess dataAccess;
        private readonly DbContextHelper dbContextHelper;

        public WeeeSentOnDataAccessTests()
        {
            context = A.Fake<WeeeContext>();
            dataAccess = new WeeeSentOnDataAccess(context);
            dbContextHelper = new DbContextHelper();
        }
        
        [Fact]
        public void Submit_GivenSentOnData_ValuesShouldBeAddedToContext()
        {
            var aatf = A.Fake<Aatf>();
            var @return = A.Fake<Return>();

            var aatfAddress = new AatfAddress(
                "Name",
                "Address",
                A.Dummy<string>(),
                "TownOrCity",
                A.Dummy<string>(),
                A.Dummy<string>(),
                A.Dummy<Country>());

            var weeeSentOn = new WeeeSentOn(aatfAddress, aatf, @return);

            var weeeSentDbSet = dbContextHelper.GetAsyncEnabledDbSet(new List<WeeeSentOn>());

            A.CallTo(() => context.WeeeSentOn).Returns(weeeSentDbSet);

            dataAccess.Submit(weeeSentOn);

            context.WeeeSentOn.Should().AllBeEquivalentTo(weeeSentOn);
        }

        [Fact]
        public void Submit_GivenSentOnData_SaveChangesAsyncShouldBeCalled()
        {
            var site = new WeeeSentOn();

            dataAccess.Submit(site);

            A.CallTo(() => context.WeeeSentOn.Add(site)).MustHaveHappened(Repeated.Exactly.Once)
                .Then(A.CallTo(() => context.SaveChangesAsync()).MustHaveHappened(Repeated.Exactly.Once));
        }

        [Fact]
        public async Task GetWeeeSentOnOperatorAddress_GivenWeeeSentOnId_AddressShouldBeReturned()
        {
            var aatfId = Guid.NewGuid();
            var returnId = Guid.NewGuid();

            var aatfAddressMatch = A.Fake<AatfAddress>();
            var weeeSentOn = A.Fake<WeeeSentOn>();

            A.CallTo(() => context.AatfAddress).Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<AatfAddress>() { aatfAddressMatch }));
            A.CallTo(() => weeeSentOn.AatfId).Returns(aatfId);
            A.CallTo(() => weeeSentOn.ReturnId).Returns(returnId);
            A.CallTo(() => weeeSentOn.OperatorAddress).Returns(aatfAddressMatch);
            A.CallTo(() => context.WeeeSentOn).Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<WeeeSentOn>() { weeeSentOn }));

            var result = await dataAccess.GetWeeeSentOnOperatorAddress(weeeSentOn.Id);

            result.Should().BeEquivalentTo(aatfAddressMatch);
        }

        [Fact]
        public async Task GetWeeeSentOnSiteAddress_GivenWeeeSentOnId_AddressShouldBeReturned()
        {
            var aatfId = Guid.NewGuid();
            var returnId = Guid.NewGuid();

            var aatfAddressMatch = A.Fake<AatfAddress>();
            var weeeSentOn = A.Fake<WeeeSentOn>();

            A.CallTo(() => context.AatfAddress).Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<AatfAddress>() { aatfAddressMatch }));
            A.CallTo(() => weeeSentOn.AatfId).Returns(aatfId);
            A.CallTo(() => weeeSentOn.ReturnId).Returns(returnId);
            A.CallTo(() => weeeSentOn.SiteAddress).Returns(aatfAddressMatch);
            A.CallTo(() => context.WeeeSentOn).Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<WeeeSentOn>() { weeeSentOn }));

            var result = await dataAccess.GetWeeeSentOnSiteAddress(weeeSentOn.Id);

            result.Should().BeEquivalentTo(aatfAddressMatch);
        }

        [Fact]
        public async Task GetWeeeSentOnByReturnAndAatf_GivenAatfAndReturnId_WeeeSentOnListReturned()
        {
            var aatfId = Guid.NewGuid();
            var returnId = Guid.NewGuid();

            var weeeSentOn = A.Fake<WeeeSentOn>();

            A.CallTo(() => weeeSentOn.AatfId).Returns(aatfId);
            A.CallTo(() => weeeSentOn.ReturnId).Returns(returnId);
            A.CallTo(() => context.WeeeSentOn).Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<WeeeSentOn>() { weeeSentOn }));

            var result = await dataAccess.GetWeeeSentOnByReturnAndAatf(aatfId, returnId);

            result.Should().BeEquivalentTo(weeeSentOn);
        }

        [Fact]
        public async Task GetWeeeSentOnById_GivenWeeeSentOn_CorrectWeeeSentOnShouldBeReturned()
        {
            var weeeSentOn = A.Fake<WeeeSentOn>();

            A.CallTo(() => context.WeeeSentOn).Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<WeeeSentOn>() { weeeSentOn }));

            var result = await dataAccess.GetWeeeSentOnById(weeeSentOn.Id);

            result.Should().BeEquivalentTo(weeeSentOn);
        }

        [Fact]
        public async Task UpdateWithOperatorAddress_GivenAatfAndReturnId_UpdateMustHaveHappened()
        {
            var aatfId = Guid.NewGuid();
            var returnId = Guid.NewGuid();

            var operatorAddress = new AatfAddress("TEST", "TEST", "TEST", "TEST", "TEST", "GU22 7UY", A.Dummy<Country>());
            var weeeSentOn = A.Fake<WeeeSentOn>();

            A.CallTo(() => context.AatfAddress).Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<AatfAddress>() { operatorAddress }));
            A.CallTo(() => weeeSentOn.AatfId).Returns(aatfId);
            A.CallTo(() => weeeSentOn.ReturnId).Returns(returnId);
            A.CallTo(() => context.WeeeSentOn).Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<WeeeSentOn>() { weeeSentOn }));

            await dataAccess.UpdateWithOperatorAddress(weeeSentOn, operatorAddress);

            A.CallTo(() => weeeSentOn.UpdateWithOperatorAddress(operatorAddress)).MustHaveHappened(Repeated.Exactly.Once).Then(A.CallTo(() => context.SaveChangesAsync()).MustHaveHappened(Repeated.Exactly.Once));
        }
    }
}
