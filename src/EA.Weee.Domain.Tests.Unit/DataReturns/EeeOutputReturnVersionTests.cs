namespace EA.Weee.Domain.Tests.Unit.DataReturns
{
    using System;
    using Domain.DataReturns;
    using Domain.Producer;
    using FakeItEasy;
    using Lookup;
    using Obligation;
    using Xunit;

    public class EeeOutputReturnVersionTests
    {
        [Fact]
        public void ConstructsEeeOutputReturnVersion_WithNullDataReturnVersion_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new EeeOutputReturnVersion(null));
        }

        [Fact]
        public void ConstructEeeOutputReturnVersion_AddsDataReturnVersion_ToDataReturnVersionsList()
        {
            // Arrange
            var dataReturnVersion = new DataReturnVersion(A.Dummy<DataReturn>());

            // Act
            var eeeOutputReturnVersion = new EeeOutputReturnVersion(dataReturnVersion);

            // Assert
            Assert.Contains(dataReturnVersion, eeeOutputReturnVersion.DataReturnVersions);
        }

        [Fact]
        public void AddEeeOutputAmount_AddsToEeeOutputAmounts()
        {
            // Arrange
            var eeeOutputReturnVersion = new EeeOutputReturnVersion(A.Fake<DataReturnVersion>());
            var eeeOutputAmount = new EeeOutputAmount(ObligationType.B2B, A<WeeeCategory>._, A<decimal>._, A.Fake<RegisteredProducer>());

            // Act
            eeeOutputReturnVersion.AddEeeOutputAmount(eeeOutputAmount);

            // Assert
            Assert.Contains(eeeOutputAmount, eeeOutputReturnVersion.EeeOutputAmounts);
        }
    }
}
