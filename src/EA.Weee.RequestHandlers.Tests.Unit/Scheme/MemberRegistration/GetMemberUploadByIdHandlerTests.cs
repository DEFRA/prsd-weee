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
    using Helpers;
    using Mappings;
    using RequestHandlers.Scheme.MemberRegistration;
    using RequestHandlers.Security;
    using Requests.Scheme.MemberRegistration;
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

            return new GetMemberUploadByIdHandler(AuthorizationBuilder.CreateUserWithAllRights(), context, new MemberUploadMap());
        }

        [Fact]
        public async Task GetMemberUploadByIdHandler_NotOrganisationUser_ThrowsSecurityException()
        {
            var authorization = AuthorizationBuilder.CreateUserWithNoRights();

            var handler = new GetMemberUploadByIdHandler(authorization, A<WeeeContext>._, A<MemberUploadMap>._);
            var message = new GetMemberUploadById(Guid.NewGuid());

            await Assert.ThrowsAsync<SecurityException>(async () => await handler.HandleAsync(message));
        }

        [Fact]
        public async Task GetMemberUploadByIdHandler_InvalidMemberUploadId_ArgumentNullException()
        {
            var memberUploads = new[]
            {
                new MemberUpload(pcsId, "Test data", new List<MemberUploadError>(), 0, 2016, Guid.NewGuid())
            };

            var handler = GetPreparedHandler(memberUploads);

            await
                Assert.ThrowsAsync<ArgumentNullException>(
                    async () => await handler.HandleAsync(new GetMemberUploadById(Guid.NewGuid())));
        }

        [Fact]
        public async Task GetMemberUploadByIdHandler_ValidMemberUploadId_ReturnsMemberUpload()
        {
            var memberUploads = new[]
            {
                new MemberUpload(pcsId, "Test data", new List<MemberUploadError>(), 0, 2016, Guid.NewGuid())
            };

            var handler = GetPreparedHandler(memberUploads);

            var memberUpload = await handler.HandleAsync(new GetMemberUploadById(memberUploads.First().Id));

            Assert.NotNull(memberUpload);
        }
    }
}
