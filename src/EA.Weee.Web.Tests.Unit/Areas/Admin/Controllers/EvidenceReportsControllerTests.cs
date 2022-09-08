namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers
{
    using Api.Client;
    using AutoFixture;
    using Core.AatfEvidence;
    using Core.Helpers;
    using Core.Shared;
    using FakeItEasy;
    using FluentAssertions;
    using IdentityModel;
    using Services;
    using System;
    using System.Linq;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Core.Admin;
    using Web.Areas.Admin.Controllers;
    using Web.Areas.Admin.ViewModels.EvidenceReports;
    using Web.Areas.Admin.ViewModels.Home;
    using Web.Areas.Admin.ViewModels.Reports;
    using Web.Infrastructure;
    using Weee.Requests.AatfEvidence.Reports;
    using Weee.Requests.Admin;
    using Weee.Requests.Shared;
    using Weee.Tests.Core;
    using Weee.Tests.Core.DataHelpers;
    using Xunit;

    public class EvidenceReportsControllerTests : SimpleUnitTestBase
    {
        private readonly EvidenceReportsController controller;
        private readonly IWeeeClient apiClient;
        private readonly BreadcrumbService breadcrumbService;
        private readonly ConfigurationService configurationService;
        private const string UserClaimIdentity = "claimIdentity";

        public EvidenceReportsControllerTests()
        {
            apiClient = A.Fake<IWeeeClient>();
            breadcrumbService = A.Fake<BreadcrumbService>();
            configurationService = A.Fake<ConfigurationService>();

            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetAdminUserStatus>._)).Returns(UserStatus.Active);
            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetEvidenceNoteReportRequest>._)).Returns(new CSVFileData()
            {
                FileContent = "content",
                FileName = "fileName"
            });
            
            controller = new EvidenceReportsController(() => apiClient, breadcrumbService, configurationService);

            SetUpControllerContext();
        }

        [Fact]
        public void EvidenceReportsController_ShouldDeriveFromReportsBaseController()
        {
            typeof(EvidenceReportsController).Should().BeDerivedFrom<ReportsBaseController>();
        }

        [Fact]
        public void EvidenceNoteReportGet_ShouldHaveHttpGetAttribute()
        {
            typeof(EvidenceReportsController).GetMethod("EvidenceNoteReport", Type.EmptyTypes).Should()
                .BeDecoratedWith<HttpGetAttribute>();
        }

        [Fact]
        public void ChooseReportGet_ShouldHaveHttpGetAttribute()
        {
            typeof(EvidenceReportsController).GetMethod("ChooseReport", Type.EmptyTypes).Should()
                .BeDecoratedWith<HttpGetAttribute>();
        }

        [Fact]
        public void DownloadEvidenceNoteReportGet_ShouldHaveHttpGetAttribute()
        {
            typeof(EvidenceReportsController).GetMethod("DownloadEvidenceNoteReport").Should()
                .BeDecoratedWith<HttpGetAttribute>();
        }

        [Fact]
        public void ChooseReportPost_ShouldHaveHttpPostAttribute()
        {
            typeof(EvidenceReportsController).GetMethod("ChooseReport", new[] { typeof(ChooseEvidenceReportViewModel) }).Should().BeDecoratedWith<HttpPostAttribute>();
        }

        [Fact]
        public void EvidenceNoteReportPost_ShouldHaveHttpPostAttribute()
        {
            typeof(EvidenceReportsController).GetMethod("EvidenceNoteReport", new[] { typeof(EvidenceReportViewModel) }).Should().BeDecoratedWith<HttpPostAttribute>();
        }

        [Fact]
        public void ChooseReportPost_ShouldHaveValidateAntiforgeryAttribute()
        {
            typeof(EvidenceReportsController).GetMethod("ChooseReport", new[] { typeof(ChooseEvidenceReportViewModel) }).Should().BeDecoratedWith<ValidateAntiForgeryTokenAttribute>();
        }

        [Fact]
        public void EvidenceNoteReportPost_ShouldHaveValidateAntiforgeryAttribute()
        {
            typeof(EvidenceReportsController).GetMethod("EvidenceNoteReport", new[] { typeof(EvidenceReportViewModel) }).Should().BeDecoratedWith<ValidateAntiForgeryTokenAttribute>();
        }

        [Fact]
        public async Task ChooseReportGet_ShouldSetBreadCrumb()
        {
            //act
            await controller.ChooseReport();

            //assert
            breadcrumbService.InternalActivity.Should().Be(InternalUserActivity.ViewReports);
        }

        [Fact]
        public async Task ChooseReportGet_ShouldRetrieveUserStatus()
        {
            //act
            await controller.ChooseReport();

            //assert
            A.CallTo(() => apiClient.SendAsync(A<string>._,
                    A<GetAdminUserStatus>.That.Matches(g => g.UserId.Equals(UserClaimIdentity))))
                .MustHaveHappenedOnceExactly();
        }

        [Theory]
        [ClassData(typeof(UserStatusCoreData))]
        public async Task ChooseReportGet_GivenUserIsNotActive_ShouldRedirectToUnauthorisedView(UserStatus status)
        {
            if (status == UserStatus.Active)
            {
                return;
            }

            //arrange
            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetAdminUserStatus>._)).Returns(status);

            //act
            var result = await controller.ChooseReport() as RedirectToRouteResult;

            //assert
            result.RouteValues["action"].Should().Be("InternalUserAuthorisationRequired");
            result.RouteValues["controller"].Should().Be("Account");
            result.RouteValues["userStatus"].Should().Be(status);
        }

        [Fact]
        public async Task ChooseReportGet_GivenUserIsActive_ShouldRedirectReturnDefaultView()
        {
            //arrange
            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetAdminUserStatus>._)).Returns(UserStatus.Active);

            //act
            var result = await controller.ChooseReport() as ViewResult;

            //assert
            result.ViewName.Should().BeEmpty();
        }

        [Fact]
        public async Task ChooseReportGet_GivenUserIsActive_ShouldReturnChooseEvidenceReportViewModel()
        {
            //arrange
            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetAdminUserStatus>._)).Returns(UserStatus.Active);

            //act
            var result = await controller.ChooseReport() as ViewResult;

            //assert
            result.Model.Should().BeOfType<ChooseEvidenceReportViewModel>();
        }

        [Fact]
        public void ChooseReportPost_GivenInvalidViewModel_DefaultViewShouldBeReturned()
        {
            //arrange
            var model = new ChooseEvidenceReportViewModel();
            controller.ModelState.AddModelError("error", "error");

            //act
            var result = controller.ChooseReport(model) as ViewResult;

            //assert
            result.ViewName.Should().BeEmpty();
        }

        [Fact]
        public void ChooseReportPost_GivenInvalidViewModel_ModelShouldBeReturned()
        {
            //arrange
            var model = new ChooseEvidenceReportViewModel();
            controller.ModelState.AddModelError("error", "error");

            //act
            var result = controller.ChooseReport(model) as ViewResult;

            //assert
            result.Model.Should().Be(model);
        }

        [Fact]
        public void ChooseReportPost_GivenViewModelAndEvidenceNoteReportSelected_ShouldRedirectToEvidenceNoteReport()
        {
            //arrange
            var model = new ChooseEvidenceReportViewModel()
            {
                SelectedValue = Reports.EvidenceNoteData
            };

            //act
            var result = controller.ChooseReport(model) as RedirectToRouteResult;

            //assert
            result.RouteValues["action"].Should().Be("EvidenceNoteReport");
            result.RouteValues["controller"].Should().Be("EvidenceReports");
        }

        [Fact]
        public void ChooseReportPost_GivenViewModelAndEvidenceTransfersReportSelected_ShouldRedirectToHolding()
        {
            //arrange
            var model = new ChooseEvidenceReportViewModel()
            {
                SelectedValue = Reports.EvidenceTransfersData
            };

            //act
            var result = controller.ChooseReport(model) as RedirectToRouteResult;

            //assert
            result.RouteValues["action"].Should().Be("Index");
            result.RouteValues["controller"].Should().Be("AdminHolding");
        }

        [Fact]
        public void ChooseReportPost_GivenInvalidSelectedValue_NotSupportedExceptionExcepted()
        {
            //arrange
            var model = new ChooseEvidenceReportViewModel()
            {
                SelectedValue = "invalid"
            };

            //act
            var exception = Record.Exception(() => controller.ChooseReport(model));

            //assert
            exception.Should().BeOfType<NotSupportedException>();
        }

        [Fact]
        public async Task EvidenceNoteReportGet_ShouldSetBreadCrumb()
        {
            //act
            await controller.EvidenceNoteReport();

            //assert
            breadcrumbService.InternalActivity.Should().Be(InternalUserActivity.ViewReports);
        }

        [Fact]
        public async Task EvidenceNoteReportGet_ShouldRetrieveUserStatus()
        {
            //act
            await controller.EvidenceNoteReport();

            //assert
            A.CallTo(() => apiClient.SendAsync(A<string>._,
                    A<GetAdminUserStatus>.That.Matches(g => g.UserId.Equals(UserClaimIdentity))))
                .MustHaveHappenedOnceExactly();
        }

        [Theory]
        [ClassData(typeof(UserStatusCoreData))]
        public async Task EvidenceNoteReportGet_GivenUserIsNotActive_ShouldRedirectToUnauthorisedView(UserStatus status)
        {
            if (status == UserStatus.Active)
            {
                return;
            }

            //arrange
            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetAdminUserStatus>._)).Returns(status);

            //act
            var result = await controller.EvidenceNoteReport() as RedirectToRouteResult;

            //assert
            result.RouteValues["action"].Should().Be("InternalUserAuthorisationRequired");
            result.RouteValues["controller"].Should().Be("Account");
            result.RouteValues["userStatus"].Should().Be(status);
        }

        [Fact]
        public async Task EvidenceNoteReportGet_GivenUserIsActive_ShouldRedirectReturnDefaultView()
        {
            //arrange
            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetAdminUserStatus>._)).Returns(UserStatus.Active);

            //act
            var result = await controller.EvidenceNoteReport() as ViewResult;

            //assert
            result.ViewName.Should().BeEmpty();
        }

        [Fact]
        public async Task EvidenceNoteReportGet_GivenUserIsActive_ShouldReturnEvidenceReportViewModel()
        {
            //arrange
            var configurationDate = new DateTime(2019, 1, 1);
            var apiDate = new DateTime(2021, 1, 1);

            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetAdminUserStatus>._)).Returns(UserStatus.Active);
            A.CallTo(() => configurationService.CurrentConfiguration.EvidenceNotesSiteSelectionDateFrom).Returns(configurationDate);
            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(apiDate);

            //act
            var result = await controller.EvidenceNoteReport() as ViewResult;

            //assert
            var convertedModel = (EvidenceReportViewModel)result.Model;
            convertedModel.Should().NotBeNull();

            convertedModel.ComplianceYears.Count().Should().Be(3);
            convertedModel.ComplianceYears.ElementAt(0).Text.Should().Be("2021");
            convertedModel.ComplianceYears.ElementAt(1).Text.Should().Be("2020");
            convertedModel.ComplianceYears.ElementAt(2).Text.Should().Be("2019");
            convertedModel.SelectedYear.Should().Be(0);

            convertedModel.SelectedTonnageToDisplay.Should().Be(0);
            convertedModel.TonnageToDisplayOptions.ElementAt(0).Text.Should()
                .Be(TonnageToDisplayReportEnum.OriginalTonnages.ToDisplayString());
        }

        [Fact]
        public async Task EvidenceNoteReportGet_ViewBagTriggerDownloadShouldBeSetToFalse()
        {
            //arrange
            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetAdminUserStatus>._)).Returns(UserStatus.Active);

            //act
            await controller.EvidenceNoteReport();

            //assert
            ((bool)controller.ViewBag.TriggerDownload).Should().BeFalse();
        }

        [Fact]
        public async Task EvidenceNoteReportPost_GivenValidModel_ViewBadTriggerDownloadShouldBeSetToTrue()
        {
            //arrange
            var model = new EvidenceReportViewModel();

            //act
            await controller.EvidenceNoteReport(model);

            //assert
            ((bool)controller.ViewBag.TriggerDownload).Should().BeTrue();
        }

        [Fact]
        public async Task EvidenceNoteReportPost_GivenInValidModel_ViewBadTriggerDownloadShouldBeSetToFalse()
        {
            //arrange
            var model = new EvidenceReportViewModel();
            controller.ModelState.AddModelError("error", "error");

            //act
            await controller.EvidenceNoteReport(model);

            //assert
            ((bool)controller.ViewBag.TriggerDownload).Should().BeFalse();
        }

        [Fact]
        public async Task EvidenceNoteReportPost_ShouldRedirectReturnDefaultView()
        {
            //arrange
            var model = new EvidenceReportViewModel();

            //act
            var result = await controller.EvidenceNoteReport(model) as ViewResult;

            //assert
            result.ViewName.Should().BeEmpty();
        }

        [Fact]
        public async Task EvidenceNoteReportPost__ShouldReturnEvidenceReportViewModel()
        {
            //arrange
            var model = new EvidenceReportViewModel();
            var configurationDate = new DateTime(2019, 1, 1);
            var apiDate = new DateTime(2021, 1, 1);

            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetAdminUserStatus>._)).Returns(UserStatus.Active);
            A.CallTo(() => configurationService.CurrentConfiguration.EvidenceNotesSiteSelectionDateFrom).Returns(configurationDate);
            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(apiDate);

            //act
            var result = await controller.EvidenceNoteReport(model) as ViewResult;

            //assert
            var convertedModel = (EvidenceReportViewModel)result.Model;
            convertedModel.Should().NotBeNull();

            convertedModel.ComplianceYears.Count().Should().Be(3);
            convertedModel.ComplianceYears.ElementAt(0).Text.Should().Be("2021");
            convertedModel.ComplianceYears.ElementAt(1).Text.Should().Be("2020");
            convertedModel.ComplianceYears.ElementAt(2).Text.Should().Be("2019");
            convertedModel.SelectedYear.Should().Be(0);

            convertedModel.SelectedTonnageToDisplay.Should().Be(0);
            convertedModel.TonnageToDisplayOptions.ElementAt(0).Text.Should()
                .Be(TonnageToDisplayReportEnum.OriginalTonnages.ToDisplayString());
        }

        [Fact]
        public async Task DownloadEvidenceNoteReportGet_ShouldRetrieveUserStatus()
        {
            //act
            await controller.DownloadEvidenceNoteReport(TestFixture.Create<int>(), TonnageToDisplayReportEnum.OriginalTonnages);

            //assert
            A.CallTo(() => apiClient.SendAsync(A<string>._,
                    A<GetAdminUserStatus>.That.Matches(g => g.UserId.Equals(UserClaimIdentity))))
                .MustHaveHappenedOnceExactly();
        }

        [Theory]
        [ClassData(typeof(UserStatusCoreData))]
        public async Task DownloadEvidenceNoteReportGet_GivenUserIsNotActive_ShouldRedirectToUnauthorisedView(UserStatus status)
        {
            if (status == UserStatus.Active)
            {
                return;
            }

            //arrange
            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetAdminUserStatus>._)).Returns(status);

            //act
            var result = await controller.DownloadEvidenceNoteReport(TestFixture.Create<int>(), TestFixture.Create<TonnageToDisplayReportEnum>()) as RedirectToRouteResult;

            //assert
            result.RouteValues["action"].Should().Be("InternalUserAuthorisationRequired");
            result.RouteValues["controller"].Should().Be("Account");
            result.RouteValues["userStatus"].Should().Be(status);
        }

        [Fact]
        public async Task DownloadEvidenceNoteReportGet_GivenRouteValues_ReportRequestShouldBeCalled()
        {
            //arrange
            var complianceYear = TestFixture.Create<int>();
            var tonnageToDisplay = TestFixture.Create<TonnageToDisplayReportEnum>();

            //act
            await controller.DownloadEvidenceNoteReport(complianceYear, tonnageToDisplay);

            //assert
            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetEvidenceNoteReportRequest>.That.Matches(g =>
                g.ComplianceYear == complianceYear &&
                g.OriginatorOrganisationId == null &&
                g.RecipientOrganisationId == null &&
                g.TonnageToDisplay == tonnageToDisplay))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task DownloadEvidenceNoteReportGet_GivenCsvData_FileContentResultShouldBeReturned()
        {
            //arrange
            var csvFile = TestFixture.Create<CSVFileData>();
            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetEvidenceNoteReportRequest>._)).Returns(csvFile);

            //act
            var result = await controller.DownloadEvidenceNoteReport(TestFixture.Create<int>(), TestFixture.Create<TonnageToDisplayReportEnum>()) as FileContentResult;

            //assert
            result.FileContents.Should().BeEquivalentTo(new UTF8Encoding().GetBytes(csvFile.FileContent));
            result.FileDownloadName.Should().Be(CsvFilenameFormat.FormatFileName(csvFile.FileName));
        }

        private void SetUpControllerContext()
        {
            var httpContextBase = A.Fake<HttpContextBase>();
            var principal = new ClaimsPrincipal(httpContextBase.User);
            var claimsIdentity = new ClaimsIdentity(httpContextBase.User.Identity);
            
            var weeeClaimsIdentity = new ClaimsIdentity(Constants.WeeeAuthType);
            weeeClaimsIdentity.AddClaim(new Claim(JwtClaimTypes.Subject, UserClaimIdentity));

            principal.AddIdentity(claimsIdentity);
            principal.AddIdentity(weeeClaimsIdentity);

            A.CallTo(() => httpContextBase.User).Returns(principal);

            var context = new ControllerContext(httpContextBase, new RouteData(), controller);
            controller.ControllerContext = context;
        }
    }
}
