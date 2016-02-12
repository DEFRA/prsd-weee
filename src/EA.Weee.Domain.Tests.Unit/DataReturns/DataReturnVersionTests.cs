namespace EA.Weee.Domain.Tests.Unit.DataReturns
{
    using System;
    using System.Linq;
    using Domain.DataReturns;
    using Domain.Producer;
    using Domain.Scheme;
    using Events;
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
        public void Submit_AddSubmissionEventToEventsList()
        {
            // Arrange
            var dataReturn = new DataReturn(A.Dummy<Scheme>(), A.Dummy<Quarter>());
            var dataReturnVersion = new DataReturnVersion(dataReturn);

            // Act
            dataReturnVersion.Submit("test@co.uk");

            // Assert
            Assert.Equal(1, dataReturnVersion.Events.Count());
            Assert.IsType<SchemeDataReturnSubmissionEvent>(dataReturnVersion.Events.Single());
            Assert.Same(dataReturnVersion, ((SchemeDataReturnSubmissionEvent)dataReturnVersion.Events.Single()).DataReturnVersion);
        }

        [Fact]
        public void ConstructsDataReturnVersion_WithNullDataReturn_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new DataReturnVersion(null));
        }
    }
}
