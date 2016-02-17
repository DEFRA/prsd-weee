namespace EA.Weee.RequestHandlers.Tests.Unit.DataReturns.Upload
{
    using System;
    using System.Collections.Generic;
    using System.Security;
    using System.Threading.Tasks;
    using Domain.DataReturns;
    using Domain.Scheme;
    using FakeItEasy;
    using RequestHandlers.DataReturns.Upload;
    using RequestHandlers.Security;
    using Weee.Tests.Core;
    using Xunit;

    public class GetUploadInfoByDataReturnUploadIdTests
    {
        [Fact]
        public async Task HandleAsync_NoAccessToScheme_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenySchemeAccess().Build();

            var handler = new GetUploadInfoByDataReturnUploadIdHandler(authorization, A.Dummy<IFetchDataReturnUploadDataAccess>());

            await Assert.ThrowsAsync<SecurityException>(() => handler.HandleAsync(A.Dummy<Requests.DataReturns.GetUploadInfoByDataReturnUploadId>()));
        }

        [Fact]
        public async Task HandleAsync_Happypath_ReturnsQuarterInfo()
        {
            DataReturnUpload dataReturnsUpload = new DataReturnUpload(
               A.Dummy<Scheme>(),
               A.Dummy<string>(),
               new List<DataReturnUploadError>(),
               A.Dummy<string>(),
               2016,
               1);
          
            IFetchDataReturnUploadDataAccess dataAccess = A.Fake<IFetchDataReturnUploadDataAccess>();
            A.CallTo(() => dataAccess.FetchDataReturnUploadByIdAsync(A<Guid>._)).Returns(dataReturnsUpload);

            GetUploadInfoByDataReturnUploadIdHandler handler = new GetUploadInfoByDataReturnUploadIdHandler(A.Dummy<IWeeeAuthorization>(), dataAccess);

            Requests.DataReturns.GetUploadInfoByDataReturnUploadId request = new Requests.DataReturns.GetUploadInfoByDataReturnUploadId(
                A.Dummy<Guid>());

            var result = await handler.HandleAsync(request);

            Assert.Equal(2016, result.Year);
            Assert.Equal(1, (int)result.Quarter.Value);
        }
    }
}
