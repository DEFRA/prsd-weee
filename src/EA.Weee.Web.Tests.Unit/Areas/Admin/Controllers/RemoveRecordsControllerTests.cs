namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using Core.Shared;
    using EA.Weee.Core.Scheme;
    using EA.Weee.Requests;
    using EA.Weee.Requests.Admin;
    using EA.Weee.Web.Areas.Admin.Controllers.Interfaces;
    using EA.Weee.Web.Areas.Admin.ViewModels.RemoveRecords;
    using EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers.Stubs;
    using FakeItEasy;
    using FluentAssertions;
    using Services;
    using Services.Caching;
    using TestHelpers;
    using Web.Areas.Admin.Controllers;
    using Web.Areas.Admin.Controllers.Base;
    using Xunit;
    using static EA.Weee.Requests.Admin.GetSchemes;

    public class RemoveRecordsControllerTests
    {
        private readonly IWeeeClient weeeClient;

        public RemoveRecordsControllerTests()
        {
            weeeClient = A.Fake<IWeeeClient>();
        }

        [Fact]
        public void Controller_ShouldInheritFromAdminBaseController()
        {
            typeof(RemoveRecordsController).Should().BeDerivedFrom<AdminController>();
        }

        [Fact]
        public void GetChooseActivity_ReturnsView()
        {
            var result = RemoveRecordsController().ChooseActivity();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void PostChooseActivity_ModelIsInvalid_ShouldRedirectViewWithModel()
        {
            var controller = RemoveRecordsController();
            controller.ModelState.AddModelError("Key", "Any error");

            var model = new RemoveRecordsViewModel
            {
                SelectedValue = "Value"
            };
            var result = controller.ChooseActivity(model);

            Assert.IsType<ViewResult>(result);
            Assert.Equal(model, ((ViewResult)(result)).Model);
            Assert.False(controller.ModelState.IsValid);
        }

        [Fact]
        public void PostChooseActivity_RemovePCSSelected_RedirectsToRemovePCS()
        {
            var result = RemoveRecordsController().ChooseActivity(new RemoveRecordsViewModel
            {
                SelectedValue = InternalRemoveRecordsActivity.RemovePCS
            });

            Assert.IsType<RedirectToRouteResult>(result);

            var routeValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("RemovePCS", routeValues["action"]);
        }

        [Fact]
        public async Task GetDeleted_CallsSendAsync()
        {
            await RemoveRecordsController().Deleted(A.Dummy<Guid>(), A.Dummy<int>());

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSchemes>._))
                .WhenArgumentsMatch(a => ((GetSchemes)a[1]).Filter == FilterType.ApprovedOrWithdrawn)
                .MustHaveHappened();
        }

        [Fact]
        public async Task GetDeleted_ReturnsView()
        {
            var result = await RemoveRecordsController().Deleted(A.Dummy<Guid>(), A.Dummy<int>());

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task GetConfirmDeletion_CallsSendAsync()
        {
            await RemoveRecordsController().ConfirmDeletion(A.Dummy<Guid>(), 2016);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSchemesForComplianceYear>._))
                .WhenArgumentsMatch(a => ((GetSchemesForComplianceYear)a[1]).ComplianceYear == 2016)
                .MustHaveHappened();
        }

        [Fact]
        public async Task GetConfirmDeletion_ReturnsView()
        {
            var result = await RemoveRecordsController().ConfirmDeletion(A.Dummy<Guid>(), A.Dummy<int>());

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task PostConfirmDeletion_InvalidModel_ReturnsView()
        {
            var controller = RemoveRecordsController();
            controller.ModelState.AddModelError("Key", "Any error");

            var model = new RemovePCSRecordsConfirmDeletionViewModel
            {
                SchemeId = A.Dummy<Guid>(),
                ComplianceYear = A.Dummy<int>(),
                PCSName = A.Dummy<string>(),
                ApprovalNumber = A.Dummy<string>()
            };
            var result = await controller.ConfirmDeletion(model);

            Assert.IsType<ViewResult>(result);
            Assert.Equal(model, ((ViewResult)(result)).Model);
            Assert.False(controller.ModelState.IsValid);
        }

        [Fact]
        public async Task PostConfirmDeletion_ThrowsException_ReturnsView()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturnValueFromRemovingPCSRecords>._))
                .Returns(-1);

            var model = new RemovePCSRecordsConfirmDeletionViewModel
            {
                SchemeId = A.Dummy<Guid>(),
                ComplianceYear = A.Dummy<int>(),
                PCSName = A.Dummy<string>(),
                ApprovalNumber = A.Dummy<string>()
            };
            var result = await RemoveRecordsController().ConfirmDeletion(model);

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task PostConfirmDeletion_NoErrors_RedirectsToAction()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturnValueFromRemovingPCSRecords>._))
                .Returns(0);

            var model = new RemovePCSRecordsConfirmDeletionViewModel
            {
                SchemeId = A.Dummy<Guid>(),
                ComplianceYear = A.Dummy<int>(),
                PCSName = A.Dummy<string>(),
                ApprovalNumber = A.Dummy<string>()
            };
            var result = await RemoveRecordsController().ConfirmDeletion(model);

            Assert.IsType<RedirectToRouteResult>(result);
        }

        [Fact]
        public async Task GetRemovePCS_HasModel_ReturnsView()
        {
            // ARRANGE
            IGetAllYears getAllYearsStub = new GetAllYearsStub(new List<string>());
            IGetSchemesForYear getSchemesForYearStub = new GetSchemesForYearStub(new List<SchemeData>());
            IGetSortedResults getSortedResultsStub = new GetSortedResultsStub(new List<DataAccess.StoredProcedure.SchemeDataExceedingRetentionPeriod>());

            RemoveRecordsController().GetAllYearsStub = getAllYearsStub;
            RemoveRecordsController().GetSchemesForYearStub = getSchemesForYearStub;
            RemoveRecordsController().GetSortedResultsStub = getSortedResultsStub;

            // ACT
            var result = await RemoveRecordsController().RemovePCS(1, null, null);

            // ASSERT
            Assert.IsType<ViewResult>(result);

            var model = ((ViewResult)result).Model as RemovePCSRecordsFilterViewModel;

            Assert.NotNull(model);
            Assert.Equal(model.SelectedYear, "All Years");
            Assert.Equal(model.SelectedScheme, "All PCSs");
        }

        [Fact]
        public async Task PostRemovePCS_HasModel_ReturnsView()
        {
            // ARRANGE
            IGetAllYears getAllYearsStub = new GetAllYearsStub(new List<string>());
            IGetSchemesForYear getSchemesForYearStub = new GetSchemesForYearStub(new List<SchemeData>());
            IGetSortedResults getSortedResultsStub = new GetSortedResultsStub(new List<DataAccess.StoredProcedure.SchemeDataExceedingRetentionPeriod>());

            RemoveRecordsController().GetAllYearsStub = getAllYearsStub;
            RemoveRecordsController().GetSchemesForYearStub = getSchemesForYearStub;
            RemoveRecordsController().GetSortedResultsStub = getSortedResultsStub;

            RemovePCSRecordsFilterViewModel inputModel = new RemovePCSRecordsFilterViewModel();
            inputModel.SelectedYear = "All Years";
            inputModel.SelectedScheme = "All PCSs";

            // ACT
            var result = await RemoveRecordsController().RemovePCS(inputModel, 1);

            // ASSERT
            Assert.IsType<ViewResult>(result);

            var model = ((ViewResult)result).Model as RemovePCSRecordsFilterViewModel;

            Assert.NotNull(model);
            Assert.Equal(model.SelectedYear, "All Years");
            Assert.Equal(model.SelectedScheme, "All PCSs");
        }

        private RemoveRecordsController RemoveRecordsController()
        {
            IAppConfiguration configService = A.Fake<IAppConfiguration>();
            var controller = new RemoveRecordsController(() => weeeClient, configService, A.Fake<BreadcrumbService>());
            new HttpContextMocker().AttachToController(controller);
            return controller;
        }
    }
}
