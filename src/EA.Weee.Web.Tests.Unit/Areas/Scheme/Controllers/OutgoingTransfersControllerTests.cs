﻿namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.Controllers
{
    using Api.Client;
    using AutoFixture;
    using Constant;
    using Core.AatfEvidence;
    using Core.Helpers;
    using Core.Scheme;
    using EA.Prsd.Core.Mapper;
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
    using System.Web;
    using System.Web.Mvc;
    using Web.Areas.Scheme.Attributes;
    using Web.Areas.Scheme.Mappings.ToViewModels;
    using Web.Areas.Scheme.Requests;
    using Web.Areas.Scheme.ViewModels;
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
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private readonly Guid organisationId;
        private readonly TransferEvidenceNoteData transferEvidenceNoteData;
        private readonly List<EvidenceNoteData> evidenceNoteData;
        private readonly TransferEvidenceTonnageViewModel transferEvidenceTonnageViewModel;

        public OutgoingTransfersControllerTests()
        {
            weeeClient = A.Fake<IWeeeClient>();
            breadcrumb = A.Fake<BreadcrumbService>();
            cache = A.Fake<IWeeeCache>();
            mapper = A.Fake<IMapper>();
            sessionService = A.Fake<ISessionService>();
            transferEvidenceRequestCreator = A.Fake<ITransferEvidenceRequestCreator>();

            organisationId = Guid.NewGuid();

            outgoingTransferEvidenceController =
                new OutgoingTransfersController(mapper, breadcrumb, cache, () => weeeClient, sessionService,
                    transferEvidenceRequestCreator);

            transferEvidenceNoteData = TestFixture.Build<TransferEvidenceNoteData>()
                .With(n => n.Status, NoteStatus.Submitted).Create();

            evidenceNoteData = TestFixture.CreateMany<EvidenceNoteData>().ToList();
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

            A.CallTo(() => sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(
                A<HttpSessionStateBase>._, A<string>._)).Returns(null);

            A.CallTo(() =>
                    transferEvidenceRequestCreator.EditSelectTonnageToRequest(A<EditTransferEvidenceNoteRequest>._,
                        A<TransferEvidenceTonnageViewModel>._))
                .Returns(TestFixture.Create<EditTransferEvidenceNoteRequest>());
        }

        [Fact]
        public void OutgoingTransfersControllerInheritsCheckSchemeEvidenceBaseController()
        {
            typeof(OutgoingTransfersController).BaseType.Name.Should().Be(nameof(SchemeEvidenceBaseController));
        }

        [Fact]
        public void EditTransferFromGet_ShouldHaveHttpGetAttribute()
        {
            typeof(OutgoingTransfersController).GetMethod("EditTransferFrom", new[] { typeof(Guid), typeof(Guid) })
                .Should()
                .BeDecoratedWith<HttpGetAttribute>();
        }

        [Fact]
        public void EditTransferFromGet_ShouldHaveCheckCanEditTransferNoteAttribute()
        {
            typeof(OutgoingTransfersController).GetMethod("EditTransferFrom", new[] { typeof(Guid), typeof(Guid) })
                .Should()
                .BeDecoratedWith<CheckCanEditTransferNoteAttribute>();
        }

        [Fact]
        public void EditTonnagesGet_ShouldHaveHttpGetAttribute()
        {
            typeof(OutgoingTransfersController).GetMethod("EditTonnages", new[] { typeof(Guid), typeof(Guid), typeof(bool?) }).Should()
                .BeDecoratedWith<HttpGetAttribute>();
        }

        [Fact]
        public void EditTonnagesGet_ShouldHaveCheckCanEditTransferNoteAttribute()
        {
            typeof(OutgoingTransfersController).GetMethod("EditTonnages", new[] { typeof(Guid), typeof(Guid), typeof(bool?) }).Should()
                .BeDecoratedWith<CheckCanEditTransferNoteAttribute>();
        }

        [Fact]
        public void EditCategoriesGet_ShouldHaveHttpGetAttribute()
        {
            typeof(OutgoingTransfersController).GetMethod("EditCategories", new[] { typeof(Guid), typeof(Guid) })
                .Should().BeDecoratedWith<HttpGetAttribute>();
        }

        [Fact]
        public void EditCategoriesGet_ShouldHaveCheckCanEditTransferNoteAttribute()
        {
            typeof(OutgoingTransfersController).GetMethod("EditCategories", new[] { typeof(Guid), typeof(Guid) })
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
                    new[] { typeof(Guid), typeof(Guid), typeof(bool?), typeof(string) }).Should()
                .BeDecoratedWith<HttpGetAttribute>();
        }

        [Fact]
        public void EditDraftTransfer_ShouldHaveCheckCanEditTransferNoteAttribute()
        {
            typeof(OutgoingTransfersController).GetMethod("EditDraftTransfer",
                    new[] { typeof(Guid), typeof(Guid), typeof(bool?), typeof(string) }).Should()
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
        public async Task EditTonnagesGet_GivenOrganisationId_SchemeShouldBeRetrievedFromCache()
        {
            //act
            await outgoingTransferEvidenceController.EditTonnages(organisationId, TestFixture.Create<Guid>(), TestFixture.Create<bool?>());

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
            await outgoingTransferEvidenceController.EditTonnages(organisationId, TestFixture.Create<Guid>(), TestFixture.Create<bool?>());

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
            await outgoingTransferEvidenceController.EditTonnages(organisationId, TestFixture.Create<Guid>(), TestFixture.Create<bool?>());

            // assert
            breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.PbsManageEvidence);
            breadcrumb.OrganisationId.Should().Be(organisationId);
        }

        [Fact]
        public async Task EditDraftTransfer_GivenOrganisationId_SchemeShouldBeRetrievedFromCache()
        {
            //act
            await outgoingTransferEvidenceController.EditDraftTransfer(organisationId, TestFixture.Create<Guid>(), null, null);

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
            await outgoingTransferEvidenceController.EditTonnages(organisationId, evidenceNoteId, TestFixture.Create<bool?>());

            //assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                    A<GetTransferEvidenceNoteForSchemeRequest>.That.Matches(r => r.EvidenceNoteId == evidenceNoteId)))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditTonnagesGet_ExistingSelectedEvidenceNotes_ShouldBeRetrievedFromSession()
        {
            //act
            await outgoingTransferEvidenceController.EditTonnages(organisationId, TestFixture.Create<Guid>(), TestFixture.Create<bool?>());

            //assert
            A.CallTo(() => sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(
                    outgoingTransferEvidenceController.Session, SessionKeyConstant.OutgoingTransferKey))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditTonnagesGet_WithExistingEditTransferEvidenceTonnageViewModel_ShouldBeRetrievedFromSession()
        {
            //act
            await outgoingTransferEvidenceController.EditTonnages(organisationId, TestFixture.Create<Guid>(), TestFixture.Create<bool?>());

            //assert
            A.CallTo(() => sessionService.GetTransferSessionObject<TransferEvidenceTonnageViewModel>(
                    outgoingTransferEvidenceController.Session, SessionKeyConstant.EditTransferEvidenceTonnageViewModel))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditTonnagesGet_WithExistingEditTransferEvidenceTonnageViewModel_ShouldClearTransferSessionObject()
        {
            //act
            await outgoingTransferEvidenceController.EditTonnages(organisationId, TestFixture.Create<Guid>(), TestFixture.Create<bool?>());

            //assert
            A.CallTo(() => sessionService.ClearTransferSessionObject(outgoingTransferEvidenceController.Session, SessionKeyConstant.EditTransferEvidenceTonnageViewModel))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task
            EditTonnagesGet_GivenExistingSelectedEvidenceNotesAlongWithTransferNoteData_TransferNotesShouldBeRetrieved()
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
                A<HttpSessionStateBase>._, A<string>._)).Returns(request);

            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetTransferEvidenceNoteForSchemeRequest>._)).Returns(transferEvidence);

            //act
            await outgoingTransferEvidenceController.EditTonnages(organisationId, TestFixture.Create<Guid>(), TestFixture.Create<bool?>());

            //assert
            var expectedEvidenceNoteIds = transferEvidence.TransferEvidenceNoteTonnageData
                .Select(t => t.OriginalNoteId)
                .ToList().Union(sessionEvidenceNotes)
                .Distinct().ToList();

            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                    A<GetEvidenceNotesForTransferRequest>.That.Matches(r =>
                        r.Categories.SequenceEqual(sessionCategories) &&
                        r.OrganisationId == organisationId &&
                        r.EvidenceNotes.SequenceEqual(expectedEvidenceNoteIds))))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task
            EditTonnagesGet_GivenExistingSelectedEvidenceNotesThatHaveBeenDeselectedAndSessionTransferRequest_TransferNotesShouldBeRetrieved()
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
                A<HttpSessionStateBase>._, A<string>._)).Returns(request);

            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetTransferEvidenceNoteForSchemeRequest>._)).Returns(transferEvidence);

            //act
            await outgoingTransferEvidenceController.EditTonnages(organisationId, TestFixture.Create<Guid>(), TestFixture.Create<bool?>());

            //assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                    A<GetEvidenceNotesForTransferRequest>.That.Matches(r =>
                        r.Categories.SequenceEqual(sessionCategories) &&
                        r.OrganisationId == organisationId &&
                        !r.EvidenceNotes.Contains(removedEvidenceNoteId) &&
                        r.EvidenceNotes.Contains(newlySelectedEvidenceNoteId) &&
                        r.EvidenceNotes.Count() == 1)))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task
            EditTonnagesGet_GivenExistingSelectedEvidenceNotesThatHaveBeenDeselectedAndNullSessionTransferRequest_TransferNotesShouldBeRetrieved()
        {
            //arrange
            var removedEvidenceNoteId = TestFixture.Create<Guid>();
            var newlySelectedEvidenceNoteId = TestFixture.Create<Guid>();

            var transferEvidence = TestFixture.Build<TransferEvidenceNoteData>()
                .With(t => t.TransferEvidenceNoteTonnageData, new List<TransferEvidenceNoteTonnageData>()
                {
                    TestFixture.Build<TransferEvidenceNoteTonnageData>()
                        .With(e => e.OriginalNoteId, removedEvidenceNoteId).Create()
                }).Create();

            A.CallTo(() => sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(
                A<HttpSessionStateBase>._, A<string>._)).Returns(null);

            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetTransferEvidenceNoteForSchemeRequest>._)).Returns(transferEvidence);

            //act
            await outgoingTransferEvidenceController.EditTonnages(organisationId, TestFixture.Create<Guid>(), TestFixture.Create<bool?>());

            //assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                    A<GetEvidenceNotesForTransferRequest>.That.Matches(r =>
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
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetTransferEvidenceNoteForSchemeRequest>._)).Returns(transferEvidenceNoteData);

            var categories = transferEvidenceNoteData.CategoryIds;
            var existingNotes = transferEvidenceNoteData.TransferEvidenceNoteTonnageData.Select(t => t.OriginalNoteId)
                .ToList();

            //act
            await outgoingTransferEvidenceController.EditTonnages(organisationId, TestFixture.Create<Guid>(), TestFixture.Create<bool?>());

            //assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                    A<GetEvidenceNotesForTransferRequest>.That.Matches(r =>
                        r.OrganisationId == organisationId
                        && r.Categories.SequenceEqual(categories) &&
                        r.EvidenceNotes.SequenceEqual(existingNotes))))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditTonnagesGet_GivenTransferNoteDataAndNullSessionData_TransferNotesShouldBeRetrieved()
        {
            //arrange
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetTransferEvidenceNoteForSchemeRequest>._)).Returns(transferEvidenceNoteData);

            A.CallTo(() => sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(
                A<HttpSessionStateBase>._, A<string>._)).Returns(null);

            var categories = transferEvidenceNoteData.CategoryIds;
            var existingNotes = transferEvidenceNoteData.TransferEvidenceNoteTonnageData.Select(t => t.OriginalNoteId)
                .ToList();

            //act
            await outgoingTransferEvidenceController.EditTonnages(organisationId, TestFixture.Create<Guid>(), TestFixture.Create<bool?>());

            //assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                    A<GetEvidenceNotesForTransferRequest>.That.Matches(r =>
                        r.OrganisationId == organisationId &&
                        r.Categories.SequenceEqual(categories) &&
                        r.EvidenceNotes.SequenceEqual(existingNotes))))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditTonnagesGet_GivenTransferNoteAndTransferNotes_ModelMapperShouldBeCalled()
        {
            //arrange
            var returnToEditDraftTransfer = TestFixture.Create<bool?>();

            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetTransferEvidenceNoteForSchemeRequest>._)).Returns(transferEvidenceNoteData);

            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetEvidenceNotesForTransferRequest>._)).Returns(evidenceNoteData);

            var request = TestFixture.Create<TransferEvidenceNoteRequest>();

            A.CallTo(() => sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(
                A<HttpSessionStateBase>._, A<string>._)).Returns(request);

            //act
            await outgoingTransferEvidenceController.EditTonnages(organisationId, TestFixture.Create<Guid>(), returnToEditDraftTransfer);

            //assert
            A.CallTo(() => mapper.Map<TransferEvidenceNotesViewModelMapTransfer, TransferEvidenceTonnageViewModel>(
                    A<TransferEvidenceNotesViewModelMapTransfer>.That.Matches(t => t.TransferAllTonnage == false &&
                        t.TransferEvidenceNoteData == transferEvidenceNoteData &&
                        t.Notes.SequenceEqual(evidenceNoteData) &&
                        t.OrganisationId == organisationId &&
                        t.Request == request &&
                        t.ReturnToEditDraftTransfer == returnToEditDraftTransfer)))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditTonnagesGet_GivenMappedModel_ModelShouldBeReturned()
        {
            //arrange
            A.CallTo(() => mapper.Map<TransferEvidenceNotesViewModelMapTransfer, TransferEvidenceTonnageViewModel>(
                A<TransferEvidenceNotesViewModelMapTransfer>._)).Returns(transferEvidenceTonnageViewModel);

            //act
            var result =
                await outgoingTransferEvidenceController.EditTonnages(organisationId, TestFixture.Create<Guid>(), TestFixture.Create<bool?>()) as
                    ViewResult;

            //assert
            result.Model.Should().Be(transferEvidenceTonnageViewModel);
        }

        [Fact]
        public async Task EditTonnagesGet_ShouldReturnView()
        {
            //act
            var result =
                await outgoingTransferEvidenceController.EditTonnages(organisationId, TestFixture.Create<Guid>(), TestFixture.Create<bool?>()) as
                    ViewResult;

            //assert
            result.ViewName.Should().Be("EditTonnages");
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
            await outgoingTransferEvidenceController.EditDraftTransfer(organisationId, TestFixture.Create<Guid>(), null);

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
            await outgoingTransferEvidenceController.EditDraftTransfer(organisationId, TestFixture.Create<Guid>(), null, tab);

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
            await outgoingTransferEvidenceController.EditDraftTransfer(organisationId, TestFixture.Create<Guid>(), returnToView);

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
                await outgoingTransferEvidenceController.EditDraftTransfer(organisationId, TestFixture.Create<Guid>(), null) as ViewResult;

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
            var result = await outgoingTransferEvidenceController.EditDraftTransfer(organisationId, TestFixture.Create<Guid>(), null) as ViewResult;

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
        public async Task SubmittedTransferGet_GivenOrganisationId_SchemeShouldBeRetrievedFromCache()
        {
            //act
            await outgoingTransferEvidenceController.SubmittedTransfer(organisationId, TestFixture.Create<Guid>(), TestFixture.Create<bool>(), null);

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
                    DisplayExtensions.ToDisplayString(ManageEvidenceNotesDisplayOptions.ReviewSubmittedEvidence)) as ViewResult;

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
            outgoingTransferEvidenceController.ModelState.AddModelError("error", "error");

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
            outgoingTransferEvidenceController.ModelState.AddModelError("error", "error");

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
            outgoingTransferEvidenceController.ModelState.AddModelError("error", "error");

            //act
            var result = await outgoingTransferEvidenceController.SubmittedTransfer(model) as ViewResult;

            //assert
            result.Model.Should().Be(model);
        }

        [Theory]
        [InlineData("Approve evidence note transfer", NoteUpdatedStatusEnum.Approved)]
        [InlineData("Reject evidence note transfer", NoteUpdatedStatusEnum.Rejected)]
        [InlineData("Return evidence note transfer", NoteUpdatedStatusEnum.Returned)]
        public async Task SubmittedTransferPost_GivenValidModelMappedModel_TempDataShouldBeSet(string selectedValue, NoteUpdatedStatusEnum expectedStatus)
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
            outgoingTransferEvidenceController.ModelState.AddModelError("error", "error");

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
                A<HttpSessionStateBase>._, A<string>._)).Returns(TestFixture.Create<TransferEvidenceNoteRequest>());

            //act
            await outgoingTransferEvidenceController.EditTransferFrom(organisationId, evidenceNoteId);

            //assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                    A<GetTransferEvidenceNoteForSchemeRequest>.That.Matches(r => r.EvidenceNoteId == evidenceNoteId)))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditTransferFromGet_GivenTransferSessionTransferRequestIsNull_ShouldRedirectToManageEvidenceNotes()
        {
            //arrange
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetTransferEvidenceNoteForSchemeRequest>._)).Returns(transferEvidenceNoteData);

            A.CallTo(() =>
                sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(A<HttpSessionStateBase>._,
                    A<string>._)).Returns(null);

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

        [Fact]
        public async Task EditTransferFromGet_GivenTransferNoteAndCategories_ModelMapperShouldBeCalled()
        {
            //arrange
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetTransferEvidenceNoteForSchemeRequest>._)).Returns(transferEvidenceNoteData);

            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetEvidenceNotesForTransferRequest>._)).Returns(evidenceNoteData);

            var request = TestFixture.Create<TransferEvidenceNoteRequest>();
            A.CallTo(() => sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(
                A<HttpSessionStateBase>._, A<string>._)).Returns(request);

            //act
            await outgoingTransferEvidenceController.EditTransferFrom(organisationId, TestFixture.Create<Guid>());

            //assert
            A.CallTo(() => mapper.Map<TransferEvidenceNotesViewModelMapTransfer, TransferEvidenceNotesViewModel>(
                    A<TransferEvidenceNotesViewModelMapTransfer>.That.Matches(t => t.TransferAllTonnage == false &&
                        t.TransferEvidenceNoteData == transferEvidenceNoteData &&
                        t.Request == request &&
                        t.Notes.SequenceEqual(evidenceNoteData) &&
                        t.OrganisationId == organisationId)))
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
                A<HttpSessionStateBase>._, A<string>._)).Returns(TestFixture.Create<TransferEvidenceNoteRequest>());

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
                A<HttpSessionStateBase>._, A<string>._)).Returns(TestFixture.Create<TransferEvidenceNoteRequest>());

            //act
            var result =
                await outgoingTransferEvidenceController.EditTransferFrom(organisationId, TestFixture.Create<Guid>()) as
                    ViewResult;

            //assert
            result.ViewName.Should().Be("EditTransferFrom");
        }

        [Fact]
        public async Task EditTransferFromPost_GivenInvalidModel_ViewShouldBeReturned()
        {
            //arrange
            var model = TestFixture.Create<TransferEvidenceNotesViewModel>();
            outgoingTransferEvidenceController.ModelState.AddModelError("error", "error");

            //act
            var result = await outgoingTransferEvidenceController.EditTransferFrom(model) as ViewResult;

            //assert
            result.ViewName.Should().Be("EditTransferFrom");
        }

        [Fact]
        public async Task EditTransferFromPost_GivenInvalidModel_SchemeShouldBeRetrievedFromCache()
        {
            var model = TestFixture.Create<TransferEvidenceNotesViewModel>();
            outgoingTransferEvidenceController.ModelState.AddModelError("error", "error");

            //act
            await outgoingTransferEvidenceController.EditTransferFrom(model);

            //asset
            A.CallTo(() => cache.FetchSchemePublicInfo(model.PcsId)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditTransferFromPost_GivenInvalidModelAndSchemeIsNotBalancingScheme_BreadcrumbShouldBeSet()
        {
            //arrange
            var model = TestFixture.Create<TransferEvidenceNotesViewModel>();
            var schemeInfo = TestFixture.Build<SchemePublicInfo>().With(s => s.IsBalancingScheme, false).Create();
            outgoingTransferEvidenceController.ModelState.AddModelError("error", "error");
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
            var model = TestFixture.Create<TransferEvidenceNotesViewModel>();
            var schemeInfo = TestFixture.Build<SchemePublicInfo>().With(s => s.IsBalancingScheme, true).Create();
            outgoingTransferEvidenceController.ModelState.AddModelError("error", "error");
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
        public async Task EditTransferFromPost_GivenValidViewModel_SessionRequestShouldBeUpdated()
        {
            //arrange
            var selectedId = TestFixture.Create<Guid>();
            var notSelectedId = TestFixture.Create<Guid>();
            var selectedValues = new List<GenericControlPair<Guid, bool>>()
            {
                new GenericControlPair<Guid, bool>(selectedId, true),
                new GenericControlPair<Guid, bool>(notSelectedId, false)
            };

            var model = TestFixture.Build<TransferEvidenceNotesViewModel>()
                .With(m => m.SelectedEvidenceNotePairs, selectedValues).Create();

            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetTransferEvidenceNoteForSchemeRequest>._)).Returns(transferEvidenceNoteData);

            var request = TestFixture.Create<TransferEvidenceNoteRequest>();
            A.CallTo(() => sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(
                A<HttpSessionStateBase>._, A<string>._)).Returns(request);

            //act
            await outgoingTransferEvidenceController.EditTransferFrom(model);

            //assert
            A.CallTo(() => sessionService.SetTransferSessionObject(outgoingTransferEvidenceController.Session,
                A<object>.That.Matches(o => ((TransferEvidenceNoteRequest)o).OrganisationId == model.PcsId &&
                                            ((TransferEvidenceNoteRequest)o).RecipientId == model.RecipientId &&
                                            ((TransferEvidenceNoteRequest)o).CategoryIds.SequenceEqual(request.CategoryIds) &&
                                            ((TransferEvidenceNoteRequest)o).EvidenceNoteIds.Contains(selectedId) &&
                                            !((TransferEvidenceNoteRequest)o).EvidenceNoteIds.Contains(notSelectedId) &&
                                            ((TransferEvidenceNoteRequest)o).EvidenceNoteIds.Count() == 1),
                SessionKeyConstant.OutgoingTransferKey)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditTransferFromPost_GivenValidViewModelAndNullSessionTransferRequest_ShouldRedirectToManageEvidenceNotes()
        {
            //arrange
            var model = TestFixture.Build<TransferEvidenceNotesViewModel>().Create();

            A.CallTo(() => sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(
                A<HttpSessionStateBase>._, A<string>._)).Returns(null);

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
            A.CallTo(() => sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(
                A<HttpSessionStateBase>._, A<string>._)).Returns(TestFixture.Create<TransferEvidenceNoteRequest>());

            var model = TestFixture.Create<TransferEvidenceNotesViewModel>();

            //act
            var result = await outgoingTransferEvidenceController.EditTransferFrom(model) as RedirectToRouteResult;

            //assert
            result.RouteName.Should().Be("Scheme_edit_transfer_tonnages");
            result.RouteValues["pcsId"].Should().Be(model.PcsId);
            result.RouteValues["evidenceNoteId"].Should().Be(model.ViewTransferNoteViewModel.EvidenceNoteId);
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

            var recipients = TestFixture.CreateMany<OrganisationSchemeData>().ToList();
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
            outgoingTransferEvidenceController.ModelState.AddModelError("error", "error");
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
            outgoingTransferEvidenceController.ModelState.AddModelError("error", "error");
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
            A.CallTo(() => sessionService.SetTransferSessionObject(outgoingTransferEvidenceController.Session, request,
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
            outgoingTransferEvidenceController.ModelState.AddModelError("error", "error");

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
            outgoingTransferEvidenceController.ModelState.AddModelError("error", "error");

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
            outgoingTransferEvidenceController.ModelState.AddModelError("error", "error");

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
            outgoingTransferEvidenceController.ModelState.AddModelError("error", "error");

            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetTransferEvidenceNoteForSchemeRequest>._)).Returns(transferEvidenceNoteData);

            var recipientData = TestFixture.CreateMany<OrganisationSchemeData>().ToList();
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
            outgoingTransferEvidenceController.ModelState.AddModelError("error", "error");

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

            outgoingTransferEvidenceController.ModelState.AddModelError("error", "error");

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

            outgoingTransferEvidenceController.ModelState.AddModelError("error", "error");

            //act
            await outgoingTransferEvidenceController.EditTonnages(model);

            //assert
            A.CallTo(() => sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(
                    outgoingTransferEvidenceController.Session, SessionKeyConstant.OutgoingTransferKey))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditTonnagesPost_GivenGivenInvalidModelAndExistingSelectedEvidenceNotesAlongWithTransferNoteData_TransferNotesShouldBeRetrieved()
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
                A<HttpSessionStateBase>._, A<string>._)).Returns(request);

            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetTransferEvidenceNoteForSchemeRequest>._)).Returns(transferEvidence);

            var model = TestFixture.Build<TransferEvidenceTonnageViewModel>()
                .With(m => m.PcsId, organisationId)
                .Create();

            outgoingTransferEvidenceController.ModelState.AddModelError("error", "error");

            //act
            await outgoingTransferEvidenceController.EditTonnages(model);

            //assert
            var expectedEvidenceNoteIds = transferEvidence.TransferEvidenceNoteTonnageData
                .Select(t => t.OriginalNoteId)
                .ToList().Union(sessionEvidenceNotes)
                .Distinct().ToList();

            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                    A<GetEvidenceNotesForTransferRequest>.That.Matches(r =>
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

            A.CallTo(() => sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(
                A<HttpSessionStateBase>._, A<string>._)).Returns(request);

            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetTransferEvidenceNoteForSchemeRequest>._)).Returns(transferEvidence);

            var model = TestFixture.Build<TransferEvidenceTonnageViewModel>()
                .With(m => m.PcsId, organisationId)
                .Create();

            outgoingTransferEvidenceController.ModelState.AddModelError("error", "error");

            //act
            await outgoingTransferEvidenceController.EditTonnages(model);

            //assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                    A<GetEvidenceNotesForTransferRequest>.That.Matches(r =>
                        r.Categories.SequenceEqual(sessionCategories) &&
                        r.OrganisationId == organisationId &&
                        !r.EvidenceNotes.Contains(removedEvidenceNoteId) &&
                        r.EvidenceNotes.Contains(newlySelectedEvidenceNoteId) &&
                        r.EvidenceNotes.Count() == 1)))
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

            A.CallTo(() => sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(
                A<HttpSessionStateBase>._, A<string>._)).Returns(null);

            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetTransferEvidenceNoteForSchemeRequest>._)).Returns(transferEvidence);

            var model = TestFixture.Build<TransferEvidenceTonnageViewModel>()
                .With(m => m.PcsId, organisationId)
                .Create();

            outgoingTransferEvidenceController.ModelState.AddModelError("error", "error");

            //act
            await outgoingTransferEvidenceController.EditTonnages(model);

            //assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                    A<GetEvidenceNotesForTransferRequest>.That.Matches(r =>
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

            outgoingTransferEvidenceController.ModelState.AddModelError("error", "error");

            //act
            await outgoingTransferEvidenceController.EditTonnages(model);

            //assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                    A<GetEvidenceNotesForTransferRequest>.That.Matches(r =>
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

            A.CallTo(() => sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(
                A<HttpSessionStateBase>._, A<string>._)).Returns(null);

            var categories = transferEvidenceNoteData.CategoryIds;
            var existingNotes = transferEvidenceNoteData.TransferEvidenceNoteTonnageData.Select(t => t.OriginalNoteId)
                .ToList();

            var model = TestFixture.Build<TransferEvidenceTonnageViewModel>()
                .With(m => m.PcsId, organisationId)
                .Create();

            outgoingTransferEvidenceController.ModelState.AddModelError("error", "error");

            //act
            await outgoingTransferEvidenceController.EditTonnages(model);

            //assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                    A<GetEvidenceNotesForTransferRequest>.That.Matches(r =>
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
                A<GetEvidenceNotesForTransferRequest>._)).Returns(evidenceNoteData);

            var request = TestFixture.Create<TransferEvidenceNoteRequest>();

            A.CallTo(() => sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(
                A<HttpSessionStateBase>._, A<string>._)).Returns(request);

            var model = TestFixture.Build<TransferEvidenceTonnageViewModel>()
                .With(m => m.PcsId, organisationId)
                .Create();

            outgoingTransferEvidenceController.ModelState.AddModelError("error", "error");

            //act
            await outgoingTransferEvidenceController.EditTonnages(model);

            //assert
            A.CallTo(() => mapper.Map<TransferEvidenceNotesViewModelMapTransfer, TransferEvidenceTonnageViewModel>(
                    A<TransferEvidenceNotesViewModelMapTransfer>.That.Matches(t => t.TransferAllTonnage == false &&
                        t.TransferEvidenceNoteData == transferEvidenceNoteData &&
                        t.Notes.SequenceEqual(evidenceNoteData) &&
                        t.OrganisationId == organisationId &&
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

            outgoingTransferEvidenceController.ModelState.AddModelError("error", "error");

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

            outgoingTransferEvidenceController.ModelState.AddModelError("error", "error");

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
                    sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(
                        outgoingTransferEvidenceController.Session, SessionKeyConstant.OutgoingTransferKey))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditTonnagesPost_GivenValidModelAndNullExistingRequest_RequestShouldBeCreated()
        {
            //arrange
            A.CallTo(() =>
                sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(
                    outgoingTransferEvidenceController.Session, SessionKeyConstant.TransferNoteKey)).Returns(null);

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
                sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(
                    outgoingTransferEvidenceController.Session, SessionKeyConstant.OutgoingTransferKey)).Returns(request);

            A.CallTo(() => transferEvidenceRequestCreator.EditSelectTonnageToRequest(A<TransferEvidenceNoteRequest>._,
                A<TransferEvidenceTonnageViewModel>._)).Returns(TestFixture.Create<EditTransferEvidenceNoteRequest>());

            //act
            await outgoingTransferEvidenceController.EditTonnages(transferEvidenceTonnageViewModel);

            //assert
            A.CallTo(() =>
                    transferEvidenceRequestCreator.EditSelectTonnageToRequest(request, transferEvidenceTonnageViewModel))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditTonnagesPost_GivenValid_ApiShouldBeCalled()
        {
            //arrange
            var request = TestFixture.Create<EditTransferEvidenceNoteRequest>();

            A.CallTo(() => transferEvidenceRequestCreator.EditSelectTonnageToRequest(A<TransferEvidenceNoteRequest>._, A<TransferEvidenceTonnageViewModel>._)).Returns(request);

            //act
            await outgoingTransferEvidenceController.EditTonnages(transferEvidenceTonnageViewModel);

            //assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._, request)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditTonnagesPost_GivenValidModelAndActionIsSaveAndNoteIsReturned_TempDataNotificationShouldBeSetToReturnedSaved()
        {
            //arrange
            transferEvidenceTonnageViewModel.ViewTransferNoteViewModel.Status = NoteStatus.Returned;
            transferEvidenceTonnageViewModel.Action = ActionEnum.Save;

            //act
            await outgoingTransferEvidenceController.EditTonnages(transferEvidenceTonnageViewModel);

            //assert
            outgoingTransferEvidenceController.TempData[ViewDataConstant.TransferEvidenceNoteDisplayNotification].Should().Be(NoteUpdatedStatusEnum.ReturnedSaved);
        }

        [Theory]
        [InlineData(NoteStatus.Draft)]
        [InlineData(NoteStatus.Submitted)]
        public async Task EditTonnagesPost_GivenValidModelAndActionIsSubmitAndNoteIsReturned_TempDataNotificationShouldBeSetToRequestAction(NoteStatus status)
        {
            //arrange
            var request = TestFixture.Build<EditTransferEvidenceNoteRequest>().With(r => r.Status, status).Create();

            A.CallTo(() => transferEvidenceRequestCreator.EditSelectTonnageToRequest(A<EditTransferEvidenceNoteRequest>._,
                        A<TransferEvidenceTonnageViewModel>._)).Returns(request);

            transferEvidenceTonnageViewModel.ViewTransferNoteViewModel.Status = NoteStatus.Returned;
            transferEvidenceTonnageViewModel.Action = ActionEnum.Submit;

            //act
            await outgoingTransferEvidenceController.EditTonnages(transferEvidenceTonnageViewModel);

            //assert
            outgoingTransferEvidenceController.TempData[ViewDataConstant.TransferEvidenceNoteDisplayNotification].Should().Be((NoteUpdatedStatusEnum)status);
        }

        [Theory]
        [ClassData(typeof(NoteStatusCoreData))]
        public async Task EditTonnagesPost_GivenValidModelAndActionIsSaveAndNoteIsNotReturned_TempDataNotificationShouldBeSetToRequestAction(NoteStatus status)
        {
            if (status == NoteStatus.Returned)
            {
                return;
            }

            //arrange
            var request = TestFixture.Build<EditTransferEvidenceNoteRequest>().With(r => r.Status, status).Create();

            A.CallTo(() => transferEvidenceRequestCreator.EditSelectTonnageToRequest(A<EditTransferEvidenceNoteRequest>._,
                A<TransferEvidenceTonnageViewModel>._)).Returns(request);

            transferEvidenceTonnageViewModel.ViewTransferNoteViewModel.Status = status;
            transferEvidenceTonnageViewModel.Action = ActionEnum.Save;

            //act
            await outgoingTransferEvidenceController.EditTonnages(transferEvidenceTonnageViewModel);

            //assert
            outgoingTransferEvidenceController.TempData[ViewDataConstant.TransferEvidenceNoteDisplayNotification].Should().Be((NoteUpdatedStatusEnum)status);
        }

        [Fact]
        public async Task EditTonnagesPost_GivenValidModelAndSave_ShouldRedirectToViewTransferNote()
        {
            //arrange
            transferEvidenceTonnageViewModel.Action = ActionEnum.Save;

            //act
            var result = await outgoingTransferEvidenceController.EditTonnages(transferEvidenceTonnageViewModel) as RedirectToRouteResult;

            //assert
            result.RouteName.Should().Be(SchemeTransferEvidenceRedirect.ViewDraftTransferEvidenceRouteName);
            result.RouteValues["pcsId"].Should().Be(transferEvidenceTonnageViewModel.PcsId);
            result.RouteValues["evidenceNoteId"].Should().Be(transferEvidenceTonnageViewModel.ViewTransferNoteViewModel.EvidenceNoteId);
            result.RouteValues["redirectTab"].Should().Be(DisplayExtensions.ToDisplayString(
                ManageEvidenceNotesDisplayOptions.OutgoingTransfers));
        }

        [Fact]
        public async Task EditTonnagesPost_GivenValidModelAndSubmit_ShouldRedirectToViewTransferNote()
        {
            //arrange
            transferEvidenceTonnageViewModel.Action = ActionEnum.Submit;

            //act
            var result = await outgoingTransferEvidenceController.EditTonnages(transferEvidenceTonnageViewModel) as RedirectToRouteResult;

            //assert
            result.RouteName.Should().Be(SchemeTransferEvidenceRedirect.ViewSubmittedTransferEvidenceRouteName);
            result.RouteValues["pcsId"].Should().Be(transferEvidenceTonnageViewModel.PcsId);
            result.RouteValues["evidenceNoteId"].Should().Be(transferEvidenceTonnageViewModel.ViewTransferNoteViewModel.EvidenceNoteId);
            result.RouteValues["redirectTab"].Should().Be(DisplayExtensions.ToDisplayString(
                ManageEvidenceNotesDisplayOptions.OutgoingTransfers));
        }

        [Fact]
        public async Task EditTonnagesPost_GivenModelActionIsBack_ShouldSetTransferSessionObject()
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

            outgoingTransferEvidenceController.ModelState.AddModelError("error", "error");

            // act
            await outgoingTransferEvidenceController.EditTonnages(model);

            // assert
            A.CallTo(() => sessionService.SetTransferSessionObject(outgoingTransferEvidenceController.Session,
          A<object>.That.Matches(o => ((TransferEvidenceTonnageViewModel)o).TransferCategoryValues.SequenceEqual(transferCategoryValues) &&
                                      ((TransferEvidenceTonnageViewModel)o).RecipientId == recipientId &&
                                      ((TransferEvidenceTonnageViewModel)o).PcsId == pcsIds &&
                                      ((TransferEvidenceTonnageViewModel)o).ComplianceYear == complianceYear &&
                                      ((TransferEvidenceTonnageViewModel)o).Action == ActionEnum.Back),
          SessionKeyConstant.EditTransferEvidenceTonnageViewModel)).MustHaveHappenedOnceExactly();
        }
    }
}
