namespace EA.Weee.RequestHandlers.Tests.Unit.Factories
{
    using DataAccess.DataAccess;
    using Domain.DataReturns;
    using Domain.Lookup;
    using FakeItEasy;
    using FluentAssertions;
    using RequestHandlers.Factories;
    using System;
    using System.Threading.Tasks;
    using Xunit;

    public class QuarterWindowFactoryTests
    {
        private readonly QuarterWindowFactory quarterWindowFactory;
        private readonly IQuarterWindowTemplateDataAccess dataAccess;

        public QuarterWindowFactoryTests()
        {
            dataAccess = A.Fake<IQuarterWindowTemplateDataAccess>();

            quarterWindowFactory = new QuarterWindowFactory(dataAccess);
        }

        [Fact]
        public async Task GetQuarter_GivenFirstQuarter_ValidQuarterStartAndEndDatesShouldBeCalculated()
        {
            var quarter = new Quarter(2019, QuarterType.Q1);
            var quarterWindowTemplate = QuarterWindowTemplate(4, 1, 0, 1);

            A.CallTo(() => dataAccess.GetByQuarter((int)quarter.Q)).Returns(quarterWindowTemplate);

            var result = await quarterWindowFactory.GetAnnualQuarter(quarter);

            result.StartDate.Year.Should().Be(2019);
            result.StartDate.Month.Should().Be(1);
            result.StartDate.Day.Should().Be(1);
            result.EndDate.Year.Should().Be(2019);
            result.EndDate.Month.Should().Be(3);
            result.EndDate.Day.Should().Be(31);
        }

        [Fact]
        public async Task GetQuarter_GivenSecondQuarter_ValidQuarterStartAndEndDatesShouldBeCalculated()
        {
            var quarter = new Quarter(2019, QuarterType.Q2);
            var quarterWindowTemplate = QuarterWindowTemplate(7, 1, 0, 1);

            A.CallTo(() => dataAccess.GetByQuarter((int)quarter.Q)).Returns(quarterWindowTemplate);

            var result = await quarterWindowFactory.GetAnnualQuarter(quarter);

            result.StartDate.Year.Should().Be(2019);
            result.StartDate.Month.Should().Be(4);
            result.StartDate.Day.Should().Be(1);
            result.EndDate.Year.Should().Be(2019);
            result.EndDate.Month.Should().Be(6);
            result.EndDate.Day.Should().Be(30);
        }

        [Fact]
        public async Task GetQuarter_GivenThirdQuarter_ValidQuarterStartAndEndDatesShouldBeCalculated()
        {
            var quarter = new Quarter(2019, QuarterType.Q3);
            var quarterWindowTemplate = QuarterWindowTemplate(10, 1, 0, 1);

            A.CallTo(() => dataAccess.GetByQuarter((int)quarter.Q)).Returns(quarterWindowTemplate);

            var result = await quarterWindowFactory.GetAnnualQuarter(quarter);

            result.StartDate.Year.Should().Be(2019);
            result.StartDate.Month.Should().Be(7);
            result.StartDate.Day.Should().Be(1);
            result.EndDate.Year.Should().Be(2019);
            result.EndDate.Month.Should().Be(9);
            result.EndDate.Day.Should().Be(30);
        }

        [Fact]
        public async Task GetQuarter_GivenFourthQuarter_ValidQuarterStartAndEndDatesShouldBeCalculated()
        {
            var quarter = new Quarter(2019, QuarterType.Q4);
            var quarterWindowTemplate = QuarterWindowTemplate(1, 1, 1, 1);

            A.CallTo(() => dataAccess.GetByQuarter((int)quarter.Q)).Returns(quarterWindowTemplate);

            var result = await quarterWindowFactory.GetAnnualQuarter(quarter);

            result.StartDate.Year.Should().Be(2019);
            result.StartDate.Month.Should().Be(10);
            result.StartDate.Day.Should().Be(1);
            result.EndDate.Year.Should().Be(2019);
            result.EndDate.Month.Should().Be(12);
            result.EndDate.Day.Should().Be(31);
        }

        [Fact]
        public async Task GetNextQuarterWindow_CurrentIsQ1_ReturnsQ2()
        {
            Quarter quarter = new Quarter(2019, QuarterType.Q1);
            QuarterWindowTemplate quarterWindowTemplate = QuarterWindowTemplate(7, 1, 0, 1);

            A.CallTo(() => dataAccess.GetByQuarter(((int)quarter.Q) + 1)).Returns(quarterWindowTemplate);

            QuarterWindow result = await quarterWindowFactory.GetNextQuarterWindow(QuarterType.Q1, 2019);

            result.StartDate.Year.Should().Be(2019);
            result.StartDate.Month.Should().Be(07);
            result.StartDate.Day.Should().Be(01);
            result.EndDate.Year.Should().Be(2020);
            result.EndDate.Month.Should().Be(03);
            result.EndDate.Day.Should().Be(16);
            Assert.Equal(QuarterType.Q2, result.QuarterType);
        }

