namespace EA.Weee.Web.Tests.Unit.ViewModels.Returns
{
    using System;
    using Core.AatfReturn;
    using Core.DataReturns;
    using FluentAssertions;
    using Web.ViewModels.Returns;
    using Xunit;

    public class SubmittedReturnViewModelTests
    {
        [Fact]
        public void SubmittedReturnViewModel_GivenQuarterIsNull_ArgumentNullExceptionExpected()
        {
            var returnData = new ReturnData()
            {
                QuarterWindow = new QuarterWindow(new DateTime(2019, 1, 1), new DateTime(2020, 1, 1), (int)QuarterType.Q1)
            };

            Action action = () =>
            {
                var submittedReturnViewModel = new SubmittedReturnViewModel(returnData);
            };

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SubmittedReturnViewModel_GivenQuarterWindowIsNull_ArgumentNullExceptionExpected()
        {
            var returnData = new ReturnData()
            {
                Quarter = new Quarter(2019, QuarterType.Q1)
            };

            Action action = () =>
            {
                var submittedReturnViewModel = new SubmittedReturnViewModel(returnData);
            };

            action.Should().Throw<ArgumentNullException>();
        }
    }
}
