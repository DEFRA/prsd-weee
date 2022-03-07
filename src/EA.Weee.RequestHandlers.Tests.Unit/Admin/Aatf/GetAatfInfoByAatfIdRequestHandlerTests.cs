namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.Aatf
{
    using System;
    using System.Security;
    using System.Threading.Tasks;

    using AutoFixture;

    using Domain.Lookup;

    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Admin;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Shared;
    using EA.Weee.Domain;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.RequestHandlers.Admin.GetAatfs;
    using EA.Weee.RequestHandlers.Mappings;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Security;
    using EA.Weee.Tests.Core;

    using FakeItEasy;

    using FluentAssertions;

    using Xunit;

    public class GetAatfInfoByAatfIdRequestHandlerTests
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IMap<Aatf, AatfData> fakeMapper;
        private readonly IGetAatfsDataAccess dataAccess;
        private readonly GetAatfInfoByAatfIdRequestHandler handler;
        private readonly Fixture fixture;

        public GetAatfInfoByAatfIdRequestHandlerTests()
        {
            this.authorization = AuthorizationBuilder.CreateUserWithAllRights();
            this.dataAccess = A.Dummy<IGetAatfsDataAccess>();
            this.fakeMapper = A.Fake<IMap<Aatf, AatfData>>();

            this.fixture = new Fixture();

            this.handler = new GetAatfInfoByAatfIdRequestHandler(this.authorization, this.fakeMapper, this.dataAccess);
        }

        [Fact]
        public async Task HandleAsync_WhenUserCannotAccessInternalArea_ThrowsSecurityException()
        {
            // Arrange
            IWeeeAuthorization unuthorization = new AuthorizationBuilder().DenyInternalAreaAccess().Build();

            GetAatfInfoByAatfIdRequestHandler handler = new GetAatfInfoByAatfIdRequestHandler(
                unuthorization,
                A.Dummy<IMap<Aatf, AatfData>>(),
                A.Dummy<IGetAatfsDataAccess>());

            // Act
            Func<Task> testCode = async () => await handler.HandleAsync(A.Dummy<GetAatfById>());

            // Assert
            await Assert.ThrowsAsync<SecurityException>(testCode);
        }

        [Fact]
        public async Task HandleAsync_GivenReturn_MappedObjectShouldBeReturned()
        {
            DateTime date = DateTime.Now;

            var aatfData = this.fixture.Create<AatfData>();
            var aatf = this.fixture.Create<Aatf>();

            A.CallTo(() => this.fakeMapper.Map(aatf)).Returns(aatfData);
            A.CallTo(() => this.dataAccess.GetAatfById(A.Dummy<Guid>())).Returns(aatf);

            var result = await this.handler.HandleAsync(new GetAatfById(A.Dummy<Guid>()));

            Assert.Equal(aatfData.Id, result.Id);
            Assert.Equal(aatfData.Name, result.Name);
            Assert.Equal(aatfData.ApprovalNumber, result.ApprovalNumber);
            Assert.Equal(aatfData.AatfStatus, result.AatfStatus);
            Assert.Equal(aatfData.Size, result.Size);
            Assert.Equal(aatfData.ApprovalDate, result.ApprovalDate);
            Assert.Equal(aatfData.ComplianceYear, result.ComplianceYear);
        }

        [Fact]
        public async Task HandleAsync_GivenReturnAndAuthorisation_CanEditPropertiesShouldBeSet()
        {
            var aatfData = this.fixture.Create<AatfData>();
            var aatf = this.fixture.Create<Aatf>();
            var canEdit = this.fixture.Create<bool>();

            A.CallTo(() => this.fakeMapper.Map(aatf)).Returns(aatfData);
            A.CallTo(() => this.dataAccess.GetAatfById(A.Dummy<Guid>())).Returns(aatf);
            A.CallTo(() => authorization.CheckUserInRole(Roles.InternalAdmin)).Returns(canEdit);

            var result = await this.handler.HandleAsync(new GetAatfById(A.Dummy<Guid>()));

            result.CanEdit.Should().Be(canEdit);
            result.Contact.CanEditContactDetails.Should().Be(canEdit);
        }

        [Fact]
        public async Task HandleAsync_GivenGetAatfsReturnRequest_DataAccessFetchIsCalled()
        {
            IWeeeAuthorization authorization = new AuthorizationBuilder().AllowInternalAreaAccess().Build();

            var request = new GetAatfById(Guid.NewGuid());
            var aatfData = this.fixture.Create<AatfData>();

            A.CallTo(() => this.fakeMapper.Map(A<Aatf>._)).Returns(aatfData);

            await this.handler.HandleAsync(request);

            A.CallTo(() => this.dataAccess.GetAatfById(request.AatfId)).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task HandleAsync_ProvideNonExistantAatfId_ReturnsException()
        {
            var aatfData = this.fixture.Create<AatfData>();

            A.CallTo(() => this.dataAccess.GetAatfById(A<Guid>._)).Returns((Aatf)null);

            Func<Task> action = async () => await this.handler.HandleAsync(A.Dummy<GetAatfById>());

            await Assert.ThrowsAsync<ArgumentException>(action);
        }
    }
}
