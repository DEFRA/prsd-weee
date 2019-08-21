﻿namespace EA.Weee.Domain.Tests.Unit.Scheme
{
    using Domain.DataReturns;
    using Domain.Scheme;
    using Error;
    using FakeItEasy;
    using System;
    using System.Collections.Generic;
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
        public void Submit_WhenContainsErrors_ThrowInvalidOperationException()
        {
            var error = new DataReturnUploadError(ErrorLevel.Error, A.Dummy<UploadErrorType>(), A.Dummy<string>());

            var dataReturnUpload = new DataReturnUpload(
                 A.Dummy<Scheme>(),
                 A.Dummy<string>(),
                 new List<DataReturnUploadError> { error },
                 A.Dummy<string>(),
                 A.Dummy<int>(),
                 A.Dummy<int>());

            dataReturnUpload.SetDataReturnVersion(new DataReturnVersion(A.Dummy<DataReturn>()));

            Assert.Throws<InvalidOperationException>(() => dataReturnUpload.Submit("test@co.uk"));
        }

        [Fact]
        public void Submit_WhenContainsErrorsAndWarnings_ThrowInvalidOperationException()
        {
            var error = new DataReturnUploadError(ErrorLevel.Error, A.Dummy<UploadErrorType>(), A.Dummy<string>());
            var warning = new DataReturnUploadError(ErrorLevel.Warning, A.Dummy<UploadErrorType>(), A.Dummy<string>());

            var dataReturnUpload = new DataReturnUpload(
                 A.Dummy<Scheme>(),
                 A.Dummy<string>(),
                 new List<DataReturnUploadError> { error, warning },
                 A.Dummy<string>(),
                 A.Dummy<int>(),
                 A.Dummy<int>());

            dataReturnUpload.SetDataReturnVersion(new DataReturnVersion(A.Dummy<DataReturn>()));

            Assert.Throws<InvalidOperationException>(() => dataReturnUpload.Submit("test@co.uk"));
        }

        [Fact]
        public void Submit_WhenContainsWarnings_SubmitsWithNoException()
        {
            var warning = new DataReturnUploadError(ErrorLevel.Warning, A.Dummy<UploadErrorType>(), A.Dummy<string>());

            var dataReturnUpload = new DataReturnUpload(
                 A.Dummy<Scheme>(),
                 A.Dummy<string>(),
                 new List<DataReturnUploadError> { warning },
                 A.Dummy<string>(),
                 A.Dummy<int>(),
                 A.Dummy<int>());

            dataReturnUpload.SetDataReturnVersion(new DataReturnVersion(A.Dummy<DataReturn>()));

            var exception = Record.Exception(() => dataReturnUpload.Submit("test@co.uk"));

            Assert.Null(exception);
            Assert.True(dataReturnUpload.DataReturnVersion.IsSubmitted);
        }

        [Fact]
        public void Submit_WhenContainsNoErrorsOrWarnings_SubmitsWithNoException()
        {
            var dataReturnUpload = new DataReturnUpload(
                 A.Dummy<Scheme>(),
                 A.Dummy<string>(),
                 null,
                 A.Dummy<string>(),
                 A.Dummy<int>(),
                 A.Dummy<int>());

            dataReturnUpload.SetDataReturnVersion(new DataReturnVersion(A.Dummy<DataReturn>()));

            var exception = Record.Exception(() => dataReturnUpload.Submit("test@co.uk"));

            Assert.Null(exception);
            Assert.True(dataReturnUpload.DataReturnVersion.IsSubmitted);
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
