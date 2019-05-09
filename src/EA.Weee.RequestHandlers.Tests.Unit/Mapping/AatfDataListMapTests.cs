namespace EA.Weee.RequestHandlers.Tests.Unit.Mapping
{
    using System;
    using System.Collections.Generic;
    using Domain.AatfReturn;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Domain;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.RequestHandlers.Mappings;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class AatfDataListMapTests
    {
        private readonly AatfDataListMap map;
        public AatfDataListMapTests()
        {
            map = new AatfDataListMap();
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
            var competentAuthority = A.Fake<UKCompetentAuthority>();
            var aatfContact = A.Fake<AatfContact>();
            var @operator = A.Fake<Operator>();
            var approvalNumber = "123456789";
            var status = AatfStatus.Approved;

            var source = new Aatf(name, competentAuthority, approvalNumber, status, @operator, aatfContact);

            var result = map.Map(source);

            result.Name.Should().Be(name);
            result.ApprovalNumber.Should().Be(approvalNumber);
            result.AatfStatus.Should().Be(status);
            result.CompetentAuthority.Should().Be(competentAuthority);
            result.Operator.Should().Be(@operator);
        }
    }
}
