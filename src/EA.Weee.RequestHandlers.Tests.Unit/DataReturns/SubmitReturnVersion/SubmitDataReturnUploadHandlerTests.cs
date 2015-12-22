namespace EA.Weee.RequestHandlers.Tests.Unit.DataReturns.SubmitReturnVersion
{
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using Domain.DataReturns;
    using FakeItEasy;
    using RequestHandlers.DataReturns.FetchDataReturnForSubmission;
    using RequestHandlers.DataReturns.SubmitReturnVersion;
    using RequestHandlers.Security;
    using Requests.DataReturns;
    using Weee.Tests.Core;
    using Xunit;

    public class SubmitDataReturnUploadHandlerTests
    {
        [Fact]
        public async Task HandleAsync_NoAccessToScheme_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenySchemeAccess().Build();

            var handler = new SubmitDataReturnUploadHandler(authorization,
                A.Dummy<ISubmitReturnVersionDataAccess>(),
                A.Dummy<IFetchDataReturnForSubmissionDataAccess>());

            await Assert.ThrowsAsync<SecurityException>(() => handler.HandleAsync(A.Dummy<SubmitDataReturnUpload>()));
        }

        [Fact]
        public async Task HandleAsync_SubmitsAssociatedDataReturnVersion()
        {
            var submitReturnVersionDataAccess = A.Fake<ISubmitReturnVersionDataAccess>();
            var fetchDataReturnForSubmissionDataAccess = A.Fake<IFetchDataReturnForSubmissionDataAccess>();

            var dataReturnVersion = A.Dummy<DataReturnVersion>();

            var upload = A.Fake<DataReturnUpload>();
            A.CallTo(() => upload.DataReturnVersion)
                .Returns(dataReturnVersion);

            A.CallTo(() => fetchDataReturnForSubmissionDataAccess.FetchDataReturnUploadAsync(A<Guid>._))
                .Returns(upload);

            var handler = new SubmitDataReturnUploadHandler(A.Dummy<IWeeeAuthorization>(), submitReturnVersionDataAccess, fetchDataReturnForSubmissionDataAccess);

            await handler.HandleAsync(A.Dummy<SubmitDataReturnUpload>());

            A.CallTo(() => submitReturnVersionDataAccess.Submit(dataReturnVersion))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
