namespace EA.Weee.RequestHandlers.Tests.Unit.Scheme.MemberRegistration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain.Scheme;
    using FakeItEasy;
    using Helpers;
    using RequestHandlers.Scheme.MemberRegistration;
    using Requests.Scheme.MemberRegistration;
    using Xunit;

    public class MemberUploadSubmissionHandlerTests
    {
        private readonly DbContextHelper helper = new DbContextHelper();

        private readonly Guid pcsId = Guid.NewGuid();

        private MemberUploadSubmissionHandler GetPreparedHandler(IEnumerable<MemberUpload> memberUploads)
        {
            var memberUploadsDbSet = helper.GetAsyncEnabledDbSet(memberUploads);

            var context = A.Fake<WeeeContext>();

            A.CallTo(() => context.MemberUploads).Returns(memberUploadsDbSet);

            return new MemberUploadSubmissionHandler(context);
        }

        [Fact]
        public async Task MemberUploadSubmissionHandler_InvalidMemberUploadId_ArgumentNullException()
        {
            var memberUploads = new[]
            {
                new MemberUpload(pcsId, "Test data", new List<MemberUploadError>(), 0, 2016,  Guid.NewGuid())
            };

            var handler = GetPreparedHandler(memberUploads);

            await
                Assert.ThrowsAsync<ArgumentNullException>(
                    async () => await handler.HandleAsync(new MemberUploadSubmission(Guid.NewGuid())));
        }

        [Fact]
        public async Task MemberUploadSubmissionHandler_ValidMemberUploadId_ReturnsSubmittedMemberUploadId()
        {
            var memberUploads = new[]
            {
                new MemberUpload(pcsId, "Test data", new List<MemberUploadError>(), 0, 2016, Guid.NewGuid())
            };

            var handler = GetPreparedHandler(memberUploads);

            var memberUploadId = await handler.HandleAsync(new MemberUploadSubmission(memberUploads.First().Id));

            Assert.NotNull(memberUploadId);
            Assert.Equal(memberUploadId, memberUploads.First().Id);
        }

        [Fact]
        public async Task MemberUploadSubmissionHandler_ValidMemberUploadIdAlreadySubmitted_ReturnsAlreadySubmittedMemberUploadId()
        {
            var memberUpload = new MemberUpload(pcsId, "Test data", new List<MemberUploadError>(), 0, 2016, Guid.NewGuid());

            memberUpload.Submit();

            var handler = GetPreparedHandler(new[]
            {
                memberUpload
            });

            var memberUploadId = await handler.HandleAsync(new MemberUploadSubmission(memberUpload.Id));

            Assert.NotNull(memberUploadId);
            Assert.Equal(memberUploadId, memberUpload.Id);
        }
    }
}
