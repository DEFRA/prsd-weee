namespace EA.Weee.Domain.Tests.Unit.DataReturns
{
    using System;
    using Domain.DataReturns;
    using Domain.Producer;
    using Domain.Scheme;
    using FakeItEasy;
    using Lookup;
    using Xunit;

    public class DataReturnVersionTests
    {
        [Fact]
        public void Submit_WhenNotYetSubmitted_MarksDataReturnVersionAsSubmitted()
        {
            // Arrange
            var dataReturn = new DataReturn(A.Dummy<Scheme>(), A.Dummy<Quarter>());
            var dataReturnVersion = new DataReturnVersion(dataReturn);

            // Act
            dataReturnVersion.Submit("test@co.uk");

            // Assert
            Assert.True(dataReturnVersion.IsSubmitted);
            Assert.Equal(dataReturnVersion.SubmittingUserId, "test@co.uk");
            Assert.Equal(dataReturnVersion.DataReturn.Id, dataReturn.Id);           
        }

        [Fact]
        public void Submit_WhenAlreadySubmitted_ThrowInvalidOperationException()
        {
            // Arrange
            var dataReturn = new DataReturn(A.Dummy<Scheme>(), A.Dummy<Quarter>());
            var dataReturnVersion = new DataReturnVersion(dataReturn);

            // Act
            dataReturnVersion.Submit("test@co.uk");

            // Act
            Action action = () => dataReturnVersion.Submit("test@co.uk");

            // Assert
            Assert.Throws<InvalidOperationException>(action);
        }
                   
        [Fact]
        public void ConstructsDataReturnVersion_WithNullDataReturn_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new DataReturnVersion(null));
        }

        [Fact]
        public void AddEeeOutputAmount_AddsToEeeOutputAmounts()
        {
            // Arrange
            var returnVersion = new DataReturnVersion(A.Fake<DataReturn>());
            var eeeOutputAmount = new EeeOutputAmount(ObligationType.B2B, A<WeeeCategory>._, A<decimal>._, A.Fake<RegisteredProducer>(), A.Fake<DataReturnVersion>());

            // Act
            returnVersion.AddEeeOutputAmount(eeeOutputAmount);

            // Assert
            Assert.Contains(eeeOutputAmount, returnVersion.EeeOutputAmounts);
        }

        [Fact]
        public void AddWeeeCollectedReturnVersion_AddsToWeeeCollectedReturnVersion()
        {
            // Arrange
            var returnVersion = new DataReturnVersion(A.Fake<DataReturn>());            
            WeeeCollectedReturnVersion collectedReturnVersion = new WeeeCollectedReturnVersion();
            
            WeeeCollectedAmount amount = new WeeeCollectedAmount(A<WeeeCollectedAmountSourceType>._, ObligationType.B2B, A<WeeeCategory>._, A<decimal>._);
            collectedReturnVersion.AddWeeeCollectedAmount(amount);

            // Act
            returnVersion.AddWeeeCollectedReturnVersion(collectedReturnVersion);           

            // Assert
            Assert.Contains(amount, collectedReturnVersion.WeeeCollectedAmounts);
        }

        [Fact]
        public void AddAatfDeliveryLocation_AddsToAatfDeliveryLocations()
        {
            // Arrange
            var returnVersion = new DataReturnVersion(A.Fake<DataReturn>());
            var deliveredAmount = new AatfDeliveredAmount(ObligationType.B2B, A<WeeeCategory>._, A<decimal>._, A.Fake<AatfDeliveryLocation>(), A.Fake<DataReturnVersion>());

            // Act
            returnVersion.AddAatfDeliveredAmount(deliveredAmount);

            // Assert
            Assert.Contains(deliveredAmount, returnVersion.AatfDeliveredAmounts);
        }

        [Fact]
        public void AddAeDeliveryLocation_AddsToAeDeliveryLocations()
        {
            // Arrange
            var returnVersion = new DataReturnVersion(A.Fake<DataReturn>());
            var deliveredAmount = new AeDeliveredAmount(ObligationType.B2B, A<WeeeCategory>._, A<decimal>._, A.Fake<AeDeliveryLocation>(), A.Fake<DataReturnVersion>());

            // Act
            returnVersion.AddAeDeliveredAmount(deliveredAmount);

            // Assert
            Assert.Contains(deliveredAmount, returnVersion.AeDeliveredAmounts);
        }
    }
}
