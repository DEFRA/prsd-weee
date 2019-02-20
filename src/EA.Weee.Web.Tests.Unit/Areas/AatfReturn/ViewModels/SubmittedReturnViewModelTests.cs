namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.ViewModels
{
    using System;
    using Core.AatfReturn;
    using Core.DataReturns;
    using FakeItEasy;
    using FluentAssertions;
    using Web.Areas.AatfReturn.ViewModels;
    using Xunit;

    public class SubmittedReturnViewModelTests
    {
        [Fact]
        public void SubmittedReturnViewModel_GivenQuarterIsNull_ArgumentNullExceptionExpected()
        {
            Action action = () =>
            {
                var submittedReturnViewModel = new SubmittedReturnViewModel(null, A.Fake<QuarterWindow>(), 1);
            };

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SubmittedReturnViewModel_GivenQuarterWindowIsNull_ArgumentNullExceptionExpected()
        {
            Action action = () =>
            {
                var submittedReturnViewModel = new SubmittedReturnViewModel(new Quarter(2019, QuarterType.Q1), null, 1);
            };

            action.Should().Throw<ArgumentNullException>();
        }
    }
}
