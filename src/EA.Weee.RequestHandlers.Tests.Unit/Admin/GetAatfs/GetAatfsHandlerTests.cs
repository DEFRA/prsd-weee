namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.GetAatfs
{
    using System;
    using System.Collections.Generic;
    using System.Security;
    using System.Threading.Tasks;
    using AutoFixture;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Shared;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.Admin.GetAatfs;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using Requests.Admin;
    using Xunit;
    using FacilityType = Core.AatfReturn.FacilityType;

    public class GetAatfsHandlerTests
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGetAatfsDataAccess dataAccess;
        private readonly IMap<Aatf, AatfDataList> fakeMapper;
        private readonly GetAatfsHandler handler;
        private readonly Fixture fixture;

        public GetAatfsHandlerTests()
        {
            authorization = A.Fake<IWeeeAuthorization>();
            dataAccess = A.Fake<IGetAatfsDataAccess>();
            fakeMapper = A.Dummy<IMap<Aatf, AatfDataList>>();
            fixture = new Fixture();

            handler = new GetAatfsHandler(authorization, fakeMapper, dataAccess);
        }

        [Fact]
        public async Task HandleAsync_WhenUserCannotAccessInternalArea_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyInternalAreaAccess().Build();

            var handler = new GetAatfsHandler(authorization, A.Dummy<IMap<Aatf, AatfDataList>>(), dataAccess);

            Func<Task> aatfs = async () => await handler.HandleAsync(A.Dummy<GetAatfs>());

            await Assert.ThrowsAsync<SecurityException>(aatfs);
        }

        [Fact]
        public async Task HandleAsync_GivenGetAatfRequestWithoutFilter_DataAccessFetchIsCalled()
        {
            var request = new GetAatfs(fixture.Create<FacilityType>());
            await handler.HandleAsync(request);
            A.CallTo(() => dataAccess.GetAatfs()).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task HandleAsync_GivenGetAatfRequestWithFilter_DataAccessFetchIsCalled()
        {
            var filter = fixture.Create<AatfFilter>();
            var request = new GetAatfs(fixture.Create<FacilityType>(), filter);
            await handler.HandleAsync(request);
            A.CallTo(() => dataAccess.GetFilteredAatfs(filter)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Theory]
        [InlineData(FacilityType.Aatf)]
        [InlineData(FacilityType.Ae)]
        public async Task HandleAsync_GivenGetAatfRequestWithSpecifiedFacilityType_CorrectFacilityReturned(FacilityType facilityType)
        {
            var aatfs = CreateAatfAeList();

            A.CallTo(() => dataAccess.GetAatfs()).Returns(aatfs);

            var request = new GetAatfs(facilityType);
            var returnDataList = new AatfDataList(A.Dummy<Guid>(), A.Dummy<string>(), A.Dummy<UKCompetentAuthorityData>(), A.Dummy<string>(), A.Dummy<Core.AatfReturn.AatfStatus>(), A.Dummy<OrganisationData>(), facilityType, (Int16)2019);

            A.CallTo(() => fakeMapper.Map(A<Aatf>._)).Returns(returnDataList);

            var result = await handler.HandleAsync(request);

            result.Count.Should().Be(1);
            result[0].FacilityType.Should().Be(facilityType);
        }

        private static List<Aatf> CreateAatfAeList()
        {
            var aatf = A.Dummy<Aatf>();
            var ae = A.Dummy<Aatf>();

            A.CallTo(() => aatf.FacilityType).Returns(Domain.AatfReturn.FacilityType.Aatf);
            A.CallTo(() => ae.FacilityType).Returns(Domain.AatfReturn.FacilityType.Ae);

            return new List<Aatf>() { aatf, ae };
        }
    }
}
