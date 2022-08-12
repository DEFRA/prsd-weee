namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers
{
    using System;
    using AutoFixture;
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Requests.Shared;
    using EA.Weee.Tests.Core;
    using EA.Weee.Web.Areas.Admin.Controllers;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using FakeItEasy;

    public class ManageEvidenceNotesControllerTestsBase : SimpleUnitTestBase
    {
        protected readonly ManageEvidenceNotesController ManageEvidenceController;
        protected readonly ConfigurationService ConfigurationService;
        protected readonly IWeeeClient WeeeClient;
        protected readonly IMapper Mapper;
        protected readonly BreadcrumbService Breadcrumb;
        protected readonly IWeeeCache Cache;
        protected readonly Guid EvidenceNoteId;
        protected readonly EvidenceNoteData EvidenceNoteData;
        protected readonly TransferEvidenceNoteData TransferEvidenceNoteData;
        protected readonly ISessionService SessionService;

        protected readonly int PageSize = 25;

        public ManageEvidenceNotesControllerTestsBase()
        {
            WeeeClient = A.Fake<IWeeeClient>();
            Breadcrumb = A.Fake<BreadcrumbService>();
            ConfigurationService = A.Fake<ConfigurationService>();
            Mapper = A.Fake<IMapper>();
            Cache = A.Fake<IWeeeCache>();
            SessionService = A.Fake<ISessionService>();
            EvidenceNoteId = Guid.NewGuid();

            EvidenceNoteData = TestFixture.Create<EvidenceNoteData>();
            TransferEvidenceNoteData = TestFixture.Create<TransferEvidenceNoteData>();
            A.CallTo(() => ConfigurationService.CurrentConfiguration.DefaultPagingPageSize).Returns(PageSize);

            ManageEvidenceController = new ManageEvidenceNotesController(Mapper, Breadcrumb, Cache, () => WeeeClient, ConfigurationService);

            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(SystemTime.Now);
        }
    }
}
