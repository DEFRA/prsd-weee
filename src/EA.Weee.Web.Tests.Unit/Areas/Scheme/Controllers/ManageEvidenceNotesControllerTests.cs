namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.Controllers
{
    using AutoFixture;
    using Core.Helpers;
    using EA.Prsd.Core;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Core.Admin.Obligation;
    using EA.Weee.Core.Scheme;
    using EA.Weee.Requests.AatfEvidence;
    using EA.Weee.Requests.Scheme;
    using EA.Weee.Web.Areas.Aatf.ViewModels;
    using EA.Weee.Web.Areas.Scheme.Controllers;
    using EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels;
    using EA.Weee.Web.Areas.Scheme.ViewModels;
    using EA.Weee.Web.Areas.Scheme.ViewModels.ManageEvidenceNotes;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Requests.Base;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Tests.Unit.Areas.Aatf.Controller;
    using EA.Weee.Web.ViewModels.Shared;
    using EA.Weee.Web.ViewModels.Shared.Mapping;
    using FakeItEasy;
    using FluentAssertions;
    using Org.BouncyCastle.Asn1.Ocsp;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Web.Filters;
    using Weee.Requests.Shared;
    using Xunit;

    public class ManageEvidenceNotesControllerTests : ManageEvidenceNotesControllerTestsBase
    {
        protected new readonly ManageEvidenceNotesController ManageEvidenceController;
        protected readonly IRequestCreator<TransferEvidenceNoteCategoriesViewModel, TransferEvidenceNoteRequest> TransferNoteRequestCreator;
        private readonly ConfigurationService configurationService;

        public ManageEvidenceNotesControllerTests()
        {
            configurationService = A.Fake<ConfigurationService>();
            A.CallTo(() => configurationService.CurrentConfiguration.DefaultExternalPagingPageSize).Returns(10);

            TransferNoteRequestCreator = A.Fake<IRequestCreator<TransferEvidenceNoteCategoriesViewModel, TransferEvidenceNoteRequest>>();
            ManageEvidenceController = new ManageEvidenceNotesController(Mapper, Breadcrumb, Cache, () => WeeeClient, SessionService, TemplateExecutor, PdfDocumentProvider, configurationService);

            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(DateTime.Now);
            A.CallTo(() => Cache.FetchSchemePublicInfo(A<Guid>._)).Returns(new SchemePublicInfo() { Name = TestFixture.Create<string>() });
        }

        [Fact]
        public void ManageEvidenceNotesControllerInheritsCheckSchemeEvidenceBaseController()
        {
            typeof(ManageEvidenceNotesController).BaseType.Name.Should().Be(nameof(SchemeEvidenceBaseController));
        }

        [Fact]
        public void IndexGet_ShouldHaveHttpGetAttribute()
        {
            typeof(ManageEvidenceNotesController).GetMethod("Index", new[]
                {
                    typeof(Guid),
                    typeof(string),
                    typeof(int?),
                    typeof(int?),
                    typeof(DateTime?),
                    typeof(DateTime?),
                    typeof(Guid?),
                    typeof(int?),
                    typeof(int?),
                    typeof(string),
                    typeof(int?),
                    typeof(Guid?)
                }).Should()
                .BeDecoratedWith<HttpGetAttribute>();
        }

        [Fact]
        public void IndexGet_ShouldHaveNoCacheAttribute()
        {
            typeof(ManageEvidenceNotesController).GetMethod("Index", new[]
               {
                    typeof(Guid),
                    typeof(string),
                    typeof(int?),
                    typeof(int?),
                    typeof(DateTime?),
                    typeof(DateTime?),
                    typeof(Guid?),
                    typeof(int?),
                    typeof(int?),
                    typeof(string),
                    typeof(int?),
                    typeof(Guid?)
                }).Should()
                .BeDecoratedWith<NoCacheFilterAttribute>();
        }

        [Fact]
        public void IndexPost_ShouldHaveHttpPostAttribute()
        {
            typeof(ManageEvidenceNotesController).GetMethod("Index", new[]
                {
                    typeof(Guid),
                    typeof(string),
                    typeof(ManageEvidenceNoteViewModel),
                    typeof(int)
                }).Should()
                .BeDecoratedWith<HttpPostAttribute>();
        }

        [Fact]
        public void IndexPost_ShouldHaveValidateAntiForgeryTokenAttribute()
        {
            typeof(ManageEvidenceNotesController).GetMethod("Index", new[]
               {
                    typeof(Guid),
                    typeof(string),
                    typeof(ManageEvidenceNoteViewModel),
                    typeof(int)
                }).Should()
                .BeDecoratedWith<ValidateAntiForgeryTokenAttribute>();
        }

        [Fact]
        public void TransferPost_ShouldHaveHttpPostAttribute()
        {
            typeof(ManageEvidenceNotesController).GetMethod("Transfer", new[] { typeof(Guid), typeof(int) }).Should()
                .BeDecoratedWith<HttpPostAttribute>();
        }

        [Fact]
        public void TransferPost_ShouldHaveAntiForgeryAttribute()
        {
            typeof(ManageEvidenceNotesController).GetMethod("Transfer", new[] { typeof(Guid), typeof(int) }).Should()
                .BeDecoratedWith<ValidateAntiForgeryTokenAttribute>();
        }

        [Fact]
        public void ViewEvidenceNoteGet_ShouldHaveHttpGetAttribute()
        {
            typeof(ManageEvidenceNotesController).GetMethod("ViewEvidenceNote", new[] { typeof(Guid), typeof(Guid), typeof(string), typeof(int), typeof(bool), typeof(string) }).Should().BeDecoratedWith<HttpGetAttribute>();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("view-and-transfer-evidence")]
        [InlineData("review-submitted-evidence")]
        [InlineData("evidence-summary")]
        [InlineData("outgoing-transfers")]
        public async Task IndexGet_GivenOrganisationIsNotBalancingScheme_BreadcrumbShouldBeSet(string tab)
        {
            //arrange
            var schemeName = Faker.Company.Name();
            var schemeInfo = TestFixture.Build<SchemePublicInfo>().With(s => s.IsBalancingScheme, false).Create();
            A.CallTo(() => Cache.FetchOrganisationName(OrganisationId)).Returns(schemeName);
            A.CallTo(() => Cache.FetchSchemePublicInfo(OrganisationId)).Returns(schemeInfo);
            var evidenceNotes = new ManageEvidenceNoteViewModel();

            //act
            await ManageEvidenceController.Index(OrganisationId, tab, null, null);

            //assert
            Breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.SchemeManageEvidence);
            Breadcrumb.ExternalOrganisation.Should().Be(schemeName);
            Breadcrumb.OrganisationId.Should().Be(OrganisationId);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("view-and-transfer-evidence")]
        [InlineData("review-submitted-evidence")]
        [InlineData("evidence-summary")]
        [InlineData("outgoing-transfers")]
        public async Task IndexGet_GivenOrganisationIsBalancingScheme_BreadcrumbShouldBeSet(string tab)
        {
            //arrange
            var schemeName = Faker.Company.Name();
            var schemeInfo = TestFixture.Build<SchemePublicInfo>().With(s => s.IsBalancingScheme, true).Create();
            A.CallTo(() => Cache.FetchOrganisationName(OrganisationId)).Returns(schemeName);
            A.CallTo(() => Cache.FetchSchemePublicInfo(OrganisationId)).Returns(schemeInfo);
            var evidenceNotes = new ManageEvidenceNoteViewModel();

            //act
            await ManageEvidenceController.Index(OrganisationId, tab, null, null);

            //assert
            Breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.PbsManageEvidence);
            Breadcrumb.ExternalOrganisation.Should().Be(schemeName);
            Breadcrumb.OrganisationId.Should().Be(OrganisationId);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("view-and-transfer-evidence")]
        [InlineData("review-submitted-evidence")]
        [InlineData("evidence-summary")]
        [InlineData("outgoing-transfers")]
        public async Task IndexGet_GivenOrganisationId_SchemeShouldBeRetrievedFromCache(string tab)
        {
            //arrange
            A.CallTo(() => Cache.FetchSchemePublicInfo(OrganisationId)).Returns(new SchemePublicInfo() { SchemeId = Guid.NewGuid() });
            var evidenceNotes = new ManageEvidenceNoteViewModel();

            //act
            await ManageEvidenceController.Index(OrganisationId, tab, null, null);

            //assert
            A.CallTo(() => Cache.FetchSchemePublicInfo(OrganisationId)).MustHaveHappenedTwiceExactly();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("view-and-transfer-evidence")]
        [InlineData("review-submitted-evidence")]
        [InlineData("evidence-summary")]
        [InlineData("outgoing-transfers")]
        public async Task IndexGet_CurrentSystemTimeShouldBeRetrieved(string tab)
        {
            //arrange
            A.CallTo(() => Cache.FetchSchemePublicInfo(A<Guid>._)).Returns(new SchemePublicInfo() { SchemeId = Guid.NewGuid() });
            var evidenceNotes = new ManageEvidenceNoteViewModel();

            //act
            await ManageEvidenceController.Index(OrganisationId, tab, null, null);

            //assert
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task IndexGet_GivenManageEvidenceNotesViewModelForOutgoingTransferTab_ModelMapperShouldBeCalledWithCorrectValues()
        {
            //arrange
            var complianceYear = TestFixture.Create<short>();
            var currentDate = TestFixture.Create<DateTime>();
            var recipientWasteStatusViewModel = TestFixture.Create<RecipientWasteStatusFilterViewModel>();

            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(currentDate);
            A.CallTo(() => Cache.FetchSchemePublicInfo(A<Guid>._)).Returns(new SchemePublicInfo() { SchemeId = Guid.NewGuid() });
            A.CallTo(() => Mapper.Map<RecipientWasteStatusFilterViewModel>(A<RecipientWasteStatusFilterBase>._)).Returns(recipientWasteStatusViewModel);

            var model = TestFixture.Build<ManageEvidenceNoteViewModel>()
                   .With(e => e.SelectedComplianceYear, complianceYear).Create();

            //act
            await ManageEvidenceController.Index(OrganisationId, "outgoing-transfers", null, null);

            A.CallTo(() => Mapper.Map<ManageEvidenceNoteViewModel>(A<ManageEvidenceNoteTransfer>.That.Matches(m =>
                m.RecipientWasteStatusFilterViewModel == recipientWasteStatusViewModel)))
                .MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("view-and-transfer-evidence")]
        [InlineData("review-submitted-evidence")]
        [InlineData("evidence-summary")]
        [InlineData("outgoing-transfers")]
        public async Task IndexGet_GivenMappedManageEvidenceNotesViewModel_MappedModelShouldBeReturned(string tab)
        {
            //arrange
            var currentDate = TestFixture.Create<DateTime>();
            var model = TestFixture.Create<ManageEvidenceNoteViewModel>();

            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(currentDate);
            A.CallTo(() => Cache.FetchSchemePublicInfo(A<Guid>._)).Returns(new SchemePublicInfo() { SchemeId = Guid.NewGuid() });
            A.CallTo(() => Mapper.Map<ManageEvidenceNoteViewModel>(A<ManageEvidenceNoteTransfer>._)).Returns(model);

            //act
            var result = await ManageEvidenceController.Index(OrganisationId, tab, null, null) as ViewResult;

            var convertedModel = result.Model as ManageEvidenceNoteSchemeViewModel;

            convertedModel.ManageEvidenceNoteViewModel.Should().Be(model);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("view-and-transfer-evidence")]
        [InlineData("review-submitted-evidence")]
        [InlineData("evidence-summary")]
        [InlineData("outgoing-transfers")]
        public async Task IndexGet_GivenNullManageEvidenceNotesViewModel_ModelMapperShouldBeCalledWithCorrectValues(string tab)
        {
            //arrange
            var currentDate = TestFixture.Create<DateTime>();
            var expectedComplianceYear = currentDate.Month == 1 ? currentDate.Year - 1 : currentDate.Year;

            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(currentDate);
            A.CallTo(() => Cache.FetchSchemePublicInfo(A<Guid>._)).Returns(new SchemePublicInfo() { SchemeId = Guid.NewGuid() });

            //act
            await ManageEvidenceController.Index(OrganisationId, tab, null, null);

            A.CallTo(() => Mapper.Map<ManageEvidenceNoteViewModel>(A<ManageEvidenceNoteTransfer>.That.Matches(m =>
                    m.OrganisationId == OrganisationId &&
                    m.CurrentDate == currentDate &&
                    m.ComplianceYear == expectedComplianceYear)))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task IndexGet_GivenReviewTab_SubmittedEvidenceNoteShouldBeRetrieved()
        {
            // Arrange
            var status = new List<NoteStatus>() { NoteStatus.Submitted };
            var schemeName = Faker.Company.Name();
            var evidenceData = TestFixture.Create<EvidenceNoteData>();
            var returnList = new List<EvidenceNoteData>() { evidenceData };
            var noteData = TestFixture.Build<EvidenceNoteSearchDataResult>()
                .With(e => e.Results, returnList).Create();

            var currentDate = TestFixture.Create<DateTime>();
            var expectedComplianceYear = currentDate.Month == 1 ? currentDate.Year - 1 : currentDate.Year;
            var noteTypes = new List<NoteType>() { NoteType.Evidence, NoteType.Transfer };

            A.CallTo(() => configurationService.CurrentConfiguration.DefaultExternalPagingPageSize).Returns(10);
            A.CallTo(() => Cache.FetchSchemePublicInfo(A<Guid>._)).Returns(new SchemePublicInfo() { Name = schemeName });
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNotesByOrganisationRequest>._)).Returns(noteData);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(currentDate);

            //act
            await ManageEvidenceController.Index(OrganisationId, "review-submitted-evidence", null, null);

            //assert
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNotesByOrganisationRequest>.That.Matches(
                g => g.OrganisationId.Equals(OrganisationId) &&
                     status.SequenceEqual(g.AllowedStatuses) &&
                     g.ComplianceYear.Equals(expectedComplianceYear) &&
                     g.TransferredOut == false &&
                     g.NoteTypeFilterList.SequenceEqual(noteTypes) &&
                     g.PageSize == 10 &&
                     g.PageNumber == 1 &&
                     g.SearchRef == null))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task IndexGet_GivenReviewTabAndPageNumber_SubmittedEvidenceNoteShouldBeRetrieved()
        {
            // Arrange
            var schemeName = Faker.Company.Name();
            var evidenceData = TestFixture.Create<EvidenceNoteData>();
            var returnList = new List<EvidenceNoteData>() { evidenceData };
            var noteData = TestFixture.Build<EvidenceNoteSearchDataResult>()
                .With(e => e.Results, returnList).Create();

            var currentDate = TestFixture.Create<DateTime>();

            A.CallTo(() => configurationService.CurrentConfiguration.DefaultExternalPagingPageSize).Returns(10);
            A.CallTo(() => Cache.FetchSchemePublicInfo(A<Guid>._)).Returns(new SchemePublicInfo() { Name = schemeName });
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNotesByOrganisationRequest>._)).Returns(noteData);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(currentDate);

            const int pageNumber = 10;

            //act
            await ManageEvidenceController.Index(OrganisationId, "review-submitted-evidence", (int?)null, (int?)pageNumber);

            //assert
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNotesByOrganisationRequest>.That.Matches(
                g => g.PageNumber == pageNumber && g.PageSize == 10))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task IndexGet_GivenReviewTabAndPreviouslySelectedComplianceYear_SubmittedEvidenceNoteShouldBeRetrieved()
        {
            // Arrange
            var status = new List<NoteStatus>() { NoteStatus.Submitted };
            var schemeName = Faker.Company.Name();
            var evidenceData = TestFixture.Create<EvidenceNoteData>();
            var returnList = new List<EvidenceNoteData>() { evidenceData };
            var noteData = TestFixture.Build<EvidenceNoteSearchDataResult>()
                .With(e => e.Results, returnList).Create();
            var currentDate = TestFixture.Create<DateTime>();
            var complianceYear = TestFixture.Create<short>();
            var noteTypes = new List<NoteType>() { NoteType.Evidence, NoteType.Transfer };
            var model = TestFixture.Build<ManageEvidenceNoteViewModel>()
                .With(e => e.SelectedComplianceYear, complianceYear)
                .Without(e => e.FilterViewModel)
                .Create();

            A.CallTo(() => configurationService.CurrentConfiguration.DefaultExternalPagingPageSize).Returns(10);
            A.CallTo(() => Cache.FetchSchemePublicInfo(A<Guid>._)).Returns(new SchemePublicInfo() { Name = schemeName });
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNotesByOrganisationRequest>._)).Returns(noteData);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(currentDate);

            //act
            await ManageEvidenceController.Index(OrganisationId, "review-submitted-evidence", complianceYear, null);

            //assert
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNotesByOrganisationRequest>.That.Matches(
                g => g.OrganisationId.Equals(OrganisationId) &&
                     status.SequenceEqual(g.AllowedStatuses) &&
                     g.ComplianceYear.Equals(complianceYear) &&
                     g.TransferredOut == false &&
                     g.NoteTypeFilterList.SequenceEqual(noteTypes) &&
                     g.PageSize == 10 &&
                     g.PageNumber == 1 &&
                     g.SearchRef == null))).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(NoteStatus.Approved, "review-submitted-evidence")]
        [InlineData(NoteStatus.Draft, "review-submitted-evidence")]
        [InlineData(NoteStatus.Returned, "review-submitted-evidence")]
        [InlineData(NoteStatus.Void, "review-submitted-evidence")]
        [InlineData(NoteStatus.Rejected, "review-submitted-evidence")]
        public async Task IndexGet_GivenReviewTab_SubmittedEvidenceNoteShouldNotBeRetrievedForInvalidStatus(NoteStatus status, string tab)
        {
            //act
            await ManageEvidenceController.Index(OrganisationId, tab, null, null);

            //assert
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNotesByOrganisationRequest>.That.Matches(
                g => g.AllowedStatuses.Contains(status)))).MustNotHaveHappened();
        }

        [Fact]
        public async Task IndexGet_GivenReviewTabAlongWithReturnedData_ViewModelShouldBeBuilt()
        {
            // Arrange
            var scheme = TestFixture.Create<SchemePublicInfo>();
            var noteData = TestFixture.Build<EvidenceNoteSearchDataResult>().Create();
            var currentDate = TestFixture.Create<DateTime>();

            A.CallTo(() => configurationService.CurrentConfiguration.DefaultExternalPagingPageSize).Returns(10);
            A.CallTo(() => Cache.FetchSchemePublicInfo(A<Guid>._)).Returns(scheme);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNotesByOrganisationRequest>._)).Returns(noteData);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(currentDate);

            //act
            await ManageEvidenceController.Index(OrganisationId, "review-submitted-evidence", null, null);

            //assert
            A.CallTo(() => Mapper.Map<ReviewSubmittedManageEvidenceNotesSchemeViewModel>(
                A<SchemeTabViewModelMapTransfer>.That.Matches(
                    a => a.OrganisationId.Equals(OrganisationId) &&
                         a.NoteData == noteData &&
                         a.Scheme.Equals(scheme) &&
                         a.CurrentDate.Equals(currentDate) &&
                         a.PageNumber == 1 &&
                         a.PageSize == 10))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task IndexGet_GivenReviewTabAlongWithReturnedDataAndPageNumber_ViewModelShouldBeBuilt()
        {
            // Arrange
            var scheme = TestFixture.Create<SchemePublicInfo>();
            var noteData = TestFixture.Build<EvidenceNoteSearchDataResult>().Create();
            var currentDate = TestFixture.Create<DateTime>();

            A.CallTo(() => configurationService.CurrentConfiguration.DefaultExternalPagingPageSize).Returns(10);
            A.CallTo(() => Cache.FetchSchemePublicInfo(A<Guid>._)).Returns(scheme);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNotesByOrganisationRequest>._)).Returns(noteData);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(currentDate);

            const int pageNumber = 10;

            //act
            await ManageEvidenceController.Index(OrganisationId, "review-submitted-evidence", (int?)null, (int?)pageNumber);

            //assert
            A.CallTo(() => Mapper.Map<ReviewSubmittedManageEvidenceNotesSchemeViewModel>(
                A<SchemeTabViewModelMapTransfer>.That.Matches(a => a.PageNumber == pageNumber &&
                                                                   a.PageSize == 10))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task IndexGet_GivenReviewTabAlongWithReturnedDataAndManageEvidenceNoteViewModel_ViewModelShouldBeBuilt()
        {
            // Arrange
            var scheme = A.Fake<SchemePublicInfo>();
            var noteData = TestFixture.Build<EvidenceNoteSearchDataResult>().Create();
            var currentDate = TestFixture.Create<DateTime>();
            var model = TestFixture.Create<ManageEvidenceNoteViewModel>();

            A.CallTo(() => configurationService.CurrentConfiguration.DefaultExternalPagingPageSize).Returns(10);
            A.CallTo(() => Cache.FetchSchemePublicInfo(A<Guid>._)).Returns(scheme);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNotesByOrganisationRequest>._)).Returns(noteData);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(currentDate);

            //act

            await ManageEvidenceController.Index(OrganisationId, "review-submitted-evidence", null, null);

            //assert
            A.CallTo(() => Mapper.Map<ReviewSubmittedManageEvidenceNotesSchemeViewModel>(
                A<SchemeTabViewModelMapTransfer>.That.Matches(
                    a => a.OrganisationId.Equals(OrganisationId) &&
                         a.NoteData == noteData &&
                         a.Scheme.Equals(scheme) &&
                         a.CurrentDate.Equals(currentDate) &&
                         a.PageNumber == 1 &&
                         a.PageSize == 10))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task IndexGet_GivenViewAndTransferTabAlongWithReturnedData_ViewAndTransferEvidenceNotesViewModelShouldBeBuilt()
        {
            // arrange
            var scheme = TestFixture.Create<SchemePublicInfo>();
            var noteData = TestFixture.Build<EvidenceNoteSearchDataResult>().Create();
            var currentDate = TestFixture.Create<DateTime>();

            A.CallTo(() => Cache.FetchSchemePublicInfo(A<Guid>._)).Returns(scheme);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNotesByOrganisationRequest>._)).Returns(noteData);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(currentDate);

            //act
            await ManageEvidenceController.Index(OrganisationId, ManageEvidenceNotesDisplayOptions.ViewAndTransferEvidence.ToDisplayString(), null, null);

            //assert
            A.CallTo(() => Mapper.Map<SchemeViewAndTransferManageEvidenceSchemeViewModel>(
                A<SchemeTabViewModelMapTransfer>.That.Matches(
                    a => a.OrganisationId.Equals(OrganisationId)
                         && a.NoteData.Equals(noteData) &&
                         a.Scheme.Equals(scheme) &&
                         a.CurrentDate.Equals(currentDate) &&
                         a.PageNumber == 1 &&
                         a.PageSize == 10))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task IndexGet_GivenViewAndTransferTabAlongWithReturnedDataAndPageNumber_ViewAndTransferEvidenceNotesViewModelShouldBeBuilt()
        {
            // Arrange
            var scheme = TestFixture.Create<SchemePublicInfo>();
            var noteData = TestFixture.Build<EvidenceNoteSearchDataResult>().Create();
            var currentDate = TestFixture.Create<DateTime>();

            A.CallTo(() => Cache.FetchSchemePublicInfo(A<Guid>._)).Returns(scheme);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNotesByOrganisationRequest>._)).Returns(noteData);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(currentDate);

            const int pageNumber = 10;

            //act
            await ManageEvidenceController.Index(OrganisationId, ManageEvidenceNotesDisplayOptions.ViewAndTransferEvidence.ToDisplayString(), (int?)null, (int?)pageNumber);

            //assert
            A.CallTo(() => Mapper.Map<SchemeViewAndTransferManageEvidenceSchemeViewModel>(
                A<SchemeTabViewModelMapTransfer>.That.Matches(
                    a =>
                         a.PageNumber == pageNumber &&
                         a.PageSize == 10))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task IndexGet_GivenViewAndTransferTabAlongWithReturnedDataAndManageEvidenceNoteViewModel_ViewAndTransferEvidenceNotesViewModelShouldBeBuilt()
        {
            // Arrange
            var scheme = TestFixture.Create<SchemePublicInfo>();
            var noteData = TestFixture.Build<EvidenceNoteSearchDataResult>().Create();
            var currentDate = TestFixture.Create<DateTime>();
            var model = TestFixture.Create<ManageEvidenceNoteViewModel>();

            A.CallTo(() => Cache.FetchSchemePublicInfo(A<Guid>._)).Returns(scheme);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNotesByOrganisationRequest>._)).Returns(noteData);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(currentDate);

            //act
            await ManageEvidenceController.Index(OrganisationId, ManageEvidenceNotesDisplayOptions.ViewAndTransferEvidence.ToDisplayString(), null, null);

            //assert
            A.CallTo(() => Mapper.Map<SchemeViewAndTransferManageEvidenceSchemeViewModel>(
                A<SchemeTabViewModelMapTransfer>.That.Matches(
                    a => a.OrganisationId.Equals(OrganisationId) &&
                         a.NoteData == noteData &&
                         a.Scheme.Equals(scheme) &&
                         a.CurrentDate.Equals(currentDate) &&
                         a.PageNumber == 1 &&
                         a.PageSize == 10))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task IndexGet_GivenReviewSubmittedEvidenceNotesViewModel_ReviewSubmittedEvidenceNotesViewModelShouldBeReturned()
        {
            //arrange
            var model = TestFixture.Create<ReviewSubmittedManageEvidenceNotesSchemeViewModel>();

            A.CallTo(() => Mapper.Map<ReviewSubmittedManageEvidenceNotesSchemeViewModel>(A<SchemeTabViewModelMapTransfer>._)).Returns(model);

            //act
            var result = await ManageEvidenceController.Index(OrganisationId, "review-submitted-evidence", null, null) as ViewResult;

            //assert
            result.Model.Should().Be(model);
        }

        [Fact]
        public async Task IndexGet_GivenViewAndTransferTabAlongWithReturnedData_EvidenceNotesShouldBeRetrieved()
        {
            // Arrange
            var schemeName = Faker.Company.Name();
            var currentDate = TestFixture.Create<DateTime>();
            var expectedComplianceYear = currentDate.Month == 1 ? currentDate.Year - 1 : currentDate.Year;
            var status = new List<NoteStatus>()
            {
                NoteStatus.Approved,
                NoteStatus.Rejected,
                NoteStatus.Void,
                NoteStatus.Returned,
                NoteStatus.Cancelled
            };
            var noteTypes = new List<NoteType>() { NoteType.Evidence, NoteType.Transfer };

            A.CallTo(() => Cache.FetchSchemePublicInfo(A<Guid>._)).Returns(new SchemePublicInfo() { Name = schemeName });
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(currentDate);

            //act
            await ManageEvidenceController.Index(OrganisationId, ManageEvidenceNotesDisplayOptions.ViewAndTransferEvidence.ToDisplayString(), null, null);

            //assert
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNotesByOrganisationRequest>.That.Matches(
                g => g.OrganisationId.Equals(OrganisationId) &&
                     status.SequenceEqual(g.AllowedStatuses) &&
                     g.ComplianceYear.Equals(expectedComplianceYear) &&
                     g.TransferredOut == false &&
                     g.NoteTypeFilterList.SequenceEqual(noteTypes) &&
                     g.PageSize == 10 &&
                     g.PageNumber == 1 &&
                     g.SearchRef == null))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task IndexGet_GivenViewAndTransferTabAlongWithReturnedDataAndPageNumber_EvidenceNotesShouldBeRetrieved()
        {
            // Arrange
            var schemeName = Faker.Company.Name();
            var currentDate = TestFixture.Create<DateTime>();
            A.CallTo(() => Cache.FetchSchemePublicInfo(A<Guid>._)).Returns(new SchemePublicInfo() { Name = schemeName });
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(currentDate);

            const int pageNumber = 10;

            //act
            await ManageEvidenceController.Index(OrganisationId, "review-submitted-evidence", (int?)null, (int?)pageNumber);

            //assert
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNotesByOrganisationRequest>
                .That.Matches(g => g.PageSize == 10 &&
                                   g.PageNumber == pageNumber &&
                                   g.SearchRef == null))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task IndexGet_GivenViewAndTransferTabAlongWithReturnedDataAndManageEvidenceNoteViewModel_EvidenceNotesShouldBeRetrieved()
        {
            // Arrange
            var schemeName = Faker.Company.Name();
            var currentDate = TestFixture.Create<DateTime>();
            var status = new List<NoteStatus>()
            {
                NoteStatus.Approved,
                NoteStatus.Rejected,
                NoteStatus.Void,
                NoteStatus.Returned,
                NoteStatus.Cancelled
            };
            var complianceYear = TestFixture.Create<short>();
            var model = TestFixture.Build<ManageEvidenceNoteViewModel>()
                .With(e => e.SelectedComplianceYear, complianceYear)
                .Without(e => e.FilterViewModel)
                .Create();
            var noteTypes = new List<NoteType>() { NoteType.Evidence, NoteType.Transfer };

            A.CallTo(() => Cache.FetchSchemePublicInfo(A<Guid>._)).Returns(new SchemePublicInfo() { Name = schemeName });
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(currentDate);

            //act
            await ManageEvidenceController.Index(OrganisationId, ManageEvidenceNotesDisplayOptions.ViewAndTransferEvidence.ToDisplayString(), complianceYear, 1);

            //assert
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNotesByOrganisationRequest>.That.Matches(
                g => g.OrganisationId.Equals(OrganisationId) &&
                     status.SequenceEqual(g.AllowedStatuses) &&
                     g.ComplianceYear.Equals(model.SelectedComplianceYear) &&
                     g.NoteTypeFilterList.SequenceEqual(noteTypes) &&
                     g.PageSize == 10 &&
                     g.PageNumber == 1 &&
                     g.SearchRef == null))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task IndexGet_GivenViewAndTransferEvidenceNotesViewModel_SubmittedEvidenceNoteShouldNotBeRetrievedForInvalidStatus()
        {
            //act
            await ManageEvidenceController.Index(OrganisationId, ManageEvidenceNotesDisplayOptions.ViewAndTransferEvidence.ToDisplayString(), null, null);

            //asset
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNotesByOrganisationRequest>.That.Matches(
                g => g.AllowedStatuses.Contains(NoteStatus.Submitted)))).MustNotHaveHappened();
        }

        [Theory]
        [InlineData("view-and-transfer-evidence")]
        [InlineData("review-submitted-evidence")]
        [InlineData("outgoing-transfers")]
        public void IndexPost_GivenManageEvidenceNotesViewModel_ShouldRedirectWithCorrectFilterValues(string tab)
        {
            //arrange
            var complianceYear = TestFixture.Create<short>();

            var model = TestFixture.Build<ManageEvidenceNoteViewModel>()
                    .With(e => e.SelectedComplianceYear, complianceYear).Create();

            //act
            var result = ManageEvidenceController.Index(OrganisationId, tab, model, 1) as RedirectToRouteResult;

            //assert
            result.Should().NotBeNull();
            result.RouteValues["action"].Should().Be("Index");
            result.RouteValues["pcsId"].Should().Be(OrganisationId);
            result.RouteValues["tab"].Should().Be(tab);
            result.RouteValues["selectedComplianceYear"].Should().Be((int)complianceYear);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("evidence-summary")]
        public void IndexPost_GivenDefaultAndSummaryEvidenceTab_GivenPreviouslySelectedComplianceYear_ShouldRedirectWithCorrectValues(string tab)
        {
            // arrange
            var complianceYear = TestFixture.Create<short>();
            var model = TestFixture.Build<ManageEvidenceNoteViewModel>()
                .With(e => e.SelectedComplianceYear, complianceYear).Create();

            // act
            var result = ManageEvidenceController.Index(OrganisationId, tab, model, 1) as RedirectToRouteResult;

            // assert
            result.Should().NotBeNull();
            result.RouteValues["action"].Should().Be("Index");
            result.RouteValues["pcsId"].Should().Be(OrganisationId);
            result.RouteValues["tab"].Should().Be(tab);
            result.RouteValues["selectedComplianceYear"].Should().Be((int)complianceYear);
        }

        [Theory]
        [InlineData(null, true)]
        [InlineData("evidence-summary", true)]
        [InlineData(null, false)]
        [InlineData("evidence-summary", false)]
        public void IndexPost_GivenDefaultAndSummaryEvidenceTabAndManageEvidenceNoteViewModel_ShouldRedirectWithCorrectValues(string tab, bool balancingScheme)
        {
            // arrange
            var complianceYear = TestFixture.Create<short>();
            var model = TestFixture.Build<ManageEvidenceNoteViewModel>()
                .With(e => e.SelectedComplianceYear, complianceYear).Create();

            // act
            var result = ManageEvidenceController.Index(OrganisationId, tab, model, 1) as RedirectToRouteResult;

            // assert
            result.Should().NotBeNull();
            result.RouteValues["action"].Should().Be("Index");
            result.RouteValues["pcsId"].Should().Be(OrganisationId);
            result.RouteValues["tab"].Should().Be(tab);
            result.RouteValues["selectedComplianceYear"].Should().Be((int)complianceYear);
        }
    }
}