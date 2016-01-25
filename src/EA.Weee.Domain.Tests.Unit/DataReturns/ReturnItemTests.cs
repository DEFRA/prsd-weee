namespace EA.Weee.Domain.Tests.Unit.DataReturns
{
    using System;
    using Domain.DataReturns;
    using FakeItEasy;
    using Lookup;
    using Obligation;
    using Xunit;

    public class ReturnItemTests
    {
        [Fact]
        public void ConstructsReturnItem_WithTonnageLessThanZero_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new ReturnItem(ObligationType.B2B, A<WeeeCategory>._, -1));
        }

        [Theory]
        [InlineData(ObligationType.None)]
        [InlineData(ObligationType.Both)]
        public void ConstructsReturnItem_WithObligationTypeNotB2BOrB2C_ThrowsInvalidOperationException(ObligationType obligationType)
        {
            Assert.Throws<InvalidOperationException>(() => new ReturnItem(obligationType, A<WeeeCategory>._, A<decimal>._));
        }
    }
}
