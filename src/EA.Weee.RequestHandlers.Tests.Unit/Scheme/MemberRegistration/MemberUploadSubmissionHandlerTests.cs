namespace EA.Weee.RequestHandlers.Tests.Unit.Scheme.MemberRegistration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain.Scheme;
    using FakeItEasy;
    using RequestHandlers.Scheme.MemberRegistration;
    using Requests.Scheme.MemberRegistration;
    using Weee.Tests.Core;
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

            return new MemberUploadSubmissionHandler(AuthorizationBuilder.CreateUserAllowedToAccessOrganisation(), context);
        }

        [Fact]
        public async Task MemberUploadSubmissionHandler_NotOrganisationUser_ThrowsSecurityException()
        {
            var denyingAuthorization = AuthorizationBuilder.CreateUserDeniedFromAccessingOrganisation();

            var handler = new MemberUploadSubmissionHandler(denyingAuthorization, A<WeeeContext>._);
            var message = new MemberUploadSubmission(Guid.NewGuid(), Guid.NewGuid());

            await Assert.ThrowsAsync<SecurityException>(async () => await handler.HandleAsync(message));
        }

        [Fact]
        public async Task MemberUploadSubmissionHandler_InvalidMemberUploadId_ArgumentNullException()
        {
            var memberUploads = new[]
            {
                new MemberUpload(pcsId, "Test data", new List<MemberUploadError>(), 0, 2016, Guid.NewGuid(), "File name")
            };

            var handler = GetPreparedHandler(memberUploads);

            await
                Assert.ThrowsAsync<ArgumentNullException>(
                    async () => await handler.HandleAsync(new MemberUploadSubmission(Guid.NewGuid(), Guid.NewGuid())));
        }

        [Fact]
        public async Task MemberUploadSubmissionHandler_MemberUploadNotOwnedByOrg_ThrowsArgumentException()
        {
            var memberUploads = new[]
            {
                new MemberUpload(pcsId, "Test data", new List<MemberUploadError>(), 0, 2016, Guid.NewGuid(), "File name")
            };

            var otherPcsId = Guid.NewGuid();

            var handler = GetPreparedHandler(memberUploads);
            var message = new MemberUploadSubmission(otherPcsId, memberUploads.First().Id);

            var exception = await Assert.ThrowsAsync<ArgumentException>(async () => await handler.HandleAsync(message));

            Assert.True(exception.Message.ToUpperInvariant().Contains("MEMBER UPLOAD"));
            Assert.True(exception.Message.Contains(otherPcsId.ToString()));
            Assert.True(exception.Message.Contains(memberUploads.First().Id.ToString()));
        }

        [Fact]
        public async Task MemberUploadSubmissionHandler_ValidMemberUploadId_ReturnsSubmittedMemberUploadId()
        {
            var memberUploads = new[]
            {
                new MemberUpload(pcsId, "Test data", new List<MemberUploadError>(), 0, 2016, Guid.NewGuid(), "File name")
            };

            var handler = GetPreparedHandler(memberUploads);

            var memberUploadId = await handler.HandleAsync(new MemberUploadSubmission(pcsId, memberUploads.First().Id));

            Assert.NotNull(memberUploadId);
            Assert.Equal(memberUploadId, memberUploads.First().Id);
        }

        [Fact]
        public async Task MemberUploadSubmissionHandler_ValidMemberUploadIdAlreadySubmitted_ReturnsAlreadySubmittedMemberUploadId()
        {
            var memberUpload = new MemberUpload(pcsId, "Test data", new List<MemberUploadError>(), 0, 2016, Guid.NewGuid(), "File name");

            memberUpload.Submit();

            var handler = GetPreparedHandler(new[]
            {
                memberUpload
            });

            var memberUploadId = await handler.HandleAsync(new MemberUploadSubmission(pcsId, memberUpload.Id));

            Assert.NotNull(memberUploadId);
            Assert.Equal(memberUploadId, memberUpload.Id);
        }
    }
}
