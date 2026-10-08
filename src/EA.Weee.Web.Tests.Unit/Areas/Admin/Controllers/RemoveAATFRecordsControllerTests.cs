namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Requests.Admin.RemoveAATFOrAeRecords;
    using EA.Weee.Web.Areas.Admin.Controllers;
    using EA.Weee.Web.Areas.Admin.Controllers.Base;
    using EA.Weee.Web.Areas.Admin.ViewModels.RemoveAATFOrAeRecords;
    using EA.Weee.Web.Services;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Xunit;

    public class RemoveAATFRecordsControllerTests
    {
        private readonly IWeeeClient apiClient;
        private readonly BreadcrumbService breadcrumb;
        private readonly IMapper mapper;
        private readonly RemoveAatfOrAeRecordsController controller;
        private readonly ConfigurationService configurationService;

        public RemoveAATFRecordsControllerTests()
        {
            apiClient = A.Fake<IWeeeClient>();
            breadcrumb = A.Fake<BreadcrumbService>();
            mapper = A.Fake<IMapper>();
            configurationService = A.Fake<ConfigurationService>();
            controller = new RemoveAatfOrAeRecordsController(() => apiClient, breadcrumb, mapper, configurationService);
        }

        [Fact]
        public void RemoveAATFRecordsControllerController_ShouldInheritFromAdminBaseController()
        {
            typeof(AatfController).Should().BeDerivedFrom<AdminController>();
        }

        [Fact]
        public async Task GetIndex_RemoveAATFRecords()
        {
            // Arrange
            BreadcrumbService breadcrumb = A.Dummy<BreadcrumbService>();

            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetAatfOrAeRetentionPeriodComplianceYears>._))
                                    .Returns(new List<int> { 2018, 2017, 2016 });

            Func<IWeeeClient> weeeClientFunc = A.Fake<Func<IWeeeClient>>();
            A.CallTo(() => weeeClientFunc()).Returns(apiClient);

            // Act
            ActionResult result = await controller.Index(FacilityType.Aatf);

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);

            Assert.True(string.IsNullOrEmpty(viewResult.ViewName) || viewResult.ViewName.ToLowerInvariant() == "index");

            RemoveAatfOrAeViewModel resultsViewModel = viewResult.Model as RemoveAatfOrAeViewModel;
            Assert.NotNull(resultsViewModel);

            Assert.Null(resultsViewModel.SelectedComplianceYear);
            Assert.Equal(3, resultsViewModel.AatfStatuses.Count);
        }

        [Fact]
        public async Task PostIndex_RemoveAATFRecords()
        {
            // Arrange
            A.CallTo(() => apiClient.SendAsync(
                    A<string>._,
                    A<GetAatfOrAeRetentionPeriodComplianceYears>._))
                .Returns(new List<int> { 2018, 2017, 2016 });

            var model = new RemoveAatfOrAeViewModel
            {
                AatfStatuses = new List<Core.AatfReturn.AatfStatus>(),
                ComplianceYearList = new List<int> { 2018, 2017, 2016 }
            };

            // Act
            var result = await controller.Index(model, 1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewModel = Assert.IsType<RemoveAatfOrAeViewModel>(viewResult.Model);

            Assert.Equal(model.ComplianceYearList, viewModel.ComplianceYearList);
        }

        [Fact]
        public async Task GetConfirmDeletion_RemoveAATFRecords()
        {
            // Act
            ActionResult result = await controller.ConfirmDeletion(
                Guid.Empty,
                "Test",
                "WEE/TEST0004ZS/ATF",
                2018,
                "Approved",
                FacilityType.Aatf);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<RemoveAatfOrAeRecordConfirmViewModel>(viewResult.Model);

            Assert.Equal(Guid.Empty, model.AATFId);
            Assert.Equal("Test", model.Name);
            Assert.Equal("WEE/TEST0004ZS/ATF", model.ApprovalNumber);
            Assert.Equal(2018, model.ComplianceYear);
            Assert.Equal("Approved", model.Status);
        }

        [Fact]
        public async Task ConfirmDeletion_WhenDeletionSucceeds_RedirectsToDeleted()
        {
            // Arrange
            var model = new RemoveAatfOrAeRecordConfirmViewModel
            {
                AATFId = Guid.NewGuid()
            };

            A.CallTo(() => apiClient.SendAsync(
                    A<string>._,
                    A<DeleteAatfOrAeRecordById>._))
                .Returns(true);

            // Act
            var result = await controller.ConfirmDeletion(model);

            // Assert
            var redirectResult = Assert.IsType<RedirectToRouteResult>(result);

            Assert.Equal(
                "Deleted",
                redirectResult.RouteValues["action"]);
        }

        [Fact]
        public async Task ConfirmDeletion_WhenDeletionFails_RedirectsToDeleteFailure()
        {
            // Arrange
            var model = new RemoveAatfOrAeRecordConfirmViewModel
            {
                AATFId = Guid.NewGuid()
            };

            A.CallTo(() => apiClient.SendAsync(
                    A<string>._,
                    A<DeleteAatfOrAeRecordById>._))
                .Returns(false);

            // Act
            var result = await controller.ConfirmDeletion(model);

            // Assert
            var redirectResult = Assert.IsType<RedirectToRouteResult>(result);

            Assert.Equal(
                "DeleteFailure",
                redirectResult.RouteValues["action"]);
        }

        [Fact]
        public async Task ConfirmDeletion_SendsDeleteRequestWithCorrectAatfId()
        {
            // Arrange
            var aatfId = Guid.NewGuid();

            var model = new RemoveAatfOrAeRecordConfirmViewModel
            {
                AATFId = aatfId
            };

            A.CallTo(() => apiClient.SendAsync(
                    A<string>._,
                    A<DeleteAatfOrAeRecordById>._))
                .Returns(true);

            // Act
            await controller.ConfirmDeletion(model);

            // Assert
            A.CallTo(() => apiClient.SendAsync(
                    A<string>._,
                    A<DeleteAatfOrAeRecordById>.That.Matches(x => x.AatfId == aatfId)))
                .MustHaveHappenedOnceExactly();
        }
    }
}
