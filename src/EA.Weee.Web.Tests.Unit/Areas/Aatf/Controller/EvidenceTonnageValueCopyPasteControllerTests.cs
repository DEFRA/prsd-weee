namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Controller
{
    using EA.Weee.Core.Shared;
    using EA.Weee.Web.Areas.Aatf.Controllers;
    using EA.Weee.Web.Areas.Aatf.ViewModels;
    using EA.Weee.Web.Areas.AatfEvidence.Controllers;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Xunit;

    public class EvidenceTonnageValueCopyPasteControllerTests
    {
        private readonly EvidenceTonnageValueCopyPasteController evidenceTonnageValueCopyPasteController;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private readonly ISessionService sessionService;
        private readonly IPasteProcessor pasteProcessor;
        private readonly Guid organisationId;

        public EvidenceTonnageValueCopyPasteControllerTests()
        {
            breadcrumb = A.Fake<BreadcrumbService>();
            cache = A.Fake<IWeeeCache>();
            sessionService = A.Fake<ISessionService>();
            pasteProcessor = A.Fake<IPasteProcessor>();

            organisationId = new Guid();

            evidenceTonnageValueCopyPasteController = new EvidenceTonnageValueCopyPasteController(breadcrumb, cache, sessionService, pasteProcessor);
        }

        [Fact]
        public void EvidenceTonnageValueCopyPasteControllerInheritsExternalSiteController()
        {
            typeof(EvidenceTonnageValueCopyPasteController).BaseType.Name.Should().Be(nameof(AatfEvidenceBaseController));
        }

        [Fact]
        public void IndexGet_IsDecoratedWith_HttpGetAttribute()
        {
            typeof(EvidenceTonnageValueCopyPasteController).GetMethod("Index", new[] { typeof(Guid), typeof(string), typeof(bool) }).Should()
                .BeDecoratedWith<HttpGetAttribute>();
        }

        [Fact]
        public void IndexPost_IsDecoratedWith_HttpPostAttribute()
        {
            typeof(EvidenceTonnageValueCopyPasteController).GetMethod("Index", new[] { typeof(EvidenceTonnageValueCopyPasteViewModel) }).Should()
                .BeDecoratedWith<HttpPostAttribute>();
        }

        [Fact]
        public void IndexPost_IsDecoratedWith_AntiForgeryAttribute()
        {
            typeof(EvidenceTonnageValueCopyPasteController).GetMethod("Index", new[] { typeof(EvidenceTonnageValueCopyPasteViewModel) }).Should()
                .BeDecoratedWith<ValidateAntiForgeryTokenAttribute>();
        }

        [Fact]
        public async Task IndexGet_Calls_SetBreadcrumb()
        {
            //arrange
            var organisationName = Faker.Company.Name();

            A.CallTo(() => cache.FetchOrganisationName(organisationId)).Returns(organisationName);

            //act
            await evidenceTonnageValueCopyPasteController.Index(organisationId, string.Empty, true);

            //assert
            breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.AatfManageEvidence);
            breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            breadcrumb.OrganisationId.Should().Be(organisationId);
        }

        [Fact]
        public async Task IndexGet_RetrievesEvidenceModelSessionObject()
        {
            await evidenceTonnageValueCopyPasteController.Index(organisationId, string.Empty, true);

            A.CallTo(() => sessionService.GetTransferSessionObject<EditEvidenceNoteViewModel>(evidenceTonnageValueCopyPasteController.Session,
                SessionKeyConstant.EditEvidenceViewModelKey)).MustHaveHappenedOnceExactly();
        }

        //[Fact]
        //public async Task IndexGet_OnRedirectIsFalse_CreatesModelAndReturnsView()
        //{

        //}

        //[Theory]
        //public async Task IndexGet_OnRedirectIsTrue_RedirectsToCorrectPage()
        //{

        //}

        //[Theory]
        //public void IndexPost_OnRedirectIsTrue_RedirectsToCorrectPage()
        //{

        //}

        //[Fact]
        //public void IndexPost_OnNoGivenCopyPasteValues_DoesNotPopulateSession()
        //{

        //}

        //[Fact]
        //public void IndexPost_OnGivenCopyPasteValues_DoesPopulateSession()
        //{

        //}

        //[Fact]
        //public void IndexPost_OnGivenCopyPasteValues_DoesCallPasteProcessor()
        //{

        //}

        //[Fact]
        //public void IndexPost_Retrieves_EvidenceModelSessionOject()
        //{

        //}
    }
}
