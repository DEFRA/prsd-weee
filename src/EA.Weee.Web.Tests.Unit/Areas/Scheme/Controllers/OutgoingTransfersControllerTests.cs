namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.Controllers
{
    using Api.Client;
    using AutoFixture;
    using Constant;
    using Core.AatfEvidence;
    using Core.Helpers;
    using Core.Scheme;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.Shared;
    using EA.Weee.Web.Areas.Scheme.Controllers;
    using EA.Weee.Web.Areas.Scheme.ViewModels.ManageEvidenceNotes;
    using EA.Weee.Web.Extensions;
    using EA.Weee.Web.ViewModels.Shared;
    using FakeItEasy;
    using FluentAssertions;
    using Services;
    using Services.Caching;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using AutoFixture.Kernel;
    using Web.Areas.Scheme.Attributes;
    using Web.Areas.Scheme.Mappings.ToViewModels;
    using Web.Areas.Scheme.Requests;
    using Web.Areas.Scheme.ViewModels;
    using Web.Filters;
    using Web.Infrastructure;
    using Weee.Requests.AatfEvidence;
    using Weee.Requests.Scheme;
    using Weee.Tests.Core;
    using Weee.Tests.Core.DataHelpers;
    using Xunit;

    public class OutgoingTransfersControllerTests : SimpleUnitTestBase
    {
        private readonly IWeeeClient weeeClient;
        private readonly IMapper mapper;
        private readonly ISessionService sessionService;
        private readonly ITransferEvidenceRequestCreator transferEvidenceRequestCreator;
        private readonly OutgoingTransfersController outgoingTransferEvidenceController;
        private readonly ConfigurationService configurationService;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private readonly Guid organisationId;
        private readonly TransferEvidenceNoteData transferEvidenceNoteData;
        private readonly EvidenceNoteSearchDataResult evidenceNoteData;
        private readonly TransferEvidenceTonnageViewModel transferEvidenceTonnageViewModel;
        private const int DefaultPageSize = 11;

        public OutgoingTransfersControllerTests()
        {
            weeeClient = A.Fake<IWeeeClient>();
            breadcrumb = A.Fake<BreadcrumbService>();
            cache = A.Fake<IWeeeCache>();
            mapper = A.Fake<IMapper>();
            sessionService = A.Fake<ISessionService>();
            transferEvidenceRequestCreator = A.Fake<ITransferEvidenceRequestCreator>();
            configurationService = A.Fake<ConfigurationService>();

            organisationId = Guid.NewGuid();

            A.CallTo(() => configurationService.CurrentConfiguration.DefaultExternalPagingPageSize)
                .Returns(DefaultPageSize);

            outgoingTransferEvidenceController =
                new OutgoingTransfersController(mapper, breadcrumb, cache, () => weeeClient, sessionService,
                    transferEvidenceRequestCreator, configurationService);

            transferEvidenceNoteData = TestFixture.Build<TransferEvidenceNoteData>()
                .With(n => n.Status, NoteStatus.Submitted).Create();

            evidenceNoteData =
                new EvidenceNoteSearchDataResult(TestFixture.CreateMany<EvidenceNoteData>(3).ToList(), 3);

            transferEvidenceTonnageViewModel =
                TestFixture.Build<TransferEvidenceTonnageViewModel>()
                    .With(t => t.PcsId, Guid.NewGuid())
                    .With(t => t.RecipientId, Guid.NewGuid())
                    .With(t => t.ViewTransferNoteViewModel,
                        TestFixture.Build<ViewTransferNoteViewModel>()
                            .With(v => v.EvidenceNoteId, Guid.NewGuid)
                            .Create())
                    .Create();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetTransferEvidenceNoteForSchemeRequest>._))
                .Returns(transferEvidenceNoteData);

            A.CallTo(() => sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(A<string>._)).Returns(null);

            A.CallTo(() =>
                    transferEvidenceRequestCreator.EditSelectTonnageToRequest(A<EditTransferEvidenceNoteRequest>._,
                        A<TransferEvidenceTonnageViewModel>._))
                .Returns(TestFixture.Create<EditTransferEvidenceNoteRequest>());

            TestFixture.Customizations.Add(new TypeRelay(typeof(ISessionService), typeof(SessionService)));
        }

        [Fact]
        public void OutgoingTransfersController_ShouldDeriveFromTransferEvidenceBaseController()
        {
            typeof(OutgoingTransfersController).Should().BeDerivedFrom<TransferEvidenceBaseController>();
        }

        [Fact]
        public void EditTransferFromGet_ShouldHaveHttpGetAttribute()
        {
            typeof(OutgoingTransfersController)
                .GetMethod("EditTransferFrom", new[] { typeof(Guid), typeof(Guid), typeof(int), typeof(string) })
                .Should()
                .BeDecoratedWith<HttpGetAttribute>();
        }

        [Fact]
        public void EditTransferFromGet_ShouldHaveNoCacheFilterAttribute()
        {
            typeof(OutgoingTransfersController)
                .GetMethod("EditTransferFrom", new[] { typeof(Guid), typeof(Guid), typeof(int), typeof(string) })
                .Should()
                .BeDecoratedWith<NoCacheFilterAttribute>();
        }

        [Fact]
        public void EditTransferFromGet_ShouldHaveCheckCanEditTransferNoteAttribute()
        {
            typeof(OutgoingTransfersController)
                .GetMethod("EditTransferFrom", new[] { typeof(Guid), typeof(Guid), typeof(int), typeof(string) })
                .Should()
                .BeDecoratedWith<CheckCanEditTransferNoteAttribute>();
        }

        [Fact]
        public void EditTonnagesGet_ShouldHaveHttpGetAttribute()
        {
            typeof(OutgoingTransfersController)
                .GetMethod("EditTonnages", new[] { typeof(Guid), typeof(Guid), typeof(bool?) }).Should()
                .BeDecoratedWith<HttpGetAttribute>();
        }

        [Fact]
        public void EditTonnagesGet_ShouldHaveNoCacheFilterAttribute()
        {
            typeof(OutgoingTransfersController)
                .GetMethod("EditTonnages", new[] { typeof(Guid), typeof(Guid), typeof(bool?) }).Should()
                .BeDecoratedWith<NoCacheFilterAttribute>();
        }

        [Fact]
        public void EditTonnagesGet_ShouldHaveCheckCanEditTransferNoteAttribute()
        {
            typeof(OutgoingTransfersController)
                .GetMethod("EditTonnages", new[] { typeof(Guid), typeof(Guid), typeof(bool?) }).Should()
                .BeDecoratedWith<CheckCanEditTransferNoteAttribute>();
        }

        [Fact]
        public void EditCategoriesGet_ShouldHaveHttpGetAttribute()
        {
            typeof(OutgoingTransfersController)
                .GetMethod("EditCategories", new[] { typeof(Guid), typeof(Guid), typeof(int) })
                .Should().BeDecoratedWith<HttpGetAttribute>();
        }

        [Fact]
        public void EditCategoriesGet_ShouldHaveNoCacheFilterAttribute()
        {
            typeof(OutgoingTransfersController)
                .GetMethod("EditCategories", new[] { typeof(Guid), typeof(Guid), typeof(int) })
                .Should().BeDecoratedWith<NoCacheFilterAttribute>();
        }

        [Fact]
        public void EditCategoriesGet_ShouldHaveCheckCanEditTransferNoteAttribute()
        {
            typeof(OutgoingTransfersController)
                .GetMethod("EditCategories", new[] { typeof(Guid), typeof(Guid), typeof(int) })
                .Should().BeDecoratedWith<CheckCanEditTransferNoteAttribute>();
        }

        [Fact]
        public void SubmittedTransferGet_ShouldHaveHttpGetAttribute()
        {
            typeof(OutgoingTransfersController).GetMethod("SubmittedTransfer",
                    new[] { typeof(Guid), typeof(Guid), typeof(bool?), typeof(string) }).Should()
                .BeDecoratedWith<HttpGetAttribute>();
        }

        [Fact]
        public void SubmittedTransferGet_ShouldHaveNoCacheAttribute()
        {
            typeof(OutgoingTransfersController).GetMethod("SubmittedTransfer",
                    new[] { typeof(Guid), typeof(Guid), typeof(bool?), typeof(string) }).Should()
                .BeDecoratedWith<NoCacheFilterAttribute>();
        }

        [Fact]
        public void SubmittedTransferGet_ShouldHaveCheckCanEditTransferNoteAttribute()
        {
            typeof(OutgoingTransfersController).GetMethod("SubmittedTransfer",
                    new[] { typeof(Guid), typeof(Guid), typeof(bool?), typeof(string) }).Should()
                .BeDecoratedWith<CheckCanEditTransferNoteAttribute>();
        }

        [Fact]
        public void SubmittedTransferPost_ShouldHaveHttpPostAttribute()
        {
            typeof(OutgoingTransfersController)
                .GetMethod("SubmittedTransfer", new[] { typeof(ReviewTransferNoteViewModel) }).Should()
                .BeDecoratedWith<HttpPostAttribute>();
        }

        [Fact]
        public void SubmittedTransferPost_ShouldHaveAntiforgeryAttribute()
        {
            typeof(OutgoingTransfersController)
                .GetMethod("SubmittedTransfer", new[] { typeof(ReviewTransferNoteViewModel) }).Should()
                .BeDecoratedWith<ValidateAntiForgeryTokenAttribute>();
        }

        [Fact]
        public void EditTransferFromPost_ShouldHaveHttpPostAttribute()
        {
            typeof(OutgoingTransfersController)
                .GetMethod("EditTransferFrom", new[] { typeof(TransferEvidenceNotesViewModel) }).Should()
                .BeDecoratedWith<HttpPostAttribute>();
        }

        [Fact]
        public void EditDraftTransferGet_ShouldHaveHttpGetAttribute()
        {
            typeof(OutgoingTransfersController).GetMethod("EditDraftTransfer",
                    new[] { typeof(Guid), typeof(Guid), typeof(bool?), typeof(string), typeof(int), typeof(string) }).Should()
                .BeDecoratedWith<HttpGetAttribute>();
        }

        [Fact]
        public void EditDraftTransferGet_ShouldHaveNoCacheFilterAttribute()
        {
            typeof(OutgoingTransfersController).GetMethod("EditDraftTransfer",
                    new[] { typeof(Guid), typeof(Guid), typeof(bool?), typeof(string), typeof(int), typeof(string) }).Should()
                .BeDecoratedWith<NoCacheFilterAttribute>();
        }

        [Fact]
        public void EditDraftTransfer_ShouldHaveCheckCanEditTransferNoteAttribute()
        {
            typeof(OutgoingTransfersController).GetMethod("EditDraftTransfer",
                    new[] { typeof(Guid), typeof(Guid), typeof(bool?), typeof(string), typeof(int), typeof(string) }).Should()
                .BeDecoratedWith<CheckCanEditTransferNoteAttribute>();
        }

        [Fact]
        public void EditCategoriesPost_ShouldHaveHttpPostAttribute()
        {
            typeof(OutgoingTransfersController)
                .GetMethod("EditCategories", new[] { typeof(TransferEvidenceNoteCategoriesViewModel) }).Should()
                .BeDecoratedWith<HttpPostAttribute>();
        }

        [Fact]
        public void EditTonnagesPost_ShouldHaveHttpPostAttribute()
        {
            typeof(OutgoingTransfersController)
                .GetMethod("EditTonnages", new[] { typeof(TransferEvidenceTonnageViewModel) }).Should()
                .BeDecoratedWith<HttpPostAttribute>();
        }

        [Fact]
        public void EditTonnagesPost_ShouldHaveAntiforgeryAttribute()
        {
            typeof(OutgoingTransfersController)
                .GetMethod("EditTonnages", new[] { typeof(TransferEvidenceTonnageViewModel) }).Should()
                .BeDecoratedWith<ValidateAntiForgeryTokenAttribute>();
        }

        [Fact]
        public void SelectEvidenceNotePost_ShouldHaveHttpPostAttribute()
        {
            typeof(OutgoingTransfersController)
                .GetMethod("SelectEvidenceNote", new[] { typeof(TransferSelectEvidenceNoteModel), typeof(string) }).Should()
                .BeDecoratedWith<HttpPostAttribute>();
        }

        [Fact]
        public void SelectEvidenceNotePost_ShouldHaveAntiForgeryAttribute()
        {
            typeof(OutgoingTransfersController)
                .GetMethod("SelectEvidenceNote", new[] { typeof(TransferSelectEvidenceNoteModel), typeof(string) }).Should()
                .BeDecoratedWith<ValidateAntiForgeryTokenAttribute>();
        }

        [Fact]
        public void DeselectEvidenceNotePost_ShouldHaveHttpPostAttribute()
        {
            typeof(OutgoingTransfersController)
                .GetMethod("DeselectEvidenceNote", new[] { typeof(TransferDeselectEvidenceNoteModel) }).Should()
                .BeDecoratedWith<HttpPostAttribute>();
        }

        [Fact]
        public void DeselectEvidenceNotePost_ShouldHaveAntiForgeryAttribute()
        {
            typeof(OutgoingTransfersController)
                .GetMethod("DeselectEvidenceNote", new[] { typeof(TransferDeselectEvidenceNoteModel) }).Should()
                .BeDecoratedWith<ValidateAntiForgeryTokenAttribute>();
        }

        [Fact]
        public async Task EditTonnagesGet_GivenOrganisationId_SchemeShouldBeRetrievedFromCache()
        {
            //act
            await outgoingTransferEvidenceController.EditTonnages(organisationId, TestFixture.Create<Guid>(),
                TestFixture.Create<bool?>());

            //asset
            A.CallTo(() => cache.FetchSchemePublicInfo(organisationId)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditTonnagesGet_GivenSchemeIsNotBalancingScheme_BreadcrumbShouldBeSet()
        {
            // arrange 
            var organisationName = "OrganisationName";
            var schemeInfo = TestFixture.Build<SchemePublicInfo>().With(s => s.IsBalancingScheme, false).Create();
            A.CallTo(() => cache.FetchSchemePublicInfo(organisationId)).Returns(schemeInfo);
            A.CallTo(() => cache.FetchOrganisationName(organisationId)).Returns(organisationName);

            // act
            await outgoingTransferEvidenceController.EditTonnages(organisationId, TestFixture.Create<Guid>(),
                TestFixture.Create<bool?>());

            // assert
            breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.SchemeManageEvidence);
            breadcrumb.OrganisationId.Should().Be(organisationId);
        }

        [Fact]
        public async Task EditTonnagesGet_GivenSchemeIsBalancingScheme_BreadcrumbShouldBeSet()
        {
            // arrange 
            var organisationName = "OrganisationName";
            var schemeInfo = TestFixture.Build<SchemePublicInfo>().With(s => s.IsBalancingScheme, true).Create();
            A.CallTo(() => cache.FetchSchemePublicInfo(organisationId)).Returns(schemeInfo);
            A.CallTo(() => cache.FetchOrganisationName(organisationId)).Returns(organisationName);

            // act
            await outgoingTransferEvidenceController.EditTonnages(organisationId, TestFixture.Create<Guid>(),
                TestFixture.Create<bool?>());

            // assert
            breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.PbsManageEvidence);
            breadcrumb.OrganisationId.Should().Be(organisationId);
        }

        [Fact]
        public async Task EditDraftTransfer_GivenOrganisationId_SchemeShouldBeRetrievedFromCache()
        {
            //act
            await outgoingTransferEvidenceController.EditDraftTransfer(organisationId, TestFixture.Create<Guid>(), null,
                null);

            //asset
            A.CallTo(() => cache.FetchSchemePublicInfo(organisationId)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditDraftTransfer_GivenSchemeIsNotBalancingScheme_BreadcrumbShouldBeSet()
        {
            // arrange 
            var organisationName = "OrganisationName";
            var schemeInfo = TestFixture.Build<SchemePublicInfo>().With(s => s.IsBalancingScheme, false).Create();
            A.CallTo(() => cache.FetchSchemePublicInfo(organisationId)).Returns(schemeInfo);
            A.CallTo(() => cache.FetchOrganisationName(organisationId)).Returns(organisationName);

            // act
            await outgoingTransferEvidenceController.EditDraftTransfer(organisationId, TestFixture.Create<Guid>(), null,
                null);

            // assert
            breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.SchemeManageEvidence);
            breadcrumb.OrganisationId.Should().Be(organisationId);
        }

        [Fact]
        public async Task EditDraftTransfer_GivenSchemeIsBalancingScheme_BreadcrumbShouldBeSet()
        {
            // arrange 
            var organisationName = "OrganisationName";
            var schemeInfo = TestFixture.Build<SchemePublicInfo>().With(s => s.IsBalancingScheme, true).Create();
            A.CallTo(() => cache.FetchSchemePublicInfo(organisationId)).Returns(schemeInfo);
            A.CallTo(() => cache.FetchOrganisationName(organisationId)).Returns(organisationName);

            // act
            await outgoingTransferEvidenceController.EditDraftTransfer(organisationId, TestFixture.Create<Guid>(), null,
                null);

            // assert
            breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.PbsManageEvidence);
            breadcrumb.OrganisationId.Should().Be(organisationId);
        }

        [Fact]
        public async Task EditTonnagesGet_GivenEvidenceNoteId_ShouldRetrieveTransferNote()
        {
            //arrange
            var evidenceNoteId = TestFixture.Create<Guid>();

            //act
            await outgoingTransferEvidenceController.EditTonnages(organisationId, evidenceNoteId,
                TestFixture.Create<bool?>());

            //assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                    A<GetTransferEvidenceNoteForSchemeRequest>.That.Matches(r => r.EvidenceNoteId == evidenceNoteId)))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditTonnagesGet_ExistingSelectedEvidenceNotes_ShouldBeRetrievedFromSession()
        {
            //arrange
            var evidenceNoteId = TestFixture.Create<Guid>();
            var note = TestFixture.Create<TransferEvidenceNoteData>();
            note.Status = NoteStatus.Draft;
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                    A<GetTransferEvidenceNoteForSchemeRequest>.That.Matches(r => r.EvidenceNoteId == evidenceNoteId)))
                .Returns(note);

            //act
            await outgoingTransferEvidenceController.EditTonnages(organisationId, evidenceNoteId,
                TestFixture.Create<bool?>());

            //assert
            A.CallTo(() => sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(SessionKeyConstant.OutgoingTransferKey))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditTonnagesGet_WithExistingEditTransferEvidenceTonnageViewModel_ShouldBeRetrievedFromSession()
        {
            //arrange
            var evidenceNoteId = TestFixture.Create<Guid>();
            var note = TestFixture.Create<TransferEvidenceNoteData>();
            note.Status = NoteStatus.Draft;
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                    A<GetTransferEvidenceNoteForSchemeRequest>.That.Matches(r => r.EvidenceNoteId == evidenceNoteId)))
                .Returns(note);

            //act
            await outgoingTransferEvidenceController.EditTonnages(organisationId, evidenceNoteId,
                TestFixture.Create<bool?>());

            //assert
            A.CallTo(() => sessionService.GetTransferSessionObject<TransferEvidenceTonnageViewModel>(
                    SessionKeyConstant.EditTransferEvidenceTonnageViewModel))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditTonnagesGet_WithExistingEditTransferEvidenceTonnageViewModel_ShouldClearTransferSessionObject()
        {
            //arrange
            var evidenceNoteId = TestFixture.Create<Guid>();
            var note = TestFixture.Create<TransferEvidenceNoteData>();
            note.Status = NoteStatus.Draft;
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                    A<GetTransferEvidenceNoteForSchemeRequest>.That.Matches(r => r.EvidenceNoteId == evidenceNoteId)))
                .Returns(note);

            //act
            await outgoingTransferEvidenceController.EditTonnages(organisationId, evidenceNoteId,
                TestFixture.Create<bool?>());

            //assert
            A.CallTo(() => sessionService.ClearTransferSessionObject(SessionKeyConstant.EditTransferEvidenceTonnageViewModel))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditTonnagesGet_GivenExistingSelectedEvidenceNotesAlongWithTransferNoteData_TransferNotesShouldBeRetrieved()
        {
            //arrange
            var evidenceNoteId1 = TestFixture.Create<Guid>();
            var evidenceNoteId2 = TestFixture.Create<Guid>();
            var evidenceNoteId3 = TestFixture.Create<Guid>();

            var sessionEvidenceNotes = new List<Guid>()
            {
                evidenceNoteId1,
                evidenceNoteId2,
                evidenceNoteId3
            };

            var transferEvidence = TestFixture.Build<TransferEvidenceNoteData>()
                .With(t => t.TransferEvidenceNoteTonnageData, new List<TransferEvidenceNoteTonnageData>()
                {
                    TestFixture.Build<TransferEvidenceNoteTonnageData>().With(e => e.OriginalNoteId, evidenceNoteId1)
                        .Create(),
                    TestFixture.Build<TransferEvidenceNoteTonnageData>().With(e => e.OriginalNoteId, evidenceNoteId2)
                        .Create(),
                }).Create();

            var sessionCategories = new List<int>()
            {
                Core.DataReturns.WeeeCategory.MedicalDevices.ToInt(),
                Core.DataReturns.WeeeCategory.ToysLeisureAndSports.ToInt()
            };

            var request = TestFixture.Build<TransferEvidenceNoteRequest>()
                .With(t => t.EvidenceNoteIds, sessionEvidenceNotes)
                .With(c => c.CategoryIds, sessionCategories)
                .Create();

            A.CallTo(() => sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(
                A<string>._)).Returns(request);

            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetTransferEvidenceNoteForSchemeRequest>._)).Returns(transferEvidence);

            //act
            await outgoingTransferEvidenceController.EditTonnages(organisationId, TestFixture.Create<Guid>(),
                TestFixture.Create<bool?>());

            //assert
            var expectedEvidenceNoteIds = transferEvidence.TransferEvidenceNoteTonnageData
                .Select(t => t.OriginalNoteId)
                .ToList().Union(sessionEvidenceNotes)
                .Distinct().ToList();

            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                    A<GetEvidenceNotesSelectedForTransferRequest>.That.Matches(r =>
                        r.Categories.SequenceEqual(sessionCategories) &&
                        r.EvidenceNotes.SequenceEqual(expectedEvidenceNoteIds) &&
                        r.OrganisationId == organisationId)))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditTonnagesGet_GivenExistingSelectedEvidenceNotesThatHaveBeenDeselectedAndSessionTransferRequest_TransferNotesShouldBeRetrieved()
        {
            //arrange
            var removedEvidenceNoteId = TestFixture.Create<Guid>();
            var newlySelectedEvidenceNoteId = TestFixture.Create<Guid>();

            var sessionEvidenceNotes = new List<Guid>()
            {
                newlySelectedEvidenceNoteId
            };

            var transferEvidence = TestFixture.Build<TransferEvidenceNoteData>()
                .With(t => t.TransferEvidenceNoteTonnageData, new List<TransferEvidenceNoteTonnageData>()
                {
                    TestFixture.Build<TransferEvidenceNoteTonnageData>()
                        .With(e => e.OriginalNoteId, removedEvidenceNoteId).Create()
                }).Create();

            var sessionCategories = new List<int>()
            {
                Core.DataReturns.WeeeCategory.MedicalDevices.ToInt(),
                Core.DataReturns.WeeeCategory.ToysLeisureAndSports.ToInt()
            };

            var request = TestFixture.Build<TransferEvidenceNoteRequest>()
                .With(t => t.EvidenceNoteIds, sessionEvidenceNotes)
                .With(c => c.CategoryIds, sessionCategories)
                .Create();

            A.CallTo(() => sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(
                A<string>._)).Returns(request);

            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetTransferEvidenceNoteForSchemeRequest>._)).Returns(transferEvidence);

            //act
            await outgoingTransferEvidenceController.EditTonnages(organisationId, TestFixture.Create<Guid>(),
                TestFixture.Create<bool?>());

            //assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                    A<GetEvidenceNotesSelectedForTransferRequest>.That.Matches(r =>
                        r.Categories.SequenceEqual(sessionCategories) &&
                        r.EvidenceNotes.SequenceEqual(sessionEvidenceNotes) &&
                        r.OrganisationId == organisationId)))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task
            EditTonnagesGet_GivenExistingSelectedEvidenceNotesThatHaveBeenDeselectedAndNullSessionTransferRequest_TransferNotesShouldBeRetrieved()
        {
            //arrange
            var evidenceNoteId = TestFixture.Create<Guid>();

            var transferEvidence = TestFixture.Build<TransferEvidenceNoteData>()
                .With(t => t.TransferEvidenceNoteTonnageData, new List<TransferEvidenceNoteTonnageData>()
                {
                    TestFixture.Build<TransferEvidenceNoteTonnageData>()
                        .With(e => e.OriginalNoteId, evidenceNoteId).Create()
                }).Create();

            A.CallTo(() => sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(
                A<string>._)).Returns(null);

            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetTransferEvidenceNoteForSchemeRequest>._)).Returns(transferEvidence);

            //act
            await outgoingTransferEvidenceController.EditTonnages(organisationId, TestFixture.Create<Guid>(),
                TestFixture.Create<bool?>());

            //assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                    A<GetEvidenceNotesSelectedForTransferRequest>.That.Matches(r =>
                        r.Categories.SequenceEqual(transferEvidence.CategoryIds) &&
                        r.OrganisationId == organisationId &&
                        r.EvidenceNotes.SequenceEqual(transferEvidence.TransferEvidenceNoteTonnageData
                            .Select(t => t.OriginalNoteId).ToList()))))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditTonnagesGet_GivenTransferNoteData_TransferNotesShouldBeRetrieved()
        {
            //arrange
            transferEvidenceNoteData.Status = NoteStatus.Draft;
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetTransferEvidenceNoteForSchemeRequest>._)).Returns(transferEvidenceNoteData);

            var categories = transferEvidenceNoteData.CategoryIds;
            var existingNotes = transferEvidenceNoteData.TransferEvidenceNoteTonnageData.Select(t => t.OriginalNoteId)
                .ToList();

            //act
            await outgoingTransferEvidenceController.EditTonnages(organisationId, TestFixture.Create<Guid>(),
                TestFixture.Create<bool?>());

            //assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetEvidenceNotesSelectedForTransferRequest>.That.Matches(r =>
                    r.OrganisationId == organisationId &&
                    r.Categories.SequenceEqual(categories) &&
                    r.EvidenceNotes.SequenceEqual(existingNotes)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditTonnagesGet_GivenTransferNoteDataAndNullSessionData_TransferNotesShouldBeRetrieved()
        {
            //arrange
            transferEvidenceNoteData.Status = NoteStatus.Draft;

            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetTransferEvidenceNoteForSchemeRequest>._)).Returns(transferEvidenceNoteData);

            A.CallTo(() => sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(
                A<string>._)).Returns(null);

            var categories = transferEvidenceNoteData.CategoryIds;
            var existingNotes = transferEvidenceNoteData.TransferEvidenceNoteTonnageData.Select(t => t.OriginalNoteId)
                .ToList();

            //act
            await outgoingTransferEvidenceController.EditTonnages(organisationId,
                TestFixture.Create<Guid>(), TestFixture.Create<bool?>());

            //assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                    A<GetEvidenceNotesSelectedForTransferRequest>.That.Matches(r =>
                        r.OrganisationId == organisationId &&
                        r.Categories.SequenceEqual(categories) &&
                        r.EvidenceNotes.SequenceEqual(existingNotes))))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditTonnagesGet_GivenTransferNoteAndTransferNotes_ModelMapperShouldBeCalled()
        {
            //arrange
            transferEvidenceNoteData.Status = NoteStatus.Draft;
            var returnToEditDraftTransfer = TestFixture.Create<bool?>();
            var request = TestFixture.Create<TransferEvidenceNoteRequest>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetEvidenceNotesSelectedForTransferRequest>._)).Returns(evidenceNoteData);
            A.CallTo(() => sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(
                A<string>._)).Returns(request);

            //act
            await outgoingTransferEvidenceController.EditTonnages(organisationId, transferEvidenceNoteData.Id,
                returnToEditDraftTransfer);

            //assert
            A.CallTo(() => mapper.Map<TransferEvidenceNotesViewModelMapTransfer, TransferEvidenceTonnageViewModel>(
                    A<TransferEvidenceNotesViewModelMapTransfer>.That.Matches(t => t.TransferAllTonnage == false &&
                        t.TransferEvidenceNoteData == transferEvidenceNoteData &&
                        t.SelectedNotes.Equals(evidenceNoteData) &&
                        t.OrganisationId == organisationId &&
                        t.Request == request &&
                        t.ReturnToEditDraftTransfer == returnToEditDraftTransfer)))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditTonnagesGet_GivenNullReturnEditDraftTransfer_ModelMapperShouldBeCalled()
        {
            //arrange
            var request = TestFixture.Create<TransferEvidenceNoteRequest>();
            var evidenceNoteId = TestFixture.Create<Guid>();
            var note = TestFixture.Create<TransferEvidenceNoteData>();
            note.Status = NoteStatus.Draft;
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                    A<GetTransferEvidenceNoteForSchemeRequest>.That.Matches(r => r.EvidenceNoteId == evidenceNoteId)))
                .Returns(note);
            A.CallTo(() => sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(
                A<string>._)).Returns(request);

            //act
            await outgoingTransferEvidenceController.EditTonnages(organisationId, evidenceNoteId, null);

            //assert
            A.CallTo(() => mapper.Map<TransferEvidenceNotesViewModelMapTransfer, TransferEvidenceTonnageViewModel>(
                    A<TransferEvidenceNotesViewModelMapTransfer>.That.Matches(t =>
                        t.OrganisationId == organisationId &&
                        t.Request == request &&
                        t.ReturnToEditDraftTransfer == null)))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditTonnagesGet_GivenMappedModel_ModelShouldBeReturned()
        {
            //arrange
            var evidenceNoteId = TestFixture.Create<Guid>();
            var note = TestFixture.Create<TransferEvidenceNoteData>();
            note.Status = NoteStatus.Draft;
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                    A<GetTransferEvidenceNoteForSchemeRequest>.That.Matches(r => r.EvidenceNoteId == evidenceNoteId)))
                .Returns(note);
            transferEvidenceTonnageViewModel.EvidenceNotesDataList.Clear();
            transferEvidenceTonnageViewModel.EvidenceNotesDataList.Add(new ViewEvidenceNoteViewModel { Status = NoteStatus.Draft });
            A.CallTo(() => mapper.Map<TransferEvidenceNotesViewModelMapTransfer, TransferEvidenceTonnageViewModel>(
                A<TransferEvidenceNotesViewModelMapTransfer>._)).Returns(transferEvidenceTonnageViewModel);

            //act
            var result =
                await outgoingTransferEvidenceController.EditTonnages(organisationId, evidenceNoteId,
                        TestFixture.Create<bool?>()) as
                    ViewResult;

            //assert
            result.Model.Should().Be(transferEvidenceTonnageViewModel);
        }

        [Fact]
        public async Task EditTonnagesGet_ShouldReturnView()
        {
            //arrange
            var evidenceNoteId = TestFixture.Create<Guid>();
            var note = TestFixture.Create<TransferEvidenceNoteData>();
            note.Status = NoteStatus.Draft;
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                    A<GetTransferEvidenceNoteForSchemeRequest>.That.Matches(r => r.EvidenceNoteId == evidenceNoteId)))
                .Returns(note);

            //act
            var result =
                await outgoingTransferEvidenceController.EditTonnages(organisationId, evidenceNoteId,
                        TestFixture.Create<bool?>()) as
                    ViewResult;

            //assert
            result.ViewName.Should().Be("EditTonnages");
        }

        [Theory]
        [ClassData(typeof(NoteStatusCoreData))]
        public async Task EditTonnagesGet_ShouldReturnManageEvidenceView(NoteStatus status)
        {
            if (status.Equals(NoteStatus.Returned) || status.Equals(NoteStatus.Draft))
            {
                return;
            }

            //arrange
            var evidenceNoteId = TestFixture.Create<Guid>();
            var note = TestFixture.Create<TransferEvidenceNoteData>();
            note.Status = status;
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                    A<GetTransferEvidenceNoteForSchemeRequest>.That.Matches(r => r.EvidenceNoteId == evidenceNoteId)))
                .Returns(note);

            //act
            var result =
                await outgoingTransferEvidenceController.EditTonnages(organisationId, evidenceNoteId,
                        TestFixture.Create<bool?>()) as
                    RedirectToRouteResult;

            //assert
            result.RouteValues["Action"].Should().Be("Index");
            result.RouteValues["Controller"].Should().Be("ManageEvidenceNotes");
        }

        [Fact]
        public async Task EditDraftTransferGet_GivenEvidenceNoteId_ShouldRetrieveTransferNote()
        {
            //arrange
            var evidenceNoteId = TestFixture.Create<Guid>();

            //act
            await outgoingTransferEvidenceController.EditDraftTransfer(organisationId, evidenceNoteId,
                null);

            //assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                    A<GetTransferEvidenceNoteForSchemeRequest>.That.Matches(r => r.EvidenceNoteId == evidenceNoteId)))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditDraftTransferGet_GivenTransferNote_ModelMapperShouldBeCalled()
        {
            //arrange
            var transferNoteData = TestFixture.Create<TransferEvidenceNoteData>();

            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetTransferEvidenceNoteForSchemeRequest>._)).Returns(transferNoteData);

            //act
            await outgoingTransferEvidenceController.EditDraftTransfer(organisationId, TestFixture.Create<Guid>(),
                null);

            //assert
            A.CallTo(() => mapper.Map<ViewTransferNoteViewModel>(A<ViewTransferNoteViewModelMapTransfer>.That.Matches(
                    v => v.Edit == true &&
                         v.DisplayNotification == null &&
                         v.TransferEvidenceNoteData == transferNoteData &&
                         v.OrganisationId == organisationId &&
                         v.RedirectTab.Equals(
                             DisplayExtensions.ToDisplayString(ManageEvidenceNotesDisplayOptions.OutgoingTransfers)))))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditDraftTransferGet_GivenTransferNoteWithRedirectTab_ModelMapperShouldBeCalled()
        {
            //arrange
            var transferNoteData = TestFixture.Create<TransferEvidenceNoteData>();
            var tab = DisplayExtensions.ToDisplayString(ManageEvidenceNotesDisplayOptions.ReviewSubmittedEvidence);

            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetTransferEvidenceNoteForSchemeRequest>._)).Returns(transferNoteData);

            //act
            await outgoingTransferEvidenceController.EditDraftTransfer(organisationId, TestFixture.Create<Guid>(), null,
                tab);

            //assert
            A.CallTo(() => mapper.Map<ViewTransferNoteViewModel>(A<ViewTransferNoteViewModelMapTransfer>.That.Matches(
                v => v.Edit == true &&
                     v.DisplayNotification == null &&
                     v.TransferEvidenceNoteData == transferNoteData &&
                     v.OrganisationId == organisationId &&
                     v.RedirectTab.Equals(tab)))).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(true)]
        [InlineData(false)]
        public async Task EditDraftTransferGet_GivenTransferNoteAndReturnToView_ModelMapperShouldBeCalled(
            bool? returnToView)
        {
            //arrange
            var transferNoteData = TestFixture.Create<TransferEvidenceNoteData>();

            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetTransferEvidenceNoteForSchemeRequest>._)).Returns(transferNoteData);

            //act
            await outgoingTransferEvidenceController.EditDraftTransfer(organisationId, TestFixture.Create<Guid>(),
                returnToView);

            //assert
            A.CallTo(() => mapper.Map<ViewTransferNoteViewModel>(A<ViewTransferNoteViewModelMapTransfer>.That.Matches(
                    v => v.Edit == true &&
                         v.DisplayNotification == null &&
                         v.TransferEvidenceNoteData == transferNoteData &&
                         v.OrganisationId == organisationId &&
                         v.ReturnToView == returnToView &&
                         v.RedirectTab.Equals(
                             DisplayExtensions.ToDisplayString(ManageEvidenceNotesDisplayOptions.OutgoingTransfers)))))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditDraftTransferGet_GivenMappedModel_ModelShouldBeReturned()
        {
            //arrange
            var model = TestFixture.Create<ViewTransferNoteViewModel>();

            A.CallTo(() => mapper.Map<ViewTransferNoteViewModel>(A<ViewTransferNoteViewModelMapTransfer>._))
                .Returns(model);

            //act
            var result =
                await outgoingTransferEvidenceController.EditDraftTransfer(organisationId, TestFixture.Create<Guid>(),
                    null) as ViewResult;

            //assert
            result.Model.Should().Be(model);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(true)]
        [InlineData(false)]
        public async Task EditDraftTransferGet_GivenMappedModelAndReturnToView_ModelShouldBeReturned(bool? returnToView)
        {
            //arrange
            var model = TestFixture.Create<ViewTransferNoteViewModel>();

            A.CallTo(() => mapper.Map<ViewTransferNoteViewModel>(A<ViewTransferNoteViewModelMapTransfer>._))
                .Returns(model);

            //act
            var result =
                await outgoingTransferEvidenceController.EditDraftTransfer(organisationId, TestFixture.Create<Guid>(),
                    returnToView) as ViewResult;

            //assert
            result.Model.Should().Be(model);
        }

        [Fact]
        public async Task EditDraftTransferGet_ShouldReturnView()
        {
            //act
            var result =
                await outgoingTransferEvidenceController.EditDraftTransfer(organisationId, TestFixture.Create<Guid>(),
                    null) as ViewResult;

            //assert
            result.ViewName.Should().Be("EditDraftTransfer");
        }

        [Theory]
        [InlineData(null)]
        [InlineData(true)]
        [InlineData(false)]
        public async Task EditDraftTransferGet_GivenReturnToView_ShouldReturnView(bool? returnToView)
        {
            //act
            var result =
                await outgoingTransferEvidenceController.EditDraftTransfer(organisationId, TestFixture.Create<Guid>(),
                    returnToView) as ViewResult;

            //assert
            result.ViewName.Should().Be("EditDraftTransfer");
        }

        [Fact]
        public async Task EditDraftTransferGet_ShouldCallClearTransferSessionObjectForOutgoingTransferKey()
        {
            //act
            var result =
                await outgoingTransferEvidenceController.EditDraftTransfer(organisationId, TestFixture.Create<Guid>(),
                    null) as ViewResult;

            //assert
            A.CallTo(() => sessionService.ClearTransferSessionObject(SessionKeyConstant.OutgoingTransferKey)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task
            EditDraftTransferGet_ShouldCallClearTransferSessionObjectForEditTransferEvidenceTonnageViewModel()
        {
            //act
            var result =
                await outgoingTransferEvidenceController.EditDraftTransfer(organisationId, TestFixture.Create<Guid>(),
                    null) as ViewResult;

            //assert
            A.CallTo(() => sessionService.ClearTransferSessionObject(SessionKeyConstant.EditTransferEvidenceTonnageViewModel)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditDraftTransferGet_WhenQueryStringIsSetInViewBag_ViewBagShouldHaveTheQueryString()
        {
            // arrange 
            var queryString = TestFixture.Create<string>();

            //act
            await outgoingTransferEvidenceController.EditDraftTransfer(TestFixture.Create<Guid>(), TestFixture.Create<Guid>(), 
                TestFixture.Create<bool?>(), TestFixture.Create<string>(), TestFixture.Create<int>(), queryString);

            //assert
            ((string)outgoingTransferEvidenceController.ViewBag.QueryString).Should().Be(queryString);
        }

        [Fact]
        public async Task EditDraftTransferGet_WhenQueryStringIsNotSetInViewBag_QueryStringInViewBagShouldBeNull()
        {
            //act
            await outgoingTransferEvidenceController.EditDraftTransfer(TestFixture.Create<Guid>(), TestFixture.Create<Guid>(),
                TestFixture.Create<bool?>(), TestFixture.Create<string>(), TestFixture.Create<int>());

            //assert
            ((string)outgoingTransferEvidenceController.ViewBag.QueryString).Should().Be(null);
        }

        [Fact]
        public async Task SubmittedTransferGet_GivenOrganisationId_SchemeShouldBeRetrievedFromCache()
        {
            //act
            await outgoingTransferEvidenceController.SubmittedTransfer(organisationId, TestFixture.Create<Guid>(),
                TestFixture.Create<bool>(), null);

            //asset
            A.CallTo(() => cache.FetchSchemePublicInfo(organisationId)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task SubmittedTransferGet_GivenSchemeIsNotBalancingScheme_BreadcrumbShouldBeSet()
        {
            // arrange 
            var organisationName = "OrganisationName";
            var schemeInfo = TestFixture.Build<SchemePublicInfo>().With(s => s.IsBalancingScheme, false).Create();
            A.CallTo(() => cache.FetchOrganisationName(organisationId)).Returns(organisationName);
            A.CallTo(() => cache.FetchSchemePublicInfo(organisationId)).Returns(schemeInfo);

            // act
            await outgoingTransferEvidenceController.SubmittedTransfer(organisationId,
                TestFixture.Create<Guid>(),
                TestFixture.Create<bool>(),
                DisplayExtensions.ToDisplayString(ManageEvidenceNotesDisplayOptions.ReviewSubmittedEvidence));

            // assert
            breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.SchemeManageEvidence);
            breadcrumb.OrganisationId.Should().Be(organisationId);
        }

        [Fact]
        public async Task SubmittedTransferGet_GivenSchemeIsBalancingScheme_BreadcrumbShouldBeSet()
        {
            // arrange 
            var organisationName = "OrganisationName";
            var schemeInfo = TestFixture.Build<SchemePublicInfo>().With(s => s.IsBalancingScheme, true).Create();
            A.CallTo(() => cache.FetchOrganisationName(organisationId)).Returns(organisationName);
            A.CallTo(() => cache.FetchSchemePublicInfo(organisationId)).Returns(schemeInfo);

            // act
            await outgoingTransferEvidenceController.SubmittedTransfer(organisationId,
                TestFixture.Create<Guid>(),
                TestFixture.Create<bool>(),
                DisplayExtensions.ToDisplayString(ManageEvidenceNotesDisplayOptions.ReviewSubmittedEvidence));

            // assert
            breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.PbsManageEvidence);
            breadcrumb.OrganisationId.Should().Be(organisationId);
        }

        [Fact]
        public async Task SubmittedTransferGet_GivenEvidenceNoteId_ShouldRetrieveTransferNote()
        {
            //arrange
            var evidenceNoteId = TestFixture.Create<Guid>();

            //act
            await outgoingTransferEvidenceController.SubmittedTransfer(organisationId,
                evidenceNoteId,
                TestFixture.Create<bool?>(),
                DisplayExtensions.ToDisplayString(ManageEvidenceNotesDisplayOptions.ReviewSubmittedEvidence));

            //assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                    A<GetTransferEvidenceNoteForSchemeRequest>.That.Matches(r => r.EvidenceNoteId == evidenceNoteId)))
                .MustHaveHappenedOnceExactly();
        }

        [Theory]
        [ClassData(typeof(NoteStatusCoreData))]
        public async Task
            SubmittedTransferGet_GivenTransferNoteIsNotAtSubmittedStatus_ShouldRedirectToManageEvidenceNotes(
                NoteStatus status)
        {
            if (status == NoteStatus.Submitted)
            {
                return;
            }

            var redirectTab =
                DisplayExtensions.ToDisplayString(ManageEvidenceNotesDisplayOptions.ReviewSubmittedEvidence);
            var noteData = TestFixture.Build<TransferEvidenceNoteData>()
                .With(n => n.Status, status).Create();

            //arrange
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetTransferEvidenceNoteForSchemeRequest>._)).Returns(noteData);

            //act
            var result = await outgoingTransferEvidenceController.SubmittedTransfer(organisationId,
                    TestFixture.Create<Guid>(),
                    TestFixture.Create<bool?>(),
                    redirectTab) as
                RedirectToRouteResult;

            //assert
            result.RouteValues["action"].Should().Be("Index");
            result.RouteValues["controller"].Should().Be("ManageEvidenceNotes");
            result.RouteValues["pcsId"].Should().Be(organisationId);
            result.RouteValues["tab"].Should().Be(redirectTab);
        }

        [Fact]
        public async Task SubmittedTransferGet_GivenTransferNote_ModelMapperShouldBeCalled()
        {
            //arrange
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetTransferEvidenceNoteForSchemeRequest>._)).Returns(transferEvidenceNoteData);

            //act
            await outgoingTransferEvidenceController.SubmittedTransfer(organisationId,
                TestFixture.Create<Guid>(),
                TestFixture.Create<bool?>(),
                DisplayExtensions.ToDisplayString(ManageEvidenceNotesDisplayOptions.ReviewSubmittedEvidence));

            //assert
            A.CallTo(() => mapper.Map<ReviewTransferNoteViewModel>(
                A<ViewTransferNoteViewModelMapTransfer>.That.Matches(t =>
                    t.TransferEvidenceNoteData == transferEvidenceNoteData &&
                    t.OrganisationId == organisationId))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task SubmittedTransferGet_GivenMappedModel_ModelShouldBeReturned()
        {
            //arrange
            var model = TestFixture.Create<ReviewTransferNoteViewModel>();

            A.CallTo(() => mapper.Map<ReviewTransferNoteViewModel>(A<ViewTransferNoteViewModelMapTransfer>._))
                .Returns(model);

            //act
            var result = await outgoingTransferEvidenceController.SubmittedTransfer(organisationId,
                    TestFixture.Create<Guid>(),
                    TestFixture.Create<bool?>(),
                    DisplayExtensions.ToDisplayString(ManageEvidenceNotesDisplayOptions.ReviewSubmittedEvidence)) as
                ViewResult;

            //assert
            result.Model.Should().Be(model);
        }

        [Fact]
        public async Task SubmittedTransferGet_ShouldReturnView()
        {
            //act
            var result = await outgoingTransferEvidenceController.SubmittedTransfer(organisationId,
                    TestFixture.Create<Guid>(),
                    TestFixture.Create<bool?>(),
                    DisplayExtensions.ToDisplayString(ManageEvidenceNotesDisplayOptions.ReviewSubmittedEvidence)) as
                ViewResult;

            //assert
            result.ViewName.Should().Be("SubmittedTransfer");
        }

        [Fact]
        public async Task SubmittedTransferPost_GivenModel_SchemeShouldBeRetrievedFromCache()
        {
            //arrange
            var model = TestFixture.Build<ReviewTransferNoteViewModel>()
                .With(s => s.SelectedValue, "Approve evidence note transfer").Create();

            //act
            await outgoingTransferEvidenceController.SubmittedTransfer(model);

            //asset
            A.CallTo(() => cache.FetchSchemePublicInfo(model.OrganisationId)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task SubmittedTransferPost_GivenModelAndSchemeIsNotBalancingScheme_BreadCrumbShouldBeSet()
        {
            //arrange
            var model = TestFixture.Create<ReviewTransferNoteViewModel>();
            var schemeInfo = TestFixture.Build<SchemePublicInfo>().With(s => s.IsBalancingScheme, false).Create();
            model.SelectedValue = "Approve evidence note transfer";
            var organisationName = "OrganisationName";
            A.CallTo(() => cache.FetchOrganisationName(model.OrganisationId)).Returns(organisationName);
            A.CallTo(() => cache.FetchSchemePublicInfo(model.OrganisationId)).Returns(schemeInfo);

            //act
            await outgoingTransferEvidenceController.SubmittedTransfer(model);

            // assert
            breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.SchemeManageEvidence);
            breadcrumb.OrganisationId.Should().Be(model.OrganisationId);
        }

        [Fact]
        public async Task SubmittedTransferPost_GivenModelAndSchemeIsBalancingScheme_BreadCrumbShouldBeSet()
        {
            //arrange
            var model = TestFixture.Create<ReviewTransferNoteViewModel>();
            var schemeInfo = TestFixture.Build<SchemePublicInfo>().With(s => s.IsBalancingScheme, true).Create();
            model.SelectedValue = "Approve evidence note transfer";
            var organisationName = "OrganisationName";
            A.CallTo(() => cache.FetchOrganisationName(model.OrganisationId)).Returns(organisationName);
            A.CallTo(() => cache.FetchSchemePublicInfo(model.OrganisationId)).Returns(schemeInfo);

            //act
            await outgoingTransferEvidenceController.SubmittedTransfer(model);

            // assert
            breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.PbsManageEvidence);
            breadcrumb.OrganisationId.Should().Be(model.OrganisationId);
        }

        [Fact]
        public async Task SubmittedTransferPost_GivenInvalidViewModel_ShouldRetrieveTransferNote()
        {
            //arrange
            var model = TestFixture.Create<ReviewTransferNoteViewModel>();
            AddModelError();

            //act
            await outgoingTransferEvidenceController.SubmittedTransfer(model);

            //assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                    A<GetTransferEvidenceNoteForSchemeRequest>.That.Matches(r =>
                        r.EvidenceNoteId == model.ViewTransferNoteViewModel.EvidenceNoteId)))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task SubmittedTransferPost_GivenInvalidViewModel_ModelMapperShouldBeCalled()
        {
            //arrange
            var model = TestFixture.Create<ReviewTransferNoteViewModel>();
            Guid schemeId = model.ViewTransferNoteViewModel.SchemeId;
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetTransferEvidenceNoteForSchemeRequest>._))
                .Returns(transferEvidenceNoteData);

            AddModelError();

            //act
            await outgoingTransferEvidenceController.SubmittedTransfer(model);

            //assert
            A.CallTo(() => mapper.Map<ReviewTransferNoteViewModel>(
                A<ViewTransferNoteViewModelMapTransfer>.That.Matches(t =>
                    t.TransferEvidenceNoteData == transferEvidenceNoteData &&
                    t.OrganisationId == schemeId))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task SubmittedTransferPost_GivenInvalidModelMappedModel_ModelShouldBeReturned()
        {
            //arrange
            var model = TestFixture.Create<ReviewTransferNoteViewModel>();
            A.CallTo(() => mapper.Map<ReviewTransferNoteViewModel>(A<ViewTransferNoteViewModelMapTransfer>._))
                .Returns(model);

            AddModelError();

            //act
            var result = await outgoingTransferEvidenceController.SubmittedTransfer(model) as ViewResult;

            //assert
            result.Model.Should().Be(model);
        }

        [Theory]
        [InlineData("Approve evidence note transfer", NoteUpdatedStatusEnum.Approved)]
        [InlineData("Reject evidence note transfer", NoteUpdatedStatusEnum.Rejected)]
        [InlineData("Return evidence note transfer", NoteUpdatedStatusEnum.Returned)]
        public async Task SubmittedTransferPost_GivenValidModelMappedModel_TempDataShouldBeSet(string selectedValue,
            NoteUpdatedStatusEnum expectedStatus)
        {
            //arrange
            var model = A.Fake<ReviewTransferNoteViewModel>();
            model.ViewTransferNoteViewModel = TestFixture.Create<ViewTransferNoteViewModel>();
            A.CallTo(() => mapper.Map<ReviewTransferNoteViewModel>(A<ViewTransferNoteViewModelMapTransfer>._))
                .Returns(model);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetTransferEvidenceNoteForSchemeRequest>._))
                .Returns(transferEvidenceNoteData);
            A.CallTo(() => model.SelectedValue).Returns(selectedValue);
            A.CallTo(() => model.Reason).Returns(null);
            A.CallTo(() => model.OrganisationId).Returns(Guid.NewGuid());

            //act
            var result = await outgoingTransferEvidenceController.SubmittedTransfer(model) as ViewResult;

            //assert
            result.TempData[ViewDataConstant.TransferEvidenceNoteDisplayNotification].Should().Be(expectedStatus);
        }

        [Fact]
        public async Task SubmittedTransferPost_GivenValidModelMappedModel_DownloadTransferNoteShouldBeReturned()
        {
            //arrange
            var model = A.Fake<ReviewTransferNoteViewModel>();
            model.ViewTransferNoteViewModel = TestFixture.Create<ViewTransferNoteViewModel>();
            A.CallTo(() => mapper.Map<ReviewTransferNoteViewModel>(A<ViewTransferNoteViewModelMapTransfer>._))
                .Returns(model);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetTransferEvidenceNoteForSchemeRequest>._))
                .Returns(transferEvidenceNoteData);
            A.CallTo(() => model.SelectedValue).Returns("Approve evidence note transfer");
            A.CallTo(() => model.Reason).Returns(null);
            A.CallTo(() => model.OrganisationId).Returns(Guid.NewGuid());

            //act
            var result = await outgoingTransferEvidenceController.SubmittedTransfer(model) as ViewResult;

            //assert
            result.ViewName.Should().Be("DownloadTransferNote");
        }

        [Fact]
        public async Task SubmittedTransferPost_ShouldReturnView()
        {
            //arrange
            var model = TestFixture.Create<ReviewTransferNoteViewModel>();

            AddModelError();

            //act
            var result = await outgoingTransferEvidenceController.SubmittedTransfer(model) as ViewResult;

            //assert
            result.ViewName.Should().Be("SubmittedTransfer");
        }

        [Fact]
        public async Task EditTransferFromGet_GivenOrganisationId_SchemeShouldBeRetrievedFromCache()
        {
            //act
            await outgoingTransferEvidenceController.EditTransferFrom(organisationId, TestFixture.Create<Guid>());

            //asset
            A.CallTo(() => cache.FetchSchemePublicInfo(organisationId)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditTransferFromGet_GivenSchemeIsNotBalancingScheme_BreadcrumbShouldBeSet()
        {
            // arrange 
            var organisationName = "OrganisationName";
            var schemeInfo = TestFixture.Build<SchemePublicInfo>().With(s => s.IsBalancingScheme, false).Create();
            A.CallTo(() => cache.FetchOrganisationName(organisationId)).Returns(organisationName);
            A.CallTo(() => cache.FetchSchemePublicInfo(organisationId)).Returns(schemeInfo);

            // act
            await outgoingTransferEvidenceController.EditTransferFrom(organisationId, TestFixture.Create<Guid>());

            // assert
            breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.SchemeManageEvidence);
            breadcrumb.OrganisationId.Should().Be(organisationId);
        }

        [Fact]
        public async Task EditTransferFromGet_GivenSchemeIsBalancingScheme_BreadcrumbShouldBeSet()
        {
            // arrange 
            var organisationName = "OrganisationName";
            var schemeInfo = TestFixture.Build<SchemePublicInfo>().With(s => s.IsBalancingScheme, true).Create();
            A.CallTo(() => cache.FetchOrganisationName(organisationId)).Returns(organisationName);
            A.CallTo(() => cache.FetchSchemePublicInfo(organisationId)).Returns(schemeInfo);

            // act
            await outgoingTransferEvidenceController.EditTransferFrom(organisationId, TestFixture.Create<Guid>());

            // assert
            breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.PbsManageEvidence);
            breadcrumb.OrganisationId.Should().Be(organisationId);
        }

        [Fact]
        public async Task EditTransferFromGet_GivenEvidenceNoteId_ShouldRetrieveTransferNote()
        {
            //arrange
            var evidenceNoteId = TestFixture.Create<Guid>();

            A.CallTo(() => sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(
                A<string>._)).Returns(TestFixture.Create<TransferEvidenceNoteRequest>());

            //act
            await outgoingTransferEvidenceController.EditTransferFrom(organisationId, evidenceNoteId);

            //assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                    A<GetTransferEvidenceNoteForSchemeRequest>.That.Matches(r => r.EvidenceNoteId == evidenceNoteId)))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task
            EditTransferFromGet_GivenTransferSessionTransferRequestIsNull_ShouldRedirectToManageEvidenceNotes()
        {
            //arrange
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetTransferEvidenceNoteForSchemeRequest>._)).Returns(transferEvidenceNoteData);

            A.CallTo(() =>
                sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(A<string>._)).Returns(null);

            //act
            var result =
                await outgoingTransferEvidenceController.EditTransferFrom(organisationId, TestFixture.Create<Guid>()) as
                    RedirectToRouteResult;

            //assert
            result.RouteValues["action"].Should().Be("Index");
            result.RouteValues["controller"].Should().Be("ManageEvidenceNotes");
            result.RouteValues["pcsId"].Should().Be(organisationId);
            result.RouteValues["area"].Should().Be("Scheme");
            result.RouteValues["tab"].Should()
                .Be(DisplayExtensions.ToDisplayString(ManageEvidenceNotesDisplayOptions.ViewAndTransferEvidence));
        }

        [Theory]
        [InlineData(1, null)]
        [InlineData(2, null)]
        [InlineData(1, "search")]
        [InlineData(2, "search")]
        [InlineData(1, "")]
        [InlineData(2, "")]
        [InlineData(1, " ")]
        [InlineData(2, " ")]
        public async Task EditTransferFromGet_GivenNoteData_AvailableTransferNotesShouldBeRetrieved(int page, string searchRef)
        {
            //arrange
            var evidenceNoteIds = TestFixture.CreateMany<Guid>().ToList();
            var categories = TestFixture.CreateMany<int>().ToList();

            var request = TestFixture.Build<TransferEvidenceNoteRequest>()
                .With(t => t.EvidenceNoteIds, evidenceNoteIds)
                .With(t => t.CategoryIds, categories)
                .Create();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetTransferEvidenceNoteForSchemeRequest>._))
                .Returns(transferEvidenceNoteData);

            A.CallTo(() => sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(A<string>._)).Returns(request);

            //act
            await outgoingTransferEvidenceController.EditTransferFrom(organisationId, TestFixture.Create<Guid>(), page, searchRef);

            //assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetEvidenceNotesForTransferRequest>.That.Matches(g =>
                    g.PageSize == DefaultPageSize &&
                    g.PageNumber == page &&
                    g.Categories.SequenceEqual(request.CategoryIds) &&
                    g.ComplianceYear == transferEvidenceNoteData.ComplianceYear &&
                    g.OrganisationId == organisationId &&
                    g.SearchReference == searchRef &&
                    g.ExcludeEvidenceNotes.SequenceEqual(
                        evidenceNoteIds.Union(transferEvidenceNoteData.CurrentEvidenceNoteIds)))))
                .MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public async Task
            EditTransferFromGet_GivenTransferRequestWithSelectedNotes_SelectedTransferNotesShouldBeRetrieved(int page)
        {
            //arrange
            var evidenceNoteIds = TestFixture.CreateMany<Guid>(2).ToList();
            var sessionCategories = new List<int>()
            {
                Core.DataReturns.WeeeCategory.MedicalDevices.ToInt(),
                Core.DataReturns.WeeeCategory.ToysLeisureAndSports.ToInt()
            };

            var transferEvidence = TestFixture.Build<TransferEvidenceNoteData>()
                .With(t => t.TransferEvidenceNoteTonnageData, new List<TransferEvidenceNoteTonnageData>()
                {
                    TestFixture.Build<TransferEvidenceNoteTonnageData>()
                        .With(e => e.OriginalNoteId, evidenceNoteIds.ElementAt(0))
                        .With(e => e.EvidenceTonnageData, new EvidenceTonnageData(Guid.NewGuid(), Core.DataReturns.WeeeCategory.MedicalDevices, null, null, null, null))
                        .Create(),
                    TestFixture.Build<TransferEvidenceNoteTonnageData>()
                        .With(e => e.OriginalNoteId, evidenceNoteIds.ElementAt(1))
                        .With(e => e.EvidenceTonnageData, new EvidenceTonnageData(Guid.NewGuid(), Core.DataReturns.WeeeCategory.ConsumerEquipment, null, null, null, null))
                        .Create(),
                }).Create();

            var request = TestFixture.Build<TransferEvidenceNoteRequest>()
                .With(t => t.EvidenceNoteIds, evidenceNoteIds)
                .With(t => t.CategoryIds, sessionCategories)
                .Create();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetTransferEvidenceNoteForSchemeRequest>._))
                .Returns(transferEvidence);

            A.CallTo(() => sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(A<string>._)).Returns(request);

            //act
            await outgoingTransferEvidenceController.EditTransferFrom(organisationId, TestFixture.Create<Guid>(), page);

            //assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetEvidenceNotesSelectedForTransferRequest>.That.Matches(
                    g =>
                        g.Categories.SequenceEqual(request.CategoryIds) &&
                        g.OrganisationId == organisationId &&
                        g.EvidenceNotes.SequenceEqual(evidenceNoteIds))))
                .MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public async Task EditTransferFromGet_GivenTransferNoSelectedNotes_SelectedTransferNotesShouldNotBeRetrieved(
            int page)
        {
            //arrange
            var categories = TestFixture.CreateMany<int>().ToList();

            var request = TestFixture.Build<TransferEvidenceNoteRequest>()
                .With(t => t.EvidenceNoteIds, new List<Guid>())
                .With(t => t.CategoryIds, categories)
                .Create();

            var transferEvidenceNoteData = TestFixture.Build<TransferEvidenceNoteData>()
                .With(n => n.Status, NoteStatus.Submitted)
                .Without(t => t.TransferEvidenceNoteTonnageData)
                .Create();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetTransferEvidenceNoteForSchemeRequest>._))
                .Returns(transferEvidenceNoteData);

            A.CallTo(() => sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(A<string>._)).Returns(request);

            //act
            await outgoingTransferEvidenceController.EditTransferFrom(organisationId, TestFixture.Create<Guid>(), page);

            //assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetEvidenceNotesSelectedForTransferRequest>._))
                .MustNotHaveHappened();
        }

        [Fact]
        public async Task EditTransferFromGet_TransferRequestShouldBeRetrievedFromSession()
        {
            //act
            await outgoingTransferEvidenceController.EditTransferFrom(organisationId, TestFixture.Create<Guid>());

            //assert
            A.CallTo(() =>
                sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(SessionKeyConstant.OutgoingTransferKey)).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(1, null)]
        [InlineData(2, null)]
        [InlineData(1, "search")]
        [InlineData(2, "search")]
        [InlineData(1, "")]
        [InlineData(2, "")]
        [InlineData(1, " ")]
        [InlineData(2, " ")]
        public async Task EditTransferFromGet_GivenNotes_ModelMapperShouldBeCalled(int page, string searchRef)
        {
            //arrange
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetTransferEvidenceNoteForSchemeRequest>._)).Returns(transferEvidenceNoteData);

            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetEvidenceNotesForTransferRequest>._)).Returns(evidenceNoteData);

            var availableEvidenceNoteData =
                new EvidenceNoteSearchDataResult(TestFixture.CreateMany<EvidenceNoteData>(3).ToList(), 3);

            var selectedEvidenceNoteData =
                new EvidenceNoteSearchDataResult(TestFixture.CreateMany<EvidenceNoteData>(3).ToList(), 3);

            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetEvidenceNotesForTransferRequest>._)).Returns(availableEvidenceNoteData);

            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetEvidenceNotesSelectedForTransferRequest>._)).Returns(selectedEvidenceNoteData);

            var request = TestFixture.Create<TransferEvidenceNoteRequest>();
            A.CallTo(() => sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(A<string>._)).Returns(request);

            //act
            await outgoingTransferEvidenceController.EditTransferFrom(organisationId, TestFixture.Create<Guid>(), page, searchRef);

            //assert
            A.CallTo(() => mapper.Map<TransferEvidenceNotesViewModelMapTransfer, TransferEvidenceNotesViewModel>(
                    A<TransferEvidenceNotesViewModelMapTransfer>.That.Matches(t => t.TransferAllTonnage == false &&
                        t.TransferEvidenceNoteData == transferEvidenceNoteData &&
                        t.Request == request &&
                        t.SelectedNotes.Equals(selectedEvidenceNoteData) &&
                        t.AvailableNotes.Equals(availableEvidenceNoteData) &&
                        t.OrganisationId == organisationId &&
                        t.PageSize == DefaultPageSize &&
                        t.PageNumber == page &&
                        t.SearchRef == searchRef)))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditTransferFromGet_GivenMappedModel_ModelShouldBeReturned()
        {
            //arrange
            var model = TestFixture.Create<TransferEvidenceNotesViewModel>();
            A.CallTo(() => mapper.Map<TransferEvidenceNotesViewModelMapTransfer, TransferEvidenceNotesViewModel>(
                A<TransferEvidenceNotesViewModelMapTransfer>._)).Returns(model);

            A.CallTo(() => sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(
                A<string>._)).Returns(TestFixture.Create<TransferEvidenceNoteRequest>());

            //act
            var result =
                await outgoingTransferEvidenceController.EditTransferFrom(organisationId, TestFixture.Create<Guid>()) as
                    ViewResult;

            //assert
            result.Model.Should().Be(model);
        }

        [Fact]
        public async Task EditTransferFromGet_ShouldReturnView()
        {
            //arrange
            A.CallTo(() => sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(
                A<string>._)).Returns(TestFixture.Create<TransferEvidenceNoteRequest>());

            //act
            var result =
                await outgoingTransferEvidenceController.EditTransferFrom(organisationId, TestFixture.Create<Guid>()) as
                    ViewResult;

            //assert
            result.ViewName.Should().Be("EditTransferFrom");
        }

        [Fact]
        public async Task EditTransferFromPost_GivenTransferSessionTransferRequestIsNull_ShouldRedirectToManageEvidenceNotes()
        {
            //arrange
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetTransferEvidenceNoteForSchemeRequest>._)).Returns(transferEvidenceNoteData);

            A.CallTo(() =>
                sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(A<string>._)).Returns(null);

            var model = TestFixture.Create<TransferEvidenceNotesViewModel>();

            //act
            var result = await outgoingTransferEvidenceController.EditTransferFrom(model) as RedirectToRouteResult;

            //assert
            result.RouteValues["action"].Should().Be("Index");
            result.RouteValues["controller"].Should().Be("ManageEvidenceNotes");
            result.RouteValues["pcsId"].Should().Be(model.PcsId);
            result.RouteValues["area"].Should().Be("Scheme");
            result.RouteValues["tab"].Should()
                .Be(DisplayExtensions.ToDisplayString(ManageEvidenceNotesDisplayOptions.ViewAndTransferEvidence));
        }

        [Fact]
        public async Task EditTransferFromPost_TransferRequestShouldBeRetrievedFromSession()
        {
            //arrange
            var model = TestFixture.Create<TransferEvidenceNotesViewModel>();

            //act
            await outgoingTransferEvidenceController.EditTransferFrom(model);

            //assert
            A.CallTo(() =>
                sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(SessionKeyConstant.OutgoingTransferKey)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditTransferFromPost_GivenInvalidModel_ViewShouldBeReturned()
        {
            //arrange
            var model = TestFixture.Build<TransferEvidenceNotesViewModel>()
                .With(t => t.PageNumber, (int?)null)
                .With(t => t.Action, ActionEnum.Save)
                .Create();

            A.CallTo(() =>
                    sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(A<string>._))
                .Returns(TestFixture.Create<TransferEvidenceNoteRequest>());

            AddModelError();

            //act
            var result = await outgoingTransferEvidenceController.EditTransferFrom(model) as ViewResult;

            //assert
            result.ViewName.Should().Be("EditTransferFrom");
        }

        [Fact]
        public async Task EditTransferFromPost_GivenInvalidModel_SchemeShouldBeRetrievedFromCache()
        {
            // arrange
            var model = TestFixture.Build<TransferEvidenceNotesViewModel>()
                .With(t => t.PageNumber, (int?)null)
                .With(t => t.Action, ActionEnum.Save)
                .Create();

            A.CallTo(() =>
                    sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(A<string>._))
                .Returns(TestFixture.Create<TransferEvidenceNoteRequest>());

            AddModelError();

            //act
            await outgoingTransferEvidenceController.EditTransferFrom(model);

            //asset
            A.CallTo(() => cache.FetchSchemePublicInfo(model.PcsId)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditTransferFromPost_GivenInvalidModelAndSchemeIsNotBalancingScheme_BreadcrumbShouldBeSet()
        {
            //arrange
            var model = TestFixture.Build<TransferEvidenceNotesViewModel>()
                .With(t => t.PageNumber, (int?)null)
                .With(t => t.Action, ActionEnum.Save)
                .Create();

            A.CallTo(() =>
                    sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(A<string>._))
                .Returns(TestFixture.Create<TransferEvidenceNoteRequest>());

            var schemeInfo = TestFixture.Build<SchemePublicInfo>().With(s => s.IsBalancingScheme, false).Create();

            AddModelError();

            var organisationName = "OrganisationName";

            A.CallTo(() => cache.FetchOrganisationName(model.PcsId)).Returns(organisationName);
            A.CallTo(() => cache.FetchSchemePublicInfo(model.PcsId)).Returns(schemeInfo);

            //act
            await outgoingTransferEvidenceController.EditTransferFrom(model);

            // assert
            breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.SchemeManageEvidence);
            breadcrumb.OrganisationId.Should().Be(model.PcsId);
        }

        [Fact]
        public async Task EditTransferFromPost_GivenInvalidModelAndSchemeIsBalancingScheme_BreadcrumbShouldBeSet()
        {
            //arrange
            var model = TestFixture.Build<TransferEvidenceNotesViewModel>()
                .With(t => t.PageNumber, (int?)null)
                .With(t => t.Action, ActionEnum.Save)
                .Create();

            A.CallTo(() =>
                    sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(A<string>._))
                .Returns(TestFixture.Create<TransferEvidenceNoteRequest>());

            var schemeInfo = TestFixture.Build<SchemePublicInfo>().With(s => s.IsBalancingScheme, true).Create();

            AddModelError();

            var organisationName = "OrganisationName";

            A.CallTo(() => cache.FetchOrganisationName(model.PcsId)).Returns(organisationName);
            A.CallTo(() => cache.FetchSchemePublicInfo(model.PcsId)).Returns(schemeInfo);

            //act
            await outgoingTransferEvidenceController.EditTransferFrom(model);

            // assert
            breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.PbsManageEvidence);
            breadcrumb.OrganisationId.Should().Be(model.PcsId);
        }

        [Fact]
        public async Task EditTransferFromPost_GivenValidViewModelWithSelectedNotes_TransferRequestShouldBeUpdated()
        {
            //arrange
            var selectedEvidenceNotes = TestFixture.CreateMany<ViewEvidenceNoteViewModel>().ToList();

            var model = TestFixture.Build<TransferEvidenceNotesViewModel>()
                .With(m => m.EvidenceNotesDataList, selectedEvidenceNotes)
                .With(m => m.PageNumber, TestFixture.Create<int>())
                .Create();

            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetTransferEvidenceNoteForSchemeRequest>._)).Returns(transferEvidenceNoteData);

            var request = TestFixture.Build<TransferEvidenceNoteRequest>()
                .With(t => t.EvidenceNoteIds, new List<Guid>())
                .Create();

            A.CallTo(() => sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(SessionKeyConstant.OutgoingTransferKey)).Returns(request);

            //act
            await outgoingTransferEvidenceController.EditTransferFrom(model);

            //assert
            request.EvidenceNoteIds.Should()
                .BeEquivalentTo(selectedEvidenceNotes.Select(e => e.Id));
        }

        [Fact]
        public async Task EditTransferFromPost_GivenInvalidValidViewModelWithSelectedNotes_TransferRequestShouldBeUpdated()
        {
            //arrange
            var selectedEvidenceNotes = TestFixture.CreateMany<ViewEvidenceNoteViewModel>().ToList();

            var model = TestFixture.Build<TransferEvidenceNotesViewModel>()
                .With(m => m.EvidenceNotesDataList, selectedEvidenceNotes)
                .With(m => m.PageNumber, TestFixture.Create<int>())
                .Create();

            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetTransferEvidenceNoteForSchemeRequest>._)).Returns(transferEvidenceNoteData);

            var request = TestFixture.Build<TransferEvidenceNoteRequest>()
                .With(t => t.EvidenceNoteIds, new List<Guid>())
                .Create();

            A.CallTo(() => sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(SessionKeyConstant.OutgoingTransferKey)).Returns(request);

            AddModelError();

            //act
            await outgoingTransferEvidenceController.EditTransferFrom(model);

            //assert
            request.EvidenceNoteIds.Should()
                .BeEquivalentTo(selectedEvidenceNotes.Select(e => e.Id)
                    .Union(transferEvidenceNoteData.CurrentEvidenceNoteIds));
        }

        [Fact]
        public async Task EditTransferFromPost_GivenNullSessionTransferRequest_ShouldRedirectToManageEvidenceNotes()
        {
            //arrange
            var model = TestFixture.Build<TransferEvidenceNotesViewModel>()
                .With(t => t.PageNumber, (int?)null)
                .With(t => t.Action, ActionEnum.Save)
                .Create();

            A.CallTo(() => sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(A<string>._)).Returns(null);

            //act
            var result = await outgoingTransferEvidenceController.EditTransferFrom(model) as RedirectToRouteResult;

            //assert
            result.RouteValues["action"].Should().Be("Index");
            result.RouteValues["controller"].Should().Be("ManageEvidenceNotes");
            result.RouteValues["pcsId"].Should().Be(model.PcsId);
            result.RouteValues["area"].Should().Be("Scheme");
            result.RouteValues["tab"].Should()
                .Be(DisplayExtensions.ToDisplayString(ManageEvidenceNotesDisplayOptions.ViewAndTransferEvidence));
        }

        [Fact]
        public async Task EditTransferFromPost_GivenValidViewModel_ShouldRedirectToEditTonnagesRoute()
        {
            //arrange
            A.CallTo(() => sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(A<string>._)).Returns(TestFixture.Create<TransferEvidenceNoteRequest>());

            var model = TestFixture.Build<TransferEvidenceNotesViewModel>()
                .With(t => t.PageNumber, (int?)null)
                .With(t => t.Action, ActionEnum.Save)
                .Create();

            //act
            var result = await outgoingTransferEvidenceController.EditTransferFrom(model) as RedirectToRouteResult;

            //assert
            result.RouteName.Should().Be("Scheme_edit_transfer_tonnages");
            result.RouteValues["pcsId"].Should().Be(model.PcsId);
            result.RouteValues["evidenceNoteId"].Should().Be(model.ViewTransferNoteViewModel.EvidenceNoteId);
        }

        [Fact]
        public async Task EditTransferFromPost_GivenInvalidViewModel_ShouldRetrieveTransferNote()
        {
            //arrange
            var model = TestFixture.Create<TransferEvidenceNotesViewModel>();

            A.CallTo(() => sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(A<string>._)).Returns(TestFixture.Create<TransferEvidenceNoteRequest>());

            AddModelError();

            //act
            await outgoingTransferEvidenceController.EditTransferFrom(model);

            //assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetTransferEvidenceNoteForSchemeRequest>.That.Matches(r =>
                    r.EvidenceNoteId == model.ViewTransferNoteViewModel.EvidenceNoteId))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditTransferFromPost_GivenInvalidModelAndNoteData_AvailableTransferNotesShouldBeRetrieved()
        {
            //arrange
            var model = TestFixture.Create<TransferEvidenceNotesViewModel>();
            var evidenceNoteIds = TestFixture.CreateMany<Guid>().ToList();
            var categories = TestFixture.CreateMany<int>().ToList();

            var request = TestFixture.Build<TransferEvidenceNoteRequest>()
                .With(t => t.EvidenceNoteIds, evidenceNoteIds)
                .With(t => t.CategoryIds, categories)
                .Create();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetTransferEvidenceNoteForSchemeRequest>._))
                .Returns(transferEvidenceNoteData);

            A.CallTo(() => sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(A<string>._)).Returns(request);

            AddModelError();

            //act
            await outgoingTransferEvidenceController.EditTransferFrom(model);

            //assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetEvidenceNotesForTransferRequest>.That.Matches(g =>
                g.PageSize == DefaultPageSize &&
                g.PageNumber == model.PageNumber &&
                g.Categories.SequenceEqual(request.CategoryIds) &&
                g.ComplianceYear == transferEvidenceNoteData.ComplianceYear &&
                g.OrganisationId == model.PcsId &&
                g.SearchReference == null &&
                g.ExcludeEvidenceNotes.SequenceEqual(evidenceNoteIds
                    .Union(model.EvidenceNotesDataList.Select(m => m.Id))
                    .Union(transferEvidenceNoteData.CurrentEvidenceNoteIds))))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditTransferFromPost_GivenInvalidModelAndTransferRequestWithSelectedNotes_SelectedTransferNotesShouldBeRetrieved()
        {
            //arrange
            var evidenceNoteIds = TestFixture.CreateMany<Guid>().ToList();
            var categories = TestFixture.CreateMany<int>().ToList();
            var model = TestFixture.Create<TransferEvidenceNotesViewModel>();

            var request = TestFixture.Build<TransferEvidenceNoteRequest>()
                .With(t => t.EvidenceNoteIds, evidenceNoteIds)
                .With(t => t.CategoryIds, categories)
                .Create();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetTransferEvidenceNoteForSchemeRequest>._))
                .Returns(transferEvidenceNoteData);

            A.CallTo(() => sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(A<string>._)).Returns(request);

            AddModelError();

            //act
            await outgoingTransferEvidenceController.EditTransferFrom(model);

            //assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetEvidenceNotesSelectedForTransferRequest>.That.Matches(
                g =>
                    g.Categories.SequenceEqual(request.CategoryIds) &&
                    g.OrganisationId == model.PcsId &&
                    g.EvidenceNotes.SequenceEqual(evidenceNoteIds.Union(model.EvidenceNotesDataList.Select(m => m.Id))
                        .Union(transferEvidenceNoteData.CurrentEvidenceNoteIds))))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditTransferFromPost_GivenInvalidModelAndTransferRequestWithNoSelectedNotes_SelectedTransferNotesShouldNotBeRetrieved()
        {
            //arrange
            var categories = TestFixture.CreateMany<int>().ToList();
            var model = TestFixture.Build<TransferEvidenceNotesViewModel>()
                .Without(t => t.EvidenceNotesDataList).Create();
            var request = TestFixture.Build<TransferEvidenceNoteRequest>()
                .With(t => t.EvidenceNoteIds, new List<Guid>())
                .With(t => t.CategoryIds, categories)
                .Create();

            var transferEvidenceNoteData = TestFixture.Build<TransferEvidenceNoteData>()
                .With(n => n.Status, NoteStatus.Submitted)
                .Without(t => t.TransferEvidenceNoteTonnageData)
                .Create();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetTransferEvidenceNoteForSchemeRequest>._))
                .Returns(transferEvidenceNoteData);

            A.CallTo(() => sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(A<string>._)).Returns(request);

            AddModelError();

            //act
            await outgoingTransferEvidenceController.EditTransferFrom(model);

            //assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetEvidenceNotesSelectedForTransferRequest>._))
                .MustNotHaveHappened();
        }

        [Fact]
        public async Task EditTransferFromPost_GivenInvalidModelAndNoteData_ModelMapperShouldBeCalled()
        {
            //arrange
            var model = TestFixture.Create<TransferEvidenceNotesViewModel>();

            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetTransferEvidenceNoteForSchemeRequest>._)).Returns(transferEvidenceNoteData);

            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetEvidenceNotesForTransferRequest>._)).Returns(evidenceNoteData);

            var availableEvidenceNoteData =
                new EvidenceNoteSearchDataResult(TestFixture.CreateMany<EvidenceNoteData>(3).ToList(), 3);

            var selectedEvidenceNoteData =
                new EvidenceNoteSearchDataResult(TestFixture.CreateMany<EvidenceNoteData>(3).ToList(), 3);

            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetEvidenceNotesForTransferRequest>._)).Returns(availableEvidenceNoteData);

            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetEvidenceNotesSelectedForTransferRequest>._)).Returns(selectedEvidenceNoteData);

            var request = TestFixture.Create<TransferEvidenceNoteRequest>();
            A.CallTo(() => sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(A<string>._)).Returns(request);

            AddModelError();

            //act
            await outgoingTransferEvidenceController.EditTransferFrom(model);

            //assert
            A.CallTo(() => mapper.Map<TransferEvidenceNotesViewModelMapTransfer, TransferEvidenceNotesViewModel>(
                    A<TransferEvidenceNotesViewModelMapTransfer>.That.Matches(t => t.TransferAllTonnage == false &&
                        t.TransferEvidenceNoteData == transferEvidenceNoteData &&
                        t.Request == request &&
                        t.SelectedNotes.Equals(selectedEvidenceNoteData) &&
                        t.AvailableNotes.Equals(availableEvidenceNoteData) &&
                        t.OrganisationId == model.PcsId &&
                        t.PageSize == DefaultPageSize &&
                        t.PageNumber == model.PageNumber)))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditTransferFromPost_GivenInvalidModelAndMappedModel_ModelShouldBeReturned()
        {
            //arrange
            var existingModel = TestFixture.Create<TransferEvidenceNotesViewModel>();
            var model = TestFixture.Create<TransferEvidenceNotesViewModel>();
            A.CallTo(() => mapper.Map<TransferEvidenceNotesViewModelMapTransfer, TransferEvidenceNotesViewModel>(
                A<TransferEvidenceNotesViewModelMapTransfer>._)).Returns(model);

            A.CallTo(() => sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(A<string>._)).Returns(TestFixture.Create<TransferEvidenceNoteRequest>());

            AddModelError();

            //act
            var result = await outgoingTransferEvidenceController.EditTransferFrom(existingModel) as ViewResult;

            //assert
            result.Model.Should().Be(model);
        }

        [Fact]
        public async Task EditTransferFromPost_GivenInvalidModel_ShouldReturnView()
        {
            //arrange
            var model = TestFixture.Create<TransferEvidenceNotesViewModel>();
            A.CallTo(() => sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(A<string>._)).Returns(TestFixture.Create<TransferEvidenceNoteRequest>());

            AddModelError();

            //act
            var result = await outgoingTransferEvidenceController.EditTransferFrom(model) as ViewResult;

            //assert
            result.ViewName.Should().Be("EditTransferFrom");
        }

        [Fact]
        public async Task EditCategoriesGet_GivenOrganisationId_SchemeShouldBeRetrievedFromCache()
        {
            //act
            await outgoingTransferEvidenceController.EditCategories(organisationId, TestFixture.Create<Guid>());

            //asset
            A.CallTo(() => cache.FetchSchemePublicInfo(organisationId)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditCategoriesGet_GivenSchemeIsNotBalancingScheme_BreadcrumbShouldBeSet()
        {
            // arrange 
            var organisationName = "OrganisationName";
            var schemeInfo = TestFixture.Build<SchemePublicInfo>().With(s => s.IsBalancingScheme, false).Create();
            A.CallTo(() => cache.FetchOrganisationName(organisationId)).Returns(organisationName);
            A.CallTo(() => cache.FetchSchemePublicInfo(organisationId)).Returns(schemeInfo);

            // act
            await outgoingTransferEvidenceController.EditCategories(organisationId, TestFixture.Create<Guid>());

            // assert
            breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.SchemeManageEvidence);
            breadcrumb.OrganisationId.Should().Be(organisationId);
        }

        [Fact]
        public async Task EditCategoriesGet_GivenSchemeIsBalancingScheme_BreadcrumbShouldBeSet()
        {
            // arrange 
            var organisationName = "OrganisationName";
            var schemeInfo = TestFixture.Build<SchemePublicInfo>().With(s => s.IsBalancingScheme, true).Create();
            A.CallTo(() => cache.FetchOrganisationName(organisationId)).Returns(organisationName);
            A.CallTo(() => cache.FetchSchemePublicInfo(organisationId)).Returns(schemeInfo);

            // act
            await outgoingTransferEvidenceController.EditCategories(organisationId, TestFixture.Create<Guid>());

            // assert
            breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.PbsManageEvidence);
            breadcrumb.OrganisationId.Should().Be(organisationId);
        }

        [Fact]
        public async Task EditCategoriesGet_GivenEvidenceNoteId_ShouldRetrieveTransferNote()
        {
            //arrange
            var evidenceNoteId = TestFixture.Create<Guid>();

            //act
            await outgoingTransferEvidenceController.EditCategories(organisationId, evidenceNoteId);

            //assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                    A<GetTransferEvidenceNoteForSchemeRequest>.That.Matches(r => r.EvidenceNoteId == evidenceNoteId)))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditCategoriesGet_SchemesShouldBeRetrieved()
        {
            //act
            await outgoingTransferEvidenceController.EditCategories(organisationId, TestFixture.Create<Guid>());

            //assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetOrganisationScheme>.That.Matches(r => r.IncludePBS == false))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditCategoriesGet_GivenTransferNoteAndSchemes_ModelMapperShouldBeCalled()
        {
            //arrange
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetTransferEvidenceNoteForSchemeRequest>._)).Returns(transferEvidenceNoteData);

            var recipients = TestFixture.CreateMany<EntityIdDisplayNameData>().ToList();
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetOrganisationScheme>._)).Returns(recipients);

            //act
            await outgoingTransferEvidenceController.EditCategories(organisationId, TestFixture.Create<Guid>());

            //assert
            A.CallTo(() =>
                mapper.Map<TransferEvidenceNotesViewModelMapTransfer, TransferEvidenceNoteCategoriesViewModel>(
                    A<TransferEvidenceNotesViewModelMapTransfer>.That.Matches(t =>
                        t.RecipientData.SequenceEqual(recipients) &&
                        t.OrganisationId == organisationId &&
                        t.TransferEvidenceNoteData == transferEvidenceNoteData &&
                        t.ExistingTransferEvidenceNoteCategoriesViewModel == null))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditCategoriesGet_GivenTransferNote_ShouldCallGetTransferSession()
        {
            //act
            await outgoingTransferEvidenceController.EditCategories(organisationId, TestFixture.Create<Guid>());

            //assert
            A.CallTo(() => sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(SessionKeyConstant.OutgoingTransferKey))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditCategoriesGet_GivenCategoryValues_ShouldSetSelectedCategoryIdsInSession()
        {
            //arrange
            var recipientId = TestFixture.Create<Guid>();

            var selectedCategories = new List<int>()
            {
                Core.DataReturns.WeeeCategory.MedicalDevices.ToInt(),
                Core.DataReturns.WeeeCategory.ToysLeisureAndSports.ToInt(),
                Core.DataReturns.WeeeCategory.PhotovoltaicPanels.ToInt()
            };

            var totalCategory1 = TestFixture.Build<TotalCategoryValue>().With(c => c.CategoryId, 2).Create();
            var totalCategory2 = TestFixture.Build<TotalCategoryValue>().With(c => c.CategoryId, 4).Create();
            var totalCategory3 = TestFixture.Build<TotalCategoryValue>().With(c => c.CategoryId, 9).Create();

            var categoryValues = new List<TotalCategoryValue> { totalCategory1, totalCategory2, totalCategory3 };

            var request = TestFixture.Build<TransferEvidenceNoteRequest>()
                .With(c => c.CategoryIds, selectedCategories)
                .With(c => c.RecipientId, recipientId)
                .Create();

            var model = TestFixture.Build<TransferEvidenceNoteCategoriesViewModel>()
                .With(t => t.CategoryValues, categoryValues)
                .Create();

            A.CallTo(() =>
                mapper.Map<TransferEvidenceNotesViewModelMapTransfer, TransferEvidenceNoteCategoriesViewModel>(
                    A<TransferEvidenceNotesViewModelMapTransfer>._)).Returns(model);

            A.CallTo(() => sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(A<string>._)).Returns(request);

            //act
            var result =
                await outgoingTransferEvidenceController.EditCategories(organisationId, TestFixture.Create<Guid>()) as
                    ViewResult;

            //assert
            result.Model.Should().Be(model);

            var viewModel = (TransferEvidenceNoteCategoriesViewModel)result.Model;

            viewModel.CategoryBooleanViewModels.Where(c => c.CategoryId == totalCategory1.CategoryId)
                .Select(c => c.Selected)
                .First().Should().BeFalse();
            viewModel.CategoryBooleanViewModels.Where(c => c.CategoryId == totalCategory2.CategoryId)
                .Select(c => c.Selected)
                .First().Should().BeFalse();
            viewModel.CategoryBooleanViewModels.Where(c => c.CategoryId == totalCategory3.CategoryId)
                .Select(c => c.Selected)
                .First().Should().BeFalse();
            viewModel.CategoryBooleanViewModels.Where(c => c.CategoryId == selectedCategories[0])
                .Select(c => c.Selected)
                .First().Should().BeTrue();
            viewModel.CategoryBooleanViewModels.Where(c => c.CategoryId == selectedCategories[1])
                .Select(c => c.Selected)
                .First().Should().BeTrue();
            viewModel.CategoryBooleanViewModels.Where(c => c.CategoryId == selectedCategories[2])
                .Select(c => c.Selected)
                .First().Should().BeTrue();
            viewModel.SelectedSchema.Should().Be(recipientId);
        }

        [Fact]
        public async Task EditCategoriesGet_GivenTransferSessionIsNull_ShouldNotSetSelectedCategoryIdsInSession()
        {
            //arrange
            var model = TestFixture.Create<TransferEvidenceNoteCategoriesViewModel>();

            A.CallTo(() =>
                mapper.Map<TransferEvidenceNotesViewModelMapTransfer, TransferEvidenceNoteCategoriesViewModel>(
                    A<TransferEvidenceNotesViewModelMapTransfer>._)).Returns(model);

            A.CallTo(() => sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(A<string>._)).Returns(null);

            //act
            var result =
                await outgoingTransferEvidenceController.EditCategories(organisationId, TestFixture.Create<Guid>()) as
                    ViewResult;

            //assert
            result.Model.Should().Be(model);

            var viewModel = (TransferEvidenceNoteCategoriesViewModel)result.Model;

            viewModel.CategoryBooleanViewModels.All(c => c.Selected == false).Should().BeTrue();
        }

        [Fact]
        public async Task EditCategoriesGet_GivenMappedModel_ModelShouldBeReturned()
        {
            //arrange
            var model = TestFixture.Create<TransferEvidenceNoteCategoriesViewModel>();
            A.CallTo(() =>
                mapper.Map<TransferEvidenceNotesViewModelMapTransfer, TransferEvidenceNoteCategoriesViewModel>(
                    A<TransferEvidenceNotesViewModelMapTransfer>._)).Returns(model);

            //act
            var result =
                await outgoingTransferEvidenceController.EditCategories(organisationId, TestFixture.Create<Guid>()) as
                    ViewResult;

            //assert
            result.Model.Should().Be(model);
        }

        [Fact]
        public async Task EditCategoriesGet_ShouldReturnView()
        {
            //act
            var result =
                await outgoingTransferEvidenceController.EditCategories(organisationId, TestFixture.Create<Guid>()) as
                    ViewResult;

            //assert
            result.ViewName.Should().Be("EditCategories");
        }

        [Fact]
        public async Task IndexGet_GivenOrganisationId_SchemeShouldBeRetrievedFromCache()
        {
            //arrange
            var model = TestFixture.Create<TransferEvidenceNoteCategoriesViewModel>();

            //act
            await outgoingTransferEvidenceController.EditCategories(model);

            //asset
            A.CallTo(() => cache.FetchSchemePublicInfo(model.PcsId)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditCategoriesPost_GivenSchemeIsNotBalancingScheme_ShouldSetBreadcrumb()
        {
            //arrange
            var model = TestFixture.Create<TransferEvidenceNoteCategoriesViewModel>();
            var schemeInfo = TestFixture.Build<SchemePublicInfo>().With(s => s.IsBalancingScheme, false).Create();

            AddModelError();

            var organisationName = "OrganisationName";

            A.CallTo(() => cache.FetchOrganisationName(model.PcsId)).Returns(organisationName);
            A.CallTo(() => cache.FetchSchemePublicInfo(model.PcsId)).Returns(schemeInfo);

            //act
            await outgoingTransferEvidenceController.EditCategories(model);

            // assert
            breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.SchemeManageEvidence);
            breadcrumb.OrganisationId.Should().Be(model.PcsId);
        }

        [Fact]
        public async Task EditCategoriesPost_GivenSchemeIsBalancingScheme_ShouldSetBreadcrumb()
        {
            //arrange
            var model = TestFixture.Create<TransferEvidenceNoteCategoriesViewModel>();
            var schemeInfo = TestFixture.Build<SchemePublicInfo>().With(s => s.IsBalancingScheme, true).Create();

            AddModelError();

            var organisationName = "OrganisationName";

            A.CallTo(() => cache.FetchOrganisationName(model.PcsId)).Returns(organisationName);
            A.CallTo(() => cache.FetchSchemePublicInfo(model.PcsId)).Returns(schemeInfo);

            //act
            await outgoingTransferEvidenceController.EditCategories(model);

            // assert
            breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.PbsManageEvidence);
            breadcrumb.OrganisationId.Should().Be(model.PcsId);
        }

        [Fact]
        public async Task EditCategoriesPost_GivenValidModel_TransferRequestShouldBeBuilt()
        {
            //arrange
            var model = TestFixture.Create<TransferEvidenceNoteCategoriesViewModel>();

            //act
            await outgoingTransferEvidenceController.EditCategories(model);

            //assert
            A.CallTo(() => transferEvidenceRequestCreator.SelectCategoriesToRequest(model, null))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditCategoriesPost_GivenValidModelAndTransferRequest_TransferRequestShouldBeSetInSession()
        {
            //arrange
            var model = TestFixture.Create<TransferEvidenceNoteCategoriesViewModel>();
            var request = TestFixture.Create<TransferEvidenceNoteRequest>();

            A.CallTo(() =>
                    transferEvidenceRequestCreator.SelectCategoriesToRequest(A<TransferEvidenceNoteCategoriesViewModel>
                        ._, null))
                .Returns(request);

            //act
            await outgoingTransferEvidenceController.EditCategories(model);

            //assert
            A.CallTo(() => sessionService.SetTransferSessionObject(request, SessionKeyConstant.OutgoingTransferKey)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task
            EditCategoriesPost_GivenValidModelAndNullSessionTransferObject_TransferRequestShouldBeSetInSession()
        {
            //arrange
            var model = TestFixture.Create<TransferEvidenceNoteCategoriesViewModel>();
            var request = TestFixture.Create<TransferEvidenceNoteRequest>();

            A.CallTo(() =>
                    sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(A<string>._)).Returns(null);

            A.CallTo(() =>
                    transferEvidenceRequestCreator.SelectCategoriesToRequest(A<TransferEvidenceNoteCategoriesViewModel>
                        ._, null))
                .Returns(request);

            //act
            await outgoingTransferEvidenceController.EditCategories(model);

            //assert
            A.CallTo(() => sessionService.SetTransferSessionObject(request, SessionKeyConstant.OutgoingTransferKey)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task
            EditCategoriesPost_GivenValidModelAndValidSessionTransferObject_SelectCategoriesToRequestIsCalledWithTheRequest()
        {
            //arrange
            var model = TestFixture.Create<TransferEvidenceNoteCategoriesViewModel>();
            var categoryIds = TestFixture.CreateMany<int>().ToList();
            var recipientId = TestFixture.Create<Guid>();
            var organisationId = TestFixture.Create<Guid>();
            var status = NoteStatus.Approved;
            var complianceYear = TestFixture.Create<int>();
            var evidenceNoteIds = TestFixture.CreateMany<Guid>().ToList();
            var transferValues = TestFixture.CreateMany<TransferTonnageValue>().ToList();
            var request = TestFixture.Build<TransferEvidenceNoteRequest>()
                .With(c => c.CategoryIds, categoryIds)
                .With(r => r.RecipientId, recipientId)
                .With(e => e.EvidenceNoteIds, evidenceNoteIds)
                .With(o => o.OrganisationId, organisationId)
                .With(s => s.Status, status)
                .With(t => t.TransferValues, transferValues)
                .With(c => c.ComplianceYear, complianceYear)
                .Create();

            A.CallTo(() =>
                sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(A<string>._)).Returns(request);

            //act
            await outgoingTransferEvidenceController.EditCategories(model);

            //assert
            A.CallTo(() =>
                    transferEvidenceRequestCreator.SelectCategoriesToRequest(
                        A<TransferEvidenceNoteCategoriesViewModel>._,
                        A<TransferEvidenceNoteRequest>.That.Matches(r => r.CategoryIds.SequenceEqual(categoryIds) &&
                                                                         r.RecipientId.Equals(recipientId) &&
                                                                         r.EvidenceNoteIds.SequenceEqual(
                                                                             evidenceNoteIds) &&
                                                                         r.OrganisationId.Equals(organisationId) &&
                                                                         r.Status.Equals(status) &&
                                                                         r.TransferValues
                                                                             .SequenceEqual(transferValues) &&
                                                                         r.ComplianceYear.Equals(complianceYear))))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task
            EditCategoriesPost_GivenValidModelAndValidSessionTransferObject_SetTransferSessionObjectWithTheRequest()
        {
            //arrange
            var model = TestFixture.Create<TransferEvidenceNoteCategoriesViewModel>();
            var existingRequest = TestFixture.Create<TransferEvidenceNoteRequest>();

            var categoryIds = TestFixture.CreateMany<int>().ToList();
            var recipientId = TestFixture.Create<Guid>();
            var organisationId = TestFixture.Create<Guid>();
            var status = NoteStatus.Approved;
            var complianceYear = TestFixture.Create<int>();
            var evidenceNoteIds = TestFixture.CreateMany<Guid>().ToList();
            var transferValues = TestFixture.CreateMany<TransferTonnageValue>().ToList();
            var request = TestFixture.Build<TransferEvidenceNoteRequest>()
                .With(c => c.CategoryIds, categoryIds)
                .With(r => r.RecipientId, recipientId)
                .With(e => e.EvidenceNoteIds, evidenceNoteIds)
                .With(o => o.OrganisationId, organisationId)
                .With(s => s.Status, status)
                .With(t => t.TransferValues, transferValues)
                .With(c => c.ComplianceYear, complianceYear)
                .Create();

            A.CallTo(() =>
                sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(A<string>._)).Returns(existingRequest);

            A.CallTo(() => transferEvidenceRequestCreator.SelectCategoriesToRequest(model, existingRequest))
                .Returns(request);

            //act
            await outgoingTransferEvidenceController.EditCategories(model);

            //assert
            A.CallTo(() => sessionService.SetTransferSessionObject(
                A<object>.That.Matches(o => ((TransferEvidenceNoteRequest)o).CategoryIds.SequenceEqual(categoryIds) &&
                                            ((TransferEvidenceNoteRequest)o).RecipientId == recipientId &&
                                            ((TransferEvidenceNoteRequest)o).ComplianceYear == complianceYear &&
                                            ((TransferEvidenceNoteRequest)o).EvidenceNoteIds.SequenceEqual(
                                                evidenceNoteIds) &&
                                            ((TransferEvidenceNoteRequest)o).OrganisationId == organisationId &&
                                            ((TransferEvidenceNoteRequest)o).CategoryIds.SequenceEqual(categoryIds) &&
                                            ((TransferEvidenceNoteRequest)o).Status == status &&
                                            ((TransferEvidenceNoteRequest)o).TransferValues.SequenceEqual(
                                                transferValues)),
                SessionKeyConstant.OutgoingTransferKey)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditCategoriesPost_GivenValidModel_ShouldBeRedirectedToEditNotes()
        {
            //arrange
            var model = TestFixture.Create<TransferEvidenceNoteCategoriesViewModel>();

            //act
            var result = await outgoingTransferEvidenceController.EditCategories(model) as RedirectToRouteResult;

            //assert
            result.RouteName.Should().Be("Scheme_edit_transfer_notes");
            result.RouteValues["pcsId"].Should().Be(model.PcsId);
            result.RouteValues["evidenceNoteId"].Should().Be(model.ViewTransferNoteViewModel.EvidenceNoteId);
        }

        [Fact]
        public async Task EditCategoriesPost_GivenInvalidModel_ViewShouldBeReturned()
        {
            //arrange
            var model = TestFixture.Create<TransferEvidenceNoteCategoriesViewModel>();

            AddModelError();

            //act
            var result = await outgoingTransferEvidenceController.EditCategories(model) as ViewResult;

            //assert
            result.ViewName.Should().Be("EditCategories");
        }

        [Fact]
        public async Task EditCategoriesPost_GivenEvidenceNoteId_ShouldRetrieveTransferNote()
        {
            //arrange
            var model = TestFixture.Create<TransferEvidenceNoteCategoriesViewModel>();

            AddModelError();

            //act
            await outgoingTransferEvidenceController.EditCategories(model);

            //assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetTransferEvidenceNoteForSchemeRequest>.That.Matches(r =>
                    r.EvidenceNoteId == model.ViewTransferNoteViewModel.EvidenceNoteId))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditCategoriesPost_RecipientsShouldBeRetrieved()
        {
            //arrange
            var model = TestFixture.Create<TransferEvidenceNoteCategoriesViewModel>();
            AddModelError();

            //act
            await outgoingTransferEvidenceController.EditCategories(model);

            //assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetOrganisationScheme>.That.Matches(r => r.IncludePBS == false))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditCategoriesPost_GivenTransferNoteAndSchemes_ModelMapperShouldBeCalled()
        {
            //arrange
            var model = TestFixture.Create<TransferEvidenceNoteCategoriesViewModel>();

            AddModelError();

            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetTransferEvidenceNoteForSchemeRequest>._)).Returns(transferEvidenceNoteData);

            var recipientData = TestFixture.CreateMany<EntityIdDisplayNameData>().ToList();
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetOrganisationScheme>._)).Returns(recipientData);

            //act
            await outgoingTransferEvidenceController.EditCategories(model);

            //assert
            A.CallTo(() =>
                mapper.Map<TransferEvidenceNotesViewModelMapTransfer, TransferEvidenceNoteCategoriesViewModel>(
                    A<TransferEvidenceNotesViewModelMapTransfer>.That.Matches(t =>
                        t.RecipientData.SequenceEqual(recipientData) &&
                        t.OrganisationId == model.PcsId &&
                        t.TransferEvidenceNoteData == transferEvidenceNoteData &&
                        t.ExistingTransferEvidenceNoteCategoriesViewModel == model))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditCategoriesPost_GivenMappedModel_ModelShouldBeReturned()
        {
            //arrange
            var model = TestFixture.Create<TransferEvidenceNoteCategoriesViewModel>();

            AddModelError();

            var refreshedModel = TestFixture.Create<TransferEvidenceNoteCategoriesViewModel>();
            A.CallTo(() =>
                mapper.Map<TransferEvidenceNotesViewModelMapTransfer, TransferEvidenceNoteCategoriesViewModel>(
                    A<TransferEvidenceNotesViewModelMapTransfer>._)).Returns(refreshedModel);

            //act
            var result = await outgoingTransferEvidenceController.EditCategories(model) as ViewResult;

            //assert
            result.Model.Should().Be(refreshedModel);
        }

        [Fact]
        public async Task EditTonnagesPost_GivenModel_SchemeShouldBeRetrievedFromCache()
        {
            //arrange
            var model = TestFixture.Build<TransferEvidenceTonnageViewModel>()
                .With(m => m.PcsId, organisationId).Create();

            //act
            await outgoingTransferEvidenceController.EditTonnages(model);

            //asset
            A.CallTo(() => cache.FetchSchemePublicInfo(model.PcsId)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditTonnagesPost_GivenModelAndSchemeIsNotBalancingScheme_BreadcrumbShouldBeSet()
        {
            // arrange 
            var organisationName = "OrganisationName";
            var model = TestFixture.Build<TransferEvidenceTonnageViewModel>()
                .With(m => m.PcsId, organisationId).Create();
            var schemeInfo = TestFixture.Build<SchemePublicInfo>().With(s => s.IsBalancingScheme, false).Create();

            A.CallTo(() => cache.FetchOrganisationName(model.PcsId)).Returns(organisationName);
            A.CallTo(() => cache.FetchSchemePublicInfo(model.PcsId)).Returns(schemeInfo);

            // act
            await outgoingTransferEvidenceController.EditTonnages(model);

            // assert
            breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.SchemeManageEvidence);
            breadcrumb.OrganisationId.Should().Be(organisationId);
        }

        [Fact]
        public async Task EditTonnagesPost_GivenModelAndSchemeIsBalancingScheme_BreadcrumbShouldBeSet()
        {
            // arrange 
            var organisationName = "OrganisationName";
            var model = TestFixture.Build<TransferEvidenceTonnageViewModel>()
                .With(m => m.PcsId, organisationId).Create();
            var schemeInfo = TestFixture.Build<SchemePublicInfo>().With(s => s.IsBalancingScheme, true).Create();

            A.CallTo(() => cache.FetchOrganisationName(model.PcsId)).Returns(organisationName);
            A.CallTo(() => cache.FetchSchemePublicInfo(model.PcsId)).Returns(schemeInfo);

            // act
            await outgoingTransferEvidenceController.EditTonnages(model);

            // assert
            breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.PbsManageEvidence);
            breadcrumb.OrganisationId.Should().Be(organisationId);
        }

        [Fact]
        public async Task EditTonnagesPost_GivenInvalidModel_ShouldRetrieveTransferNote()
        {
            //arrange
            var model = TestFixture.Build<TransferEvidenceTonnageViewModel>()
                .With(m => m.PcsId, organisationId)
                .Create();

            AddModelError();

            //act
            await outgoingTransferEvidenceController.EditTonnages(model);

            //assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetTransferEvidenceNoteForSchemeRequest>.That.Matches(r =>
                    r.EvidenceNoteId == model.ViewTransferNoteViewModel.EvidenceNoteId))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditTonnagesPost_GivenInvalidModel_ShouldBeRetrievedFromSession()
        {
            //arrange
            var model = TestFixture.Build<TransferEvidenceTonnageViewModel>()
                .With(m => m.PcsId, organisationId)
                .Create();

            AddModelError();

            //act
            await outgoingTransferEvidenceController.EditTonnages(model);

            //assert
            A.CallTo(() => sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(SessionKeyConstant.OutgoingTransferKey))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task
            EditTonnagesPost_GivenGivenInvalidModelAndExistingSelectedEvidenceNotesAlongWithTransferNoteData_TransferNotesShouldBeRetrieved()
        {
            //arrange
            var evidenceNoteId1 = TestFixture.Create<Guid>();
            var evidenceNoteId2 = TestFixture.Create<Guid>();
            var evidenceNoteId3 = TestFixture.Create<Guid>();

            var sessionEvidenceNotes = new List<Guid>()
            {
                evidenceNoteId1,
                evidenceNoteId2,
                evidenceNoteId3
            };

            var transferEvidence = TestFixture.Build<TransferEvidenceNoteData>()
                .With(t => t.TransferEvidenceNoteTonnageData, new List<TransferEvidenceNoteTonnageData>()
                {
                    TestFixture.Build<TransferEvidenceNoteTonnageData>().With(e => e.OriginalNoteId, evidenceNoteId1)
                        .Create(),
                    TestFixture.Build<TransferEvidenceNoteTonnageData>().With(e => e.OriginalNoteId, evidenceNoteId2)
                        .Create(),
                }).Create();

            var sessionCategories = new List<int>()
            {
                Core.DataReturns.WeeeCategory.MedicalDevices.ToInt(),
                Core.DataReturns.WeeeCategory.ToysLeisureAndSports.ToInt()
            };

            var request = TestFixture.Build<TransferEvidenceNoteRequest>()
                .With(t => t.EvidenceNoteIds, sessionEvidenceNotes)
                .With(c => c.CategoryIds, sessionCategories)
                .Create();

            A.CallTo(() => sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(A<string>._)).Returns(request);

            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetTransferEvidenceNoteForSchemeRequest>._)).Returns(transferEvidence);

            var model = TestFixture.Build<TransferEvidenceTonnageViewModel>()
                .With(m => m.PcsId, organisationId)
                .Create();

            AddModelError();

            //act
            await outgoingTransferEvidenceController.EditTonnages(model);

            //assert
            var expectedEvidenceNoteIds = transferEvidence.TransferEvidenceNoteTonnageData
                .Select(t => t.OriginalNoteId)
                .ToList().Union(sessionEvidenceNotes)
                .Distinct().ToList();

            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                    A<GetEvidenceNotesSelectedForTransferRequest>.That.Matches(r =>
                        r.Categories.SequenceEqual(sessionCategories) &&
                        r.OrganisationId == organisationId &&
                        r.EvidenceNotes.SequenceEqual(expectedEvidenceNoteIds))))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task
            EditTonnagesPost_GivenInvalidViewModelAndExistingSelectedEvidenceNotesThatHaveBeenDeselectedAndSessionTransferRequest_TransferNotesShouldBeRetrieved()
        {
            //arrange
            var removedEvidenceNoteId = TestFixture.Create<Guid>();
            var newlySelectedEvidenceNoteId = TestFixture.Create<Guid>();

            var sessionEvidenceNotes = new List<Guid>()
            {
                newlySelectedEvidenceNoteId
            };

            var transferEvidence = TestFixture.Build<TransferEvidenceNoteData>()
                .With(t => t.TransferEvidenceNoteTonnageData, new List<TransferEvidenceNoteTonnageData>()
                {
                    TestFixture.Build<TransferEvidenceNoteTonnageData>()
                        .With(e => e.OriginalNoteId, removedEvidenceNoteId).Create()
                }).Create();

            var sessionCategories = new List<int>()
            {
                Core.DataReturns.WeeeCategory.MedicalDevices.ToInt(),
                Core.DataReturns.WeeeCategory.ToysLeisureAndSports.ToInt()
            };

            var request = TestFixture.Build<TransferEvidenceNoteRequest>()
                .With(t => t.EvidenceNoteIds, sessionEvidenceNotes)
                .With(c => c.CategoryIds, sessionCategories)
                .Create();

            A.CallTo(() => sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(A<string>._)).Returns(request);

            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetTransferEvidenceNoteForSchemeRequest>._)).Returns(transferEvidence);

            var model = TestFixture.Build<TransferEvidenceTonnageViewModel>()
                .With(m => m.PcsId, organisationId)
                .Create();

            AddModelError();

            //act
            await outgoingTransferEvidenceController.EditTonnages(model);

            //assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                    A<GetEvidenceNotesSelectedForTransferRequest>.That.Matches(r =>
                        r.Categories.SequenceEqual(sessionCategories) &&
                        r.OrganisationId == organisationId &&
                        r.EvidenceNotes.SequenceEqual(sessionEvidenceNotes))))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task
            EditTonnagesPost_GivenInvalidViewModelAndExistingSelectedEvidenceNotesThatHaveBeenDeselectedAndNullSessionTransferRequest_TransferNotesShouldBeRetrieved()
        {
            //arrange
            var removedEvidenceNoteId = TestFixture.Create<Guid>();

            var transferEvidence = TestFixture.Build<TransferEvidenceNoteData>()
                .With(t => t.TransferEvidenceNoteTonnageData, new List<TransferEvidenceNoteTonnageData>()
                {
                    TestFixture.Build<TransferEvidenceNoteTonnageData>()
                        .With(e => e.OriginalNoteId, removedEvidenceNoteId).Create()
                }).Create();

            A.CallTo(() => sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(A<string>._)).Returns(null);

            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetTransferEvidenceNoteForSchemeRequest>._)).Returns(transferEvidence);

            var model = TestFixture.Build<TransferEvidenceTonnageViewModel>()
                .With(m => m.PcsId, organisationId)
                .Create();

            AddModelError();

            //act
            await outgoingTransferEvidenceController.EditTonnages(model);

            //assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                    A<GetEvidenceNotesSelectedForTransferRequest>.That.Matches(r =>
                        r.Categories.SequenceEqual(transferEvidence.CategoryIds) &&
                        r.OrganisationId == organisationId &&
                        r.EvidenceNotes.SequenceEqual(transferEvidence.TransferEvidenceNoteTonnageData
                            .Select(t => t.OriginalNoteId).ToList()))))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditTonnagesPost_GivenInvalidModelAndTransferNoteData_TransferNotesShouldBeRetrieved()
        {
            //arrange
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetTransferEvidenceNoteForSchemeRequest>._)).Returns(transferEvidenceNoteData);

            var categories = transferEvidenceNoteData.CategoryIds;
            var existingNotes = transferEvidenceNoteData.TransferEvidenceNoteTonnageData.Select(t => t.OriginalNoteId)
                .ToList();

            var model = TestFixture.Build<TransferEvidenceTonnageViewModel>()
                .With(m => m.PcsId, organisationId)
                .Create();

            AddModelError();

            //act
            await outgoingTransferEvidenceController.EditTonnages(model);

            //assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                    A<GetEvidenceNotesSelectedForTransferRequest>.That.Matches(r =>
                        r.OrganisationId == organisationId
                        && r.Categories.SequenceEqual(categories) &&
                        r.EvidenceNotes.SequenceEqual(existingNotes))))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task
            EditTonnagesPost_GivenInvalidModelAndTransferNoteDataAndNullSessionData_TransferNotesShouldBeRetrieved()
        {
            //arrange
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetTransferEvidenceNoteForSchemeRequest>._)).Returns(transferEvidenceNoteData);

            A.CallTo(() => sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(A<string>._)).Returns(null);

            var categories = transferEvidenceNoteData.CategoryIds;
            var existingNotes = transferEvidenceNoteData.TransferEvidenceNoteTonnageData.Select(t => t.OriginalNoteId)
                .ToList();

            var model = TestFixture.Build<TransferEvidenceTonnageViewModel>()
                .With(m => m.PcsId, organisationId)
                .Create();

            AddModelError();

            //act
            await outgoingTransferEvidenceController.EditTonnages(model);

            //assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                    A<GetEvidenceNotesSelectedForTransferRequest>.That.Matches(r =>
                        r.OrganisationId == organisationId &&
                        r.Categories.SequenceEqual(categories) &&
                        r.EvidenceNotes.SequenceEqual(existingNotes))))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditTonnagesPost_GivenInvalidModelTransferNoteAndTransferNotes_ModelMapperShouldBeCalled()
        {
            //arrange
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetTransferEvidenceNoteForSchemeRequest>._)).Returns(transferEvidenceNoteData);

            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetEvidenceNotesSelectedForTransferRequest>._)).Returns(evidenceNoteData);

            var request = TestFixture.Create<TransferEvidenceNoteRequest>();

            A.CallTo(() => sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(A<string>._)).Returns(request);

            var model = TestFixture.Build<TransferEvidenceTonnageViewModel>()
                .With(m => m.PcsId, organisationId)
                .Create();

            AddModelError();

            //act
            await outgoingTransferEvidenceController.EditTonnages(model);

            //assert
            A.CallTo(() => mapper.Map<TransferEvidenceNotesViewModelMapTransfer, TransferEvidenceTonnageViewModel>(
                    A<TransferEvidenceNotesViewModelMapTransfer>.That.Matches(t => t.TransferAllTonnage == false &&
                        t.TransferEvidenceNoteData == transferEvidenceNoteData &&
                        t.SelectedNotes.Equals(evidenceNoteData) &&
                        t.OrganisationId == organisationId &&
                        t.ReturnToEditDraftTransfer == false &&
                        t.Request == request)))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditTonnagesPost_GivenInvalidViewModelMappedModel_ModelShouldBeReturned()
        {
            //arrange
            A.CallTo(() => mapper.Map<TransferEvidenceNotesViewModelMapTransfer, TransferEvidenceTonnageViewModel>(
                A<TransferEvidenceNotesViewModelMapTransfer>._)).Returns(transferEvidenceTonnageViewModel);

            var model = TestFixture.Build<TransferEvidenceTonnageViewModel>()
                .With(m => m.PcsId, organisationId)
                .Create();

            AddModelError();

            //act
            var result = await outgoingTransferEvidenceController.EditTonnages(model) as ViewResult;

            //assert
            result.Model.Should().Be(transferEvidenceTonnageViewModel);
        }

        [Fact]
        public async Task EditTonnagesPost_GivenInvalidModel_ShouldReturnView()
        {
            //arrange
            var model = TestFixture.Build<TransferEvidenceTonnageViewModel>()
                .With(m => m.PcsId, organisationId)
                .Create();

            AddModelError();

            //act
            var result = await outgoingTransferEvidenceController.EditTonnages(model) as ViewResult;

            //assert
            result.ViewName.Should().Be("EditTonnages");
        }

        [Fact]
        public async Task EditTonnagesPost_GivenValidModel_SessionObjectShouldBeRetrieved()
        {
            //act
            await outgoingTransferEvidenceController.EditTonnages(transferEvidenceTonnageViewModel);

            //assert
            A.CallTo(() =>
                    sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(SessionKeyConstant.OutgoingTransferKey))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditTonnagesPost_GivenValidModelAndNullExistingRequest_RequestShouldBeCreated()
        {
            //arrange
            A.CallTo(() =>
                sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(SessionKeyConstant.TransferNoteKey)).Returns(null);

            //act
            await outgoingTransferEvidenceController.EditTonnages(transferEvidenceTonnageViewModel);

            //assert
            A.CallTo(() =>
                    transferEvidenceRequestCreator.EditSelectTonnageToRequest(null, transferEvidenceTonnageViewModel))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditTonnagesPost_GivenValidModelAndExistingRequest_RequestShouldBeCreated()
        {
            //arrange
            var request = TestFixture.Create<TransferEvidenceNoteRequest>();

            A.CallTo(() =>
                    sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(SessionKeyConstant.OutgoingTransferKey))
                .Returns(request);

            A.CallTo(() => transferEvidenceRequestCreator.EditSelectTonnageToRequest(A<TransferEvidenceNoteRequest>._,
                A<TransferEvidenceTonnageViewModel>._)).Returns(TestFixture.Create<EditTransferEvidenceNoteRequest>());

            //act
            await outgoingTransferEvidenceController.EditTonnages(transferEvidenceTonnageViewModel);

            //assert
            A.CallTo(() =>
                    transferEvidenceRequestCreator.EditSelectTonnageToRequest(request,
                        transferEvidenceTonnageViewModel))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditTonnagesPost_GivenValid_ApiShouldBeCalled()
        {
            //arrange
            var request = TestFixture.Create<EditTransferEvidenceNoteRequest>();

            A.CallTo(() => transferEvidenceRequestCreator.EditSelectTonnageToRequest(A<TransferEvidenceNoteRequest>._,
                A<TransferEvidenceTonnageViewModel>._)).Returns(request);

            //act
            await outgoingTransferEvidenceController.EditTonnages(transferEvidenceTonnageViewModel);

            //assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._, request)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task
            EditTonnagesPost_GivenValidModelAndActionIsSaveAndNoteIsReturned_TempDataNotificationShouldBeSetToReturnedSaved()
        {
            //arrange
            transferEvidenceTonnageViewModel.ViewTransferNoteViewModel.Status = NoteStatus.Returned;
            transferEvidenceTonnageViewModel.Action = ActionEnum.Save;

            //act
            await outgoingTransferEvidenceController.EditTonnages(transferEvidenceTonnageViewModel);

            //assert
            outgoingTransferEvidenceController.TempData[ViewDataConstant.TransferEvidenceNoteDisplayNotification]
                .Should().Be(NoteUpdatedStatusEnum.ReturnedSaved);
        }

        [Theory]
        [ClassData(typeof(NoteStatusCoreData))]
        public async Task
            EditTonnagesPost_GivenValidModelAndActionIsSaveAndNoteIsNotReturned_TempDataNotificationShouldBeSetToRequestAction(
                NoteStatus status)
        {
            if (status == NoteStatus.Returned)
            {
                return;
            }

            //arrange
            var request = TestFixture.Build<EditTransferEvidenceNoteRequest>().With(r => r.Status, status).Create();

            A.CallTo(() => transferEvidenceRequestCreator.EditSelectTonnageToRequest(
                A<EditTransferEvidenceNoteRequest>._,
                A<TransferEvidenceTonnageViewModel>._)).Returns(request);

            transferEvidenceTonnageViewModel.ViewTransferNoteViewModel.Status = status;
            transferEvidenceTonnageViewModel.Action = ActionEnum.Save;

            //act
            await outgoingTransferEvidenceController.EditTonnages(transferEvidenceTonnageViewModel);

            //assert
            outgoingTransferEvidenceController.TempData[ViewDataConstant.TransferEvidenceNoteDisplayNotification]
                .Should().Be((NoteUpdatedStatusEnum)status);
        }

        [Fact]
        public async Task EditTonnagesPost_GivenValidModelAndSave_ShouldRedirectToViewTransferNote()
        {
            //arrange
            transferEvidenceTonnageViewModel.Action = ActionEnum.Save;

            //act
            var result =
                await outgoingTransferEvidenceController.EditTonnages(transferEvidenceTonnageViewModel) as
                    RedirectToRouteResult;

            //assert
            result.RouteName.Should().Be(SchemeTransferEvidenceRedirect.ViewDraftTransferEvidenceRouteName);
            result.RouteValues["pcsId"].Should().Be(transferEvidenceTonnageViewModel.PcsId);
            result.RouteValues["evidenceNoteId"].Should()
                .Be(transferEvidenceTonnageViewModel.ViewTransferNoteViewModel.EvidenceNoteId);
            result.RouteValues["redirectTab"].Should().Be(DisplayExtensions.ToDisplayString(
                ManageEvidenceNotesDisplayOptions.OutgoingTransfers));
        }

        [Fact]
        public async Task EditTonnagesPost_GivenValidModelAndSubmit_ShouldRedirectToViewTransferNote()
        {
            //arrange
            transferEvidenceTonnageViewModel.Action = ActionEnum.Submit;

            //act
            var result =
                await outgoingTransferEvidenceController.EditTonnages(transferEvidenceTonnageViewModel) as
                    RedirectToRouteResult;

            //assert
            result.RouteName.Should().Be(SchemeTransferEvidenceRedirect.ViewSubmittedTransferEvidenceRouteName);
            result.RouteValues["pcsId"].Should().Be(transferEvidenceTonnageViewModel.PcsId);
            result.RouteValues["evidenceNoteId"].Should()
                .Be(transferEvidenceTonnageViewModel.ViewTransferNoteViewModel.EvidenceNoteId);
            result.RouteValues["redirectTab"].Should().Be(DisplayExtensions.ToDisplayString(
                ManageEvidenceNotesDisplayOptions.OutgoingTransfers));
        }

        [Theory]
        [InlineData(ActionEnum.Save, NoteUpdatedStatusEnum.ReturnedSaved)]
        [InlineData(ActionEnum.Submit, NoteUpdatedStatusEnum.ReturnedSubmitted)]
        public async Task EditTonnagesPost_GivenValidModel_AndReturnedNote_AndAction_ShouldSetCorrectStatus(
            ActionEnum action, NoteUpdatedStatusEnum expectedStatus)
        {
            //arrange
            transferEvidenceTonnageViewModel.Action = action;
            transferEvidenceTonnageViewModel.ViewTransferNoteViewModel.Status = NoteStatus.Returned;

            //act
            await outgoingTransferEvidenceController.EditTonnages(transferEvidenceTonnageViewModel);

            //assert
            outgoingTransferEvidenceController.TempData[ViewDataConstant.TransferEvidenceNoteDisplayNotification]
                .Should().Be(expectedStatus);
        }

        [Fact]
        public async Task EditTonnagesPost_GivenModelActionIsBack_ShouldSetTransferSessionObjectWithGivenProperties()
        {
            // arrange 
            var complianceYear = TestFixture.Create<int>();
            var recipientId = TestFixture.Create<Guid>();
            var pcsIds = TestFixture.Create<Guid>();
            var transferCategoryValues = TestFixture.CreateMany<TransferEvidenceCategoryValue>().ToList();

            var model = TestFixture.Build<TransferEvidenceTonnageViewModel>()
                .With(t => t.ComplianceYear, complianceYear)
                .With(t => t.RecipientId, recipientId)
                .With(t => t.PcsId, pcsIds)
                .With(t => t.TransferCategoryValues, transferCategoryValues)
                .With(t => t.Action, ActionEnum.Back)
                .Create();

            AddModelError();

            // act
            await outgoingTransferEvidenceController.EditTonnages(model);

            // assert
            A.CallTo(() => sessionService.SetTransferSessionObject(
                A<object>.That.Matches(o =>
                    ((TransferEvidenceTonnageViewModel)o).TransferCategoryValues
                    .SequenceEqual(transferCategoryValues) &&
                    ((TransferEvidenceTonnageViewModel)o).RecipientId == recipientId &&
                    ((TransferEvidenceTonnageViewModel)o).PcsId == pcsIds &&
                    ((TransferEvidenceTonnageViewModel)o).ComplianceYear == complianceYear &&
                    ((TransferEvidenceTonnageViewModel)o).Action == ActionEnum.Back),
                SessionKeyConstant.EditTransferEvidenceTonnageViewModel)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task
            EditTonnagesPost_GivenModelActionIsBackAndReturnToEditDraftTransferIsTrue_ShouldRedirectToEditDraftTransfer()
        {
            // arrange 
            var pcsIds = TestFixture.Create<Guid>();

            var model = TestFixture.Build<TransferEvidenceTonnageViewModel>()
                .With(t => t.Action, ActionEnum.Back)
                .With(t => t.ReturnToEditDraftTransfer, true)
                .With(t => t.PcsId, pcsIds)
                .Create();

            AddModelError();

            // act
            var result = await outgoingTransferEvidenceController.EditTonnages(model) as RedirectToRouteResult;

            // assert
            result.RouteValues["pcsId"].Should().Be(model.PcsId);
            result.RouteValues["evidenceNoteId"].Should().Be(model.ViewTransferNoteViewModel.EvidenceNoteId);
            result.RouteValues["returnToView"].Should().Be(false);
            result.RouteValues["action"].Should().Be("EditDraftTransfer");
            result.RouteValues["controller"].Should().Be("OutgoingTransfers");
        }

        [Fact]
        public async Task
            EditTonnagesPost_GivenModelActionIsBackAndReturnToEditDraftTransferIsFalse_ShouldRedirectToEditDraftTransfer()
        {
            // arrange 
            var pcsIds = TestFixture.Create<Guid>();

            var model = TestFixture.Build<TransferEvidenceTonnageViewModel>()
                .With(t => t.Action, ActionEnum.Back)
                .With(t => t.ReturnToEditDraftTransfer, false)
                .With(t => t.PcsId, pcsIds)
                .Create();

            AddModelError();

            // act
            var result = await outgoingTransferEvidenceController.EditTonnages(model) as RedirectToRouteResult;

            // assert
            result.RouteValues["pcsId"].Should().Be(model.PcsId);
            result.RouteValues["evidenceNoteId"].Should().Be(model.ViewTransferNoteViewModel.EvidenceNoteId);
            result.RouteValues["action"].Should().Be("EditTransferFrom");
            result.RouteValues["controller"].Should().Be("OutgoingTransfers");
        }

        private void AddModelError()
        {
            outgoingTransferEvidenceController.ModelState.AddModelError("error", "error");
        }

        [Fact]
        public async Task EditCategoriesGet_GivenPageNumber_ViewBagShouldBePopulatedWithPageNumber()
        {
            // Arrange
            var pageNumber = 3;

            //act
            var result =
                await outgoingTransferEvidenceController.EditCategories(TestFixture.Create<Guid>(),
                    TestFixture.Create<Guid>(), pageNumber) as ViewResult;

            //assert
            Assert.Equal(pageNumber, result.ViewBag.Page);
        }

        [Fact]
        public void DeselectEvidenceNotePost_TransferRequestShouldBeRetrievedFromSession()
        {
            //arrange
            var model = TestFixture.Create<TransferDeselectEvidenceNoteModel>();

            A.CallTo(() => sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(SessionKeyConstant.OutgoingTransferKey))
                .Returns(TestFixture.Create<TransferEvidenceNoteRequest>());

            //act
            outgoingTransferEvidenceController.DeselectEvidenceNote(model);

            //assert
            A.CallTo(() => sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(
                SessionKeyConstant.OutgoingTransferKey)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void DeselectEvidenceNotePost_GivenDeSelectedEvidenceNote_TransferRequestShouldBeUpdated()
        {
            //arrange
            var model = TestFixture.Create<TransferDeselectEvidenceNoteModel>();

            var request = TestFixture.Build<TransferEvidenceNoteRequest>()
                .With(r => r.EvidenceNoteIds, new List<Guid>() { model.DeselectedEvidenceNoteId }).Create();

            A.CallTo(() => sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(SessionKeyConstant.OutgoingTransferKey)).Returns(request);

            //act
            outgoingTransferEvidenceController.DeselectEvidenceNote(model);

            //assert
            request.DeselectedEvidenceNoteIds.Should().Contain(model.DeselectedEvidenceNoteId);
            request.EvidenceNoteIds.Should().NotContain(model.DeselectedEvidenceNoteId);
        }

        [Fact]
        public void DeselectEvidenceNotePost_ShouldRedirectToAction()
        {
            //arrange
            var model = TestFixture.Create<TransferDeselectEvidenceNoteModel>();

            A.CallTo(() => sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(SessionKeyConstant.OutgoingTransferKey))
                .Returns(TestFixture.Create<TransferEvidenceNoteRequest>());

            //act
            var result = outgoingTransferEvidenceController.DeselectEvidenceNote(model) as RedirectToRouteResult;

            //assert
            result.RouteValues["action"].Should().Be("EditTransferFrom");
            result.RouteValues["controller"].Should().BeNull();
            result.RouteValues["pcsId"].Should().Be(model.PcsId);
            result.RouteValues["evidenceNoteId"].Should().Be(model.EditEvidenceNoteId);
            result.RouteValues["page"].Should().Be(model.Page);
        }

        [Fact]
        public async Task SelectEvidenceNotePost_TransferRequestShouldBeRetrievedFromSession()
        {
            //arrange
            var model = TestFixture.Create<TransferSelectEvidenceNoteModel>();

            A.CallTo(() => sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(SessionKeyConstant.OutgoingTransferKey))
                .Returns(TestFixture.Create<TransferEvidenceNoteRequest>());

            //act
            await outgoingTransferEvidenceController.SelectEvidenceNote(model);

            //assert
            A.CallTo(() => sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(SessionKeyConstant.OutgoingTransferKey)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task SelectEvidenceNotePost_GivenValidModel_ShouldRedirectToAction()
        {
            //arrange
            var model = TestFixture.Create<TransferSelectEvidenceNoteModel>();

            A.CallTo(() => sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(SessionKeyConstant.OutgoingTransferKey))
                .Returns(TestFixture.Create<TransferEvidenceNoteRequest>());

            //act
            var result = await outgoingTransferEvidenceController.SelectEvidenceNote(model) as RedirectToRouteResult;

            //assert
            result.RouteValues["action"].Should().Be("EditTransferFrom");
            result.RouteValues["controller"].Should().BeNull();
            result.RouteValues["pcsId"].Should().Be(model.PcsId);
            result.RouteValues["page"].Should().Be(model.NewPage);
        }

        [Fact]
        public async Task SelectEvidenceNotePost_GivenSelectedEvidenceNoteAndModelIsValid_TransferRequestShouldBeUpdated()
        {
            //arrange
            var model = TestFixture.Create<TransferSelectEvidenceNoteModel>();

            var request = TestFixture.Build<TransferEvidenceNoteRequest>()
                .With(r => r.EvidenceNoteIds, new List<Guid>())
                .With(r => r.DeselectedEvidenceNoteIds, new List<Guid>() { model.SelectedEvidenceNoteId })
                .Create();

            A.CallTo(() => sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(SessionKeyConstant.OutgoingTransferKey)).Returns(request);

            //act
            await outgoingTransferEvidenceController.SelectEvidenceNote(model);

            //assert
            request.DeselectedEvidenceNoteIds.Should().NotContain(model.SelectedEvidenceNoteId);
            request.EvidenceNoteIds.Should().Contain(model.SelectedEvidenceNoteId);
        }

        [Fact]
        public async Task SelectEvidenceNotePost_GivenSelectedEvidenceNoteAndModelIsNotValid_TransferRequestShouldNotBeUpdated()
        {
            //arrange
            var model = TestFixture.Create<TransferSelectEvidenceNoteModel>();
            var request = TestFixture.Build<TransferEvidenceNoteRequest>()
                .With(r => r.DeselectedEvidenceNoteIds, new List<Guid>() { model.SelectedEvidenceNoteId })
                .Create();

            A.CallTo(() => sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(SessionKeyConstant.OutgoingTransferKey)).Returns(request);

            AddModelError();

            //act
            await outgoingTransferEvidenceController.SelectEvidenceNote(model);

            //assert
            request.DeselectedEvidenceNoteIds.Should().Contain(model.SelectedEvidenceNoteId);
            request.EvidenceNoteIds.Should().NotContain(model.SelectedEvidenceNoteId);
        }

        [Fact]
        public async Task SelectEvidenceNotePost_GivenInvalidModel_ShouldRetrieveTransferNote()
        {
            //arrange
            var model = TestFixture.Create<TransferSelectEvidenceNoteModel>();

            A.CallTo(() => sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(A<string>._)).Returns(TestFixture.Create<TransferEvidenceNoteRequest>());

            AddModelError();

            //act
            await outgoingTransferEvidenceController.SelectEvidenceNote(model);

            //assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetTransferEvidenceNoteForSchemeRequest>.That.Matches(r => r.EvidenceNoteId == model.EditEvidenceNoteId))).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("search")]
        [InlineData("")]
        [InlineData(" ")]
        public async Task SelectEvidenceNotePost_GivenInvalidModel_AvailableTransferNotesShouldBeRetrieved(string search)
        {
            //arrange
            var model = TestFixture.Create<TransferSelectEvidenceNoteModel>();
            var evidenceNoteIds = TestFixture.CreateMany<Guid>().ToList();
            var categories = TestFixture.CreateMany<int>().ToList();

            var request = TestFixture.Build<TransferEvidenceNoteRequest>()
                .With(t => t.EvidenceNoteIds, evidenceNoteIds)
                .With(t => t.CategoryIds, categories)
                .Create();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetTransferEvidenceNoteForSchemeRequest>._))
                .Returns(transferEvidenceNoteData);

            A.CallTo(() => sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(A<string>._)).Returns(request);

            AddModelError();

            //act
            await outgoingTransferEvidenceController.SelectEvidenceNote(model, search);

            //assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetEvidenceNotesForTransferRequest>.That
                .Matches(g =>
                    g.PageSize == DefaultPageSize &&
                    g.PageNumber == model.Page &&
                    g.Categories.SequenceEqual(request.CategoryIds) &&
                    g.ComplianceYear == transferEvidenceNoteData.ComplianceYear &&
                    g.OrganisationId == model.PcsId &&
                    g.SearchReference == search &&
                    g.ExcludeEvidenceNotes.SequenceEqual(evidenceNoteIds.Union(transferEvidenceNoteData.CurrentEvidenceNoteIds))))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task SelectEvidenceNotePost_GivenInvalidModelAndTransferRequestWithSelectedNotes_SelectedTransferNotesShouldBeRetrieved()
        {
            //arrange
            var model = TestFixture.Create<TransferSelectEvidenceNoteModel>();
            var evidenceNoteIds = TestFixture.CreateMany<Guid>().ToList();
            var categories = TestFixture.CreateMany<int>().ToList();

            var request = TestFixture.Build<TransferEvidenceNoteRequest>()
                .With(t => t.EvidenceNoteIds, evidenceNoteIds)
                .With(t => t.CategoryIds, categories)
                .Create();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetTransferEvidenceNoteForSchemeRequest>._))
                .Returns(transferEvidenceNoteData);

            A.CallTo(() => sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(A<string>._)).Returns(request);

            AddModelError();

            //act
            await outgoingTransferEvidenceController.SelectEvidenceNote(model);

            //assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetEvidenceNotesSelectedForTransferRequest>.That.Matches(g =>
                g.Categories.SequenceEqual(request.CategoryIds) &&
                g.OrganisationId == model.PcsId &&
                g.EvidenceNotes.SequenceEqual(evidenceNoteIds.Union(transferEvidenceNoteData.CurrentEvidenceNoteIds))))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task SelectEvidenceNotePost_GivenInvalidModelAndTransferRequestWithNoSelectedNotes_SelectedTransferNotesShouldNotBeRetrieved()
        {
            //arrange
            var model = TestFixture.Create<TransferSelectEvidenceNoteModel>();
            var categories = TestFixture.CreateMany<int>().ToList();
            var request = TestFixture.Build<TransferEvidenceNoteRequest>()
                .With(t => t.EvidenceNoteIds, new List<Guid>())
                .With(t => t.CategoryIds, categories)
                .Create();

            var transferEvidenceNoteData = TestFixture.Build<TransferEvidenceNoteData>()
                .With(n => n.Status, NoteStatus.Submitted)
                .Without(t => t.TransferEvidenceNoteTonnageData)
                .Create();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetTransferEvidenceNoteForSchemeRequest>._))
                .Returns(transferEvidenceNoteData);

            A.CallTo(() => sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(A<string>._)).Returns(request);

            AddModelError();

            //act
            await outgoingTransferEvidenceController.SelectEvidenceNote(model);

            //assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetEvidenceNotesSelectedForTransferRequest>._)).MustNotHaveHappened();
        }

        [Fact]
        public async Task SelectEvidenceNotePost_GivenInvalidModelAndData_ModelMapperShouldBeCalled()
        {
            //arrange
            var model = TestFixture.Create<TransferSelectEvidenceNoteModel>();

            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetTransferEvidenceNoteForSchemeRequest>._)).Returns(transferEvidenceNoteData);

            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetEvidenceNotesForTransferRequest>._)).Returns(evidenceNoteData);

            var availableEvidenceNoteData =
                new EvidenceNoteSearchDataResult(TestFixture.CreateMany<EvidenceNoteData>(3).ToList(), 3);

            var selectedEvidenceNoteData =
                new EvidenceNoteSearchDataResult(TestFixture.CreateMany<EvidenceNoteData>(3).ToList(), 3);

            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetEvidenceNotesForTransferRequest>._)).Returns(availableEvidenceNoteData);

            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetEvidenceNotesSelectedForTransferRequest>._)).Returns(selectedEvidenceNoteData);

            var request = TestFixture.Create<TransferEvidenceNoteRequest>();
            A.CallTo(() => sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(A<string>._)).Returns(request);

            AddModelError();

            //act
            await outgoingTransferEvidenceController.SelectEvidenceNote(model);

            //assert
            A.CallTo(() => mapper.Map<TransferEvidenceNotesViewModelMapTransfer, TransferEvidenceNotesViewModel>(
                    A<TransferEvidenceNotesViewModelMapTransfer>.That.Matches(t => t.TransferAllTonnage == false &&
                        t.TransferEvidenceNoteData == transferEvidenceNoteData &&
                        t.Request == request &&
                        t.SelectedNotes.Equals(selectedEvidenceNoteData) &&
                        t.AvailableNotes.Equals(availableEvidenceNoteData) &&
                        t.OrganisationId == model.PcsId &&
                        t.PageSize == DefaultPageSize &&
                        t.PageNumber == model.Page)))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task SelectEvidenceNotePost_GivenInvalidModelAndMappedModel_ModelShouldBeReturned()
        {
            //arrange
            var existingModel = TestFixture.Create<TransferSelectEvidenceNoteModel>();
            var model = TestFixture.Create<TransferEvidenceNotesViewModel>();
            A.CallTo(() => mapper.Map<TransferEvidenceNotesViewModelMapTransfer, TransferEvidenceNotesViewModel>(
                A<TransferEvidenceNotesViewModelMapTransfer>._)).Returns(model);

            A.CallTo(() => sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(A<string>._)).Returns(TestFixture.Create<TransferEvidenceNoteRequest>());

            AddModelError();

            //act
            var result = await outgoingTransferEvidenceController.SelectEvidenceNote(existingModel) as ViewResult;

            //assert
            result.Model.Should().Be(model);
        }

        [Fact]
        public async Task SelectEvidenceNotePost_GivenInvalidModel_ViewShouldBeReturned()
        {
            //arrange
            var model = TestFixture.Create<TransferSelectEvidenceNoteModel>();

            A.CallTo(() => sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(A<string>._)).Returns(TestFixture.Create<TransferEvidenceNoteRequest>());

            AddModelError();

            //act
            var result = await outgoingTransferEvidenceController.SelectEvidenceNote(model) as ViewResult;

            //assert
            result.ViewName.Should().Be("EditTransferFrom");
        }

        [Fact]
        public async Task SelectEvidenceNotePost_SchemeShouldBeRetrievedFromCache()
        {
            // arrange
            var model = TestFixture.Create<TransferSelectEvidenceNoteModel>();

            A.CallTo(() =>
                    sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(A<string>._)).Returns(TestFixture.Create<TransferEvidenceNoteRequest>());

            AddModelError();

            //act
            await outgoingTransferEvidenceController.SelectEvidenceNote(model);

            //asset
            A.CallTo(() => cache.FetchSchemePublicInfo(model.PcsId)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task SelectEvidenceNotePost_GivenSchemeIsNotBalancingScheme_BreadcrumbShouldBeSet()
        {
            //arrange
            var model = TestFixture.Create<TransferSelectEvidenceNoteModel>();

            A.CallTo(() =>
                    sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(A<string>._))
                .Returns(TestFixture.Create<TransferEvidenceNoteRequest>());

            var schemeInfo = TestFixture.Build<SchemePublicInfo>().With(s => s.IsBalancingScheme, false).Create();

            AddModelError();

            var organisationName = "OrganisationName";

            A.CallTo(() => cache.FetchOrganisationName(model.PcsId)).Returns(organisationName);
            A.CallTo(() => cache.FetchSchemePublicInfo(model.PcsId)).Returns(schemeInfo);

            //act
            await outgoingTransferEvidenceController.SelectEvidenceNote(model);

            // assert
            breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.SchemeManageEvidence);
            breadcrumb.OrganisationId.Should().Be(model.PcsId);
        }

        [Fact]
        public async Task SelectEvidenceNotePost_GivenSchemeIsBalancingScheme_BreadcrumbShouldBeSet()
        {
            //arrange
            var model = TestFixture.Create<TransferSelectEvidenceNoteModel>();

            A.CallTo(() =>
                    sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(A<string>._))
                .Returns(TestFixture.Create<TransferEvidenceNoteRequest>());

            var schemeInfo = TestFixture.Build<SchemePublicInfo>().With(s => s.IsBalancingScheme, true).Create();

            AddModelError();

            var organisationName = "OrganisationName";

            A.CallTo(() => cache.FetchOrganisationName(model.PcsId)).Returns(organisationName);
            A.CallTo(() => cache.FetchSchemePublicInfo(model.PcsId)).Returns(schemeInfo);

            //act
            await outgoingTransferEvidenceController.SelectEvidenceNote(model);

            // assert
            breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.PbsManageEvidence);
            breadcrumb.OrganisationId.Should().Be(model.PcsId);
        }
    }
}
