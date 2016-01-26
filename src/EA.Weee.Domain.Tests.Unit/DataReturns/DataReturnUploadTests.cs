namespace EA.Weee.Domain.Tests.Unit.Scheme
{
    using System;
    using System.Collections.Generic;
    using Domain.DataReturns;
    using Domain.Scheme;
    using FakeItEasy;
    using Xunit;

    public class DataReturnUploadTests
    {
        [Fact]
        public void DataReturnUpload_SchemeNotDefined_ThrowsArugmentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new DataReturnUpload(
                 null,
                 A.Dummy<string>(),
                 A.Dummy<List<DataReturnUploadError>>(),
                 A.Dummy<string>(),
                 A.Dummy<int>(),
                 A.Dummy<int>()));
        }

        [Fact]
        public void Submit_WhenAlreadySubmitted_ThrowInvalidOperationException()
        {
            // Arrange
            var dataReturnUpload = new DataReturnUpload(
                 A.Dummy<Scheme>(),
                 A.Dummy<string>(),
                 A.Dummy<List<DataReturnUploadError>>(),
                 A.Dummy<string>(),
                 A.Dummy<int>(),
                 A.Dummy<int>());

            dataReturnUpload.SetDataReturnVersion(new DataReturnVersion(A.Dummy<DataReturn>()));
            dataReturnUpload.Submit("test@co.uk");

            // Act
            Action action = () => dataReturnUpload.Submit("test@co.uk");

            // Assert
            Assert.Throws<InvalidOperationException>(action);
        }

        [Fact]
        public void SetDataReturnVersion_ThrowsArugmentNullException()
        {
            // Arrange
            var dataReturnUpload = new DataReturnUpload(
                 A.Dummy<Scheme>(),
                 A.Dummy<string>(),
                 A.Dummy<List<DataReturnUploadError>>(),
                 A.Dummy<string>(),
                 A.Dummy<int>(),
                 A.Dummy<int>());

            // Act
            Action action = () => dataReturnUpload.SetDataReturnVersion(null);

            // Assert
            Assert.Throws<ArgumentNullException>(action);
        }

        [Fact]
        public void SetProcessTime_WhenCurrentValueIsZero_SetProcessTime()
        {
            // Arrange
            var dataReturnUpload = new DataReturnUpload(
                 A.Dummy<Scheme>(),
                 A.Dummy<string>(),
                 A.Dummy<List<DataReturnUploadError>>(),
                 A.Dummy<string>(),
                 A.Dummy<int>(),
                 A.Dummy<int>());

            // Act
            dataReturnUpload.SetProcessTime(TimeSpan.FromSeconds(15));

            // Assert
            Assert.Equal(TimeSpan.FromSeconds(15), dataReturnUpload.ProcessTime);
        }

        [Fact]
        public void SetProcessTime_WhenCurrentValueIsNotZero_ThrowInvalidOperationException()
        {
            // Arrange
            var dataReturnUpload = new DataReturnUpload(
                 A.Dummy<Scheme>(),
                 A.Dummy<string>(),
                 A.Dummy<List<DataReturnUploadError>>(),
                 A.Dummy<string>(),
                 A.Dummy<int>(),
                 A.Dummy<int>());

            dataReturnUpload.SetProcessTime(TimeSpan.FromSeconds(15));

            // Act
            Action action = () => dataReturnUpload.SetProcessTime(TimeSpan.FromSeconds(25));

            // Assert
            Assert.Throws<InvalidOperationException>(action);
        }
    }
}
