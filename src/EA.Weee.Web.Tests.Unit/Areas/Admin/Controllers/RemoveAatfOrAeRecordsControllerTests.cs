namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Requests.Admin.RemoveAATFOrAeRecords;
    using EA.Weee.Web.Areas.Admin.Controllers;
    using EA.Weee.Web.Areas.Admin.ViewModels.RemoveAATFOrAeRecords;
    using EA.Weee.Web.Services;
    using FakeItEasy;
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Xunit;

    public class RemoveAatfOrAeRecordsControllerTests
    {
        private readonly IWeeeClient apiClient;
        private readonly BreadcrumbService breadcrumb;
        private readonly IMapper mapper;
        private readonly ConfigurationService configurationService;
        private readonly RemoveAatfOrAeRecordsController controller;

        public RemoveAatfOrAeRecordsControllerTests()
        {
            apiClient = A.Fake<IWeeeClient>();
            breadcrumb = new BreadcrumbService();
            mapper = A.Fake<IMapper>();

            configurationService = CreateConfigurationService();

            controller = new RemoveAatfOrAeRecordsController(() => apiClient,
                                                                   breadcrumb,
                                                                   mapper,
                                                                   configurationService);

            SetupControllerContext();
        }

        private void SetupControllerContext()
        {
            var httpContext = A.Fake<HttpContextBase>();
            var request = A.Fake<HttpRequestBase>();
            var response = A.Fake<HttpResponseBase>();
            var session = A.Fake<HttpSessionStateBase>();

            var userId = Guid.NewGuid().ToString();

            var principal = new ClaimsPrincipal(
                new ClaimsIdentity(new[]
                                    {
                                        new Claim(ClaimTypes.NameIdentifier, userId),
                                        new Claim("access_token", "test-access-token")
                                    },
                                    "TestAuthentication"));

            A.CallTo(() => httpContext.Request).Returns(request);
            A.CallTo(() => httpContext.Response).Returns(response);
            A.CallTo(() => httpContext.Session).Returns(session);
            A.CallTo(() => httpContext.User).Returns(principal);

            controller.ControllerContext = new ControllerContext(httpContext, new RouteData(), controller);
        }

        private static ConfigurationService CreateConfigurationService()
        {
            return A.Fake<ConfigurationService>();
        }

        private RemoveAatfOrAeRecordConfirmViewModel CreateConfirmationModel()
        {
            return new RemoveAatfOrAeRecordConfirmViewModel
            {
                AATFId = Guid.NewGuid(),
                Name = "Test Facility",
                ApprovalNumber = "WEE123",
                ComplianceYear = 2023,
                Status = "Approved",
                FacilityType = FacilityType.Aatf
            };
        }

        [Fact]
        public async Task Index_Get_WhenFacilityTypeIsAatf_ReturnsCorrectViewModel()
        {
            // Arrange
            var complianceYears = new List<int> { 2022, 2023, 2024 };
            var data = new List<AatfDataList>();

            A.CallTo(() => apiClient.SendAsync(A<string>.Ignored, A<GetAatfOrAeRetentionPeriodComplianceYears>.Ignored))
                                    .Returns(Task.FromResult(complianceYears));

            A.CallTo(() => apiClient.SendAsync(A<string>.Ignored, A<GetAatfOrAeRetentionPeriodData>.Ignored))
                                    .Returns(Task.FromResult(data));

            // Act
            var result = await controller.Index(FacilityType.Aatf,
                                                1,
                                                "Test Facility",
                                                "WEE123",
                                                2023,
                                                1);

            // Assert
            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<RemoveAatfOrAeViewModel>(view.Model);

            Assert.Equal(FacilityType.Aatf, model.FacilityType);
            Assert.Equal("Test Facility", model.Name);
            Assert.Equal("WEE123", model.ApprovalNumber);
            Assert.Equal(2023, model.SelectedComplianceYear);
            Assert.Equal(1, model.SelectedAatfStatus);

            Assert.Equal(complianceYears, model.ComplianceYearList);
            Assert.Equal(configurationService.CurrentConfiguration.RetentionPeriod, model.RetentionPeriod);

            Assert.Equal("Remove AATF records", breadcrumb.InternalActivity);

            Assert.Equal(new[]
                               {
                                  AatfStatus.Approved,
                                  AatfStatus.Suspended,
                                  AatfStatus.Cancelled
                               },
                               model.AatfStatuses);
        }

        [Fact]
        public async Task Index_Get_WhenFacilityTypeIsAe_SetsAeBreadcrumb()
        {
            // Arrange
            A.CallTo(() => apiClient.SendAsync(A<string>.Ignored, A<GetAatfOrAeRetentionPeriodComplianceYears>.Ignored))
                                    .Returns(Task.FromResult(new List<int>()));

            A.CallTo(() => apiClient.SendAsync(A<string>.Ignored, A<GetAatfOrAeRetentionPeriodData>.Ignored))
                                    .Returns(Task.FromResult(new List<AatfDataList>()));

            // Act
            var result = await controller.Index(FacilityType.Ae);

            // Assert
            Assert.IsType<ViewResult>(result);
            Assert.Equal("Remove AE records", breadcrumb.InternalActivity);
        }

        [Fact]
        public async Task Index_Post_ReturnsViewWithSubmittedFilters()
        {
            // Arrange
            A.CallTo(() => apiClient.SendAsync(A<string>.Ignored, A<GetAatfOrAeRetentionPeriodData>.Ignored))
                                    .Returns(Task.FromResult(new List<AatfDataList>()));

            A.CallTo(() => apiClient.SendAsync(A<string>.Ignored, A<GetAatfOrAeRetentionPeriodComplianceYears>.Ignored))
                                    .Returns(Task.FromResult(new List<int> { 2022, 2023, 2024 }));

            var model = new RemoveAatfOrAeViewModel
            {
                FacilityType = FacilityType.Aatf,
                Name = "Facility A",
                ApprovalNumber = "WEE123",
                SelectedComplianceYear = 2023,
                SelectedAatfStatus = 2
            };

            // Act
            var result = await controller.Index(model, 2);

            // Assert
            var view = Assert.IsType<ViewResult>(result);
            var returnedModel = Assert.IsType<RemoveAatfOrAeViewModel>(view.Model);

            Assert.Equal("Facility A", returnedModel.Name);
            Assert.Equal("WEE123", returnedModel.ApprovalNumber);
            Assert.Equal(2023, returnedModel.SelectedComplianceYear);
            Assert.Equal(2, returnedModel.SelectedAatfStatus);

            Assert.Equal(FacilityType.Aatf, returnedModel.FacilityType);
            Assert.Equal(configurationService.CurrentConfiguration.RetentionPeriod, returnedModel.RetentionPeriod);

            Assert.Equal("Remove AATF records", breadcrumb.InternalActivity);
        }

        [Theory]
        [InlineData(FacilityType.Aatf, "Remove AATF records")]
        [InlineData(FacilityType.Ae, "Remove AE records")]
        public async Task ConfirmDeletion_Get_ReturnsConfirmationViewModel(FacilityType facilityType, string expectedBreadcrumb)
        {
            // Arrange
            var id = Guid.NewGuid();

            // Act
            var result = await controller.ConfirmDeletion(id,
                                                          "Test Facility",
                                                          "WEE123",
                                                          2023,
                                                          "Approved",
                                                          facilityType);

            // Assert
            var view = Assert.IsType<ViewResult>(result);

            var model =
                Assert.IsType<RemoveAatfOrAeRecordConfirmViewModel>(view.Model);

            Assert.Equal(id, model.AATFId);
            Assert.Equal("Test Facility", model.Name);
            Assert.Equal("WEE123", model.ApprovalNumber);
            Assert.Equal(2023, model.ComplianceYear);
            Assert.Equal("Approved", model.Status);
            Assert.Equal(facilityType, model.FacilityType);

            Assert.Equal(expectedBreadcrumb, breadcrumb.InternalActivity);
        }

        [Fact]
        public async Task ConfirmDeletion_Post_WhenApiReturnsTrue_RedirectsToDeleted()
        {
            // Arrange
            var model = CreateConfirmationModel();

            A.CallTo(() => apiClient.SendAsync(A<string>.Ignored, A<DeleteAatfOrAeRecordById>.Ignored))
                                    .Returns(Task.FromResult(true));

            // Act
            var result = await controller.ConfirmDeletion(model);

            // Assert
            var redirect = Assert.IsType<RedirectToRouteResult>(result);

            Assert.Equal("Deleted", redirect.RouteValues["action"]);
        }

        [Fact]
        public async Task ConfirmDeletion_Post_WhenApiReturnsFalse_RedirectsToDeleteFailure()
        {
            // Arrange
            var model = CreateConfirmationModel();

            A.CallTo(() => apiClient.SendAsync(A<string>.Ignored, A<DeleteAatfOrAeRecordById>.Ignored))
                                    .Returns(Task.FromResult(false));

            // Act
            var result = await controller.ConfirmDeletion(model);

            // Assert
            var redirect = Assert.IsType<RedirectToRouteResult>(result);

            Assert.Equal("DeleteFailure", redirect.RouteValues["action"]);
        }

        [Fact]
        public void Deleted_ReturnsViewWithProvidedModel()
        {
            // Arrange
            var model = CreateConfirmationModel();

            // Act
            var result = controller.Deleted(model);

            // Assert
            var view = Assert.IsType<ViewResult>(result);
            Assert.Same(model, view.Model);
        }

        [Fact]
        public void DeleteFailure_ReturnsViewWithProvidedModel()
        {
            // Arrange
            var model = CreateConfirmationModel();

            // Act
            var result = controller.DeleteFailure(model);

            // Assert
            var view = Assert.IsType<ViewResult>(result);
            Assert.Same(model, view.Model);
        }
    }
}
