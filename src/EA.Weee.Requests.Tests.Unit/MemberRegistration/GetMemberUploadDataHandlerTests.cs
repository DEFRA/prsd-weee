namespace EA.Weee.Requests.Tests.Unit.MemberRegistration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
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

        [Fact]
        public async Task GetMemberUploadDataHandler_WithSeveralErrors_MappedCorrectly()
        {
            var memberUploads = helper.GetAsyncEnabledDbSet(new[]
            {
                GetExampleMemberUpload(),
            });

            var context = A.Fake<WeeeContext>();

            A.CallTo(() => context.MemberUploads).Returns(memberUploads);

            var handler = new GetMemberUploadDataHandler(context, new MemberUploadErrorMap());

            var memberUploadErrorDataList = await handler.HandleAsync(new GetMemberUploadData(memberUploads.First().Id));

            Assert.True(memberUploadErrorDataList.Count(me => me.ErrorLevel == Core.Shared.ErrorLevel.Warning) == 1);
            Assert.True(memberUploadErrorDataList.Count(me => me.ErrorLevel == Core.Shared.ErrorLevel.Error) == 2);
            Assert.True(memberUploadErrorDataList.Count(me => me.ErrorLevel == Core.Shared.ErrorLevel.Fatal) == 3);
        }

        [Fact]
        public async Task GetMemberUploadDataHandler_WithNoErrors_MappedCorrectly()
        {
            var memberUploads = helper.GetAsyncEnabledDbSet(new[]
            {
                new MemberUpload(Guid.NewGuid(), "FAKE DATA"), 
            });

            var context = A.Fake<WeeeContext>();

            A.CallTo(() => context.MemberUploads).Returns(memberUploads);

            var handler = new GetMemberUploadDataHandler(context, new MemberUploadErrorMap());

            var memberUploadErrorDataList = await handler.HandleAsync(new GetMemberUploadData(memberUploads.First().Id));

            Assert.Empty(memberUploadErrorDataList);
        }

        [Fact]
        public async Task GetMemberUploadHandler_NonExistentMemberUpload_ArgumentNullException()
        {
            var memberUploads = helper.GetAsyncEnabledDbSet(new[]
            {
                new MemberUpload(Guid.NewGuid(), "FAKE DATA"), 
            });

            var context = A.Fake<WeeeContext>();

            A.CallTo(() => context.MemberUploads).Returns(memberUploads);

            var handler = new GetMemberUploadDataHandler(context, new MemberUploadErrorMap());

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await handler.HandleAsync(new GetMemberUploadData(Guid.NewGuid())));
        }

        private MemberUpload GetExampleMemberUpload()
        {
            return new MemberUpload(Guid.NewGuid(), "FAKE DATA", new List<MemberUploadError>
            {
                new MemberUploadError(ErrorLevel.Warning, "FAKE WARNING"),
                new MemberUploadError(ErrorLevel.Error, "FAKE ERROR"),
                new MemberUploadError(ErrorLevel.Error, "FAKE ERROR"),
                new MemberUploadError(ErrorLevel.Fatal, "FAKE FATAL"),
                new MemberUploadError(ErrorLevel.Fatal, "FAKE FATAL"),
                new MemberUploadError(ErrorLevel.Fatal, "FAKE FATAL"),
            });
        }
    }
}
