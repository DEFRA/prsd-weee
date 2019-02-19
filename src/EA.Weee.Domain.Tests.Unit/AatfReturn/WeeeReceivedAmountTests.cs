namespace EA.Weee.Domain.Tests.Unit.AatfReturn
{
    using System;
    using Domain.AatfReturn;
    using Xunit;

    public class WeeeReceivedAmountTests
    {
        [Fact]
        public void WeeeReceivedAmount_WeeeReceivedNotDefined_ThrowsArugmentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new WeeeReceivedAmount(null, 2, 2, 3));
        }
    }
}