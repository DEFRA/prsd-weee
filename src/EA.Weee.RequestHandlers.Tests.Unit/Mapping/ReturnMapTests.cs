namespace EA.Weee.RequestHandlers.Tests.Unit.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Domain.AatfReturn;
    using Domain.DataReturns;
    using Domain.Organisation;
    using FluentAssertions;
    using Mappings;
    using Xunit;

    public class ReturnMapTests
    {
        private readonly ReturnMap map;

        public ReturnMapTests()
        {
            map = new ReturnMap();
        }

        [Fact]
        public void Map_GivenSourceIsNull_ArgumentNullExceptionExpected()
        {
            Action action = () => map.Map(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenSource_PropertiesShouldBeMapped()
        {
            var organisation = Organisation.CreatePartnership("trading name");
            var @operator = new Operator(organisation);
            var quarter = new Quarter(2019, QuarterType.Q1);
            var startTime = DateTime.Now;
            var endTime = DateTime.Now.AddDays(1);
            var @return = new Return(@operator, quarter, ReturnStatus.Created);
            var quarterWindow = new QuarterWindow(startTime, endTime);

            var source = new ReturnQuarterWindow(@return, quarterWindow);

            var result = map.Map(source);

            result.Quarter.Q.Should().Be(EA.Weee.Core.DataReturns.QuarterType.Q1);
            result.Quarter.Year.Should().Be(2019);
            result.QuarterWindow.EndDate.Should().Be(endTime);
            result.QuarterWindow.StartDate.Should().Be(startTime);
        }
    }
}
