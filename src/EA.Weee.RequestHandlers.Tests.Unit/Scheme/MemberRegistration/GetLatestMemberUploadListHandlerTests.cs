namespace EA.Weee.RequestHandlers.Tests.Unit.Scheme.MemberRegistration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using Core.Helpers;
    using Core.Scheme;
    using DataAccess;
    using Domain.Scheme;
    using FakeItEasy;
    using Helpers;
    using Mappings;
    using Prsd.Core.Mapper;
    using RequestHandlers.Scheme.MemberRegistration;
    using RequestHandlers.Security;
    using Requests.Scheme.MemberRegistration;
    using Xunit;

    public class GetLatestMemberUploadListHandlerTests
    {
        private readonly WeeeContext weeeContext;
        private readonly DbContextHelper weeeContextHelper;
        private readonly IMap<IEnumerable<MemberUpload>, LatestMemberUploadList> mapper;

        private readonly IWeeeAuthorization permissiveAuthorization = AuthorizationBuilder.CreateUserAllowedToAccessOrganisation();

        private long memberUploadRowVersion;

        public GetLatestMemberUploadListHandlerTests()
        {
            weeeContext = A.Fake<WeeeContext>();
            weeeContextHelper = new DbContextHelper();
            mapper = A.Fake<IMap<IEnumerable<MemberUpload>, LatestMemberUploadList>>();

            memberUploadRowVersion = 0;
        }

        [Fact]
        public async Task GetLatestMemberUploadListHandler_NotOrganisationUser_ThrowsSecurityException()
        {
            var denyingAuthorization = AuthorizationBuilder.CreateUserDeniedFromAccessingOrganisation();

            var handler = new GetLatestMemberUploadListHandler(denyingAuthorization, A<WeeeContext>._, A<LatestMemberUploadListMap>._);
            var message = new GetLatestMemberUploadList(Guid.NewGuid());

            await Assert.ThrowsAsync<SecurityException>(async () => await handler.HandleAsync(message));
        }

        [Fact]
        public async void GetLatestMemberUploadListHandler_MemberUploadDoesNotExistForPcs_ReturnsEmptyListOfMemberUploadSummaries()
        {
            var pcsId = Guid.NewGuid();

            A.CallTo(() => weeeContext.MemberUploads)
                .Returns(weeeContextHelper.GetAsyncEnabledDbSet(new List<MemberUpload>()));

            IEnumerable<MemberUpload> returnedMemberUploads = new List<MemberUpload>();

            A.CallTo(() => mapper.Map(A<IEnumerable<MemberUpload>>._))
                .Invokes((IEnumerable<MemberUpload> u) => returnedMemberUploads = u);

            await GetLatestMemberUploadSummaryHandler().HandleAsync(new GetLatestMemberUploadList(pcsId));

            Assert.Empty(returnedMemberUploads);
        }

        [Fact]
        public async void GetLatestMemberUploadListHandler_MemberUploadExistsForPcs_ReturnsMemberUpload()
        {
            var pcsId = Guid.NewGuid();

            A.CallTo(() => weeeContext.MemberUploads)
                .Returns(weeeContextHelper.GetAsyncEnabledDbSet(new List<MemberUpload>
                {
                    ValidMemberUpload(pcsId)
                }));

            IEnumerable<MemberUpload> returnedMemberUploads = new List<MemberUpload>();

            A.CallTo(() => mapper.Map(A<IEnumerable<MemberUpload>>._))
                .Invokes((IEnumerable<MemberUpload> u) => returnedMemberUploads = u);

            await GetLatestMemberUploadSummaryHandler().HandleAsync(new GetLatestMemberUploadList(pcsId));

            Assert.Equal(1, returnedMemberUploads.Count());
        }

        [Fact]
        public async void GetLatestMemberUploadListHandler_MultipleMemberUploadsExistsForSameComplianceYear_ReturnsLatest()
        {
            var pcsId = Guid.NewGuid();

            A.CallTo(() => weeeContext.MemberUploads)
                .Returns(weeeContextHelper.GetAsyncEnabledDbSet(new List<MemberUpload>
                {
                    ValidMemberUpload(pcsId),
                    ValidMemberUpload(pcsId)
                }));

            IEnumerable<MemberUpload> returnedMemberUploads = new List<MemberUpload>();

            A.CallTo(() => mapper.Map(A<IEnumerable<MemberUpload>>._))
                .Invokes((IEnumerable<MemberUpload> u) => returnedMemberUploads = u);

            await GetLatestMemberUploadSummaryHandler().HandleAsync(new GetLatestMemberUploadList(pcsId));

            Assert.Equal(1, returnedMemberUploads.Count());
            Assert.Equal(memberUploadRowVersion.ToByteArray(), returnedMemberUploads.Single().RowVersion);
        }

        private GetLatestMemberUploadListHandler GetLatestMemberUploadSummaryHandler()
        {
            return new GetLatestMemberUploadListHandler(permissiveAuthorization, weeeContext, mapper);
        }

        private MemberUpload ValidMemberUpload(Guid pcsId)
        {
            memberUploadRowVersion++;

            var upload = new MemberUpload(pcsId, "<xml>somexml</xml>", new List<MemberUploadError>(), 0, 2016)
            {
                RowVersion = memberUploadRowVersion.ToByteArray(),
            };

            upload.Submit();

            return upload;
        }
    }
}
