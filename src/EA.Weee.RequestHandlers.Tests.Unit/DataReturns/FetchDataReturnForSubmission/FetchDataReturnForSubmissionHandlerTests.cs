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
            DataReturnUploadError dataReturnsUploadError = new DataReturnUploadError(
                ErrorLevel.Fatal,
                UploadErrorType.Business,
                A<string>._);

            IFetchDataReturnForSubmissionDataAccess dataAccess = FetchDummyDataReturn(dataReturnsUploadError);

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
            DataReturnUploadError dataReturnsUploadError = new DataReturnUploadError(
                ErrorLevel.Error,
                UploadErrorType.Business,
                A<string>._);

            IFetchDataReturnForSubmissionDataAccess dataAccess = FetchDummyDataReturn(dataReturnsUploadError);

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
            DataReturnUploadError dataReturnsUploadError = new DataReturnUploadError(
                ErrorLevel.Warning,
                UploadErrorType.Business,
                A<string>._);

            IFetchDataReturnForSubmissionDataAccess dataAccess = FetchDummyDataReturn(dataReturnsUploadError);

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
            DataReturnUploadError dataReturnsUploadError = new DataReturnUploadError(
                ErrorLevel.Debug,
                UploadErrorType.Business,
                A<string>._);

            IFetchDataReturnForSubmissionDataAccess dataAccess = FetchDummyDataReturn(dataReturnsUploadError);

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
            DataReturnUploadError dataReturnsUploadError = new DataReturnUploadError(
                ErrorLevel.Info,
                UploadErrorType.Business,
                A<string>._);

            IFetchDataReturnForSubmissionDataAccess dataAccess = FetchDummyDataReturn(dataReturnsUploadError);

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
            DataReturnUploadError dataReturnsUploadError = new DataReturnUploadError(
                ErrorLevel.Trace,
                UploadErrorType.Business,
                A<string>._);

            IFetchDataReturnForSubmissionDataAccess dataAccess = FetchDummyDataReturn(dataReturnsUploadError);

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

        private static IFetchDataReturnForSubmissionDataAccess FetchDummyDataReturn(DataReturnUploadError dataReturnsUploadError)
        {
            DataReturnUpload dataReturnsUpload = new DataReturnUpload(
                A.Dummy<Scheme>(),
                A.Dummy<string>(),
                new List<DataReturnUploadError>() { dataReturnsUploadError },
                A.Dummy<string>(),
                null,
                null,
                null);
            
            IFetchDataReturnForSubmissionDataAccess dataAccess = A.Fake<IFetchDataReturnForSubmissionDataAccess>();
            A.CallTo(() => dataAccess.FetchDataReturnUploadAsync(A<Guid>._)).Returns(dataReturnsUpload);
            return dataAccess;
        }
    }
}
