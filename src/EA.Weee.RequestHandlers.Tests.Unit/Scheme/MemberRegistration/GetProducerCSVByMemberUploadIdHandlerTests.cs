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
    using RequestHandlers.Scheme.MemberRegistration;
    using Requests.Scheme.MemberRegistration;
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

            return new GetProducerCSVByMemberUploadIdHandler(AuthorizationBuilder.CreateUserAllowedToAccessOrganisation(), context);
        }

        [Fact]
        public async Task GetProducerCSVByMemberUploadIdHandler_MemberUploadNotOwnedByOrg_ThrowsArgumentException()
        {
            var memberUploads = new[]
            {
                new MemberUpload(pcsId, "Test data", new List<MemberUploadError>(), 0, 2016, Guid.NewGuid())
            };
            var handler = GetPreparedHandler(memberUploads);

            var message = new GetProducerCSVByMemberUploadId(Guid.NewGuid(), memberUploads.First().Id);

            await Assert.ThrowsAsync<ArgumentException>(async () => await handler.HandleAsync(message));
        }

        [Fact]
        public async Task GetProducerCSVByMemberUploadIdHandler_NotOrganisationUser_ThrowsSecurityException()
        {
            var denyingAuthorization = AuthorizationBuilder.CreateUserDeniedFromAccessingOrganisation();

            var handler = new GetProducerCSVByMemberUploadIdHandler(denyingAuthorization, A<WeeeContext>._);
            var message = new GetProducerCSVByMemberUploadId(Guid.NewGuid(), Guid.NewGuid());

            await Assert.ThrowsAsync<SecurityException>(async () => await handler.HandleAsync(message));
        }

        [Fact]
        public async Task GetProducerCSVByMemberUploadIdHandler_InvalidMemberUploadId_ArgumentException()
        {
            var memberUploads = new[]
            {
                new MemberUpload(pcsId, "Test data", new List<MemberUploadError>(), 0, 2016, Guid.NewGuid())
            };

            var handler = GetPreparedHandler(memberUploads);

            await
                Assert.ThrowsAsync<ArgumentException>(
                    async () => await handler.HandleAsync(new GetProducerCSVByMemberUploadId(Guid.NewGuid(), Guid.NewGuid())));
        }
    }
}
