namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn.ObligatedReused
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn.ObligatedGeneric;
    using EA.Weee.RequestHandlers.AatfReturn.ObligatedReused;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn.Obligated;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class GetAatfSiteRequestHandlerTests
    {
        private readonly IMap<AatfAddress, AddressData> mapper;
        private readonly IGetAatfSiteDataAccess dataAccess;
        private readonly IWeeeAuthorization authorization;
        private readonly GetAatfSiteRequestHandler handler;

        public GetAatfSiteRequestHandlerTests()
        {
            mapper = A.Fake<IMap<AatfAddress, AddressData>>();
            dataAccess = A.Fake<IGetAatfSiteDataAccess>();
            authorization = A.Fake<IWeeeAuthorization>();
            handler = new GetAatfSiteRequestHandler(authorization, dataAccess, mapper);
        }

        [Fact]
        public async Task HandleAsync_NoExternalAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyExternalAreaAccess().Build();

            var handler = new GetAatfSiteRequestHandler(authorization, dataAccess, mapper);

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
        public async void HandleAsync_GivenAatfAddressData_AatfAddressDataShouldBeMapped()
        {
            var aatfAddresses = AatfAddresses();

            A.CallTo(() => dataAccess.GetAddresses(A<Guid>._, A<Guid>._)).Returns(aatfAddresses);

            await handler.HandleAsync(A.Dummy<GetAatfSite>());

            for (var i = 0; i < aatfAddresses.Count; i++)
            {
                A.CallTo(() => mapper.Map(aatfAddresses.ElementAt(i))).MustHaveHappened(Repeated.Exactly.Once);
            }
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
    }
}