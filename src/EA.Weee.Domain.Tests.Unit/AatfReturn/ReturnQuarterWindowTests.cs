namespace EA.Weee.Domain.Tests.Unit.AatfReturn
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Core.AatfReturn;
    using Domain.AatfReturn;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class ReturnQuarterWindowTests
    {
        [Fact]
        public void ReturnQuarterWindow_GivenNullReturn_ArgumentNullExceptionExpected()
        {
            Action action = () =>
            {
                var returnQuarterWindow = new ReturnQuarterWindow(null, A.Dummy<EA.Weee.Domain.DataReturns.QuarterWindow>());
            };

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ReturnQuarterWindow_GivenNullQuarterWindow_ArgumentNullExceptionExpected()
        {
            Action action = () =>
            {
                var returnQuarterWindow = new ReturnQuarterWindow(A.Dummy<Return>(), null);
            };

            action.Should().Throw<ArgumentNullException>();
        }
    }
}
