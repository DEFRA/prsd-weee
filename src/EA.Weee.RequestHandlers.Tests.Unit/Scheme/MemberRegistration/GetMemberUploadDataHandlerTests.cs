namespace EA.Weee.RequestHandlers.Tests.Unit.Scheme.MemberRegistration
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
                            new MemberUploadError(ErrorLevel.Warning, UploadErrorType.Schema, "FAKE WARNING"),
                            new MemberUploadError(ErrorLevel.Error, UploadErrorType.Business, "FAKE ERROR"),
                            new MemberUploadError(ErrorLevel.Error, UploadErrorType.Schema, "FAKE ERROR"),
                            new MemberUploadError(ErrorLevel.Fatal, UploadErrorType.Business, "FAKE FATAL"),
                            new MemberUploadError(ErrorLevel.Fatal, UploadErrorType.Schema, "FAKE FATAL"),
                            new MemberUploadError(ErrorLevel.Fatal, UploadErrorType.Business, "FAKE FATAL")
                        },
                    0,
                    2016,
                    Guid.NewGuid(), "File name")
            };

            var handler = GetPreparedHandler(memberUploadsWithSeveralErrors);

            var memberUploadErrorDataList =
                await handler.HandleAsync(new GetMemberUploadData(pcsId, memberUploadsWithSeveralErrors.First().Id));

            Assert.True(memberUploadErrorDataList.Count(me => me.ErrorLevel == Core.Shared.ErrorLevel.Warning) == 1);
            Assert.True(memberUploadErrorDataList.Count(me => me.ErrorLevel == Core.Shared.ErrorLevel.Error) == 2);
            Assert.True(memberUploadErrorDataList.Count(me => me.ErrorLevel == Core.Shared.ErrorLevel.Fatal) == 3);
        }

        [Fact]
        public async Task GetMemberUploadDataHandler_WithSeveralErrorsWithLineNumber_ShouldbeOrderByLineNumber()
        {
            var memberUploadsWithSeveralErrors = new[]
            {
                new MemberUpload(
                    pcsId,
                    "FAKE DATA",
                    new List<MemberUploadError>
                        {
                            new MemberUploadError(ErrorLevel.Warning, UploadErrorType.Schema, "FAKE WARNING 250", 250),
                            new MemberUploadError(ErrorLevel.Error, UploadErrorType.Business, "FAKE ERROR 50", 50),
                            new MemberUploadError(ErrorLevel.Error, UploadErrorType.Schema, "FAKE ERROR 5", 5),
                            new MemberUploadError(ErrorLevel.Fatal, UploadErrorType.Business, "FAKE FATAL 28", 28),
                            new MemberUploadError(ErrorLevel.Fatal, UploadErrorType.Schema, "FAKE FATAL 178", 178),
                            new MemberUploadError(ErrorLevel.Fatal, UploadErrorType.Business, "FAKE FATAL")
                        },
                    0,
                    2016,
                    Guid.NewGuid(), "File name")
            };

            var handler = GetPreparedHandler(memberUploadsWithSeveralErrors);

            var memberUploadErrorDataList =
                await handler.HandleAsync(new GetMemberUploadData(pcsId, memberUploadsWithSeveralErrors.First().Id));

            Assert.True(memberUploadErrorDataList[0].Description == "FAKE FATAL");
            Assert.True(memberUploadErrorDataList[1].Description == "FAKE ERROR 5");
            Assert.True(memberUploadErrorDataList[2].Description == "FAKE FATAL 28");
            Assert.True(memberUploadErrorDataList[3].Description == "FAKE ERROR 50");
            Assert.True(memberUploadErrorDataList[4].Description == "FAKE FATAL 178");
            Assert.True(memberUploadErrorDataList[5].Description == "FAKE WARNING 250");
        }

        [Fact]
        public async Task GetMemberUploadDataHandler_WithNoErrors_MappedCorrectly()
        {
            var memberUploadsWithNoErrors = new[]
            {
                new MemberUpload(pcsId, schemeId, "FAKE DATA", "File name")
            };

            var handler = GetPreparedHandler(memberUploadsWithNoErrors);

            var memberUploadErrorDataList =
                await handler.HandleAsync(new GetMemberUploadData(pcsId, memberUploadsWithNoErrors.First().Id));

            Assert.Empty(memberUploadErrorDataList);
        }

        [Fact]
        public async Task GetMemberUploadHandler_NonExistentMemberUpload_ArgumentNullException()
        {
            var memberUploadsThatWontBeReturnedForRandomGuid = new[] { new MemberUpload(pcsId, schemeId, "FAKE DATA", "File name") };

            var handler = GetPreparedHandler(memberUploadsThatWontBeReturnedForRandomGuid);

            await
                Assert.ThrowsAsync<ArgumentNullException>(
                    async () => await handler.HandleAsync(new GetMemberUploadData(pcsId, Guid.NewGuid())));
        }

        [Fact]
        public async Task GetMemberUploadHandler_WrongPcsId_ArgumentException()
        {
            var someOtherPcsId = Guid.NewGuid();

            var memberUploadsForSomeOtherPcs = new[] { new MemberUpload(someOtherPcsId, schemeId, "FAKE DATA", "File name") };

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
