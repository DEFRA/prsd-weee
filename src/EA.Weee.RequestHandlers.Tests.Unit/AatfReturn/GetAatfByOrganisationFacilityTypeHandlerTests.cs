namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn
{
    using Core.AatfReturn;
    using Domain.AatfReturn;
    using EA.Weee.Domain;
    using EA.Weee.Domain.Lookup;
    using EA.Weee.Domain.Organisation;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core.Mapper;
    using RequestHandlers.AatfReturn;
    using RequestHandlers.AatfReturn.Specification;
    using Requests.AatfReturn;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DataAccess.DataAccess;
    using Xunit;

    public class GetAatfByOrganisationFacilityTypeHandlerTests
    {
        private readonly GetAatfByOrganisationFacilityTypeHandler handler;
        private readonly IMap<Aatf, AatfData> mapper;
        private readonly IGenericDataAccess dataAccess;

        public GetAatfByOrganisationFacilityTypeHandlerTests()
        {
            mapper = A.Fake<IMap<Aatf, AatfData>>();
            dataAccess = A.Fake<IGenericDataAccess>();
            handler = new GetAatfByOrganisationFacilityTypeHandler(mapper, dataAccess);
        }

        [Fact]
        public async void HandleAsync_GivenRequest_DataAccessShouldBeCalled()
        {
            var id = Guid.NewGuid();
            var type = Core.AatfReturn.FacilityType.Aatf;
            await handler.HandleAsync(new GetAatfByOrganisationFacilityType(id, type));

            A.CallTo(() => dataAccess.GetManyByExpression(A<AatfsByOrganisationAndFacilityTypeSpecification>.That.Matches(c => c.OrganisationId == id))).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => dataAccess.GetManyByExpression(A<AatfsByOrganisationAndFacilityTypeSpecification>.That.Matches(c => c.FacilityType.Equals(type)))).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async void HandleAsync_GivenAatfData_AatfDataShouldBeMapped()
        {
            var aatfs = new List<Aatf>()
            {
                A.Fake<Aatf>(),
                A.Fake<Aatf>()
            };

            A.CallTo(() => dataAccess.GetManyByExpression(A<AatfsByOrganisationAndFacilityTypeSpecification>._)).Returns(aatfs);

            await handler.HandleAsync(A.Dummy<GetAatfByOrganisationFacilityType>());

            for (var i = 0; i < aatfs.Count; i++)
            {
                A.CallTo(() => mapper.Map(aatfs.ElementAt(i))).MustHaveHappened(1, Times.Exactly);
            }
        }

        [Fact]
        public async void HandleAsync_GivenAatfAeData_AatfDataShouldBeReturn()
        {
            var facilityType = Weee.Core.AatfReturn.FacilityType.Aatf;
            DateTime date = DateTime.Now;
            var id = Guid.NewGuid();
            var aatfData1 = new AatfData(Guid.NewGuid(), "name", "approval number", 2019, A.Dummy<Core.Shared.UKCompetentAuthorityData>(),
               Core.AatfReturn.AatfStatus.Approved, A.Dummy<AatfAddressData>(), Core.AatfReturn.AatfSize.Large, DateTime.Now,
               A.Dummy<Core.Shared.PanAreaData>(), null)
            {
                FacilityType = facilityType
            };

            var aatfData2 = new AatfData(Guid.NewGuid(), "name", "approval number", 2019, A.Dummy<Core.Shared.UKCompetentAuthorityData>(),
              Core.AatfReturn.AatfStatus.Approved, A.Dummy<AatfAddressData>(), Core.AatfReturn.AatfSize.Large, DateTime.Now,
              A.Dummy<Core.Shared.PanAreaData>(), null)
            {
                FacilityType = facilityType
            };

            var aatfData3 = new AatfData(Guid.NewGuid(), "name", "approval number", 2019, A.Dummy<Core.Shared.UKCompetentAuthorityData>(),
              Core.AatfReturn.AatfStatus.Approved, A.Dummy<AatfAddressData>(), Core.AatfReturn.AatfSize.Large, DateTime.Now,
              A.Dummy<Core.Shared.PanAreaData>(), null)
            {
                FacilityType = Weee.Core.AatfReturn.FacilityType.Ae
            };

            var aatfDatas = new List<AatfData>()
            {
                aatfData1, aatfData2, aatfData3
            }.ToArray();

            Aatf aatf = new Aatf("name", A.Dummy<UKCompetentAuthority>(), "1234", Domain.AatfReturn.AatfStatus.Approved, A.Fake<Organisation>(), A.Dummy<AatfAddress>(), Domain.AatfReturn.AatfSize.Large, date, A.Fake<AatfContact>(), Domain.AatfReturn.FacilityType.Aatf, 2019, A.Fake<LocalArea>(), A.Fake<PanArea>());
            Aatf aatf2 = new Aatf("name", A.Dummy<UKCompetentAuthority>(), "1234", Domain.AatfReturn.AatfStatus.Approved, A.Fake<Organisation>(), A.Dummy<AatfAddress>(), Domain.AatfReturn.AatfSize.Large, date, A.Fake<AatfContact>(), Domain.AatfReturn.FacilityType.Aatf, 2019, A.Fake<LocalArea>(), A.Fake<PanArea>());
            Aatf aatf3 = new Aatf("name", A.Dummy<UKCompetentAuthority>(), "1234", Domain.AatfReturn.AatfStatus.Approved, A.Fake<Organisation>(), A.Dummy<AatfAddress>(), Domain.AatfReturn.AatfSize.Large, date, A.Fake<AatfContact>(), Domain.AatfReturn.FacilityType.Ae, 2019, A.Fake<LocalArea>(), A.Fake<PanArea>());

            var aatfs = new List<Aatf>()
            {
               aatf, aatf2
            };

            A.CallTo(() => dataAccess.GetManyByExpression(A<AatfsByOrganisationAndFacilityTypeSpecification>._)).Returns(aatfs);

            A.CallTo(() => mapper.Map(aatfs.ElementAt(0))).Returns(aatfDatas.ElementAt(0));
            A.CallTo(() => mapper.Map(aatfs.ElementAt(1))).Returns(aatfDatas.ElementAt(1));

            var result = await handler.HandleAsync(new GetAatfByOrganisationFacilityType(id, Core.AatfReturn.FacilityType.Aatf));

            result.Count().Should().Be(2);
        }
    }
}
