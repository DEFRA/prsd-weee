namespace EA.Weee.Requests.Tests.Unit.MemberRegistration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Domain.PCS;
    using Domain.Producer;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain;
    using EA.Weee.RequestHandlers.Mappings;
    using EA.Weee.RequestHandlers.PCS.MemberRegistration;
    using EA.Weee.Requests.PCS.MemberRegistration;
    using EA.Weee.Requests.Tests.Unit.Helpers;
    using FakeItEasy;
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

            return new GetProducerCSVByMemberUploadIdHandler(context);
        }

        [Fact]
        public async Task GetProducerCSVByMemberUploadIdHandler_InvalidMemberUploadId_ArgumentException()
        {
            var memberUploads = new[] { new MemberUpload(pcsId, "Test data", new List<MemberUploadError>(), 0, Guid.NewGuid()) };

            var handler = GetPreparedHandler(memberUploads);

            await Assert.ThrowsAsync<ArgumentException>(async () => await handler.HandleAsync(new GetProducerCSVByMemberUploadId(Guid.NewGuid())));
        }
    }
}
