namespace EA.Weee.Domain.Tests.Unit.DataReturns
{
    using System;
    using Domain.Producer;
    using EA.Weee.Domain.DataReturns;
    using FakeItEasy;
    using Lookup;
    using Xunit;
    using Scheme = Domain.Scheme.Scheme;

    public class DataReturnVersionTests
    {
        [Fact]
        public void AddEeeOutputAmount_AddNull_ThrowsArgumentNullException()
        {
            var returnVersion = new DataReturnVersion(A.Fake<DataReturn>());

            Assert.Throws<ArgumentNullException>(() => returnVersion.AddEeeOutputAmount(null));
        }

        [Fact]
        public void AddEeeOutputAmount_WithEeeOutputAmountProducerNotInCurrentScheme_ThrowsInvalidOperationException()
        {
            var scheme1 = A.Fake<Scheme>();
            A.CallTo(() => scheme1.Id).Returns(Guid.NewGuid());

            var scheme2 = A.Fake<Scheme>();
            A.CallTo(() => scheme2.Id).Returns(Guid.NewGuid());

            var registeredProducer = A.Fake<RegisteredProducer>();
            var outputAmount = A.Fake<EeeOutputAmount>();
            A.CallTo(() => outputAmount.RegisteredProducer).Returns(registeredProducer);
            A.CallTo(() => registeredProducer.Scheme).Returns(scheme1);

            var dataReturn = A.Fake<DataReturn>();
            A.CallTo(() => dataReturn.Scheme).Returns(scheme2);

            var returnVersion = new DataReturnVersion(dataReturn);

            Assert.Throws<InvalidOperationException>(() => returnVersion.AddEeeOutputAmount(outputAmount));
        }

        [Fact]
        public void AddEeeOutputAmount_AddsToEeeOutputAmounts()
        {
            // Arrange
            var returnVersion = new DataReturnVersion(A.Fake<DataReturn>());
            var eeeOutputAmount = new EeeOutputAmount(A<ObligationType>._, A<WeeeCategory>._, A<decimal>._, A.Fake<RegisteredProducer>(), A.Fake<DataReturnVersion>());

            // Act
            returnVersion.AddEeeOutputAmount(eeeOutputAmount);

            // Assert
            Assert.Contains(eeeOutputAmount, returnVersion.EeeOutputAmounts);
        }
    }
}
