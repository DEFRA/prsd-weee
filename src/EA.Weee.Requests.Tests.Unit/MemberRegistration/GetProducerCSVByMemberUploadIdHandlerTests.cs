namespace EA.Weee.Requests.Tests.Unit.MemberRegistration
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain.PCS;
    using FakeItEasy;
    using Helpers;
    using PCS.MemberRegistration;
    using RequestHandlers.PCS.MemberRegistration;
    using Xunit;

    public class GetProducerCSVByMemberUploadIdHandlerTests
    {
        private readonly DbContextHelper helper = new DbContextHelper();

        private readonly Guid pcsId = Guid.NewGuid();

        private GetProducerCSVByMemberUploadIdHandler GetPreparedHandler(IEnumerable<MemberUpload> memberUploads)
        {
            var memberUploadsDbSet = helper.GetAsyncEnabledDbSet(memberUploads);

            var context = A.Fake<WeeeContext>();

            A.CallTo(() => context.MemberUploads).Returns(memberUploadsDbSet);

            return new GetProducerCSVByMemberUploadIdHandler(context);
        }

        [Fact]
        public async Task GetProducerCSVByMemberUploadIdHandler_InvalidMemberUploadId_ArgumentException()
        {
            var memberUploads = new[]
            {
                new MemberUpload(pcsId, "Test data", new List<MemberUploadError>(), 0, Guid.NewGuid())
            };

            var handler = GetPreparedHandler(memberUploads);

            await
                Assert.ThrowsAsync<ArgumentException>(
                    async () => await handler.HandleAsync(new GetProducerCSVByMemberUploadId(Guid.NewGuid())));
        }
    }
}
