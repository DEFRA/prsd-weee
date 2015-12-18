namespace EA.Weee.Domain.Tests.Unit.DataReturns
{
    using System;
    using Domain.Producer;
    using EA.Weee.Domain.DataReturns;
    using FakeItEasy;
    using Lookup;
    using Xunit;

    public class EeeOutputAmountTests
    {
        [Fact]
        public void ConstructsEeeOutputAmount_WithNullRegisteredProducer_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new EeeOutputAmount(ObligationType.B2B, A<WeeeCategory>._, A<decimal>._, null));
        }
    }
}
