namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.GetAatfs
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Shared;
    using EA.Weee.Domain;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.Admin.GetAatfs;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Admin;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using Xunit;

    public class GetAatfsByOperatorIdHandlerTests
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGetAatfsDataAccess dataAccess;
        private readonly IMap<Aatf, AatfDataList> aatfmap;
        private GetAatfsByOperatorIdHandler handler;

        public GetAatfsByOperatorIdHandlerTests()
        {
            this.authorization = A.Fake<IWeeeAuthorization>();
            this.dataAccess = A.Fake<IGetAatfsDataAccess>();
            this.aatfmap = A.Fake<IMap<Aatf, AatfDataList>>();

            handler = new GetAatfsByOperatorIdHandler(authorization, aatfmap, dataAccess);
        }

        [Fact]
        public async Task HandleAsync_NoInternalAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyInternalAreaAccess().Build();

            handler = new GetAatfsByOperatorIdHandler(authorization, aatfmap, dataAccess);

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<GetAatfsByOperatorId>());

            await action.Should().ThrowAsync<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_GetAatfsShouldBeCalled()
        {
            var result = await handler.HandleAsync(A.Dummy<GetAatfsByOperatorId>());

            A.CallTo(() => dataAccess.GetAatfs()).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task HandleAsync_GivenAatfData_AatfDataMustBeMapped()
        {
            var aatfs = A.CollectionOfFake<Aatf>(2).ToList();

            A.CallTo(() => dataAccess.GetAatfs()).ReturnsNextFromSequence(aatfs);

            await handler.HandleAsync(A.Dummy<GetAatfsByOperatorId>());

            foreach (var aatf in aatfs)
            {
                A.CallTo(() => aatfmap.Map(aatf)).MustHaveHappened(Repeated.Exactly.Once);
            }
        }

        [Fact]
        public async void HandleAsync_GivenMappedAatfData_AatfDataShouldBeReturn()
        {
            var aatfs = new List<Aatf>()
            {
                A.Fake<Aatf>(),
                A.Fake<Aatf>(),
            };

            var aatfDatas = new List<AatfDataList>()
            {
                A.Fake<AatfDataList>(),
                A.Fake<AatfDataList>()
            }.ToArray();

            A.CallTo(() => dataAccess.GetAatfs()).Returns(aatfs);
            A.CallTo(() => aatfmap.Map(A<Aatf>._)).ReturnsNextFromSequence(aatfDatas);

            var result = await handler.HandleAsync(A.Dummy<GetAatfsByOperatorId>());

            foreach (var aatfData in aatfDatas)
            {
                result.Should().Contain(aatfData);
            }

            result.Count().Should().Be(aatfDatas.Length);
        }
        /*
        [Fact]
        public async void HandleAsync_GivenMappedAatfData_AatfDataShouldBeReturn2()
        {
            var aatf1 = new Aatf("B", A.Fake<UKCompetentAuthority>(), "TEST", A.Fake<Domain.AatfReturn.AatfStatus>(), A.Fake<Operator>(), A.Fake<AatfAddress>(), A.Fake<Domain.AatfReturn.AatfSize>(), DateTime.Now, A.Fake<AatfContact>(), A.Fake<Domain.AatfReturn.FacilityType>());
            var aatf2 = new Aatf("A", A.Fake<UKCompetentAuthority>(), "TEST", A.Fake<Domain.AatfReturn.AatfStatus>(), A.Fake<Operator>(), A.Fake<AatfAddress>(), A.Fake<Domain.AatfReturn.AatfSize>(), DateTime.Now, A.Fake<AatfContact>(), A.Fake<Domain.AatfReturn.FacilityType>());
            var aatf3 = new Aatf("C", A.Fake<UKCompetentAuthority>(), "TEST", A.Fake<Domain.AatfReturn.AatfStatus>(), A.Fake<Operator>(), A.Fake<AatfAddress>(), A.Fake<Domain.AatfReturn.AatfSize>(), DateTime.Now, A.Fake<AatfContact>(), A.Fake<Domain.AatfReturn.FacilityType>());

            var aatfs = new List<Aatf>()
            {
                aatf1,
                aatf2,
                aatf3
            };

            var aatfDatas = new List<AatfDataList>()
            {
                new AatfDataList(Guid.NewGuid(), "B", A.Fake<UKCompetentAuthorityData>(), "TEST", A.Fake<Core.AatfReturn.AatfStatus>(), A.Fake<OperatorData>(), Core.AatfReturn.FacilityType.Aatf),
                new AatfDataList(Guid.NewGuid(), "A", A.Fake<UKCompetentAuthorityData>(), "TEST", A.Fake<Core.AatfReturn.AatfStatus>(), A.Fake<OperatorData>(), Core.AatfReturn.FacilityType.Aatf),
                new AatfDataList(Guid.NewGuid(), "C", A.Fake<UKCompetentAuthorityData>(), "TEST", A.Fake<Core.AatfReturn.AatfStatus>(), A.Fake<OperatorData>(), Core.AatfReturn.FacilityType.Aatf)
            }.ToArray();

            A.CallTo(() => dataAccess.GetAatfs()).Returns(aatfs);
            A.CallTo(() => aatfmap.Map(A<Aatf>._)).ReturnsNextFromSequence(aatfDatas);

            var result = await handler.HandleAsync(A.Dummy<GetAatfsByOperatorId>());

            result.Should().Contain(aatfDatas.ElementAt(0));
            result.Should().Contain(aatfDatas.ElementAt(1));
            result.Should().Contain(aatfDatas.ElementAt(2));
            result.Count().Should().Be(3);
        }
        */
    }
}
