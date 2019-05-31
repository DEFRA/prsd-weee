namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.Aatf
{
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Shared;
    using EA.Weee.Domain;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.RequestHandlers.Admin.GetAatfs;
    using EA.Weee.RequestHandlers.Mappings;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using Xunit;

    public class GetAatfInfoByAatfIdRequestHandlerTests
    {
        private readonly IWeeeAuthorization authorization;
        private readonly AatfMap mapper;
        private readonly IMap<Aatf, AatfData> fakeMapper;
        private readonly IGetAatfsDataAccess dataAccess;
        private readonly GetAatfInfoByAatfIdRequestHandler handler;

        public GetAatfInfoByAatfIdRequestHandlerTests()
        {
            authorization = AuthorizationBuilder.CreateUserWithAllRights();
            mapper = new AatfMap(A.Fake<IMap<Domain.UKCompetentAuthority, UKCompetentAuthorityData>>(),
                A.Fake<IMap<Domain.AatfReturn.AatfStatus, Core.AatfReturn.AatfStatus>>(),
                A.Fake<IMap<Domain.AatfReturn.AatfSize, Core.AatfReturn.AatfSize>>(),
                A.Fake<IMap<AatfAddress, AatfAddressData>>(),
                A.Fake<IMap<AatfContact, AatfContactData>>(),
                A.Fake<IMap<Organisation, OrganisationData>>(),
                A.Fake<IMap<Domain.AatfReturn.FacilityType, Core.AatfReturn.FacilityType>>());
            dataAccess = A.Dummy<IGetAatfsDataAccess>();

            fakeMapper = A.Fake<IMap<Aatf, AatfData>>();

             handler = new GetAatfInfoByAatfIdRequestHandler(authorization, mapper, dataAccess);
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

            // Asert
            await Assert.ThrowsAsync<SecurityException>(testCode);
        }

        [Fact]
        public async Task HandleAsync_GivenReturn_MappedObjectShouldBeReturned()
        {
            DateTime date = DateTime.Now;

            Aatf aatf = new Aatf("name", A.Dummy<UKCompetentAuthority>(), "1234", Domain.AatfReturn.AatfStatus.Approved, A.Fake<Organisation>(), A.Dummy<AatfAddress>(), Domain.AatfReturn.AatfSize.Large, date, A.Fake<AatfContact>(), Domain.AatfReturn.FacilityType.Aatf, 2019);
            A.CallTo(() => dataAccess.GetAatfById(A.Dummy<Guid>())).Returns(aatf);

            AatfData aatfData = new AatfData(Guid.Empty, "name", "1234", (Int16)2019, A.Dummy<UKCompetentAuthorityData>(), A.Fake<Core.AatfReturn.AatfStatus>(), A.Dummy<AatfAddressData>(), A.Fake<Core.AatfReturn.AatfSize>(), date);
            A.CallTo(() => fakeMapper.Map(A<Aatf>._)).Returns(aatfData);

            var result = await handler.HandleAsync(new GetAatfById(A.Dummy<Guid>()));

            Assert.Equal(aatfData.Id, result.Id);
            Assert.Equal(aatfData.Name, result.Name);
            Assert.Equal(aatfData.ApprovalNumber, result.ApprovalNumber);
            Assert.Equal(aatfData.AatfStatus, result.AatfStatus);
            Assert.Equal(aatfData.Size, result.Size);
            Assert.Equal(aatfData.ApprovalDate, result.ApprovalDate);
            Assert.Equal(aatfData.ComplianceYear, result.ComplianceYear);
        }

        [Fact]
        public async Task HandleAsync_GivenGetAatfsReturnRequest_DataAccessFetchIsCalled()
        {
            IWeeeAuthorization authorization = new AuthorizationBuilder().AllowInternalAreaAccess().Build();

            await handler.HandleAsync(A.Dummy<GetAatfById>());
            A.CallTo(() => dataAccess.GetAatfById(A.Dummy<Guid>())).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task HandleAsync_ProvideNonExistantAatfId_ReturnsException()
        {
            Aatf returnData = null;

            A.CallTo(() => dataAccess.GetAatfById(A.Dummy<Guid>())).Returns(returnData);

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<GetAatfById>());

            await Assert.ThrowsAsync<ArgumentException>(action);
        }
    }
}
