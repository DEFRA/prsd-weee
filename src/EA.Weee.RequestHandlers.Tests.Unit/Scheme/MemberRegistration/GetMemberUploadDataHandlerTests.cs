﻿namespace EA.Weee.RequestHandlers.Tests.Unit.Scheme.MemberRegistration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain;
    using Domain.Scheme;
    using FakeItEasy;
    using Mappings;
    using RequestHandlers.Scheme.MemberRegistration;
    using Requests.Scheme.MemberRegistration;
    using Weee.Tests.Core;
    using Xunit;

    public class GetMemberUploadDataHandlerTests
    {
        private readonly DbContextHelper helper = new DbContextHelper();

        private readonly Guid pcsId = Guid.NewGuid();
        private readonly Guid schemeId = Guid.NewGuid();

        [Fact]
        public async Task GetMemberUploadDataHandler_NotOrganisationUser_ThrowsSecurityException()
        {
            var denyingAuthorization = AuthorizationBuilder.CreateUserDeniedFromAccessingOrganisation();

            var handler = new GetMemberUploadDataHandler(denyingAuthorization, A<WeeeContext>._, A<MemberUploadErrorMap>._);
            var message = new GetMemberUploadData(Guid.NewGuid(), Guid.NewGuid());

            await Assert.ThrowsAsync<SecurityException>(async () => await handler.HandleAsync(message));
        }

        [Fact]
        public async Task GetMemberUploadDataHandler_WithSeveralErrors_MappedCorrectly()
        {
            var memberUploadsWithSeveralErrors = new[]
            {
                new MemberUpload(
                    pcsId,
                    "FAKE DATA",
                    new List<MemberUploadError>
                        {
                            new MemberUploadError(ErrorLevel.Warning, MemberUploadErrorType.Schema, "FAKE WARNING"),
                            new MemberUploadError(ErrorLevel.Error, MemberUploadErrorType.Business, "FAKE ERROR"),
                            new MemberUploadError(ErrorLevel.Error, MemberUploadErrorType.Schema, "FAKE ERROR"),
                            new MemberUploadError(ErrorLevel.Fatal, MemberUploadErrorType.Business, "FAKE FATAL"),
                            new MemberUploadError(ErrorLevel.Fatal, MemberUploadErrorType.Schema, "FAKE FATAL"),
                            new MemberUploadError(ErrorLevel.Fatal, MemberUploadErrorType.Business, "FAKE FATAL")
                        },
                    0,
                    2016,
                    Guid.NewGuid())
            };

            var handler = GetPreparedHandler(memberUploadsWithSeveralErrors);

            var memberUploadErrorDataList =
                await handler.HandleAsync(new GetMemberUploadData(pcsId, memberUploadsWithSeveralErrors.First().Id));

            Assert.True(memberUploadErrorDataList.Count(me => me.ErrorLevel == Core.Shared.ErrorLevel.Warning) == 1);
            Assert.True(memberUploadErrorDataList.Count(me => me.ErrorLevel == Core.Shared.ErrorLevel.Error) == 2);
            Assert.True(memberUploadErrorDataList.Count(me => me.ErrorLevel == Core.Shared.ErrorLevel.Fatal) == 3);
        }

        [Fact]
        public async Task GetMemberUploadDataHandler_WithNoErrors_MappedCorrectly()
        {
            var memberUploadsWithNoErrors = new[]
            {
                new MemberUpload(pcsId, schemeId, "FAKE DATA")
            };

            var handler = GetPreparedHandler(memberUploadsWithNoErrors);

            var memberUploadErrorDataList =
                await handler.HandleAsync(new GetMemberUploadData(pcsId, memberUploadsWithNoErrors.First().Id));

            Assert.Empty(memberUploadErrorDataList);
        }

        [Fact]
        public async Task GetMemberUploadHandler_NonExistentMemberUpload_ArgumentNullException()
        {
            var memberUploadsThatWontBeReturnedForRandomGuid = new[] { new MemberUpload(pcsId, schemeId, "FAKE DATA") };

            var handler = GetPreparedHandler(memberUploadsThatWontBeReturnedForRandomGuid);

            await
                Assert.ThrowsAsync<ArgumentNullException>(
                    async () => await handler.HandleAsync(new GetMemberUploadData(pcsId, Guid.NewGuid())));
        }

        [Fact]
        public async Task GetMemberUploadHandler_WrongPcsId_ArgumentException()
        {
            var someOtherPcsId = Guid.NewGuid();

            var memberUploadsForSomeOtherPcs = new[] { new MemberUpload(someOtherPcsId, schemeId, "FAKE DATA") };

            var handler = GetPreparedHandler(memberUploadsForSomeOtherPcs);

            await Assert.ThrowsAsync<ArgumentException>(async () =>
                await handler.HandleAsync(new GetMemberUploadData(pcsId, memberUploadsForSomeOtherPcs.First().Id)));
        }

        private GetMemberUploadDataHandler GetPreparedHandler(IEnumerable<MemberUpload> memberUploads)
        {
            var memberUploadsDbSet = helper.GetAsyncEnabledDbSet(memberUploads);

            var context = A.Fake<WeeeContext>();

            A.CallTo(() => context.MemberUploads).Returns(memberUploadsDbSet);

            return new GetMemberUploadDataHandler(AuthorizationBuilder.CreateUserAllowedToAccessOrganisation(), context, new MemberUploadErrorMap());
        }
    }
}
