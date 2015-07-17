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

    public class GetMemberUploadDataHandlerTests
    {
        private readonly DbContextHelper helper = new DbContextHelper();

        private readonly Guid pcsId = Guid.NewGuid();

        [Fact]
        public async Task GetMemberUploadDataHandler_WithSeveralErrors_MappedCorrectly()
        {
            var memberUploadsWithSeveralErrors = new[]
            {
                new MemberUpload(pcsId, "FAKE DATA", new List<MemberUploadError>
                {
                    new MemberUploadError(ErrorLevel.Warning, "FAKE WARNING"),
                    new MemberUploadError(ErrorLevel.Error, "FAKE ERROR"),
                    new MemberUploadError(ErrorLevel.Error, "FAKE ERROR"),
                    new MemberUploadError(ErrorLevel.Fatal, "FAKE FATAL"),
                    new MemberUploadError(ErrorLevel.Fatal, "FAKE FATAL"),
                    new MemberUploadError(ErrorLevel.Fatal, "FAKE FATAL"),
                }),
            };

            var handler = GetPreparedHandler(memberUploadsWithSeveralErrors);

            var memberUploadErrorDataList = await handler.HandleAsync(new GetMemberUploadData(pcsId, memberUploadsWithSeveralErrors.First().Id));

            Assert.True(memberUploadErrorDataList.Count(me => me.ErrorLevel == Core.Shared.ErrorLevel.Warning) == 1);
            Assert.True(memberUploadErrorDataList.Count(me => me.ErrorLevel == Core.Shared.ErrorLevel.Error) == 2);
            Assert.True(memberUploadErrorDataList.Count(me => me.ErrorLevel == Core.Shared.ErrorLevel.Fatal) == 3);
        }

        [Fact]
        public async Task GetMemberUploadDataHandler_WithNoErrors_MappedCorrectly()
        {
            var memberUploadsWithNoErrors = new[]
            {
                new MemberUpload(pcsId, "FAKE DATA"), 
            };

            var handler = GetPreparedHandler(memberUploadsWithNoErrors);

            var memberUploadErrorDataList = await handler.HandleAsync(new GetMemberUploadData(pcsId, memberUploadsWithNoErrors.First().Id));

            Assert.Empty(memberUploadErrorDataList);
        }

        [Fact]
        public async Task GetMemberUploadHandler_NonExistentMemberUpload_ArgumentNullException()
        {
            var memberUploadsThatWontBeReturnedForRandomGuid = new[] { new MemberUpload(pcsId, "FAKE DATA"), };

            var handler = GetPreparedHandler(memberUploadsThatWontBeReturnedForRandomGuid);

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await handler.HandleAsync(new GetMemberUploadData(pcsId, Guid.NewGuid())));
        }

        [Fact]
        public async Task GetMemberUploadHandler_WrongPcsId_ArgumentException()
        {
            var someOtherPcsId = Guid.NewGuid();

            var memberUploadsForSomeOtherPcs = new[] { new MemberUpload(someOtherPcsId, "FAKE DATA"), };

            var handler = GetPreparedHandler(memberUploadsForSomeOtherPcs);

            await Assert.ThrowsAsync<ArgumentException>(async () => 
                await handler.HandleAsync(new GetMemberUploadData(pcsId, memberUploadsForSomeOtherPcs.First().Id)));
        }

        private GetMemberUploadDataHandler GetPreparedHandler(IEnumerable<MemberUpload> memberUploads)
        {
            var memberUploadsDbSet = helper.GetAsyncEnabledDbSet(memberUploads);

            var context = A.Fake<WeeeContext>();

            A.CallTo(() => context.MemberUploads).Returns(memberUploadsDbSet);

            return new GetMemberUploadDataHandler(context, new MemberUploadErrorMap());
        }
    }
}
