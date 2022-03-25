namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Controller
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Api.Client;
    using AutoFixture;
    using FakeItEasy;
    using Prsd.Core.Mapper;
    using Services;
    using Services.Caching;
    using Web.Areas.Aatf.Controllers;
    using Web.Areas.Aatf.ViewModels;
    using Web.Requests.Base;
    using Weee.Requests.AatfEvidence;

    public class ManageEvidenceNotesControllerTestsBase
    {
        protected readonly IWeeeClient WeeeClient;
        protected readonly IMapper Mapper;
        protected readonly ManageEvidenceNotesController Controller;
        protected readonly BreadcrumbService Breadcrumb;
        protected readonly IWeeeCache Cache;
        protected readonly IRequestCreator<EvidenceNoteViewModel, EvidenceNoteBaseRequest> CreateRequestCreator;
        protected readonly Fixture Fixture;
        protected readonly Guid OrganisationId;
        protected readonly Guid AatfId;
        protected readonly int EvidenceNoteId;

        public ManageEvidenceNotesControllerTestsBase()
        {
            WeeeClient = A.Fake<IWeeeClient>();
            Breadcrumb = A.Fake<BreadcrumbService>();
            Cache = A.Fake<IWeeeCache>();
            Mapper = A.Fake<IMapper>();
            CreateRequestCreator = A.Fake<IRequestCreator<EvidenceNoteViewModel, EvidenceNoteBaseRequest>>();
            Fixture = new Fixture();
            OrganisationId = Guid.NewGuid();
            AatfId = Guid.NewGuid();
            EvidenceNoteId = 10;
            Controller = new ManageEvidenceNotesController(Mapper, Breadcrumb, Cache, () => WeeeClient, CreateRequestCreator);
        }
    }
}
