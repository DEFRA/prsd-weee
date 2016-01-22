namespace EA.Weee.Domain.Tests.Unit.DataReturns
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Domain.DataReturns;
    using Domain.Producer;
    using FakeItEasy;
    using Lookup;
    using Xunit;

    public class EeeOutputReturnVersionTests
    {
        [Fact]
        public void AddEeeOutputAmount_AddsToEeeOutputAmounts()
        {
            // Arrange
            var eeeOutputReturnVersion = new EeeOutputReturnVersion();

            var registeredProducer = A.Fake<RegisteredProducer>();
            A.CallTo(() => registeredProducer.Equals(A<RegisteredProducer>._))
                .Returns(true);

            var eeeOutputAmount = new EeeOutputAmount(ObligationType.B2B, A<WeeeCategory>._, A<decimal>._, registeredProducer);

            // Act
            eeeOutputReturnVersion.AddEeeOutputAmount(eeeOutputAmount);

            // Assert
            Assert.Contains(eeeOutputAmount, eeeOutputReturnVersion.EeeOutputAmounts);
        }
    }
}
