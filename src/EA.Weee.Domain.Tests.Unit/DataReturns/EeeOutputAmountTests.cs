namespace EA.Weee.Domain.Tests.Unit.DataReturns
{
    using System;
    using Domain.Producer;
    using EA.Weee.Domain.DataReturns;
    using EA.Weee.Domain.Lookup;
    using FakeItEasy;
    using Xunit;

    public class EeeOutputAmountTests
    {
        [Fact]
        public void ConstructsEeeOutputAmount_WithNullRegisteredProducer_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new EeeOutputAmount(A<ObligationType>._, A<Category>._, A<decimal>._, null, A.Fake<DataReturnVersion>()));
        }

        [Fact]
        public void ConstructsEeeOutputAmount_WithNullDataReturnVersion_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new EeeOutputAmount(A<ObligationType>._, A<Category>._, A<decimal>._, A.Fake<RegisteredProducer>(), null));
        }
    }
}
