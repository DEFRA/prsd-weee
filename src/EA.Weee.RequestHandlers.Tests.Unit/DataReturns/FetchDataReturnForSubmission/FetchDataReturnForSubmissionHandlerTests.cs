namespace EA.Weee.RequestHandlers.Tests.Unit.DataReturns.FetchDataReturnForSubmission
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Core.DataReturns;
    using Domain;
    using Domain.DataReturns;
    using Domain.Scheme;
    using FakeItEasy;
    using RequestHandlers.DataReturns.FetchDataReturnForSubmission;
    using RequestHandlers.Security;
    using Xunit;

    public class FetchDataReturnForSubmissionHandlerTests
    {
        /// <summary>
        /// This test ensures that data return errors with a level of "Fatal" are presented in
        /// the DataReturnForSubmission DTO as "errors" rather than "warnings".
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task HandleAsync_ForDataReturnWithOneErrorWithErrorLevelFatal_ReturnsDtoWithOneError()
        {
            // Arrange
            DataReturnsUploadError dataReturnsUploadError = new DataReturnsUploadError(
                ErrorLevel.Fatal,
                UploadErrorType.Business,
                A<string>._);

            DataReturnsUpload dataReturnsUpload = new DataReturnsUpload(
                A.Dummy<string>(),
                new List<DataReturnsUploadError>() { dataReturnsUploadError },
                null,
                A.Dummy<Scheme>(),
                A.Dummy<string>());

            IFetchDataReturnForSubmissionDataAccess dataAccess = A.Fake<IFetchDataReturnForSubmissionDataAccess>();
            A.CallTo(() => dataAccess.FetchDataReturnAsync(A<Guid>._)).Returns(dataReturnsUpload);

            FetchDataReturnForSubmissionHandler handler = new FetchDataReturnForSubmissionHandler(
                A.Dummy<IWeeeAuthorization>(),
                dataAccess);

            Requests.DataReturns.FetchDataReturnForSubmission request = new Requests.DataReturns.FetchDataReturnForSubmission(
                A.Dummy<Guid>());

            // Act
            DataReturnForSubmission result = await handler.HandleAsync(request);

            // Assert
            Assert.Equal(1, result.Errors.Count);
        }

        /// <summary>
        /// This test ensures that data return errors with a level of "Error" are presented in
        /// the DataReturnForSubmission DTO as "errors" rather than "warnings".
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task HandleAsync_ForDataReturnWithOneErrorWithErrorLevelError_ReturnsDtoWithOneError()
        {
            // Arrange
            DataReturnsUploadError dataReturnsUploadError = new DataReturnsUploadError(
                ErrorLevel.Error,
                UploadErrorType.Business,
                A<string>._);

            DataReturnsUpload dataReturnsUpload = new DataReturnsUpload(
                A.Dummy<string>(),
                new List<DataReturnsUploadError>() { dataReturnsUploadError },
                null,
                A.Dummy<Scheme>(),
                A.Dummy<string>());

            IFetchDataReturnForSubmissionDataAccess dataAccess = A.Fake<IFetchDataReturnForSubmissionDataAccess>();
            A.CallTo(() => dataAccess.FetchDataReturnAsync(A<Guid>._)).Returns(dataReturnsUpload);

            FetchDataReturnForSubmissionHandler handler = new FetchDataReturnForSubmissionHandler(
                A.Dummy<IWeeeAuthorization>(),
                dataAccess);

            Requests.DataReturns.FetchDataReturnForSubmission request = new Requests.DataReturns.FetchDataReturnForSubmission(
                A.Dummy<Guid>());

            // Act
            DataReturnForSubmission result = await handler.HandleAsync(request);

            // Assert
            Assert.Equal(1, result.Errors.Count);
        }

        /// <summary>
        /// This test ensures that data return errors with a level of "Warning" are presented in
        /// the DataReturnForSubmission DTO as "warnings" rather than "errors".
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task HandleAsync_ForDataReturnWithOneErrorWithErrorLevelWarning_ReturnsDtoWithOneWarning()
        {
            // Arrange
            DataReturnsUploadError dataReturnsUploadError = new DataReturnsUploadError(
                ErrorLevel.Warning,
                UploadErrorType.Business,
                A<string>._);

            DataReturnsUpload dataReturnsUpload = new DataReturnsUpload(
                A.Dummy<string>(),
                new List<DataReturnsUploadError>() { dataReturnsUploadError },
                null,
                A.Dummy<Scheme>(),
                A.Dummy<string>());

            IFetchDataReturnForSubmissionDataAccess dataAccess = A.Fake<IFetchDataReturnForSubmissionDataAccess>();
            A.CallTo(() => dataAccess.FetchDataReturnAsync(A<Guid>._)).Returns(dataReturnsUpload);

            FetchDataReturnForSubmissionHandler handler = new FetchDataReturnForSubmissionHandler(
                A.Dummy<IWeeeAuthorization>(),
                dataAccess);

            Requests.DataReturns.FetchDataReturnForSubmission request = new Requests.DataReturns.FetchDataReturnForSubmission(
                A.Dummy<Guid>());

            // Act
            DataReturnForSubmission result = await handler.HandleAsync(request);

            // Assert
            Assert.Equal(1, result.Warnings.Count);
        }

        /// <summary>
        /// This test ensures that data return errors with a level of "Debug" are presented in
        /// the DataReturnForSubmission DTO as "warnings" rather than "errors".
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task HandleAsync_ForDataReturnWithOneErrorWithErrorLevelDebug_ReturnsDtoWithOneWarning()
        {
            // Arrange
            DataReturnsUploadError dataReturnsUploadError = new DataReturnsUploadError(
                ErrorLevel.Debug,
                UploadErrorType.Business,
                A<string>._);

            DataReturnsUpload dataReturnsUpload = new DataReturnsUpload(
                A.Dummy<string>(),
                new List<DataReturnsUploadError>() { dataReturnsUploadError },
                null,
                A.Dummy<Scheme>(),
                A.Dummy<string>());

            IFetchDataReturnForSubmissionDataAccess dataAccess = A.Fake<IFetchDataReturnForSubmissionDataAccess>();
            A.CallTo(() => dataAccess.FetchDataReturnAsync(A<Guid>._)).Returns(dataReturnsUpload);

            FetchDataReturnForSubmissionHandler handler = new FetchDataReturnForSubmissionHandler(
                A.Dummy<IWeeeAuthorization>(),
                dataAccess);

            Requests.DataReturns.FetchDataReturnForSubmission request = new Requests.DataReturns.FetchDataReturnForSubmission(
                A.Dummy<Guid>());

            // Act
            DataReturnForSubmission result = await handler.HandleAsync(request);

            // Assert
            Assert.Equal(1, result.Warnings.Count);
        }

        /// <summary>
        /// This test ensures that data return errors with a level of "Info" are presented in
        /// the DataReturnForSubmission DTO as "warnings" rather than "errors".
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task HandleAsync_ForDataReturnWithOneErrorWithErrorLevelInfo_ReturnsDtoWithOneWarning()
        {
            // Arrange
            DataReturnsUploadError dataReturnsUploadError = new DataReturnsUploadError(
                ErrorLevel.Info,
                UploadErrorType.Business,
                A<string>._);

            DataReturnsUpload dataReturnsUpload = new DataReturnsUpload(
                A.Dummy<string>(),
                new List<DataReturnsUploadError>() { dataReturnsUploadError },
                null,
                A.Dummy<Scheme>(),
                A.Dummy<string>());

            IFetchDataReturnForSubmissionDataAccess dataAccess = A.Fake<IFetchDataReturnForSubmissionDataAccess>();
            A.CallTo(() => dataAccess.FetchDataReturnAsync(A<Guid>._)).Returns(dataReturnsUpload);

            FetchDataReturnForSubmissionHandler handler = new FetchDataReturnForSubmissionHandler(
                A.Dummy<IWeeeAuthorization>(),
                dataAccess);

            Requests.DataReturns.FetchDataReturnForSubmission request = new Requests.DataReturns.FetchDataReturnForSubmission(
                A.Dummy<Guid>());

            // Act
            DataReturnForSubmission result = await handler.HandleAsync(request);

            // Assert
            Assert.Equal(1, result.Warnings.Count);
        }

        /// <summary>
        /// This test ensures that data return errors with a level of "Trace" are presented in
        /// the DataReturnForSubmission DTO as "warnings" rather than "errors".
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task HandleAsync_ForDataReturnWithOneErrorWithErrorLevelTrace_ReturnsDtoWithOneWarning()
        {
            // Arrange
            DataReturnsUploadError dataReturnsUploadError = new DataReturnsUploadError(
                ErrorLevel.Trace,
                UploadErrorType.Business,
                A<string>._);

            DataReturnsUpload dataReturnsUpload = new DataReturnsUpload(
                A.Dummy<string>(),
                new List<DataReturnsUploadError>() { dataReturnsUploadError },
                null,
                A.Dummy<Scheme>(),
                A.Dummy<string>());

            IFetchDataReturnForSubmissionDataAccess dataAccess = A.Fake<IFetchDataReturnForSubmissionDataAccess>();
            A.CallTo(() => dataAccess.FetchDataReturnAsync(A<Guid>._)).Returns(dataReturnsUpload);

            FetchDataReturnForSubmissionHandler handler = new FetchDataReturnForSubmissionHandler(
                A.Dummy<IWeeeAuthorization>(),
                dataAccess);

            Requests.DataReturns.FetchDataReturnForSubmission request = new Requests.DataReturns.FetchDataReturnForSubmission(
                A.Dummy<Guid>());

            // Act
            DataReturnForSubmission result = await handler.HandleAsync(request);

            // Assert
            Assert.Equal(1, result.Warnings.Count);
        }

        [Fact]
        public async Task HandleAsync_ForDataReturnWithMultipleErrors_ReturnsErrorsByLineNumberOrder()
        {
            // Arrange
            DataReturnsUpload dataReturnsUpload = new DataReturnsUpload(
                A.Dummy<string>(),
                new List<DataReturnsUploadError>()
                {
                  new DataReturnsUploadError(ErrorLevel.Error, UploadErrorType.Schema, "Error on 55 line no", 55),
                  new DataReturnsUploadError(ErrorLevel.Error, UploadErrorType.Schema, "Error on 5 line no", 5),
                  new DataReturnsUploadError(ErrorLevel.Error, UploadErrorType.Schema, "Error on 75 line no", 75),
                  new DataReturnsUploadError(ErrorLevel.Error, UploadErrorType.Schema, "Error without line no")
                },
                null,
                A.Dummy<Scheme>(),
                A.Dummy<string>());

            IFetchDataReturnForSubmissionDataAccess dataAccess = A.Fake<IFetchDataReturnForSubmissionDataAccess>();
            A.CallTo(() => dataAccess.FetchDataReturnAsync(A<Guid>._)).Returns(dataReturnsUpload);

            FetchDataReturnForSubmissionHandler handler = new FetchDataReturnForSubmissionHandler(
                A.Dummy<IWeeeAuthorization>(),
                dataAccess);

            Requests.DataReturns.FetchDataReturnForSubmission request = new Requests.DataReturns.FetchDataReturnForSubmission(
                A.Dummy<Guid>());

            // Act
            DataReturnForSubmission result = await handler.HandleAsync(request);

            // Assert
            Assert.Equal(4, result.Errors.Count);
            Assert.True(result.Errors.ElementAt(0).Description == "Error without line no"
                && result.Errors.ElementAt(1).Description == "Error on 5 line no"
                && result.Errors.ElementAt(2).Description == "Error on 55 line no"
                && result.Errors.ElementAt(3).Description == "Error on 75 line no");
        }
    }
}