        [Fact]
        public async Task GetNextQuarterWindow_CurrentIsQ2_ReturnsQ3()
        {
            Quarter quarter = new Quarter(2019, QuarterType.Q2);
            QuarterWindowTemplate quarterWindowTemplate = QuarterWindowTemplate(10, 1, 0, 1);

            A.CallTo(() => dataAccess.GetByQuarter(((int)quarter.Q) + 1)).Returns(quarterWindowTemplate);

            QuarterWindow result = await quarterWindowFactory.GetNextQuarterWindow(QuarterType.Q2, 2019);

            result.StartDate.Year.Should().Be(2019);
            result.StartDate.Month.Should().Be(10);
            result.StartDate.Day.Should().Be(01);
            result.EndDate.Year.Should().Be(2020);
            result.EndDate.Month.Should().Be(03);
            result.EndDate.Day.Should().Be(16);
            Assert.Equal(QuarterType.Q3, result.QuarterType);
        }

        [Fact]
        public async Task GetNextQuarterWindow_CurrentIsQ3_ReturnsQ4()
        {
            Quarter quarter = new Quarter(2019, QuarterType.Q3);
            QuarterWindowTemplate quarterWindowTemplate = QuarterWindowTemplate(1, 1, 1, 1);

            A.CallTo(() => dataAccess.GetByQuarter(((int)quarter.Q) + 1)).Returns(quarterWindowTemplate);

            QuarterWindow result = await quarterWindowFactory.GetNextQuarterWindow(QuarterType.Q3, 2019);

            result.StartDate.Year.Should().Be(2020);
            result.StartDate.Month.Should().Be(01);
            result.StartDate.Day.Should().Be(01);
            result.EndDate.Year.Should().Be(2020);
            result.EndDate.Month.Should().Be(03);
            result.EndDate.Day.Should().Be(16);
            Assert.Equal(QuarterType.Q4, result.QuarterType);
        }

        [Fact]
        public async Task GetNextQuarterWindow_CurrentIsQ4_ReturnsQ1()
        {
            Quarter quarter = new Quarter(2019, QuarterType.Q4);
            QuarterWindowTemplate quarterWindowTemplate = QuarterWindowTemplate(4, 1, 0, 1);

            A.CallTo(() => dataAccess.GetByQuarter(1)).Returns(quarterWindowTemplate);

            QuarterWindow result = await quarterWindowFactory.GetNextQuarterWindow(QuarterType.Q4, 2019);

            result.StartDate.Year.Should().Be(2019);
            result.StartDate.Month.Should().Be(04);
            result.StartDate.Day.Should().Be(01);
            result.EndDate.Year.Should().Be(2020);
            result.EndDate.Month.Should().Be(03);
            result.EndDate.Day.Should().Be(16);
            Assert.Equal(QuarterType.Q1, result.QuarterType);
        }

        [Theory]
        [InlineData("2019/01/01")]
        [InlineData("2019/03/31")]
        public async Task GetAnnualQuarterForDate_GivenDateInFirstQuarter_QuarterShouldBeCorrect(DateTime date)
        {
            SetupQuarterWindowTemplates();

            var result = await quarterWindowFactory.GetAnnualQuarterForDate(date);

            result.Should().Be(QuarterType.Q1);
        }

        [Theory]
        [InlineData("2019/04/01")]
        [InlineData("2019/06/30")]
        public async Task GetAnnualQuarterForDate_GivenDateInSecondQuarter_QuarterShouldBeCorrect(DateTime date)
        {
            SetupQuarterWindowTemplates();

            var result = await quarterWindowFactory.GetAnnualQuarterForDate(date);

            result.Should().Be(QuarterType.Q2);
        }

        [Theory]
        [InlineData("2019/07/01")]
        [InlineData("2019/09/30")]
        public async Task GetAnnualQuarterForDate_GivenDateInThirdQuarter_QuarterShouldBeCorrect(DateTime date)
        {
            SetupQuarterWindowTemplates();

            var result = await quarterWindowFactory.GetAnnualQuarterForDate(date);

            result.Should().Be(QuarterType.Q3);
        }

        [Theory]
        [InlineData("2019/10/01")]
        [InlineData("2019/12/31")]
        public async Task GetAnnualQuarterForDate_GivenDateInFourthQuarter_QuarterShouldBeCorrect(DateTime date)
        {
            SetupQuarterWindowTemplates();

            var result = await quarterWindowFactory.GetAnnualQuarterForDate(date);

            result.Should().Be(QuarterType.Q4);
        }

        private QuarterWindowTemplate QuarterWindowTemplate(int startMonth, int startDay, int addStartYear, int addEndYear)
        {
            return new QuarterWindowTemplate() { StartMonth = startMonth, StartDay = startDay, AddStartYears = addStartYear, EndMonth = 3, AddEndYears = addEndYear, EndDay = 16 };
        }

        private void SetupQuarterWindowTemplates()
        {
            A.CallTo(() => dataAccess.GetByQuarter((int)QuarterType.Q1)).Returns(QuarterWindowTemplate(4, 1, 0, 1));
            A.CallTo(() => dataAccess.GetByQuarter((int)QuarterType.Q2)).Returns(QuarterWindowTemplate(7, 1, 0, 1));
            A.CallTo(() => dataAccess.GetByQuarter((int)QuarterType.Q3)).Returns(QuarterWindowTemplate(10, 1, 0, 1));
            A.CallTo(() => dataAccess.GetByQuarter((int)QuarterType.Q4)).Returns(QuarterWindowTemplate(1, 1, 1, 1));
        }
    }
}
