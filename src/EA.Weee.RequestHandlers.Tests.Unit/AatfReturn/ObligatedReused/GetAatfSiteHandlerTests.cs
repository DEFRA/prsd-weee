namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn.ObligatedReused
{
    using System;
    using System.Collections.Generic;
    using System.Security;
    using System.Threading.Tasks;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn.ObligatedGeneric;
    using EA.Weee.RequestHandlers.AatfReturn.ObligatedReused;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn.Obligated;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.Common;
    using Xunit;

    public class GetAatfSiteHandlerTests
    {
        private readonly IMap<AatfAddressObligatedAmount, AddressTonnageSummary> mapper;
        private readonly IAatfSiteDataAccess dataAccess;
        private readonly IWeeeAuthorization authorization;
        private readonly GetAatfSiteHandler handler;

        public GetAatfSiteHandlerTests()
        {
            mapper = A.Fake<IMap<AatfAddressObligatedAmount, AddressTonnageSummary>>();
            dataAccess = A.Fake<IAatfSiteDataAccess>();
            authorization = A.Fake<IWeeeAuthorization>();
            handler = new GetAatfSiteHandler(authorization, dataAccess, mapper);
        }

        [Fact]
        public async Task HandleAsync_NoExternalAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyExternalAreaAccess().Build();

            var handler = new GetAatfSiteHandler(authorization, dataAccess, mapper);

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<GetAatfSite>());

            await action.Should().ThrowAsync<SecurityException>();
        }

        [Fact]
        public async void HandleAsync_GivenRequest_DataAccessShouldBeCalled()
        {
            var id = Guid.NewGuid();

            await handler.HandleAsync(new GetAatfSite(id, id));

            A.CallTo(() => dataAccess.GetAddresses(id, id)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task HandleAsync_GivenAatfIdAndReturnId_AatfAddressDataShouldBeRetrieved()
        {
            var aatfAddresses = AatfAddresses();

            A.CallTo(() => dataAccess.GetAddresses(A<Guid>._, A<Guid>._)).Returns(aatfAddresses);

            var result = await handler.HandleAsync(A.Dummy<GetAatfSite>());

            A.CallTo(() => dataAccess.GetAddresses(A<Guid>._, A<Guid>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task HandleAsync_GivenAatfIdAndReturnId_WeeeReusedAmountShouldBeRetrieved()
        {
            var weeeReusedAmounts = WeeeReusedAmounts();

            A.CallTo(() => dataAccess.GetObligatedWeeeForReturnAndAatf(A<Guid>._, A<Guid>._)).Returns(weeeReusedAmounts);

            var result = await handler.HandleAsync(A.Dummy<GetAatfSite>());

            A.CallTo(() => dataAccess.GetObligatedWeeeForReturnAndAatf(A<Guid>._, A<Guid>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void HandleAsync_GivenAddressesAndWeeReusedAmounts_SummaryDataShouldBeMapped()
        {
            var aatfAddresses = AatfAddresses();
            var weeeReusedAmounts = WeeeReusedAmounts();

            A.CallTo(() => dataAccess.GetAddresses(A<Guid>._, A<Guid>._)).Returns(aatfAddresses);
            A.CallTo(() => dataAccess.GetObligatedWeeeForReturnAndAatf(A<Guid>._, A<Guid>._)).Returns(weeeReusedAmounts);

            await handler.HandleAsync(A.Dummy<GetAatfSite>());

            A.CallTo(() => mapper.Map(A<AatfAddressObligatedAmount>.That.Matches(a => a.AatfAddresses.IsSameOrEqualTo(aatfAddresses)
                                                                                        && a.WeeeReusedAmounts.IsSameOrEqualTo(weeeReusedAmounts)))).MustHaveHappened(Repeated.Exactly.Once);
        }

        private List<AatfAddress> AatfAddresses()
        {
            var addresses = new List<AatfAddress>()
            {
                A.Fake<AatfAddress>(),
                A.Fake<AatfAddress>()
            };
            return addresses;
        }

        private List<WeeeReusedAmount> WeeeReusedAmounts()
        {
            var addresses = new List<WeeeReusedAmount>()
            {
                A.Fake<WeeeReusedAmount>(),
                A.Fake<WeeeReusedAmount>()
            };
            return addresses;
        }
    }
}