namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.Controllers
{
    using AutoFixture;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Web.Areas.Scheme.Controllers;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Core.AatfEvidence;
    using Web.Areas.Scheme.Mappings.ToViewModels;
    using Web.Areas.Scheme.ViewModels;
    using Weee.Requests.AatfEvidence;
    using Weee.Tests.Core;
    using Xunit;

    public class OutgoingTransfersControllerTests : SimpleUnitTestBase
    {
        private readonly IWeeeClient weeeClient;
        private readonly IMapper mapper;
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
            organisationId = Guid.NewGuid();

            outgoingTransferEvidenceController = new OutgoingTransfersController(mapper, breadcrumb, cache, () => weeeClient);

            transferEvidenceNoteData = TestFixture.Create<TransferEvidenceNoteData>();
            evidenceNoteData = TestFixture.CreateMany<EvidenceNoteData>().ToList();
            transferEvidenceTonnageViewModel = TestFixture.Create<TransferEvidenceTonnageViewModel>();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetTransferEvidenceNoteForSchemeRequest>._))
                .Returns(transferEvidenceNoteData);
        }

        [Fact]
        public void OutgoingTransfersControllerInheritsCheckSchemeEvidenceBaseController()
        {
            typeof(OutgoingTransfersController).BaseType.Name.Should().Be(nameof(SchemeEvidenceBaseController));
        }

        [Fact]
        public void EditTransferFrom_ShouldHaveHttpGetAttribute()
        {
            typeof(OutgoingTransfersController).GetMethod("EditTransferFrom", new[] { typeof(Guid), typeof(Guid) }).Should()
                .BeDecoratedWith<HttpGetAttribute>();
        }

        [Fact]
        public void EditTonnagesGet_ShouldHaveHttpGetAttribute()
        {
            typeof(OutgoingTransfersController).GetMethod("EditTonnages", new[] { typeof(Guid), typeof(Guid) }).Should()
                .BeDecoratedWith<HttpGetAttribute>();
        }

        [Fact]
        public void SubmittedTransferGet_ShouldHaveHttpGetAttribute()
        {
            typeof(OutgoingTransfersController).GetMethod("SubmittedTransfer", new[] { typeof(Guid), typeof(Guid), typeof(int?), typeof(bool?) }).Should()
                .BeDecoratedWith<HttpGetAttribute>();
        }

        [Fact]
        public void SubmittedTransferPost_ShouldHaveHttpPostAttribute()
        {
            typeof(OutgoingTransfersController).GetMethod("SubmittedTransfer", new[] { typeof(ReviewTransferNoteViewModel) }).Should()
                .BeDecoratedWith<HttpPostAttribute>();
        }

        [Fact]
        public void EditDraftTransfer_ShouldHaveHttpGetAttribute()
        {
            typeof(OutgoingTransfersController).GetMethod("EditDraftTransfer", new[] { typeof(Guid), typeof(Guid), typeof(int?), typeof(bool?) }).Should()
                .BeDecoratedWith<HttpGetAttribute>();
        }

        [Fact]
        public async Task EditTonnagesGet_GivenValidOrganisation_BreadcrumbShouldBeSet()
        {
            // arrange 
            var organisationName = "OrganisationName";

            A.CallTo(() => cache.FetchOrganisationName(organisationId)).Returns(organisationName);

            // act
            await outgoingTransferEvidenceController.EditTonnages(organisationId, TestFixture.Create<Guid>());

            // assert
            breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.SchemeManageEvidence);
        }

        [Fact]
        public async Task EditDraftTransfer_GivenValidOrganisation_BreadcrumbShouldBeSet()
        {
            // arrange 
            var organisationName = "OrganisationName";

            A.CallTo(() => cache.FetchOrganisationName(organisationId)).Returns(organisationName);

            // act
            await outgoingTransferEvidenceController.EditDraftTransfer(organisationId, TestFixture.Create<Guid>(), null, null);

            // assert
            breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.SchemeManageEvidence);
        }

        [Fact]
        public async Task EditTonnagesGet_GivenEvidenceNoteId_ShouldRetrieveTransferNote()
        {
            //arrange
            var evidenceNoteId = TestFixture.Create<Guid>();

            //act
            await outgoingTransferEvidenceController.EditTonnages(organisationId, evidenceNoteId);

            //assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                    A<GetTransferEvidenceNoteForSchemeRequest>.That.Matches(r => r.EvidenceNoteId == evidenceNoteId)))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditTonnagesGet_GivenTransferNoteData_TransferNotesShouldBeRetrieved()
        {
            //arrange
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetTransferEvidenceNoteForSchemeRequest>._)).Returns(transferEvidenceNoteData);

            var categories = transferEvidenceNoteData.CategoryIds;
            var existingNotes = transferEvidenceNoteData.TransferEvidenceNoteTonnageData.Select(t => t.OriginalNoteId).ToList();

            //act
            await outgoingTransferEvidenceController.EditTonnages(organisationId, TestFixture.Create<Guid>());

            //assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                    A<GetEvidenceNotesForTransferRequest>.That.Matches(r => 
                        r.OrganisationId == organisationId && r.Categories.SequenceEqual(categories) &&
                        r.EvidenceNotes.SequenceEqual(existingNotes))))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditTonnagesGet_GivenTransferNoteAndTransferNotes_ModelMapperShouldBeCalled()
        {
            //arrange
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetTransferEvidenceNoteForSchemeRequest>._)).Returns(transferEvidenceNoteData);

            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetEvidenceNotesForTransferRequest>._)).Returns(evidenceNoteData);

            //act
            await outgoingTransferEvidenceController.EditTonnages(organisationId, TestFixture.Create<Guid>());

            //assert
            A.CallTo(() => mapper.Map<TransferEvidenceNotesViewModelMapTransfer, TransferEvidenceTonnageViewModel>(
                    A<TransferEvidenceNotesViewModelMapTransfer>.That.Matches(t => t.TransferAllTonnage == false &&
                        t.TransferEvidenceNoteData ==
                        transferEvidenceNoteData &&
                        t.Request == null &&
                        t.Notes.SequenceEqual(evidenceNoteData) &&
                        t.OrganisationId == organisationId)))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditTonnagesGet_GivenMappedModel_ModelShouldBeReturned()
        {
            //arrange
            A.CallTo(() => mapper.Map<TransferEvidenceNotesViewModelMapTransfer, TransferEvidenceTonnageViewModel>(
                A<TransferEvidenceNotesViewModelMapTransfer>._)).Returns(transferEvidenceTonnageViewModel);

            //act
            var result = await outgoingTransferEvidenceController.EditTonnages(organisationId, TestFixture.Create<Guid>()) as ViewResult;

            //assert
            result.Model.Should().Be(transferEvidenceTonnageViewModel);
        }

        [Fact]
        public async Task EditTonnagesGet_ShouldReturnView()
        {
            //act
            var result = await outgoingTransferEvidenceController.EditTonnages(organisationId, TestFixture.Create<Guid>()) as ViewResult;

            //assert
            result.ViewName.Should().Be("EditTonnages");
        }

        [Theory]
        [InlineData(null)]
        [InlineData(2022)]
        public async Task EditDraftTransferGet_GivenEvidenceNoteId_ShouldRetrieveTransferNote(int? complianceYear)
        {
            //arrange
            var evidenceNoteId = TestFixture.Create<Guid>();

            //act
            await outgoingTransferEvidenceController.EditDraftTransfer(organisationId, evidenceNoteId, complianceYear, null);

            //assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                    A<GetTransferEvidenceNoteForSchemeRequest>.That.Matches(r => r.EvidenceNoteId == evidenceNoteId)))
                .MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(2022)]
        public async Task EditDraftTransferGet_GivenTransferNote_ModelMapperShouldBeCalled(int? complianceYear)
        {
            //arrange
            var transferNoteData = TestFixture.Create<TransferEvidenceNoteData>();

            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                    A<GetTransferEvidenceNoteForSchemeRequest>._)).Returns(transferNoteData);

            //act
            await outgoingTransferEvidenceController.EditDraftTransfer(organisationId, TestFixture.Create<Guid>(), complianceYear, null);

            //assert
            A.CallTo(() => mapper.Map<ViewTransferNoteViewModel>(A<ViewTransferNoteViewModelMapTransfer>.That.Matches(
                    v => v.Edit == true && 
                         v.DisplayNotification == null && 
                         v.TransferEvidenceNoteData == transferNoteData && 
                         v.SchemeId == organisationId && v.SelectedComplianceYear == complianceYear))).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(true)]
        [InlineData(false)]
        public async Task EditDraftTransferGet_GivenTransferNoteAndReturnToView_ModelMapperShouldBeCalled(bool? returnToView)
        {
            //arrange
            var transferNoteData = TestFixture.Create<TransferEvidenceNoteData>();

            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetTransferEvidenceNoteForSchemeRequest>._)).Returns(transferNoteData);

            //act
            await outgoingTransferEvidenceController.EditDraftTransfer(organisationId, TestFixture.Create<Guid>(), null, returnToView);

            //assert
            A.CallTo(() => mapper.Map<ViewTransferNoteViewModel>(A<ViewTransferNoteViewModelMapTransfer>.That.Matches(
                v => v.Edit == true &&
                     v.DisplayNotification == null &&
                     v.TransferEvidenceNoteData == transferNoteData &&
                     v.SchemeId == organisationId && v.ReturnToView == returnToView))).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(2022)]
        public async Task EditDraftTransferGet_GivenMappedModel_ModelShouldBeReturned(int? complianceYear)
        {
            //arrange
            var model = TestFixture.Create<ViewTransferNoteViewModel>();

            A.CallTo(() => mapper.Map<ViewTransferNoteViewModel>(A<ViewTransferNoteViewModelMapTransfer>._)).Returns(model);

            //act
            var result = await outgoingTransferEvidenceController.EditDraftTransfer(organisationId, TestFixture.Create<Guid>(), complianceYear, null) as ViewResult;

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

            A.CallTo(() => mapper.Map<ViewTransferNoteViewModel>(A<ViewTransferNoteViewModelMapTransfer>._)).Returns(model);

            //act
            var result = await outgoingTransferEvidenceController.EditDraftTransfer(organisationId, TestFixture.Create<Guid>(), null, returnToView) as ViewResult;

            //assert
            result.Model.Should().Be(model);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(2022)]
        public async Task EditDraftTransferGet_ShouldReturnView(int? complianceYear)
        {
            //act
            var result = await outgoingTransferEvidenceController.EditDraftTransfer(organisationId, TestFixture.Create<Guid>(), complianceYear, null) as ViewResult;

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
            var result = await outgoingTransferEvidenceController.EditDraftTransfer(organisationId, TestFixture.Create<Guid>(), null, returnToView) as ViewResult;

            //assert
            result.ViewName.Should().Be("EditDraftTransfer");
        }

        [Fact]
        public async Task SubmittedTransferGet_GivenValidOrganisation_BreadcrumbShouldBeSet()
        {
            // arrange 
            var organisationName = "OrganisationName";
            A.CallTo(() => cache.FetchOrganisationName(organisationId)).Returns(organisationName);

            // act
            await outgoingTransferEvidenceController.SubmittedTransfer(organisationId, TestFixture.Create<Guid>(), TestFixture.Create<int>(), TestFixture.Create<bool>());

            // assert
            breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.SchemeManageEvidence);
        }

        [Fact]
        public async Task SubmittedTransferGet_GivenEvidenceNoteId_ShouldRetrieveTransferNote()
        {
            //arrange
            var evidenceNoteId = TestFixture.Create<Guid>();

            //act
            await outgoingTransferEvidenceController.SubmittedTransfer(organisationId, evidenceNoteId, TestFixture.Create<int?>(), TestFixture.Create<bool?>());

            //assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                    A<GetTransferEvidenceNoteForSchemeRequest>.That.Matches(r => r.EvidenceNoteId == evidenceNoteId)))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task SubmittedTransferGet_GivenTransferNote_ModelMapperShouldBeCalled()
        {
            //arrange
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetTransferEvidenceNoteForSchemeRequest>._)).Returns(transferEvidenceNoteData);

            //act
            await outgoingTransferEvidenceController.SubmittedTransfer(organisationId, TestFixture.Create<Guid>(), TestFixture.Create<int?>(), TestFixture.Create<bool?>());

            //assert
            A.CallTo(() => mapper.Map<ReviewTransferNoteViewModel>(
                    A<ViewTransferNoteViewModelMapTransfer>.That.Matches(t => t.TransferEvidenceNoteData == transferEvidenceNoteData &&
                                                                              t.SchemeId == organisationId))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task SubmittedTransferGet_GivenMappedModel_ModelShouldBeReturned()
        {
            //arrange
            var model = TestFixture.Create<ReviewTransferNoteViewModel>();
            A.CallTo(() => mapper.Map<ReviewTransferNoteViewModel>(A<ViewTransferNoteViewModelMapTransfer>._))
                .Returns(model);

            //act
            var result = await outgoingTransferEvidenceController.SubmittedTransfer(organisationId, TestFixture.Create<Guid>(), TestFixture.Create<int?>(), TestFixture.Create<bool?>()) as ViewResult;

            //assert
            result.Model.Should().Be(model);
        }

        [Fact]
        public async Task SubmittedTransferGet_ShouldReturnView()
        {
            //act
            var result = await outgoingTransferEvidenceController.SubmittedTransfer(organisationId, TestFixture.Create<Guid>(), TestFixture.Create<int?>(), TestFixture.Create<bool?>()) as ViewResult;

            //assert
            result.ViewName.Should().Be("SubmittedTransfer");
        }

        [Fact]
        public async Task SubmittedTransferPost_GivenModel_BreadCrumbShouldBeSet()
        {
            //arrange
            var model = TestFixture.Create<ReviewTransferNoteViewModel>();
            var organisationName = "OrganisationName";
            A.CallTo(() => cache.FetchOrganisationName(model.OrganisationId)).Returns(organisationName);

            //act
            await outgoingTransferEvidenceController.SubmittedTransfer(model);

            // assert
            breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.SchemeManageEvidence);
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
                    A<GetTransferEvidenceNoteForSchemeRequest>.That.Matches(r => r.EvidenceNoteId == model.ViewTransferNoteViewModel.EvidenceNoteId)))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task SubmittedTransferPost_GivenInvalidViewModel_ModelMapperShouldBeCalled()
        {
            //arrange
            var model = TestFixture.Create<ReviewTransferNoteViewModel>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetTransferEvidenceNoteForSchemeRequest>._)).Returns(transferEvidenceNoteData);

            outgoingTransferEvidenceController.ModelState.AddModelError("error", "error");

            //act
            await outgoingTransferEvidenceController.SubmittedTransfer(model);

            //assert
            A.CallTo(() => mapper.Map<ReviewTransferNoteViewModel>(
                    A<ViewTransferNoteViewModelMapTransfer>.That.Matches(t => t.TransferEvidenceNoteData == transferEvidenceNoteData &&
                                                                              t.SchemeId == model.OrganisationId))).MustHaveHappenedOnceExactly();
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
        public async Task EditTransferFromGet_GivenValidOrganisation_BreadcrumbShouldBeSet()
        {
            // arrange 
            var organisationName = "OrganisationName";

            A.CallTo(() => cache.FetchOrganisationName(organisationId)).Returns(organisationName);

            // act
            await outgoingTransferEvidenceController.EditTransferFrom(organisationId, TestFixture.Create<Guid>());

            // assert
            breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.SchemeManageEvidence);
        }

        [Fact]
        public async Task EditTransferFrom_GivenEvidenceNoteId_ShouldRetrieveTransferNote()
        {
            //arrange
            var evidenceNoteId = TestFixture.Create<Guid>();

            //act
            await outgoingTransferEvidenceController.EditTransferFrom(organisationId, evidenceNoteId);

            //assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                    A<GetTransferEvidenceNoteForSchemeRequest>.That.Matches(r => r.EvidenceNoteId == evidenceNoteId)))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditTransferFrom_GivenTransferNoteData_TransferNotesShouldBeRetrieved()
        {
            //arrange
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetTransferEvidenceNoteForSchemeRequest>._)).Returns(transferEvidenceNoteData);

            var categories = transferEvidenceNoteData.CategoryIds;

            //act
            await outgoingTransferEvidenceController.EditTransferFrom(organisationId, TestFixture.Create<Guid>());

            //assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                    A<GetEvidenceNotesForTransferRequest>.That.Matches(r =>
                        r.OrganisationId == organisationId &&
                        r.Categories.SequenceEqual(transferEvidenceNoteData.CategoryIds))))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditTransferFromGet_GivenTransferNoteAndCategories_ModelMapperShouldBeCalled()
        {
            //arrange
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetTransferEvidenceNoteForSchemeRequest>._)).Returns(transferEvidenceNoteData);

            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetEvidenceNotesForTransferRequest>._)).Returns(evidenceNoteData);

            //act
            await outgoingTransferEvidenceController.EditTransferFrom(organisationId, TestFixture.Create<Guid>());

            //assert
            A.CallTo(() => mapper.Map<TransferEvidenceNotesViewModelMapTransfer, TransferEvidenceNotesViewModel>(
                    A<TransferEvidenceNotesViewModelMapTransfer>.That.Matches(t => t.TransferAllTonnage == false &&
                        t.TransferEvidenceNoteData ==
                        transferEvidenceNoteData &&
                        t.Request == null &&
                        t.Notes.SequenceEqual(evidenceNoteData) &&
                        t.OrganisationId == organisationId)))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditTransferFrom_GivenMappedModel_ModelShouldBeReturned()
        {
            //arrange
            var model = TestFixture.Create<TransferEvidenceNotesViewModel>();
            A.CallTo(() => mapper.Map<TransferEvidenceNotesViewModelMapTransfer, TransferEvidenceNotesViewModel>(
                A<TransferEvidenceNotesViewModelMapTransfer>._)).Returns(model);

            //act
            var result = await outgoingTransferEvidenceController.EditTransferFrom(organisationId, TestFixture.Create<Guid>()) as ViewResult;

            //assert
            result.Model.Should().Be(model);
        }

        [Fact]
        public async Task EditTransferFrom_ShouldReturnView()
        {
            //act
            var result = await outgoingTransferEvidenceController.EditTransferFrom(organisationId, TestFixture.Create<Guid>()) as ViewResult;

            //assert
            result.ViewName.Should().Be("EditTransferFrom");
        }
    }
}
