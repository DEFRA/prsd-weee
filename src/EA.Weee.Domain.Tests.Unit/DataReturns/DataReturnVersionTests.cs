namespace EA.Weee.Domain.Tests.Unit.DataReturns
{
    using System;
    using Domain.DataReturns;
    using Domain.Scheme;
    using FakeItEasy;
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
    }
}
