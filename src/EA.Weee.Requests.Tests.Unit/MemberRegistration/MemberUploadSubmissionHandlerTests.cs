namespace EA.Weee.Requests.Tests.Unit.MemberRegistration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Domain.PCS;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain;
    using EA.Weee.RequestHandlers.Mappings;
    using EA.Weee.RequestHandlers.PCS.MemberRegistration;
    using EA.Weee.Requests.PCS.MemberRegistration;
    using EA.Weee.Requests.Tests.Unit.Helpers;
    using FakeItEasy;
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
            var memberUploads = new[] { new MemberUpload(pcsId, "Test data", new List<MemberUploadError>(), Guid.NewGuid()), };

            var handler = GetPreparedHandler(memberUploads);

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await handler.HandleAsync(new MemberUploadSubmission(Guid.NewGuid())));
        }

        [Fact]
        public async Task MemberUploadSubmissionHandler_ValidMemberUploadId_ReturnsSubmittedMemberUploadId()
        {
            var memberUploads = new[] { new MemberUpload(pcsId, "Test data", new List<MemberUploadError>(), Guid.NewGuid()), };

            var handler = GetPreparedHandler(memberUploads);

            var memberUploadId = await handler.HandleAsync(new MemberUploadSubmission(memberUploads.First().Id));

            Assert.NotNull(memberUploadId);
            Assert.Equal(memberUploadId, memberUploads.First().Id);
        }
    }
}
