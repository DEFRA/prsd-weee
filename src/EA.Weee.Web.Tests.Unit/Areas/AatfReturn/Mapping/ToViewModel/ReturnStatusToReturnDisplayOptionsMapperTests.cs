﻿namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Mapping.ToViewModel
{
    using Core.AatfReturn;
    using FluentAssertions;
    using System;
    using Web.Areas.AatfReturn.Mappings.ToViewModel;
    using Weee.Tests.Core;
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
            var displayOptions = mapper.Map((ReturnStatus.Created, QuarterWindowTestHelper.GetDefaultQuarterWindow(), new DateTime(2019, 04, 01)));

            displayOptions.DisplayContinue.Should().BeTrue();
            displayOptions.DisplayEdit.Should().BeFalse();
            displayOptions.DisplaySummary.Should().BeFalse();
        }

        [Fact]
        public void Map_GivenReturnStatusIsCreated_QuarterClosed_DisplayOptionsShouldBeSet()
        {
            var displayOptions = mapper.Map((ReturnStatus.Created, QuarterWindowTestHelper.GetDefaultQuarterWindow(), new DateTime(2019, 3, 1)));

            displayOptions.DisplayContinue.Should().BeFalse();
            displayOptions.DisplayEdit.Should().BeFalse();
            displayOptions.DisplaySummary.Should().BeFalse();
        }

        [Fact]
        public void Constructor_GivenReturnStatusIsSubmitted_QuarterOpen_DisplayOptionsShouldBeSet()
        {
            var displayOptions = mapper.Map((ReturnStatus.Submitted, QuarterWindowTestHelper.GetDefaultQuarterWindow(), new DateTime(2019, 04, 01)));

            displayOptions.DisplayContinue.Should().BeFalse();
            displayOptions.DisplayEdit.Should().BeTrue();
            displayOptions.DisplaySummary.Should().BeTrue();
        }

        [Fact]
        public void Constructor_GivenReturnStatusIsSubmitted_QuarterClosed_DisplayOptionsShouldBeSet()
        {
            var displayOptions = mapper.Map((ReturnStatus.Submitted, QuarterWindowTestHelper.GetDefaultQuarterWindow(), new DateTime(2019, 3, 1)));

            displayOptions.DisplayContinue.Should().BeFalse();
            displayOptions.DisplayEdit.Should().BeFalse();
            displayOptions.DisplaySummary.Should().BeTrue();
        }
    }
}
