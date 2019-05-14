namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Mapping.ToViewModel
{
    using System;
    using System.Collections.Generic;
    using Core.AatfReturn;
    using Core.Scheme;
    using FakeItEasy;
    using FluentAssertions;
    using Web.Areas.AatfReturn.Mappings.ToViewModel;
    using Xunit;

    public class ReturnToReturnsListRedirectOptionsMapTests
    {
        private readonly ReturnToReturnsListRedirectOptionsMap mapper;

        public ReturnToReturnsListRedirectOptionsMapTests()
        {
            mapper = new ReturnToReturnsListRedirectOptionsMap();
        }

        [Fact]
        public void Map_GivenNoReportingOptionsAreSelected_RedirectOptionsShouldBeCorrect()
        {
            var returnData = new ReturnData()
            {
                ReturnReportOns = new List<ReturnReportOn>(),
                SchemeDataItems = new List<SchemeData>() { new SchemeData() }
            };

            var result = mapper.Map(returnData);

            result.RedirectReportingOptions.Should().BeTrue();
            result.RedirectSelectYourPcs.Should().BeFalse();
            result.RedirectTaskList.Should().BeFalse();
        }

        [Fact]
        public void Map_GivenWeeeReceivedReportingOptionAndPcsAreSelected_RedirectOptionsShouldBeCorrect()
        {
            var returnData = new ReturnData()
            {
                ReturnReportOns = new List<ReturnReportOn>()
                {
                    new ReturnReportOn((int)ReportOnQuestionEnum.WeeeReceived, Guid.NewGuid()),
                },
                SchemeDataItems = new List<SchemeData>() { new SchemeData() }
            };

            var result = mapper.Map(returnData);

            result.RedirectReportingOptions.Should().BeFalse();
            result.RedirectSelectYourPcs.Should().BeFalse();
            result.RedirectTaskList.Should().BeTrue();
        }

        [Fact]
        public void Map_GivenNotWeeeReceivedReportingOptionAndPcsAreSelected_RedirectOptionsShouldBeCorrect()
        {
            var returnData = new ReturnData()
            {
                ReturnReportOns = new List<ReturnReportOn>()
                {
                    new ReturnReportOn((int)ReportOnQuestionEnum.NonObligated, Guid.NewGuid()),
                },
                SchemeDataItems = new List<SchemeData>() { new SchemeData() }
            };

            var result = mapper.Map(returnData);

            result.RedirectReportingOptions.Should().BeFalse();
            result.RedirectSelectYourPcs.Should().BeFalse();
            result.RedirectTaskList.Should().BeTrue();
        }

        [Fact]
        public void Map_GivenWeeeReceivedReportingOptionAndPcsAreNotSelected_RedirectOptionsShouldBeCorrect()
        {
            var returnData = new ReturnData()
            {
                ReturnReportOns = new List<ReturnReportOn>()
                {
                    new ReturnReportOn((int)ReportOnQuestionEnum.WeeeReceived, Guid.NewGuid()),
                },
                SchemeDataItems = new List<SchemeData>()
            };

            var result = mapper.Map(returnData);

            result.RedirectReportingOptions.Should().BeFalse();
            result.RedirectSelectYourPcs.Should().BeTrue();
            result.RedirectTaskList.Should().BeFalse();
        }
    }
}
