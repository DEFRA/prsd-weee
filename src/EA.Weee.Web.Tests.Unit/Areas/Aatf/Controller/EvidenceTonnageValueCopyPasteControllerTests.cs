﻿namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Controller
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using AutoFixture;
    using EA.Prsd.Core.Helpers;
    using EA.Weee.Core.Aatf;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Core.DataReturns;
    using EA.Weee.Core.Shared;
    using EA.Weee.Web.Areas.Aatf.Controllers;
    using EA.Weee.Web.Areas.Aatf.ViewModels;
    using EA.Weee.Web.Areas.AatfEvidence.Controllers;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using FakeItEasy;
    using FluentAssertions;
    using Weee.Tests.Core;
    using Xunit;

    public class EvidenceTonnageValueCopyPasteControllerTests : SimpleUnitTestBase
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
            typeof(EvidenceTonnageValueCopyPasteController).GetMethod("Index", new[] { typeof(Guid), typeof(string), typeof(int), typeof(bool) })
                .Should()
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
            //Arrange
            var organisationName = Faker.Company.Name();

            A.CallTo(() => cache.FetchOrganisationName(organisationId)).Returns(organisationName);

            //Act
            await evidenceTonnageValueCopyPasteController.Index(organisationId, string.Empty, TestFixture.Create<int>(), true);

            //Assert
            breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.AatfManageEvidence);
            breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            breadcrumb.OrganisationId.Should().Be(organisationId);
        }

        [Fact]
        public async Task IndexGet_RetrievesEvidenceModelSessionObject()
        {
            //Act
            await evidenceTonnageValueCopyPasteController.Index(organisationId, string.Empty, TestFixture.Create<int>(), true);

            //Assert
            A.CallTo(() => sessionService.GetTransferSessionObject<EditEvidenceNoteViewModel>(
                SessionKeyConstant.EditEvidenceViewModelKey)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task IndexGet_OnRedirectIsFalse_CreatesModelAndReturnsView()
        {
            //arrange
            var complianceYear = TestFixture.Create<int>();

            //Act
            var result = await evidenceTonnageValueCopyPasteController.Index(organisationId, string.Empty, complianceYear, false) as ViewResult;

            //Assert
            result.Model.Should().BeOfType<EvidenceTonnageValueCopyPasteViewModel>();
            var convertedModel = (EvidenceTonnageValueCopyPasteViewModel)result.Model;
            convertedModel.ComplianceYear.Should().Be(complianceYear);

            var categories = EnumHelper.GetValues(typeof(WeeeCategory));
        }

        [Theory]
        [InlineData(EvidenceCopyPasteActionConstants.CreateEvidenceNoteAction, "CreateEvidenceNote", "ManageEvidenceNotes")]
        [InlineData(EvidenceCopyPasteActionConstants.EditEvidenceNoteAction, "AATF_EditEvidence", "ManageEvidenceNotes")]
        [InlineData(EvidenceCopyPasteActionConstants.ReturnedEvidenceNoteAction, "CreateEvidenceNote", "ManageEvidenceNotes")]
        public async Task IndexGet_OnRedirectIsTrue_RedirectsToCorrectPage(string returnAction, string expectedAction, string expectedController)
        {
            //arrange
            var complianceYear = TestFixture.Create<int>();

            //Act
            var result = await evidenceTonnageValueCopyPasteController.Index(organisationId, returnAction, complianceYear, true) as RedirectToRouteResult;

            //Assert
            if (returnAction == EvidenceCopyPasteActionConstants.EditEvidenceNoteAction)
            {
                result.RouteName.Should().Be(expectedAction);
            }
            else
            {
                result.RouteValues["action"].Should().Be(expectedAction);
                result.RouteValues["controller"].Should().Be(expectedController);
            }

            result.RouteValues["returnFromCopyPaste"].Should().Be(true);
            result.RouteValues["complianceYear"].Should().Be(complianceYear);
        }

        [Theory]
        [InlineData(EvidenceCopyPasteActionConstants.CreateEvidenceNoteAction, "CreateEvidenceNote", "ManageEvidenceNotes")]
        [InlineData(EvidenceCopyPasteActionConstants.EditEvidenceNoteAction, "AATF_EditEvidence", "ManageEvidenceNotes")]
        [InlineData(EvidenceCopyPasteActionConstants.ReturnedEvidenceNoteAction, "CreateEvidenceNote", "ManageEvidenceNotes")] // TODO UPDATE once returned page ready
        public void IndexPost_OnRedirectIsTrue_RedirectsToCorrectPage(string returnAction, string expectedAction, string expectedController)
        {
            //Arrange
            var model = CreateValidModel(returnAction);

            //Act
            var result = evidenceTonnageValueCopyPasteController.Index(model) as RedirectToRouteResult;

            //Assert
            if (returnAction == EvidenceCopyPasteActionConstants.EditEvidenceNoteAction)
            {
                result.RouteName.Should().Be(expectedAction);
            }
            else
            {
                result.RouteValues["action"].Should().Be(expectedAction);
                result.RouteValues["controller"].Should().Be(expectedController);
            }

            result.RouteValues["returnFromCopyPaste"].Should().Be(true);
        }

        [Fact]
        public void IndexPost_OnNoGivenCopyPasteValues_DoesNotPopulateSession()
        {
            //Arrange
            var model = CreateValidModel(string.Empty);
            model.ReceievedPastedValues = new string[1];
            model.ReusedPastedValues = new string[1];

            //Act
            evidenceTonnageValueCopyPasteController.Index(model);

            //Assert
            A.CallTo(() => sessionService.SetTransferSessionObject(A<object>._, SessionKeyConstant.EditEvidenceViewModelKey)).MustNotHaveHappened();
        }

        [Fact]
        public void IndexPost_OnGivenCopyPasteValues_DoesPopulateSession()
        {
            //Arrange
            var model = CreateValidModel(string.Empty);
            model.ReceievedPastedValues[0] = "Test";
            model.ReusedPastedValues[0] = "Test";

            //Act
            evidenceTonnageValueCopyPasteController.Index(model);

            //Assert
            A.CallTo(() => sessionService.SetTransferSessionObject(A<object>._, SessionKeyConstant.EditEvidenceViewModelKey)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void IndexPost_OnGivenCopyPasteValues_DoesCallPasteProcessor()
        {
            //Arrange
            var model = CreateValidModel(string.Empty);
            model.ReceievedPastedValues[0] = "Test";
            model.ReceievedPastedValues[0] = "Test";

            //Act
            evidenceTonnageValueCopyPasteController.Index(model);

            //Assert
            A.CallTo(() => pasteProcessor.BuildModel(A<string>._)).MustHaveHappenedTwiceExactly();
            A.CallTo(() => pasteProcessor.ParseEvidencePastedValues(A<EvidencePastedValues>._, A<IList<EvidenceCategoryValue>>._)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void IndexPost_OnNoGivenCopyPasteValues_DoesNotCallPasteProcessor()
        {
            //Arrange
            var model = CreateValidModel(string.Empty);
            model.ReceievedPastedValues = new string[1];
            model.ReusedPastedValues = new string[1];

            //Act
            evidenceTonnageValueCopyPasteController.Index(model);

            //Assert
            A.CallTo(() => pasteProcessor.BuildModel(A<string>._)).MustNotHaveHappened();
            A.CallTo(() => pasteProcessor.ParseEvidencePastedValues(A<EvidencePastedValues>._, A<IList<EvidenceCategoryValue>>._)).MustNotHaveHappened();
        }

        [Fact]
        public void IndexPost_Retrieves_EvidenceModelSessionObject()
        {
            //Arrange
            var model = CreateValidModel(string.Empty);

            //Act
            evidenceTonnageValueCopyPasteController.Index(model);

            //Assert
            A.CallTo(() => sessionService.GetTransferSessionObject<EditEvidenceNoteViewModel>(SessionKeyConstant.EditEvidenceViewModelKey)).MustHaveHappenedOnceExactly();
        }

        private EvidenceTonnageValueCopyPasteViewModel CreateValidModel(string action)
        {
            var model = new EvidenceTonnageValueCopyPasteViewModel();
            model.Action = action;
            model.ReusedPastedValues = TestFixture.Create<string[]>();
            model.ReceievedPastedValues = TestFixture.Create<string[]>();

            return model;
        }
    }
}
