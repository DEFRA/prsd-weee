﻿namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.Controllers
{
    using AutoFixture;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Core.Scheme;
    using EA.Weee.Requests.AatfEvidence;
    using EA.Weee.Requests.Scheme;
    using EA.Weee.Web.Areas.Scheme.Controllers;
    using EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels;
    using EA.Weee.Web.Areas.Scheme.ViewModels;
    using EA.Weee.Web.Areas.Scheme.ViewModels.ManageEvidenceNotes;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Requests.Base;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Xunit;

    public class ManageEvidenceNotesControllerTests
    {
        protected readonly IWeeeClient WeeeClient;
        protected readonly IMapper Mapper;
        protected readonly ManageEvidenceNotesController ManageEvidenceController;
        protected readonly BreadcrumbService Breadcrumb;
        protected readonly IWeeeCache Cache;
        protected readonly Guid OrganisationId;
        protected readonly Fixture Fixture;
        protected readonly IRequestCreator<TransferEvidenceNoteDataViewModel, TransferEvidenceNoteRequest> TransferNoteRequestCreator;

        public ManageEvidenceNotesControllerTests()
        {
            WeeeClient = A.Fake<IWeeeClient>();
            Breadcrumb = A.Fake<BreadcrumbService>();
            Cache = A.Fake<IWeeeCache>();
            Mapper = A.Fake<IMapper>();
            OrganisationId = Guid.NewGuid();
            TransferNoteRequestCreator = A.Fake<IRequestCreator<TransferEvidenceNoteDataViewModel, TransferEvidenceNoteRequest>>();
            ManageEvidenceController = new ManageEvidenceNotesController(Mapper, Breadcrumb, Cache, () => WeeeClient, TransferNoteRequestCreator);
            Fixture = new Fixture();
        }

        [Fact]
        public void ManageEvidenceNotesControllerInheritsCheckSchemeEvidenceBaseController()
        {
            typeof(ManageEvidenceNotesController).BaseType.Name.Should().Be(nameof(SchemeEvidenceBaseController));
        }

        [Fact]
        public void IndexGet_ShouldHaveHttpGetAttribute()
        {
            typeof(ManageEvidenceNotesController).GetMethod("Index", new[] { typeof(Guid), typeof(ManageEvidenceNotesDisplayOptions) }).Should()
                .BeDecoratedWith<HttpGetAttribute>();
        }

        [Fact]
        public void TransferPost_ShouldHaveHttpPostAttribute()
        {
            typeof(ManageEvidenceNotesController).GetMethod("Transfer", new[] { typeof(Guid) }).Should()
                .BeDecoratedWith<HttpPostAttribute>();
        }

        [Fact]
        public void TransferPost_ShouldHaveAntiForgeryAttribute()
        {
            typeof(ManageEvidenceNotesController).GetMethod("Transfer", new[] { typeof(Guid) }).Should()
                .BeDecoratedWith<ValidateAntiForgeryTokenAttribute>();
        }

        [Fact]
        public void DownloadEvidenceNoteGet_ShouldHaveHttpGetAttribute()
        {
            typeof(ManageEvidenceNotesController).GetMethod("DownloadEvidenceNote", new[] { typeof(Guid), typeof(Guid) }).Should()
                .BeDecoratedWith<HttpGetAttribute>();
        }

        [Fact]
        public async Task IndexGet_BreadcrumbShouldBeSet()
        {
            //arrange
            var organisationName = Faker.Company.Name();
            var organisationId = Guid.NewGuid();

            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNotesByOrganisationRequest>._)).Returns(new List<EvidenceNoteData>());
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetSchemeByOrganisationId>._)).Returns(new SchemeData() { SchemeName = organisationName });
            A.CallTo(() => Mapper.Map<ReviewSubmittedEvidenceNotesViewModel>(A<ReviewSubmittedEvidenceNotesViewModelMapTransfer>._)).Returns(new ReviewSubmittedEvidenceNotesViewModel());
            A.CallTo(() => Cache.FetchOrganisationName(organisationId)).Returns(organisationName);

            //act
            await ManageEvidenceController.Index(organisationId);

            //assert
            Breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.SchemeManageEvidence);
            Breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            Breadcrumb.OrganisationId.Should().Be(organisationId);
        }

        [Fact]
        public async Task IndexGet_GivenOrganisationId_SchemeShouldBeRetrieved()
        {
            // Arrange
            var organisationName = Faker.Company.Name();
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetSchemeByOrganisationId>._)).Returns(new SchemeData() { SchemeName = organisationName });

            //act
            await ManageEvidenceController.Index(OrganisationId);

            //asset
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetSchemeByOrganisationId>.That.Matches(
                g => g.OrganisationId.Equals(OrganisationId)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task IndexGet_GivenOrganisationId_SubmittedEvidenceNoteShouldBeRetrieved()
        {
            // Arrange
            var organisationName = Faker.Company.Name();
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetSchemeByOrganisationId>._)).Returns(new SchemeData() { SchemeName = organisationName });

            //act
            await ManageEvidenceController.Index(OrganisationId);

            //asset
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNotesByOrganisationRequest>.That.Matches(
                g => g.OrganisationId.Equals(OrganisationId) &&
                     g.AllowedStatuses.Contains(NoteStatus.Submitted)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task IndexGet_GivenReturnedData_AndDefaultAction_DefaultViewModelShouldBeBuilt()
        {
            // Arrange
            var organisationName = Faker.Company.Name();
            var evidenceData = Fixture.Create<EvidenceNoteData>();
            List<EvidenceNoteData> returnList = new List<EvidenceNoteData>() { evidenceData };

            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNotesByOrganisationRequest>._)).Returns(returnList);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetSchemeByOrganisationId>._)).Returns(new SchemeData() { SchemeName = organisationName });

            //act
            await ManageEvidenceController.Index(OrganisationId);

            //asset
            A.CallTo(() => Mapper.Map<ReviewSubmittedEvidenceNotesViewModel>(
                A<ReviewSubmittedEvidenceNotesViewModelMapTransfer>.That.Matches(
                    a => a.OrganisationId.Equals(OrganisationId) && a.Notes.Equals(returnList) &&
                         a.SchemeName.Equals(organisationName)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task IndexGet_GivenReturnedData_AndReviewSubmittedAction_ReviewSubmittedEvidenceNotesViewModelShouldBeBuilt()
        {
            // Arrange
            var organisationName = Faker.Company.Name();
            var evidenceData = Fixture.Create<EvidenceNoteData>();
            List<EvidenceNoteData> returnList = new List<EvidenceNoteData>() { evidenceData };

            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNotesByOrganisationRequest>._)).Returns(returnList);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetSchemeByOrganisationId>._)).Returns(new SchemeData() { SchemeName = organisationName });

            //act
            await ManageEvidenceController.Index(OrganisationId, ManageEvidenceNotesDisplayOptions.ReviewSubmittedEvidence);

            //asset
            A.CallTo(() => Mapper.Map<ReviewSubmittedEvidenceNotesViewModel>(
                A<ReviewSubmittedEvidenceNotesViewModelMapTransfer>.That.Matches(
                    a => a.OrganisationId.Equals(OrganisationId) && a.Notes.Equals(returnList) &&
                         a.SchemeName.Equals(organisationName)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task IndexGet_GivenReturnedData_AndViewAndTransferAction_ViewAndTransferEvidenceNotesViewModelShouldBeBuilt()
        {
            // Arrange
            var organisationName = Faker.Company.Name();
            var evidenceData = Fixture.Create<EvidenceNoteData>();
            List<EvidenceNoteData> returnList = new List<EvidenceNoteData>() { evidenceData };

            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNotesByOrganisationRequest>._)).Returns(returnList);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetSchemeByOrganisationId>._)).Returns(new SchemeData() { SchemeName = organisationName });

            //act
            await ManageEvidenceController.Index(OrganisationId, ManageEvidenceNotesDisplayOptions.ViewAndTransferEvidence);

            //asset
            A.CallTo(() => Mapper.Map<ViewAndTransferEvidenceViewModel>(
                A<ViewAndTransferEvidenceViewModelMapTransfer>.That.Matches(
                    a => a.OrganisationId.Equals(OrganisationId) && a.Notes.Equals(returnList) &&
                         a.SchemeName.Equals(organisationName)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task IndexGet_GivenDefaultViewModel_DefaultModelShouldBeReturned()
        {
            //arrange
            var organisationName = Faker.Company.Name();
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetSchemeByOrganisationId>._)).Returns(new SchemeData() { SchemeName = organisationName });

            var model = Fixture.Create<ReviewSubmittedEvidenceNotesViewModel>();

            A.CallTo(() => Mapper.Map<ReviewSubmittedEvidenceNotesViewModel>(A<ReviewSubmittedEvidenceNotesViewModelMapTransfer>._)).Returns(model);

            //act
            var result =
                await ManageEvidenceController.Index(OrganisationId) as
                    ViewResult;

            //asset
            result.Model.Should().Be(model);
        }

        [Fact]
        public async Task IndexGet_GivenReviewSubmittedEvidenceNotesViewModel_ReviewSubmittedEvidenceNotesViewModelShouldBeReturned()
        {
            //arrange
            var organisationName = Faker.Company.Name();
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetSchemeByOrganisationId>._)).Returns(new SchemeData() { SchemeName = organisationName });

            var model = Fixture.Create<ReviewSubmittedEvidenceNotesViewModel>();

            A.CallTo(() => Mapper.Map<ReviewSubmittedEvidenceNotesViewModel>(A<ReviewSubmittedEvidenceNotesViewModelMapTransfer>._)).Returns(model);

            //act
            var result =
                await ManageEvidenceController.Index(OrganisationId, ManageEvidenceNotesDisplayOptions.ReviewSubmittedEvidence) as
                    ViewResult;

            //asset
            result.Model.Should().Be(model);
        }

        [Fact]
        public async Task IndexGet_GivenViewAndTransferEvidenceNotesViewModel_ViewAndTransferEvidenceNotesViewModelShouldBeReturned()
        {
            //arrange
            var organisationName = Faker.Company.Name();
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetSchemeByOrganisationId>._)).Returns(new SchemeData() { SchemeName = organisationName });

            var model = Fixture.Create<ViewAndTransferEvidenceViewModel>();

            A.CallTo(() => Mapper.Map<ViewAndTransferEvidenceViewModel>(A<ViewAndTransferEvidenceViewModelMapTransfer>._)).Returns(model);

            //act
            var result =
                await ManageEvidenceController.Index(OrganisationId, ManageEvidenceNotesDisplayOptions.ViewAndTransferEvidence) as
                    ViewResult;

            //asset
            result.Model.Should().Be(model);
        }

        [Fact]
        public void TransferPost_PageRedirectsToTransferPage()
        {
            var result = ManageEvidenceController.Transfer(OrganisationId) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("TransferEvidenceNote");
            result.RouteValues["controller"].Should().Be("TransferEvidence");
            result.RouteValues["pcsId"].Should().Be(OrganisationId);
        }
    }
}
