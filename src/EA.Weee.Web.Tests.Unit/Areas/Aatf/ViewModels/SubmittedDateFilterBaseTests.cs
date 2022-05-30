namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.ViewModels
{
    using System;
    using AutoFixture;
    using EA.Weee.Web.Areas.Aatf.ViewModels;
    using FluentAssertions;
    using Xunit;

    public class SubmittedDateFilterBaseTests
    {
        private readonly Fixture fixture;

        public SubmittedDateFilterBaseTests()
        {
            fixture = new Fixture();
        }
       
        [Fact]
        public void SubmittedDateFilterBase_Constructor_ShouldInitialisedDates()
        {
            var startDate = fixture.Create<DateTime>();
            var endDate = fixture.Create<DateTime>();

            var model = new SubmittedDateFilterBase(startDate, endDate);

            model.StartDate.Should().Be(startDate);
            model.EndDate.Should().Be(endDate);
        }
    }
}
