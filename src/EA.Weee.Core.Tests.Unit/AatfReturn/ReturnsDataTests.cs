namespace EA.Weee.Core.Tests.Unit.AatfReturn
{
    using System;
    using Core.AatfReturn;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class ReturnsDataTests
    {
        [Fact]
        public void ReturnsData_GivenNullReturnsList_ArgumentNullExceptionExpected()
        {
            var exception = Record.Exception(() => new ReturnsData(null, A.Dummy<ReturnQuarter>()));

            exception.Should().BeOfType<ArgumentNullException>();
        }
    }
}
