﻿namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers
{
    using System;
    using AutoFixture;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Tests.Core;
    using EA.Weee.Web.Areas.Admin.Controllers;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using FakeItEasy;

    public class ManageEvidenceNotesControllerTestsBase : SimpleUnitTestBase
    {
        protected readonly ManageEvidenceNotesController ManageEvidenceController;
        protected readonly IWeeeClient WeeeClient;
        protected readonly IMapper Mapper;
        protected readonly BreadcrumbService Breadcrumb;
        protected readonly IWeeeCache Cache;
        protected readonly Guid EvidenceNoteId;
        protected readonly EvidenceNoteData EvidenceNoteData;

        public ManageEvidenceNotesControllerTestsBase()
        {
            WeeeClient = A.Fake<IWeeeClient>();
            Breadcrumb = A.Fake<BreadcrumbService>();
            Mapper = A.Fake<IMapper>();
            Cache = A.Fake<IWeeeCache>();
            EvidenceNoteId = Guid.NewGuid();

            EvidenceNoteData = TestFixture.Create<EvidenceNoteData>();

            ManageEvidenceController = new ManageEvidenceNotesController(Mapper, Breadcrumb, Cache, () => WeeeClient);
        }
    }
}
