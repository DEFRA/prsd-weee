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
    using Scheme = Domain.Scheme.Scheme;

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

        [Fact]
        public void AddEeeOutputAmount_WithNullParameter_ThrowsArgumentNullException()
        {
            // Arrange
            var eeeOutputReturnVersion = new EeeOutputReturnVersion();

            // Act, Assert
            Assert.Throws<ArgumentNullException>(() => eeeOutputReturnVersion.AddEeeOutputAmount(null));
        }

        [Fact]
        public void AddEeeOutputAmount_WithRegisteredProducerFromAnotherScheme_ThrowsInvalidOperationException()
        {
            // Arrange
            var scheme1 = A.Fake<Scheme>();
            A.CallTo(() => scheme1.ApprovalNumber)
                .Returns("WEE/SC1000SCH");

            var scheme2 = A.Fake<Scheme>();
            A.CallTo(() => scheme2.ApprovalNumber)
                .Returns("WEE/SC2000SCH");

            var dataReturn = new DataReturn(scheme1, new Quarter(2016, QuarterType.Q2));
            var dataReturnVersion = new DataReturnVersion(dataReturn);

            var eeeOutputReturnVersion = new EeeOutputReturnVersion();
            eeeOutputReturnVersion.AddDataReturnVersion(dataReturnVersion);

            var registeredProducer = new RegisteredProducer("PRN", 2016, scheme2);

            // Act, Assert
            Assert.Throws<InvalidOperationException>(() => eeeOutputReturnVersion.AddEeeOutputAmount(
                new EeeOutputAmount(ObligationType.B2C, WeeeCategory.AutomaticDispensers, 100, registeredProducer)));
        }

        [Fact]
        public void AddEeeOutputAmount_WithRegisteredProducerFromAnotherComplianceYear_ThrowsInvalidOperationException()
        {
            // Arrange
            var scheme = A.Fake<Scheme>();
            A.CallTo(() => scheme.ApprovalNumber)
                .Returns("WEE/SC1000SCH");

            var dataReturn = new DataReturn(scheme, new Quarter(2016, QuarterType.Q2));
            var dataReturnVersion = new DataReturnVersion(dataReturn);

            var eeeOutputReturnVersion = new EeeOutputReturnVersion();
            eeeOutputReturnVersion.AddDataReturnVersion(dataReturnVersion);

            var registeredProducer = new RegisteredProducer("PRN", 2015, scheme);

            // Act, Assert
            Assert.Throws<InvalidOperationException>(() => eeeOutputReturnVersion.AddEeeOutputAmount(
                new EeeOutputAmount(ObligationType.B2C, WeeeCategory.AutomaticDispensers, 100, registeredProducer)));
        }

        [Fact]
        public void AddDataReturnVersion_WithDataReturnVersionForAnotherScheme_ThrowsInvalidOperationException()
        {
            // Arrange
            var scheme1 = A.Fake<Scheme>();
            A.CallTo(() => scheme1.ApprovalNumber)
                .Returns("WEE/SC1000SCH");

            var scheme2 = A.Fake<Scheme>();
            A.CallTo(() => scheme2.ApprovalNumber)
                .Returns("WEE/SC2000SCH");

            var eeeOutputReturnVersion = new EeeOutputReturnVersion();

            var registeredProducer = new RegisteredProducer("PRN", 2016, scheme2);
            eeeOutputReturnVersion.AddEeeOutputAmount(
                new EeeOutputAmount(ObligationType.B2C, WeeeCategory.AutomaticDispensers, 100, registeredProducer));

            var dataReturn = new DataReturn(scheme1, new Quarter(2016, QuarterType.Q2));
            var dataReturnVersion = new DataReturnVersion(dataReturn);

            // Act, Assert
            Assert.Throws<InvalidOperationException>(() => eeeOutputReturnVersion.AddDataReturnVersion(dataReturnVersion));
        }

        [Fact]
        public void AddDataReturnVersion_WithDataReturnVersionForAnotherComplianceYear_ThrowsInvalidOperationException()
        {
            // Arrange
            var scheme = A.Fake<Scheme>();
            A.CallTo(() => scheme.ApprovalNumber)
                .Returns("WEE/SC1000SCH");

            var eeeOutputReturnVersion = new EeeOutputReturnVersion();

            var registeredProducer = new RegisteredProducer("PRN", 2016, scheme);
            eeeOutputReturnVersion.AddEeeOutputAmount(
                new EeeOutputAmount(ObligationType.B2C, WeeeCategory.AutomaticDispensers, 100, registeredProducer));

            var dataReturn = new DataReturn(scheme, new Quarter(2015, QuarterType.Q2));
            var dataReturnVersion = new DataReturnVersion(dataReturn);

            // Act, Assert
            Assert.Throws<InvalidOperationException>(() => eeeOutputReturnVersion.AddDataReturnVersion(dataReturnVersion));
        }
    }
}
