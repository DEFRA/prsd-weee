namespace EA.Weee.Requests.Tests.Unit.MemberRegistration
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain;
    using EA.Weee.RequestHandlers.Mappings;
    using EA.Weee.RequestHandlers.MemberRegistration;
    using EA.Weee.Requests.MemberRegistration;
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

            Assert.True(memberUploadErrorDataList.Count(me => me.ErrorLevel == Shared.ErrorLevel.Warning) == 1);
            Assert.True(memberUploadErrorDataList.Count(me => me.ErrorLevel == Shared.ErrorLevel.Error) == 2);
            Assert.True(memberUploadErrorDataList.Count(me => me.ErrorLevel == Shared.ErrorLevel.Fatal) == 3);
        }

        [Fact]
        public async Task GetMemberUploadDataHandler_WithNoErrors_MappedCorrectly()
        {
            var memberUploads = helper.GetAsyncEnabledDbSet(new[]
            {
                new MemberUpload("FAKE DATA"), 
            });

            var context = A.Fake<WeeeContext>();

            A.CallTo(() => context.MemberUploads).Returns(memberUploads);

            var handler = new GetMemberUploadDataHandler(context, new MemberUploadErrorMap());

            var memberUploadErrorDataList = await handler.HandleAsync(new GetMemberUploadData(memberUploads.First().Id));

            Assert.Empty(memberUploadErrorDataList);
        }

        private MemberUpload GetExampleMemberUpload()
        {
            return new MemberUpload("FAKE DATA", new List<MemberUploadError>
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
