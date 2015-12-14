namespace EA.Weee.Domain.Tests.Unit.DataReturns
{
    using System;
    using Domain.DataReturns;
    using FakeItEasy;
    using Lookup;
    using Xunit;

    public class WeeeDeliveredAmountTests
    {
        [Fact]
        public void ConstructsWeeeDeliveredAmount_WithNullDataReturnVersion_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new WeeeDeliveredAmountTester(ObligationType.B2B, A<WeeeCategory>._, A<decimal>._, null));
        }

        private class WeeeDeliveredAmountTester : WeeeDeliveredAmount
        {
            public WeeeDeliveredAmountTester(ObligationType obligationType, WeeeCategory weeeCategory, decimal tonnage, DataReturnVersion dataReturnVersion) :
                base(obligationType, weeeCategory, tonnage, dataReturnVersion)
            {
            }
        }
    }
}
