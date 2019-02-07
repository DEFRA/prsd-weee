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
    using FakeItEasy;
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
        public void Map_GivenSource_QuarterPropertiesShouldBeMapped()
        {
            var source = new ReturnQuarterWindow(GetReturn(), A.Fake<QuarterWindow>());

            var result = map.Map(source);

            result.Quarter.Q.Should().Be(EA.Weee.Core.DataReturns.QuarterType.Q1);
            result.Quarter.Year.Should().Be(2019);
        }

        [Fact]
        public void Map_GivenSource_QuarterWindowPropertiesShouldBeMapped()
        {
            var startTime = DateTime.Now;
            var endTime = DateTime.Now.AddDays(1);
            var quarterWindow = new QuarterWindow(startTime, endTime);
            var source = new ReturnQuarterWindow(GetReturn(), quarterWindow);

            var result = map.Map(source);

            result.QuarterWindow.EndDate.Should().Be(endTime);
            result.QuarterWindow.StartDate.Should().Be(startTime);
        }

        [Fact]
        public void Map_GivenSource_NonObligatedValuesShouldBeMapped()
        {
            var @return = GetReturn();

            var nonObligated = new List<NonObligatedWeee>()
            {
                new NonObligatedWeee(@return, 1, true, 2),
                new NonObligatedWeee(@return, 2, false, 3)
            };

            var source = new ReturnQuarterWindow(GetReturn(), GetQuarterWindow(), nonObligated);

            var result = map.Map(source);

            result.NonObligatedData.Count(n => n.CategoryId == 1 && n.Dcf && n.Tonnage == 2).Should().Be(1);
            result.NonObligatedData.Count(n => n.CategoryId == 2 && !n.Dcf && n.Tonnage == 3).Should().Be(1);
            result.NonObligatedData.Count().Should().Be(2);
        }

        public Return GetReturn()
        {
            var organisation = Organisation.CreatePartnership("trading name");
            var @operator = new Operator(organisation);
            var quarter = new Quarter(2019, QuarterType.Q1);
            var @return = new Return(@operator, quarter, ReturnStatus.Created);

            return @return;
        }

        public QuarterWindow GetQuarterWindow()
        {
            var startTime = DateTime.Now;
            var endTime = DateTime.Now.AddDays(1);
            var quarterWindow = new QuarterWindow(startTime, endTime);

            return quarterWindow;
        }
    }
}
