namespace EA.Weee.RequestHandlers.Tests.Unit.Mapping
{
    using System;
    using System.Collections.Generic;
    using Core.Organisations;
    using Domain.AatfReturn;
    using Domain.Lookup;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Shared;
    using EA.Weee.Domain;
    using EA.Weee.RequestHandlers.Mappings;
    using FakeItEasy;
    using FluentAssertions;
    using Weee.Tests.Core.Model;
    using Xunit;
    using Organisation = Domain.Organisation.Organisation;

    public class AatfDataListMapTests
    {
        private readonly AatfDataListMap map;
        private readonly IMap<Domain.AatfReturn.AatfStatus, Core.AatfReturn.AatfStatus> statusMap;
        private readonly IMap<Domain.UKCompetentAuthority, UKCompetentAuthorityData> competentAuthorityMap;
        private readonly IMap<Domain.AatfReturn.FacilityType, Core.AatfReturn.FacilityType> facilityTypeMap;
        private readonly IMap<Organisation, OrganisationData> organisationMap;

        public AatfDataListMapTests()
        {
            statusMap = A.Fake<IMap<Domain.AatfReturn.AatfStatus, Core.AatfReturn.AatfStatus>>();
            competentAuthorityMap = A.Fake<IMap<Domain.UKCompetentAuthority, UKCompetentAuthorityData>>();
            facilityTypeMap = A.Fake<IMap<Domain.AatfReturn.FacilityType, Core.AatfReturn.FacilityType>>();
            organisationMap = A.Fake<IMap<Organisation, OrganisationData>>();
            map = new AatfDataListMap(competentAuthorityMap, statusMap, facilityTypeMap, organisationMap);
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
            const string name = "KoalsInTheWild";
            var competentAuthority = A.Fake<Domain.UKCompetentAuthority>();
            var organisation = A.Fake<Organisation>();
            const string approvalNumber = "123456789";
            var status = Domain.AatfReturn.AatfStatus.Approved;
            var facilityType = Domain.AatfReturn.FacilityType.Aatf;
            Int16 complianceYear = 2019;

            var returnStatus = Core.AatfReturn.AatfStatus.Approved;
            var returnCompetentAuthority = A.Fake<Core.Shared.UKCompetentAuthorityData>();
            var returnFacilityType = Core.AatfReturn.FacilityType.Aatf;
            var organisationData = A.Fake<OrganisationData>();

            A.CallTo(() => statusMap.Map(status)).Returns(returnStatus);
            A.CallTo(() => competentAuthorityMap.Map(competentAuthority)).Returns(returnCompetentAuthority);
            A.CallTo(() => facilityTypeMap.Map(facilityType)).Returns(returnFacilityType);
            A.CallTo(() => organisationMap.Map(organisation)).Returns(organisationData);

            var source = new Aatf(name, competentAuthority, approvalNumber, status, organisation, A.Fake<AatfAddress>(),
                A.Fake<Domain.AatfReturn.AatfSize>(), DateTime.Now, A.Fake<AatfContact>(), Domain.AatfReturn.FacilityType.Aatf, complianceYear,
                A.Fake<LocalArea>(), A.Fake<PanArea>());

            var result = map.Map(source);

            result.Name.Should().Be(name);
            result.ApprovalNumber.Should().Be(approvalNumber);
            result.AatfStatus.Should().Be(returnStatus);
            result.CompetentAuthority.Should().Be(returnCompetentAuthority);
            result.Organisation.Should().Be(organisationData);
            result.FacilityType.Should().Be(Core.AatfReturn.FacilityType.Aatf);
            result.ComplianceYear.Should().Be(complianceYear);
        }
    }
}
