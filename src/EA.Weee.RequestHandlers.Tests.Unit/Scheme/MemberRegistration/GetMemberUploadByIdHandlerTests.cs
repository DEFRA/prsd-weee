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
    using Mappings;
    using RequestHandlers.Scheme.MemberRegistration;
    using Requests.Scheme.MemberRegistration;
    using Weee.Tests.Core;
    using Xunit;

    public class GetMemberUploadByIdHandlerTests
    {
        private readonly DbContextHelper helper = new DbContextHelper();

        private readonly Guid pcsId = Guid.NewGuid();

        private GetMemberUploadByIdHandler GetPreparedHandler(IEnumerable<MemberUpload> memberUploads)
        {
            var memberUploadsDbSet = helper.GetAsyncEnabledDbSet(memberUploads);

            var context = A.Fake<WeeeContext>();

            A.CallTo(() => context.MemberUploads).Returns(memberUploadsDbSet);

            return new GetMemberUploadByIdHandler(AuthorizationBuilder.CreateUserAllowedToAccessOrganisation(), context, new MemberUploadMap());
        }

        [Fact]
        public async Task GetMemberUploadByIdHandler_NotOrganisationUser_ThrowsSecurityException()
        {
            var authorization = AuthorizationBuilder.CreateUserDeniedFromAccessingOrganisation();

            var handler = new GetMemberUploadByIdHandler(authorization, A.Dummy<WeeeContext>(), A.Dummy<MemberUploadMap>());
            var message = new GetMemberUploadById(Guid.NewGuid(), Guid.NewGuid());

            await Assert.ThrowsAsync<SecurityException>(async () => await handler.HandleAsync(message));
        }

        [Fact]
        public async Task GetMemberUploadByIdHandler_InvalidMemberUploadId_ArgumentNullException()
        {
            var memberUploads = new[]
            {
                new MemberUpload(pcsId, "Test data", new List<MemberUploadError>(), 0, 2016, A.Dummy<Scheme>(), "File name")
            };

            var handler = GetPreparedHandler(memberUploads);

            await
                Assert.ThrowsAsync<ArgumentNullException>(
                    async () => await handler.HandleAsync(new GetMemberUploadById(Guid.NewGuid(), Guid.NewGuid())));
        }

        [Fact]
        public async Task GetMemberUploadByIdHandler_MemberUploadNotOwnedByOrganisation_ThrowsArgumentException()
        {
            var memberUploads = new[]
            {
                new MemberUpload(pcsId, "Test data", new List<MemberUploadError>(), 0, 2016, A.Dummy<Scheme>(), "File name")
            };

            var otherPcsId = Guid.NewGuid();

            var handler = GetPreparedHandler(memberUploads);
            var message = new GetMemberUploadById(otherPcsId, memberUploads.First().Id);

            var exception = await Assert.ThrowsAsync<ArgumentException>(async () => await handler.HandleAsync(message));

            Assert.True(exception.Message.ToUpperInvariant().Contains("MEMBER UPLOAD"));
            Assert.True(exception.Message.Contains(otherPcsId.ToString()));
            Assert.True(exception.Message.Contains(memberUploads.First().Id.ToString()));
        }

        [Fact]
        public async Task GetMemberUploadByIdHandler_ValidMemberUploadId_ReturnsMemberUpload()
        {
            var memberUploads = new[]
            {
                new MemberUpload(pcsId, "Test data", new List<MemberUploadError>(), 0, 2016, A.Dummy<Scheme>(), "File name")
            };

            var handler = GetPreparedHandler(memberUploads);

            var memberUpload = await handler.HandleAsync(new GetMemberUploadById(pcsId, memberUploads.First().Id));

            Assert.NotNull(memberUpload);
        }
    }
}
