namespace EA.Weee.Domain.Tests.Unit.DataReturns
{
    using System;
    using Domain.DataReturns;
    using FakeItEasy;
    using Lookup;
    using Xunit;

    public class WeeeCollectedAmountTests
    {
        [Fact]
        public void ConstructsWeeeCollectedAmount_WithNullDataReturnVersion_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new WeeeCollectedAmount(A<WeeeCollectedAmountSourceType>._, ObligationType.B2B, A<WeeeCategory>._, A<decimal>._, null));
        }
    }
}
