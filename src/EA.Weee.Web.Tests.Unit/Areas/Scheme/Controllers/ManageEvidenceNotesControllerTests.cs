﻿namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.Controllers
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

        [Theory]
        [InlineData("view-and-transfer-evidence")]
        [InlineData("review-submitted-evidence")]
        [InlineData("outgoing-transfers")]
        public async void IndexGet_GivenManageEvidenceNotesViewModel_ModelMapperShouldBeCalledWithCorrectValues(string tab)
        {
            //arrange
            var complianceYear = TestFixture.Create<short>();
            var currentDate = TestFixture.Create<DateTime>();

            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(currentDate);
            A.CallTo(() => Cache.FetchSchemePublicInfo(A<Guid>._)).Returns(new SchemePublicInfo() { SchemeId = Guid.NewGuid() });

            var model = TestFixture.Build<ManageEvidenceNoteViewModel>()
                    .With(e => e.SelectedComplianceYear, complianceYear).Create();

            //act
            await ManageEvidenceController.Index(OrganisationId, tab, model, 1);

            A.CallTo(() => Mapper.Map<ManageEvidenceNoteViewModel>(A<ManageEvidenceNoteTransfer>
                    .That.Matches(m =>
                            m.OrganisationId == OrganisationId &&
                            m.CurrentDate == currentDate &&
                            m.ComplianceYear == complianceYear &&
                            m.FilterViewModel == model.FilterViewModel)))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async void IndexGet_GivenManageEvidenceNotesViewModelForOutgoingTransferTab_ModelMapperShouldBeCalledWithCorrectValues()
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
        public async void IndexGet_GivenMappedManageEvidenceNotesViewModel_MappedModelShouldBeReturned(string tab)
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
        public async void IndexGet_GivenNullManageEvidenceNotesViewModel_ModelMapperShouldBeCalledWithCorrectValues(string tab)
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
            await ManageEvidenceController.Index(OrganisationId, "review-submitted-evidence", null, pageNumber);

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
            await ManageEvidenceController.Index(OrganisationId, "review-submitted-evidence", null, pageNumber);

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
            await ManageEvidenceController.Index(OrganisationId, ManageEvidenceNotesDisplayOptions.ViewAndTransferEvidence.ToDisplayString(), null, pageNumber);

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

        [Theory]
        [InlineData("view-and-transfer-evidence")]
        [InlineData("review-submitted-evidence")]
        [InlineData("outgoing-transfers")]
        public async Task IndexGet_GivenSelectedTabAndExistingManageEvidenceNoteViewModel_EvidenceNotesShouldBeRetrieved(string tab)
        {
            // Arrange
            var manageEvidenceNoteViewModel = TestFixture.Create<ManageEvidenceNoteViewModel>();
            var currentDate = TestFixture.Create<DateTime>();

            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(currentDate);

            //act
            await ManageEvidenceController.Index(OrganisationId, tab, manageEvidenceNoteViewModel, 1);

            //assert
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNotesByOrganisationRequest>.That.Matches(
                g => g.OrganisationId.Equals(OrganisationId) &&
                     g.ComplianceYear.Equals(manageEvidenceNoteViewModel.SelectedComplianceYear) &&
                     g.PageSize == 10 &&
                     g.PageNumber == 1 &&
                     g.SearchRef == manageEvidenceNoteViewModel.FilterViewModel.SearchRef))).MustHaveHappenedOnceExactly();
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
                NoteStatus.Returned
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
            await ManageEvidenceController.Index(OrganisationId, ManageEvidenceNotesDisplayOptions.ViewAndTransferEvidence.ToDisplayString(), null, pageNumber);

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
                NoteStatus.Returned
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
        public async Task IndexGet_GivenViewAndTransferEvidenceNotesViewModel_ViewAndTransferEvidenceNotesViewModelShouldBeReturned(string tab)
        {
            //arrange
            var model = TestFixture.Create<SchemeViewAndTransferManageEvidenceSchemeViewModel>();
            A.CallTo(() => Mapper.Map<SchemeViewAndTransferManageEvidenceSchemeViewModel>(A<SchemeTabViewModelMapTransfer>._)).Returns(model);

            //act
            var result = await ManageEvidenceController.Index(OrganisationId, tab, null, null) as ViewResult;

            //assert
            result.Model.Should().Be(model);
        }

        [Theory]
        [InlineData("view-and-transfer-evidence")]
        public async Task IndexGet_GivenViewAndTransferEvidenceNotesViewModelWithExistingManageEvidenceNoteViewModel_ViewAndTransferEvidenceNotesViewModelShouldBeReturned(string tab)
        {
            //arrange
            var manageEvidenceNoteViewModel = TestFixture.Create<ManageEvidenceNoteViewModel>();
            var model = TestFixture.Create<SchemeViewAndTransferManageEvidenceSchemeViewModel>();

            A.CallTo(() => Mapper.Map<SchemeViewAndTransferManageEvidenceSchemeViewModel>(A<SchemeTabViewModelMapTransfer>._)).Returns(model);

            //act
            var result = await ManageEvidenceController.Index(OrganisationId, tab, null, null) as ViewResult;

            //assert
            result.Model.Should().Be(model);
        }

        [Theory]
        [InlineData("view-and-transfer-evidence", "ViewAndTransferEvidence")]
        [InlineData("review-submitted-evidence", "ReviewSubmittedEvidence")]
        [InlineData("evidence-summary", "SummaryEvidence")]
        [InlineData("outgoing-transfers", "OutgoingTransfers")]
        public async void Index_GivenATabName_CorrectViewShouldBeReturned(string tab, string view)
        {
            var pcs = Guid.NewGuid();

            A.CallTo(() => Cache.FetchSchemePublicInfo(A<Guid>._)).Returns(new SchemePublicInfo() { SchemeId = Guid.NewGuid() });

            var result = await ManageEvidenceController.Index(pcs, tab, null, null) as ViewResult;

            result.ViewName.Should().Be(view);
        }

        [Fact]
        public async Task IndexGet_GivenOutgoingTransfersTab_EvidenceNoteDataShouldBeRetrieved()
        {
            // Arrange
            var statuses = GetOutgoingTransfersAllowedStatuses();
            var noteTypes = new List<NoteType>() { NoteType.Transfer };
            var schemeName = Faker.Company.Name();
            var evidenceData = TestFixture.Create<EvidenceNoteData>();
            var returnList = new List<EvidenceNoteData>() { evidenceData };
            var noteData = TestFixture.Build<EvidenceNoteSearchDataResult>()
                .With(e => e.Results, returnList).Create();
            var currentDate = TestFixture.Create<DateTime>();
            var expectedComplianceYear = currentDate.Month == 1 ? currentDate.Year - 1 : currentDate.Year;
            const int pageNumber = 1;
            const int pageSize = 10;

            A.CallTo(() => Cache.FetchSchemePublicInfo(A<Guid>._)).Returns(new SchemePublicInfo() { Name = schemeName });
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNotesByOrganisationRequest>._)).Returns(noteData);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(currentDate);

            //act
            await ManageEvidenceController.Index(OrganisationId, "outgoing-transfers", null, null);

            //assert
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNotesByOrganisationRequest>.That.Matches(
                g => g.OrganisationId.Equals(OrganisationId) &&
                     statuses.SequenceEqual(g.AllowedStatuses) &&
                     g.ComplianceYear.Equals(expectedComplianceYear) &&
                     g.TransferredOut == true &&
                     noteTypes.SequenceEqual(g.NoteTypeFilterList) &&
                     g.PageSize == pageSize &&
                     g.PageNumber == pageNumber &&
                     g.SearchRef == null))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task IndexGet_GivenOutgoingTransfersTabWithNullModel_RecipientWasteStatusFilterViewModelShouldBeNull()
        {
            // Arrange
            var model = TestFixture.Create<ManageEvidenceNoteViewModel>();

            //act
            await ManageEvidenceController.Index(OrganisationId, "outgoing-transfers", null, null);

            //assert
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNotesByOrganisationRequest>.That.Matches(
                g => g.NoteStatusFilter == null))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task IndexGet_GivenOutgoingTransfersTabAndPageNumber_EvidenceNoteDataShouldBeRetrieved()
        {
            // Arrange
            var statuses = GetOutgoingTransfersAllowedStatuses();
            var noteTypes = new List<NoteType>() { NoteType.Transfer };
            var schemeName = Faker.Company.Name();
            var evidenceData = TestFixture.Create<EvidenceNoteData>();
            var returnList = new List<EvidenceNoteData>() { evidenceData };
            var noteData = TestFixture.Build<EvidenceNoteSearchDataResult>()
                .With(e => e.Results, returnList).Create();
            var currentDate = TestFixture.Create<DateTime>();

            A.CallTo(() => Cache.FetchSchemePublicInfo(A<Guid>._)).Returns(new SchemePublicInfo() { Name = schemeName });
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNotesByOrganisationRequest>._)).Returns(noteData);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(currentDate);

            const int pageNumber = 10;
            const int pageSize = 10;

            //act
            await ManageEvidenceController.Index(OrganisationId, "outgoing-transfers", null, pageNumber);

            //assert
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNotesByOrganisationRequest>
                .That.Matches(g => g.PageSize == pageSize &&
                                   g.PageNumber == pageNumber))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task IndexGet_GivenOutgoingTransfersTabAndPreviouslySelectedComplianceYear_SubmittedEvidenceNoteShouldBeRetrieved()
        {
            // Arrange
            var statuses = GetOutgoingTransfersAllowedStatuses();
            var noteTypes = new List<NoteType>() { NoteType.Transfer };
            var schemeName = Faker.Company.Name();
            var evidenceData = TestFixture.Create<EvidenceNoteData>();
            var returnList = new List<EvidenceNoteData>() { evidenceData };
            var noteData = TestFixture.Build<EvidenceNoteSearchDataResult>()
                .With(e => e.Results, returnList).Create();
            var currentDate = TestFixture.Create<DateTime>();
            var complianceYear = TestFixture.Create<short>();

            var model = TestFixture.Build<ManageEvidenceNoteViewModel>()
                .With(e => e.SelectedComplianceYear, complianceYear)
                .Without(e => e.FilterViewModel)
                .Create();

            A.CallTo(() => Cache.FetchSchemePublicInfo(A<Guid>._)).Returns(new SchemePublicInfo() { Name = schemeName });
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNotesByOrganisationRequest>._)).Returns(noteData);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(currentDate);

            const int pageNumber = 1;
            const int pageSize = 10;

            //act
            await ManageEvidenceController.Index(OrganisationId, "outgoing-transfers", complianceYear, pageNumber);

            //assert
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNotesByOrganisationRequest>.That.Matches(
                g => g.OrganisationId.Equals(OrganisationId) &&
                     statuses.SequenceEqual(g.AllowedStatuses) &&
                     g.ComplianceYear.Equals(complianceYear) &&
                     g.TransferredOut == true &&
                     noteTypes.SequenceEqual(g.NoteTypeFilterList) &&
                     g.PageSize == pageSize &&
                     g.PageNumber == pageNumber &&
                     g.SearchRef == null))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task IndexGet_GivenRequestIsCreated_SessionShouldBeUpdated()
        {
            // arrange
            A.CallTo(() => Cache.FetchSchemePublicInfo(A<Guid>._)).Returns(new SchemePublicInfo() { SchemeId = TestFixture.Create<Guid>() });

            // act
            await ManageEvidenceController.Index(OrganisationId, null, null, null);

            // assert
            A.CallTo(() => SessionService.ClearTransferSessionObject(SessionKeyConstant.TransferNoteKey))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task IndexGet_GivenRequestIsCreated_EditTransferTonnageViewModelKeySessionShouldBeUpdated()
        {
            // arrange
            A.CallTo(() => Cache.FetchSchemePublicInfo(A<Guid>._)).Returns(new SchemePublicInfo() { SchemeId = TestFixture.Create<Guid>() });

            // act
            await ManageEvidenceController.Index(OrganisationId, null, null, null);

            // assert
            A.CallTo(() => SessionService.ClearTransferSessionObject(SessionKeyConstant.EditTransferTonnageViewModelKey)).MustHaveHappenedOnceExactly();
        }

        public static IEnumerable<object[]> ManageEvidenceModelData =>
            new List<object[]>
            {
                new object[] { null },
                new object[] { new ManageEvidenceNoteViewModel() },
            };

        [Theory]
        [MemberData(nameof(ManageEvidenceModelData))]
        public async Task IndexGet_GivenOutgoingTransfersTabWithReturnedData_ViewModelShouldBeBuilt(ManageEvidenceNoteViewModel model)
        {
            // Arrange
            var scheme = TestFixture.Create<SchemePublicInfo>();
            var noteData = TestFixture.Build<EvidenceNoteSearchDataResult>().Create();
            var currentDate = TestFixture.Create<DateTime>();

            A.CallTo(() => Cache.FetchSchemePublicInfo(A<Guid>._)).Returns(scheme);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNotesByOrganisationRequest>._)).Returns(noteData);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(currentDate);

            const int pageNumber = 1;
            const int pageSize = 10;

            //act
            await ManageEvidenceController.Index(OrganisationId, "outgoing-transfers", null, null);

            //assert
            A.CallTo(() => Mapper.Map<TransferredOutEvidenceNotesSchemeViewModel>(
                A<SchemeTabViewModelMapTransfer>.That.Matches(
                    a => a.OrganisationId.Equals(OrganisationId) &&
                         a.NoteData == noteData &&
                         a.Scheme.Equals(scheme) &&
                         a.CurrentDate.Equals(currentDate) &&
                         a.PageNumber == pageNumber &&
                         a.PageSize == pageSize))).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [MemberData(nameof(ManageEvidenceModelData))]
        public async Task IndexGet_GivenOutgoingTransfersTabWithReturnedDataWithPageNumber_ViewModelShouldBeBuilt(ManageEvidenceNoteViewModel model)
        {
            // Arrange
            var scheme = TestFixture.Create<SchemePublicInfo>();
            var noteData = TestFixture.Build<EvidenceNoteSearchDataResult>().Create();
            var currentDate = TestFixture.Create<DateTime>();

            A.CallTo(() => Cache.FetchSchemePublicInfo(A<Guid>._)).Returns(scheme);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNotesByOrganisationRequest>._)).Returns(noteData);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(currentDate);

            const int pageNumber = 10;
            const int pageSize = 10;

            //act
            await ManageEvidenceController.Index(OrganisationId, "outgoing-transfers", model, pageNumber);

            //assert
            A.CallTo(() => Mapper.Map<TransferredOutEvidenceNotesSchemeViewModel>(
                A<SchemeTabViewModelMapTransfer>.That.Matches(
                    a => a.PageNumber == pageNumber &&
                         a.PageSize == pageSize))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task IndexGet_GivenTransferredOutEvidenceNotesSchemeViewModel_TransferredOutEvidenceNotesViewModelMapShouldBeReturned()
        {
            //arrange
            var model = TestFixture.Create<TransferredOutEvidenceNotesSchemeViewModel>();

            A.CallTo(() => Mapper.Map<TransferredOutEvidenceNotesSchemeViewModel>(A<SchemeTabViewModelMapTransfer>._)).Returns(model);

            //act
            var result = await ManageEvidenceController.Index(OrganisationId, "outgoing-transfers", null, null) as ViewResult;

            //assert
            result.Model.Should().Be(model);
        }

        [Fact]
        public void TransferPost_PageRedirectsToTransferPage()
        {
            //arrange
            var complianceYear = TestFixture.Create<int>();

            //act
            var result = ManageEvidenceController.Transfer(OrganisationId, complianceYear) as RedirectToRouteResult;

            //assert
            result.RouteValues["action"].Should().Be("TransferEvidenceNote");
            result.RouteValues["controller"].Should().Be("TransferEvidence");
            result.RouteValues["pcsId"].Should().Be(OrganisationId);
            result.RouteValues["complianceYear"].Should().Be(complianceYear);
        }

        [Fact]
        public async Task CreateAndPopulateEvidenceSummaryViewModel_RedirectsToSummaryEvidencePBSPage()
        {
            //arrange
            var currentDate = SystemTime.Now;
            var complianceYear = currentDate.Year;
            var schemeInfo = TestFixture.Build<SchemePublicInfo>().With(s => s.IsBalancingScheme, true).Create();
            var pcsId = TestFixture.Create<Guid>();
            var manageEvidenceNoteViewModel = TestFixture.Create<ManageEvidenceNoteViewModel>();
            var pbsSummaryViewModel = TestFixture.Create<SummaryEvidenceViewModel>();
            A.CallTo(() => Mapper.Map<SummaryEvidenceViewModel>(A<ViewEvidenceSummaryViewModelMapTransfer>._)).Returns(pbsSummaryViewModel);

            //act
            var result = await ManageEvidenceController.CreateAndPopulateEvidenceSummaryViewModel(pcsId, schemeInfo, currentDate, manageEvidenceNoteViewModel, complianceYear) as ViewResult;

            //assert
            result.Model.Should().Be(pbsSummaryViewModel);
            result.ViewName.Should().Be("SummaryEvidencePBS");
        }

        [Fact]
        public async Task IndexGet_ViewAndTransferEvidenceTab_MapperShouldCorrectlySetPageNumber()
        {
            // Arrange
            var scheme = TestFixture.Create<SchemePublicInfo>();
            var noteData = TestFixture.Build<EvidenceNoteSearchDataResult>().Create();
            var currentDate = TestFixture.Create<DateTime>();
            var model = TestFixture.Create<ManageEvidenceNoteViewModel>();
            var pageNumber = 3;

            A.CallTo(() => Cache.FetchSchemePublicInfo(A<Guid>._)).Returns(scheme);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNotesByOrganisationRequest>._)).Returns(noteData);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(currentDate);

            //act
            await ManageEvidenceController.Index(OrganisationId, "view-and-transfer-evidence", model, pageNumber);

            //assert
            A.CallTo(() => Mapper.Map<SchemeViewAndTransferManageEvidenceSchemeViewModel>(
                A<SchemeTabViewModelMapTransfer>.That.Matches(
                    a => a.OrganisationId.Equals(OrganisationId) &&
                         a.NoteData == noteData &&
                         a.Scheme.Equals(scheme) &&
                         a.CurrentDate.Equals(currentDate) &&
                         a.PageNumber == pageNumber &&
                         a.PageSize == 10))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task ViewEvidenceNoteGet_GivenPageNumber_ViewBagShouldBePopulatedWithPageNumber()
        {
            // Arrange
            var pageNumber = 3;

            //act
            var result = await ManageEvidenceController.ViewEvidenceNote(OrganisationId, TestFixture.Create<Guid>(), "view-and-transfer-evidence", pageNumber) as ViewResult;

            //assert
            Assert.Equal(pageNumber, result.ViewBag.Page);
        }

        [Fact]
        public async Task ViewEvidenceNoteGet_GivenDefaultOpenedInNewTab_MapperShouldBeCalledWithDefaultOpenInNewTab()
        {
            //act
            await ManageEvidenceController.ViewEvidenceNote(OrganisationId, TestFixture.Create<Guid>());

            //assert
            A.CallTo(() =>
                    Mapper.Map<ViewEvidenceNoteViewModel>(A<ViewEvidenceNoteMapTransfer>.That.Matches(v => v.OpenedInNewTab == false)))
                .MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ViewEvidenceNoteGet_GivenOpenedInNewTab_MapperShouldBeCalledWithOpenInNewTab(bool openedInNewTab)
        {
            //arrange
            //act
            await ManageEvidenceController.ViewEvidenceNote(OrganisationId, TestFixture.Create<Guid>(), openedInNewTab: openedInNewTab);

            //assert
            A.CallTo(() =>
                    Mapper.Map<ViewEvidenceNoteViewModel>(A<ViewEvidenceNoteMapTransfer>.That.Matches(v => v.OpenedInNewTab == openedInNewTab)))
                .MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("evidence-summary")]
        public async Task IndexGet_GivenDefaultAndSummaryEvidenceTab_GivenSchemeIsNotBalancing_ObligationEvidenceSummaryDataShouldBeRetrieved(string tab)
        {
            // arrange
            var schemeId = TestFixture.Create<Guid>();
            var scheme = TestFixture.Build<SchemePublicInfo>()
              .With(s => s.IsBalancingScheme, false)
              .With(s => s.SchemeId, schemeId)
              .Create();
            var obligationSummaryData = TestFixture.Create<ObligationEvidenceSummaryData>();
            var currentDate = TestFixture.Create<DateTime>();
            var expectedComplianceYear = currentDate.Month == 1 ? currentDate.Year - 1 : currentDate.Year;

            A.CallTo(() => Cache.FetchSchemePublicInfo(A<Guid>._)).Returns(scheme);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(currentDate);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetObligationSummaryRequest>._)).Returns(obligationSummaryData);

            // act
            await ManageEvidenceController.Index(OrganisationId, tab, null, null);

            // assert
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetObligationSummaryRequest>.That.Matches(
                g => g.OrganisationId.Equals(OrganisationId) &&
                     g.ComplianceYear.Equals(expectedComplianceYear) &&
                     g.SchemeId.Equals(schemeId)))).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("evidence-summary")]
        public async Task IndexGet_GivenDefaultAndSummaryEvidenceTab_GivenSchemeIsBalancing_ObligationEvidenceSummaryDataShouldBeRetrieved(string tab)
        {
            // arrange
            var schemeId = TestFixture.Create<Guid>();
            var scheme = TestFixture.Build<SchemePublicInfo>()
                .With(s => s.IsBalancingScheme, true)
                .With(s => s.SchemeId, schemeId)
                .Create();
            var obligationSummaryData = TestFixture.Create<ObligationEvidenceSummaryData>();
            var currentDate = TestFixture.Create<DateTime>();
            var expectedComplianceYear = currentDate.Month == 1 ? currentDate.Year - 1 : currentDate.Year;

            A.CallTo(() => Cache.FetchSchemePublicInfo(A<Guid>._)).Returns(scheme);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(currentDate);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetObligationSummaryRequest>._)).Returns(obligationSummaryData);

            // act
            await ManageEvidenceController.Index(OrganisationId, tab, null, null);

            // assert
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetObligationSummaryRequest>.That.Matches(
                g => g.OrganisationId.Equals(OrganisationId) &&
                     g.ComplianceYear.Equals(expectedComplianceYear) &&
                     g.SchemeId == null))).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("evidence-summary")]
        public async Task IndexGet_GivenDefaultAndSummaryEvidenceTab_GivenPreviouslySelectedComplianceYear_GivenSchemeIsNotBalancing_ObligationEvidenceSummaryDataShouldBeRetrieved(string tab)
        {
            // arrange
            var schemeId = TestFixture.Create<Guid>();
            var scheme = TestFixture.Build<SchemePublicInfo>()
              .With(s => s.IsBalancingScheme, false)
              .With(s => s.SchemeId, schemeId)
              .Create();
            var obligationSummaryData = TestFixture.Create<ObligationEvidenceSummaryData>();
            var currentDate = TestFixture.Create<DateTime>();
            var complianceYear = TestFixture.Create<short>();
            var model = TestFixture.Build<ManageEvidenceNoteViewModel>()
                .With(e => e.SelectedComplianceYear, complianceYear).Create();

            A.CallTo(() => Cache.FetchSchemePublicInfo(A<Guid>._)).Returns(scheme);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetObligationSummaryRequest>._))
                .Returns(obligationSummaryData);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(currentDate);

            // act
            await ManageEvidenceController.Index(OrganisationId, tab, model, 1);

            // assert
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetObligationSummaryRequest>.That.Matches(
                g => g.OrganisationId.Equals(OrganisationId) &&
                     g.ComplianceYear.Equals(complianceYear) &&
                     g.SchemeId.Equals(schemeId)))).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("evidence-summary")]
        public async Task IndexGet_GivenDefaultAndSummaryEvidenceTab_GivenPreviouslySelectedComplianceYear_GivenSchemeIsBalancing_ObligationEvidenceSummaryDataShouldBeRetrieved(string tab)
        {
            // arrange
            var schemeId = TestFixture.Create<Guid>();
            var scheme = TestFixture.Build<SchemePublicInfo>()
                .With(s => s.IsBalancingScheme, true)
                .With(s => s.SchemeId, schemeId)
                .Create();
            var obligationSummaryData = TestFixture.Create<ObligationEvidenceSummaryData>();
            var currentDate = TestFixture.Create<DateTime>();
            var complianceYear = TestFixture.Create<short>();
            var model = TestFixture.Build<ManageEvidenceNoteViewModel>()
                .With(e => e.SelectedComplianceYear, complianceYear).Create();

            A.CallTo(() => Cache.FetchSchemePublicInfo(A<Guid>._)).Returns(scheme);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetObligationSummaryRequest>._))
                .Returns(obligationSummaryData);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(currentDate);

            // act
            await ManageEvidenceController.Index(OrganisationId, tab, model, 1);

            // assert
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetObligationSummaryRequest>.That.Matches(
                g => g.OrganisationId.Equals(OrganisationId) &&
                     g.ComplianceYear.Equals(complianceYear) &&
                     g.SchemeId == null))).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(null, true)]
        [InlineData("evidence-summary", true)]
        [InlineData(null, false)]
        [InlineData("evidence-summary", false)]
        public async Task IndexGet_GivenDefaultAndSummaryEvidenceTabAlongWithReturnedData_GivenBalancingScheme_ViewModelShouldBeBuilt(string tab, bool balancingScheme)
        {
            // arrange
            var schemeId = TestFixture.Create<Guid>();
            var scheme = TestFixture.Build<SchemePublicInfo>()
              .With(s => s.IsBalancingScheme, balancingScheme)
              .With(s => s.SchemeId, schemeId)
              .Create();
            var obligationSummaryData = TestFixture.Create<ObligationEvidenceSummaryData>();
            var currentDate = TestFixture.Create<DateTime>();
            var expectedComplianceYear = currentDate.Month == 1 ? currentDate.Year - 1 : currentDate.Year;

            A.CallTo(() => Cache.FetchSchemePublicInfo(A<Guid>._)).Returns(scheme);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetObligationSummaryRequest>._)).Returns(obligationSummaryData);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(currentDate);

            // act
            await ManageEvidenceController.Index(OrganisationId, tab, null, null);

            // assert
            A.CallTo(() => Mapper.Map<SummaryEvidenceViewModel>(
                A<ViewEvidenceSummaryViewModelMapTransfer>.That.Matches(
                    a => a.OrganisationId.Equals(OrganisationId) &&
                         a.ComplianceYear.Equals(expectedComplianceYear) &&
                         a.Scheme.Equals(scheme) &&
                         a.CurrentDate.Equals(currentDate) &&
                         a.ObligationEvidenceSummaryData.Equals(obligationSummaryData)))).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(null, true)]
        [InlineData("evidence-summary", true)]
        [InlineData(null, false)]
        [InlineData("evidence-summary", false)]
        public async Task IndexGet_GivenDefaultAndSummaryEvidenceTabAlongWithReturnedDataAndManageEvidenceNoteViewModel_GivenBalancingScheme_ViewModelShouldBeBuilt(string tab, bool balancingScheme)
        {
            // arrange
            var schemeId = TestFixture.Create<Guid>();
            var scheme = TestFixture.Build<SchemePublicInfo>()
                .With(s => s.IsBalancingScheme, balancingScheme)
                .With(s => s.SchemeId, schemeId)
                .Create();
            var obligationSummaryData = TestFixture.Create<ObligationEvidenceSummaryData>();
            var currentDate = TestFixture.Create<DateTime>();
            var complianceYear = TestFixture.Create<short>();
            var model = TestFixture.Build<ManageEvidenceNoteViewModel>()
                .With(e => e.SelectedComplianceYear, complianceYear).Create();

            A.CallTo(() => Cache.FetchSchemePublicInfo(A<Guid>._)).Returns(scheme);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetObligationSummaryRequest>._)).Returns(obligationSummaryData);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(currentDate);

            // act
            await ManageEvidenceController.Index(OrganisationId, tab, model, 1);

            // assert
            A.CallTo(() => Mapper.Map<SummaryEvidenceViewModel>(
                A<ViewEvidenceSummaryViewModelMapTransfer>.That.Matches(
                    a => a.OrganisationId.Equals(OrganisationId) &&
                         a.ComplianceYear.Equals(complianceYear) &&
                         a.Scheme.Equals(scheme) &&
                         a.CurrentDate.Equals(currentDate) &&
                         a.ObligationEvidenceSummaryData.Equals(obligationSummaryData) &&
                         a.ManageEvidenceNoteViewModel.Equals(model)))).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(null, true)]
        [InlineData("evidence-summary", true)]
        [InlineData(null, false)]
        [InlineData("evidence-summary", false)]
        public async Task IndexGet_GivenDefaultAndSummaryEvidenceNotesViewModel_GivenBalancingScheme_SummaryEvidenceViewModelShouldBeReturned(string tab, bool balancingScheme)
        {
            // arrange
            var schemeId = TestFixture.Create<Guid>();
            var scheme = TestFixture.Build<SchemePublicInfo>()
              .With(s => s.IsBalancingScheme, balancingScheme)
              .With(s => s.SchemeId, schemeId)
              .Create();
            A.CallTo(() => Cache.FetchSchemePublicInfo(A<Guid>._)).Returns(scheme);

            var model = TestFixture.Create<SummaryEvidenceViewModel>();

            A.CallTo(() => Mapper.Map<SummaryEvidenceViewModel>(A<ViewEvidenceSummaryViewModelMapTransfer>._)).Returns(model);

            // act
            var result = await ManageEvidenceController.Index(OrganisationId, tab, null, null) as ViewResult;

            // assert
            result.Model.Should().Be(model);
        }

        private List<NoteStatus> GetOutgoingTransfersAllowedStatuses()
        {
            return new List<NoteStatus>
            {
                NoteStatus.Draft,
                NoteStatus.Approved,
                NoteStatus.Rejected,
                NoteStatus.Submitted,
                NoteStatus.Void,
                NoteStatus.Returned
            };
        }
    }
}