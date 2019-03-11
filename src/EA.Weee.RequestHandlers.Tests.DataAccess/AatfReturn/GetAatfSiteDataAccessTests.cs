namespace EA.Weee.RequestHandlers.Tests.DataAccess.AatfReturn
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn.ObligatedGeneric;
    using EA.Weee.RequestHandlers.AatfReturn.ObligatedReused;
    using EA.Weee.RequestHandlers.AatfReturn.Specification;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;
    using Operator = Domain.AatfReturn.Operator;
    using Organisation = Domain.Organisation.Organisation;
    using WeeeReused = Domain.AatfReturn.WeeeReused;
    using WeeeReusedAmount = Domain.AatfReturn.WeeeReusedAmount;
    using WeeeReusedSite = Domain.AatfReturn.WeeeReusedSite;

    public class GetAatfSiteDataAccessTests
    {
        private readonly WeeeContext context;
        private readonly DbContextHelper dbContextHelper;
        private readonly Organisation organisation;
        private readonly Operator @operator;
        private readonly IGetAatfSiteDataAccess dataAccess;
        private readonly IGenericDataAccess genericDataAccess;

        public GetAatfSiteDataAccessTests()
        {
            context = A.Fake<WeeeContext>();
            dbContextHelper = new DbContextHelper();
            organisation = Organisation.CreatePartnership("Dummy");
            @operator = new Operator(organisation);

            genericDataAccess = A.Fake<IGenericDataAccess>();
            dataAccess = new GetAatfSiteDataAccess(context, genericDataAccess);
        }

        [Fact]
        public async Task GetAddresses_GivenAatfAndReturnId_AddressShouldBeReturned()
        {
            var aatfId = Guid.NewGuid();
            var returnId = Guid.NewGuid();

            var aatfAddressMatch = A.Fake<AatfAddress>();
            var weeeReused = A.Fake<WeeeReused>();
            var weeeReusedSite = A.Fake<WeeeReusedSite>();

            A.CallTo(() => context.AatfAddress).Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<AatfAddress>() { aatfAddressMatch }));
            A.CallTo(() => weeeReused.AatfId).Returns(aatfId);
            A.CallTo(() => weeeReused.ReturnId).Returns(returnId);
            A.CallTo(() => context.WeeeReused).Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<WeeeReused>() { weeeReused }));
            A.CallTo(() => genericDataAccess.GetManyByExpression(A<WeeeReusedByAatfIdAndReturnIdSpecification>._)).Returns(new List<WeeeReused>() { weeeReused });
            A.CallTo(() => weeeReusedSite.WeeeReused).Returns(weeeReused);
            A.CallTo(() => weeeReusedSite.Address).Returns(aatfAddressMatch);
            A.CallTo(() => context.WeeeReusedSite).Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<WeeeReusedSite>() { weeeReusedSite }));

            var result = await dataAccess.GetAddresses(aatfId, returnId);

            result.Should().BeEquivalentTo(aatfAddressMatch);
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
    }
}
