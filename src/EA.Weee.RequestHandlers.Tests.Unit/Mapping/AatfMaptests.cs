﻿namespace EA.Weee.RequestHandlers.Tests.Unit.Mapping
{
    using System;
    using Domain.AatfReturn;
    using FakeItEasy;
    using FluentAssertions;
    using Mappings;
    using Xunit;

    public class AatfMapTests
    {
        private readonly AatfMap map;

        public AatfMapTests()
        {
            map = new AatfMap(A.Fake<UKCompetentAuthorityMap>(), A.Fake<AatfStatusMap>(), A.Fake<AatfSizeMap>(), A.Fake<AatfAddressMap>(), A.Fake<OperatorMap>(), A.Fake<AatfContactMap>(), A.Fake<OrganisationMap>(), A.Fake<FacilityTypeMap>());
        }

        [Fact]
        public void Map_GivenSourceIsNull_ArgumentNullExceptionExpected()
        {
            Action action = () => map.Map(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenSource_AatfDataPropertiesShouldBeMapped()
        {
            const string name = "name";
            const string approvalNumber = "approval";
            var id = Guid.NewGuid();

            var aatf = A.Fake<Aatf>();
            A.CallTo(() => aatf.ApprovalNumber).Returns(approvalNumber);
            A.CallTo(() => aatf.Name).Returns(name);
            A.CallTo(() => aatf.Id).Returns(id);
            
            var result = map.Map(aatf);

            result.Name.Should().Be(name);
            result.ApprovalNumber.Should().Be(approvalNumber);
            result.Id.Should().Be(id);
        }
    }
}
