namespace EA.Weee.RequestHandlers.Tests.Unit.DataReturns.FetchDataReturnForSubmission
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain.DataReturns;
    using Domain.Scheme;
    using FakeItEasy;
    using RequestHandlers.DataReturns.FetchDataReturnForSubmission;
    using Xunit;

    public class GetQuarterInfoByDataReturnUploadIdTests
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

            IFetchDataReturnForSubmissionDataAccess dataAccess = A.Fake<IFetchDataReturnForSubmissionDataAccess>();
            A.CallTo(() => dataAccess.FetchDataReturnUploadByIdAsync(A<Guid>._)).Returns(dataReturnsUpload);

            GetQuarterInfoByDataReturnUploadIdHandler handler = new GetQuarterInfoByDataReturnUploadIdHandler(dataAccess);

            Requests.DataReturns.GetQuarterInfoByDataReturnUploadId request = new Requests.DataReturns.GetQuarterInfoByDataReturnUploadId(
                A.Dummy<Guid>());

            var result = await handler.HandleAsync(request);

            Assert.Equal(2016, result.Year);
            Assert.Equal(1, (int)result.Quarter.Value);
        }
    }
}
