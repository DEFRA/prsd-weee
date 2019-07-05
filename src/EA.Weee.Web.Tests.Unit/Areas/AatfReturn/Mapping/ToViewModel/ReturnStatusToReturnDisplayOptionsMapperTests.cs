namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Mapping.ToViewModel
{
    using Core.AatfReturn;
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
        public void Map_GivenReturnStatusIsCreated_DisplayOptionsShouldBeSet()
        {
            var displayOptions = mapper.Map(ReturnStatus.Created);
            
            displayOptions.DisplayContinue.Should().BeTrue();
            displayOptions.DisplayEdit.Should().BeFalse();
            displayOptions.DisplaySummary.Should().BeFalse();
        }

        [Fact]
        public void Constructor_GivenReturnStatusIsSubmitted_DisplayOptionsShouldBeSet()
        {
            var displayOptions = mapper.Map(ReturnStatus.Submitted);

            displayOptions.DisplayContinue.Should().BeFalse();
            displayOptions.DisplayEdit.Should().BeTrue();
            displayOptions.DisplaySummary.Should().BeTrue();
        }
    }
}
