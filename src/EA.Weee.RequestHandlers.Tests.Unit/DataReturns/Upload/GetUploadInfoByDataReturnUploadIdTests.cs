namespace EA.Weee.RequestHandlers.Tests.Unit.DataReturns.Upload
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain.DataReturns;
    using Domain.Scheme;
    using FakeItEasy;
    using RequestHandlers.DataReturns.Upload;
    using Xunit;

    public class GetUploadInfoByDataReturnUploadIdTests
    {
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

            GetUploadInfoByDataReturnUploadIdHandler handler = new GetUploadInfoByDataReturnUploadIdHandler(dataAccess);

            Requests.DataReturns.GetUploadInfoByDataReturnUploadId request = new Requests.DataReturns.GetUploadInfoByDataReturnUploadId(
                A.Dummy<Guid>());

            var result = await handler.HandleAsync(request);

            Assert.Equal(2016, result.Year);
            Assert.Equal(1, (int)result.Quarter.Value);
        }
    }
}
