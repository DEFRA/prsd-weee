namespace EA.Weee.RequestHandlers.Tests.Unit.Mapping
{
    using System;
    using System.Collections.Generic;
    using Domain.AatfReturn;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Shared;
    using EA.Weee.Domain;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.RequestHandlers.Mappings;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class AatfDataListMapTests
    {
        private readonly AatfDataListMap map;
        private readonly IMap<Domain.AatfReturn.AatfStatus, Core.AatfReturn.AatfStatus> statusMap;
        private readonly IMap<Domain.UKCompetentAuthority, UKCompetentAuthorityData> competentAuthorityMap;
        public AatfDataListMapTests()
        {
            statusMap = A.Fake<IMap<Domain.AatfReturn.AatfStatus, Core.AatfReturn.AatfStatus>>();
            competentAuthorityMap = A.Fake<IMap<Domain.UKCompetentAuthority, UKCompetentAuthorityData>>();
            map = new AatfDataListMap(competentAuthorityMap, statusMap);
        }

        [Fact]
        public void Map_GivenSourceIsNull_ArgumentNullExceptionExpected()
        {
            Action action = () => map.Map(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenSource_AatfDataListPropertiesShouldBeMapped()
        {
            var name = "KoalsInTheWild";
            var competentAuthority = A.Fake<Domain.UKCompetentAuthority>();
            var @operator = A.Fake<Operator>();
            var approvalNumber = "123456789";
            var status = Domain.AatfReturn.AatfStatus.Approved;

            var returnStatus = Core.AatfReturn.AatfStatus.Approved;
            var returnCompetentAuthority = A.Fake<Core.Shared.UKCompetentAuthorityData>();

            A.CallTo(() => statusMap.Map(status)).Returns(returnStatus);
            A.CallTo(() => competentAuthorityMap.Map(competentAuthority)).Returns(returnCompetentAuthority);

            var source = new Aatf(name, competentAuthority, approvalNumber, status, @operator, A.Fake<AatfAddress>(), A.Fake<Domain.AatfReturn.AatfSize>(), DateTime.Now, A.Fake<AatfContact>(), Domain.AatfReturn.FacilityType.Aatf);

            var result = map.Map(source);

            result.Name.Should().Be(name);
            result.ApprovalNumber.Should().Be(approvalNumber);
            result.AatfStatus.Should().Be(returnStatus);
            result.CompetentAuthority.Should().Be(returnCompetentAuthority);
            result.Operator.Should().Be(@operator);
        }
    }
}
