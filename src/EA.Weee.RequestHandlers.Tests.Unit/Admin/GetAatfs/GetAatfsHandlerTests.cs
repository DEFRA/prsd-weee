namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.GetAatfs
{
    using System;
    using System.Collections.Generic;
    using System.Security;
    using System.Threading.Tasks;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Shared;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.Admin.GetAatfs;
    using EA.Weee.RequestHandlers.Mappings;
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
        private readonly AatfDataListMap mapper;
        private readonly IMap<Aatf, AatfDataList> fakeMapper;
        private readonly GetAatfsHandler handler;

        public GetAatfsHandlerTests()
        {
            authorization = A.Fake<IWeeeAuthorization>();
            dataAccess = A.Fake<IGetAatfsDataAccess>();
            mapper = new AatfDataListMap(A.Fake<IMap<Domain.UKCompetentAuthority, UKCompetentAuthorityData>>(),
            A.Fake<IMap<Domain.AatfReturn.AatfStatus, Core.AatfReturn.AatfStatus>>(),
            A.Fake<IMap<Domain.AatfReturn.FacilityType, Core.AatfReturn.FacilityType>>());

            fakeMapper = A.Fake<IMap<Aatf, AatfDataList>>();

            handler = new GetAatfsHandler(authorization, mapper, dataAccess);
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
        public async Task HandleAsync_GivenGetAatfRequest_DataAccessFetchIsCalled()
        {
            await handler.HandleAsync(A.Dummy<GetAatfs>());
            A.CallTo(() => dataAccess.GetAatfs()).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Theory]
        [InlineData(FacilityType.Aatf)]
        [InlineData(FacilityType.Ae)]
        public async Task HandleAsync_GivenGetAatfRequestWithSpecifiedFacilityType_CorrectFacilityReturned(FacilityType facilityType)
        {
            var aatfs = CreateAatfAeList();

            A.CallTo(() => dataAccess.GetAatfs()).Returns(aatfs);

            var request = new GetAatfs(facilityType);
            var returnDataList = new AatfDataList(A.Dummy<Guid>(), A.Dummy<string>(), A.Dummy<UKCompetentAuthorityData>(), A.Dummy<string>(), A.Dummy<Core.AatfReturn.AatfStatus>(), A.Dummy<Operator>(), facilityType);

            A.CallTo(() => fakeMapper.Map(A<Aatf>._)).Returns(returnDataList);

            var result = await handler.HandleAsync(request);

            result.Count.Should().Be(1);
            result[0].FacilityType.Should().Be(facilityType);
        }

        [Fact]
        public async Task HandleAsync_GivenGetAatfRequestWithNoSpecifiedFacilityType_AllFacilitiesReturned()
        {
            var aatfs = CreateAatfAeList();

            A.CallTo(() => dataAccess.GetAatfs()).Returns(aatfs);

            var result = await handler.HandleAsync(A.Dummy<GetAatfs>());

            result.Count.Should().Be(2);
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
