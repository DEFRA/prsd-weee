namespace EA.Weee.Domain.Tests.Unit.DataReturns
{
    using System;
    using Domain.DataReturns;
    using FakeItEasy;
    using Lookup;
    using Obligation;
    using Xunit;

    public class WeeeCollectedAmountTests
    {
        [Fact]
        public void AddWeeeCollectedAmount_WithNullWeeeColletctedReturnVersion_ThrowsArgumentNullException()
        {
            var weeeCollectedAmount = new WeeeCollectedAmount(A<WeeeCollectedAmountSourceType>._, ObligationType.B2B, A<WeeeCategory>._, A<decimal>._);
            Assert.Throws<ArgumentNullException>(() => weeeCollectedAmount.AddWeeeCollectedReturnVersion(null));
        }
    }
}
