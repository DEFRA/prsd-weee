namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Mapping.ToViewModel
{
    using System;
    using Core.AatfReturn;
    using EA.Prsd.Core;
    using FluentAssertions;
    using Web.Areas.AatfReturn.Mappings.ToViewModel;
    using Xunit;

    public class ReturnStatusToReturnDisplayOptionsMapperTests
    {
        private readonly ReturnStatusToReturnDisplayOptionsMap mapper;

        public ReturnStatusToReturnDisplayOptionsMapperTests()
        {
            mapper = new ReturnStatusToReturnDisplayOptionsMap();
        }

        [Fact]
        public void Map_GivenReturnStatusIsCreated_QuarterOpen_DisplayOptionsShouldBeSet()
        {
            SystemTime.Freeze(new DateTime(2019, 04, 01));
            var displayOptions = mapper.Map((ReturnStatus.Created, new QuarterWindow(new DateTime(2019, 01, 01), new DateTime(2019, 03, 31))));
            SystemTime.Unfreeze();
            
            displayOptions.DisplayContinue.Should().BeTrue();
            displayOptions.DisplayEdit.Should().BeFalse();
            displayOptions.DisplaySummary.Should().BeFalse();
        }

        [Fact]
        public void Map_GivenReturnStatusIsCreated_QuarterClosed_DisplayOptionsShouldBeSet()
        {
            var displayOptions = mapper.Map((ReturnStatus.Created, new QuarterWindow(new DateTime(2000, 01, 01), new DateTime(2000, 03, 31))));

            displayOptions.DisplayContinue.Should().BeFalse();
            displayOptions.DisplayEdit.Should().BeFalse();
            displayOptions.DisplaySummary.Should().BeFalse();
        }

        [Fact]
        public void Constructor_GivenReturnStatusIsSubmitted_QuarterOpen_DisplayOptionsShouldBeSet()
        {
            SystemTime.Freeze(new DateTime(2019, 04, 01));
            var displayOptions = mapper.Map((ReturnStatus.Submitted, new QuarterWindow(new DateTime(2019, 01, 01), new DateTime(2019, 03, 31))));
            SystemTime.Unfreeze();

            displayOptions.DisplayContinue.Should().BeFalse();
            displayOptions.DisplayEdit.Should().BeTrue();
            displayOptions.DisplaySummary.Should().BeTrue();
        }

        [Fact]
        public void Constructor_GivenReturnStatusIsSubmitted_QuarterClosed_DisplayOptionsShouldBeSet()
        {
            var displayOptions = mapper.Map((ReturnStatus.Submitted, new QuarterWindow(new DateTime(2000, 01, 01), new DateTime(2000, 03, 31))));

            displayOptions.DisplayContinue.Should().BeFalse();
            displayOptions.DisplayEdit.Should().BeTrue();
            displayOptions.DisplaySummary.Should().BeTrue();
        }
    }
}
