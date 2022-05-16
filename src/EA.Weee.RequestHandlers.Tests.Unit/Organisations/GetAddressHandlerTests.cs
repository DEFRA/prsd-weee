namespace EA.Weee.RequestHandlers.Tests.Unit.Organisations
{
    using Core.Shared;
    using Domain.Organisation;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core.Mapper;
    using RequestHandlers.AatfReturn;
    using RequestHandlers.Organisations;
    using RequestHandlers.Security;
    using Requests.Organisations;
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using DataAccess.DataAccess;
    using Weee.Tests.Core;
    using Xunit;

    public class GetAddressHandlerTests
    {
        private readonly GetAddressHandler handler;
        private readonly IWeeeAuthorization authorization;
        private readonly IGenericDataAccess dataAccess;
        private readonly IMap<Address, AddressData> mapper;

        public GetAddressHandlerTests()
        {
            authorization = AuthorizationBuilder.CreateUserAllowedToAccessOrganisation();

            dataAccess = A.Fake<IGenericDataAccess>();
            mapper = A.Fake<IMap<Address, AddressData>>();

            handler = new GetAddressHandler(authorization, dataAccess, mapper);
        }

        [Fact]
        public async Task HandleAsync_GivenNotOrganisationUser_ThrowsSecurityException()
        {
            var localAuthorization = AuthorizationBuilder.CreateUserDeniedFromAccessingOrganisation();

            var localHandler = new GetAddressHandler(localAuthorization, dataAccess, mapper);

            Func<Task<AddressData>> action = async () => await localHandler.HandleAsync(A.Dummy<GetAddress>());

            await action.Should().ThrowAsync<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_GivenAddressId_AddressShouldBeRetrieved()
        {
            var id = Guid.NewGuid();

            var result = await handler.HandleAsync(new GetAddress(id, A.Dummy<Guid>()));

            A.CallTo(() => dataAccess.GetById<Address>(id)).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task HandleAsync_GivenAddressNotFound_ArgumentExceptionExpected()
        {
            var id = Guid.NewGuid();

            A.CallTo(() => dataAccess.GetById<Address>(id)).Returns((Address)null);

            Func<Task<AddressData>> action = async () => await handler.HandleAsync(new GetAddress(id, A.Dummy<Guid>()));

            await action.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task HandleAsync_GivenAddressFound_AddressDataShouldBeMapped()
        {
            var id = Guid.NewGuid();
            var address = A.Fake<Address>();

            A.CallTo(() => dataAccess.GetById<Address>(id)).Returns(address);

            var result = await handler.HandleAsync(new GetAddress(id, A.Dummy<Guid>()));

            A.CallTo(() => mapper.Map(address)).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task HandleAsync_GivenAddressFound_AddressDataShouldBeReturned()
        {
            var id = Guid.NewGuid();
            var address = A.Fake<Address>();
            var addressData = new AddressData();

            A.CallTo(() => dataAccess.GetById<Address>(id)).Returns(address);
            A.CallTo(() => mapper.Map(address)).Returns(addressData);

            var result = await handler.HandleAsync(new GetAddress(id, A.Dummy<Guid>()));

            result.Should().Be(addressData);
        }
    }
}
