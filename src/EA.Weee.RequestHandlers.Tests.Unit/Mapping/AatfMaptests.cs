namespace EA.Weee.RequestHandlers.Tests.Unit.Mapping
{
    using Domain;
    using Domain.AatfReturn;
    using Domain.DataReturns;
    using Domain.Organisation;
    using FakeItEasy;
    using FluentAssertions;
    using Mappings;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;
    using DomainAatf = Domain.AatfReturn.Aatf;
    using DomainScheme = Domain.Scheme.Scheme;

    public class AatfMapTests
    {
        private readonly AatfMap map;

        public AatfMapTests()
        {
            map = new AatfMap();
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
