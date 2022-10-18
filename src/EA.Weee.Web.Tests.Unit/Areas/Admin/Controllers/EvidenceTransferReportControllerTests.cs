namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers
{
    using Api.Client;
    using AutoFixture;
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
    using Web.Infrastructure;
    using Weee.Requests.AatfEvidence.Reports;
    using Weee.Requests.Admin;
    using Weee.Requests.Shared;
    using Weee.Tests.Core;
    using Weee.Tests.Core.DataHelpers;
    using Xunit;

    public class EvidenceTransferReportControllerTests : SimpleUnitTestBase
    {
        private readonly EvidenceTransferReportController controller;
        private readonly IWeeeClient apiClient;
        private readonly BreadcrumbService breadcrumbService;
        private readonly ConfigurationService configurationService;
        private const string UserClaimIdentity = "claimIdentity";

        public EvidenceTransferReportControllerTests()
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
            
            controller = new EvidenceTransferReportController(() => apiClient, breadcrumbService, configurationService);

            SetUpControllerContext();
        }

        [Fact]
        public void EvidenceReportsController_ShouldDeriveFromReportsBaseController()
        {
            typeof(EvidenceTransferReportController).Should().BeDerivedFrom<ReportsBaseController>();
        }

        [Fact]
        public void EvidenceTransferNoteReportGet_ShouldHaveHttpGetAttribute()
        {
            typeof(EvidenceTransferReportController).GetMethod("EvidenceTransferNoteReport", Type.EmptyTypes).Should()
                .BeDecoratedWith<HttpGetAttribute>();
        }

        [Fact]
        public void DownloadEvidenceTransferNoteReportGet_ShouldHaveHttpGetAttribute()
        {
            typeof(EvidenceTransferReportController).GetMethod("DownloadEvidenceTransferNoteReport").Should()
                .BeDecoratedWith<HttpGetAttribute>();
        }

        [Fact]
        public void EvidenceTransferNoteReportPost_ShouldHaveHttpPostAttribute()
        {
            typeof(EvidenceTransferReportController).GetMethod("EvidenceTransferNoteReport", new[] { typeof(EvidenceTransfersReportViewModel) }).Should().BeDecoratedWith<HttpPostAttribute>();
        }

        [Fact]
        public void EvidenceTransferNoteReportPost_ShouldHaveValidateAntiforgeryAttribute()
        {
            typeof(EvidenceTransferReportController).GetMethod("EvidenceTransferNoteReport", new[] { typeof(EvidenceTransfersReportViewModel) }).Should().BeDecoratedWith<ValidateAntiForgeryTokenAttribute>();
        }

        [Fact]
        public async Task EvidenceTransferNoteReportGet_ShouldSetBreadCrumb()
        {
            //act
            await controller.EvidenceTransferNoteReport();

            //assert
            breadcrumbService.InternalActivity.Should().Be(InternalUserActivity.ViewReports);
        }

        [Fact]
        public async Task EvidenceTransferNoteReportGet_ShouldRetrieveUserStatus()
        {
            //act
            await controller.EvidenceTransferNoteReport();

            //assert
            A.CallTo(() => apiClient.SendAsync(A<string>._,
                    A<GetAdminUserStatus>.That.Matches(g => g.UserId.Equals(UserClaimIdentity))))
                .MustHaveHappenedOnceExactly();
        }

        [Theory]
        [ClassData(typeof(UserStatusCoreData))]
        public async Task EvidenceTransferNoteReportGet_GivenUserIsNotActive_ShouldRedirectToUnauthorisedView(UserStatus status)
        {
            if (status == UserStatus.Active)
            {
                return;
            }

            //arrange
            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetAdminUserStatus>._)).Returns(status);

            //act
            var result = await controller.EvidenceTransferNoteReport() as RedirectToRouteResult;

            //assert
            result.RouteValues["action"].Should().Be("InternalUserAuthorisationRequired");
            result.RouteValues["controller"].Should().Be("Account");
            result.RouteValues["userStatus"].Should().Be(status);
        }

        [Fact]
        public async Task EvidenceTransferNoteReportGet_GivenUserIsActive_ShouldRedirectReturnDefaultView()
        {
            //arrange
            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetAdminUserStatus>._)).Returns(UserStatus.Active);

            //act
            var result = await controller.EvidenceTransferNoteReport() as ViewResult;

            //assert
            result.ViewName.Should().BeEmpty();
        }

        [Fact]
        public async Task EvidenceTransferNoteReportGet_GivenUserIsActive_ShouldReturnEvidenceReportViewModel()
        {
            //arrange
            var configurationDate = new DateTime(2019, 1, 1);
            var apiDate = new DateTime(2021, 1, 1);

            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetAdminUserStatus>._)).Returns(UserStatus.Active);
            A.CallTo(() => configurationService.CurrentConfiguration.EvidenceNotesSiteSelectionDateFrom).Returns(configurationDate);
            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(apiDate);

            //act
            var result = await controller.EvidenceTransferNoteReport() as ViewResult;

            //assert
            var convertedModel = (EvidenceTransfersReportViewModel)result.Model;
            convertedModel.Should().NotBeNull();

            convertedModel.ComplianceYears.Count().Should().Be(3);
            convertedModel.ComplianceYears.ElementAt(0).Text.Should().Be("2021");
            convertedModel.ComplianceYears.ElementAt(1).Text.Should().Be("2020");
            convertedModel.ComplianceYears.ElementAt(2).Text.Should().Be("2019");
            convertedModel.SelectedYear.Should().Be(0);
        }

        [Fact]
        public async Task EvidenceTransferNoteReportGet_ViewBagTriggerDownloadShouldBeSetToFalse()
        {
            //arrange
            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetAdminUserStatus>._)).Returns(UserStatus.Active);

            //act
            await controller.EvidenceTransferNoteReport();

            //assert
            ((bool)controller.ViewBag.TriggerDownload).Should().BeFalse();
        }

        [Fact]
        public async Task EvidenceTransferNoteReportPost_GivenValidModel_ViewBadTriggerDownloadShouldBeSetToTrue()
        {
            //arrange
            var model = new EvidenceTransfersReportViewModel();

            //act
            await controller.EvidenceTransferNoteReport(model);

            //assert
            ((bool)controller.ViewBag.TriggerDownload).Should().BeTrue();
        }

        [Fact]
        public async Task EvidenceTransferNoteReportPost_GivenInValidModel_ViewBadTriggerDownloadShouldBeSetToFalse()
        {
            //arrange
            var model = new EvidenceTransfersReportViewModel();
            controller.ModelState.AddModelError("error", "error");

            //act
            await controller.EvidenceTransferNoteReport(model);

            //assert
            ((bool)controller.ViewBag.TriggerDownload).Should().BeFalse();
        }

        [Fact]
        public async Task EvidenceTransferNoteReportPost_ShouldRedirectReturnDefaultView()
        {
            //arrange
            var model = new EvidenceTransfersReportViewModel();

            //act
            var result = await controller.EvidenceTransferNoteReport(model) as ViewResult;

            //assert
            result.ViewName.Should().BeEmpty();
        }

        [Fact]
        public async Task EvidenceTransferNoteReportPost__ShouldReturnEvidenceReportViewModel()
        {
            //arrange
            var model = new EvidenceTransfersReportViewModel();
            var configurationDate = new DateTime(2019, 1, 1);
            var apiDate = new DateTime(2021, 1, 1);

            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetAdminUserStatus>._)).Returns(UserStatus.Active);
            A.CallTo(() => configurationService.CurrentConfiguration.EvidenceNotesSiteSelectionDateFrom).Returns(configurationDate);
            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(apiDate);

            //act
            var result = await controller.EvidenceTransferNoteReport(model) as ViewResult;

            //assert
            var convertedModel = (EvidenceTransfersReportViewModel)result.Model;
            convertedModel.Should().NotBeNull();

            convertedModel.ComplianceYears.Count().Should().Be(3);
            convertedModel.ComplianceYears.ElementAt(0).Text.Should().Be("2021");
            convertedModel.ComplianceYears.ElementAt(1).Text.Should().Be("2020");
            convertedModel.ComplianceYears.ElementAt(2).Text.Should().Be("2019");
            convertedModel.SelectedYear.Should().Be(0);
        }

        [Fact]
        public async Task EvidenceTransferNoteReportPost_ShouldSetBreadCrumb()
        {
            //arrange
            var model = new EvidenceTransfersReportViewModel();
         
            //act
            await controller.EvidenceTransferNoteReport(model);

            //assert
            breadcrumbService.InternalActivity.Should().Be(InternalUserActivity.ViewReports);
        }

        [Fact]
        public async Task DownloadEvidenceTransferNoteReportGet_ShouldRetrieveUserStatus()
        {
            //arrange
            var csvFile = TestFixture.Create<CSVFileData>();
            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetTransferNoteReportRequest>._)).Returns(csvFile);

            //act
            await controller.DownloadEvidenceTransferNoteReport(TestFixture.Create<int>());

            //assert
            A.CallTo(() => apiClient.SendAsync(A<string>._,
                    A<GetAdminUserStatus>.That.Matches(g => g.UserId.Equals(UserClaimIdentity))))
                .MustHaveHappenedOnceExactly();
        }

        [Theory]
        [ClassData(typeof(UserStatusCoreData))]
        public async Task DownloadEvidenceTransferNoteReportGet_GivenUserIsNotActive_ShouldRedirectToUnauthorisedView(UserStatus status)
        {
            if (status == UserStatus.Active)
            {
                return;
            }

            //arrange
            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetAdminUserStatus>._)).Returns(status);

            //act
            var result = await controller.DownloadEvidenceTransferNoteReport(TestFixture.Create<int>()) as RedirectToRouteResult;

            //assert
            result.RouteValues["action"].Should().Be("InternalUserAuthorisationRequired");
            result.RouteValues["controller"].Should().Be("Account");
            result.RouteValues["userStatus"].Should().Be(status);
        }

        [Fact]
        public async Task DownloadEvidenceTransferNoteReportGet_GivenRouteValues_ReportRequestShouldBeCalled()
        {
            //arrange
            var csvFile = TestFixture.Create<CSVFileData>();
            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetTransferNoteReportRequest>._)).Returns(csvFile);
            var complianceYear = TestFixture.Create<int>();

            //act
            await controller.DownloadEvidenceTransferNoteReport(complianceYear);

            //assert
            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetTransferNoteReportRequest>.That.Matches(g =>
                g.ComplianceYear == complianceYear &&
                g.OrganisationId == null))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task DownloadEvidenceTransferNoteReportGet_GivenCsvData_FileContentResultShouldBeReturned()
        {
            //arrange
            var csvFile = TestFixture.Create<CSVFileData>();
            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetTransferNoteReportRequest>._)).Returns(csvFile);

            //act
            var result = await controller.DownloadEvidenceTransferNoteReport(TestFixture.Create<int>()) as FileContentResult;

            //assert
            result.FileContents.Should().BeEquivalentTo(new UTF8Encoding().GetBytes(csvFile.FileContent));
            result.FileDownloadName.Should().Be(CsvFilenameFormat.FormatFileName(csvFile.FileName));
        }

        [Fact]
        public async Task DownloadEvidenceTransferNoteReportGet_ShouldSetBreadCrumb()
        {
            //arrange
            var csvFile = TestFixture.Create<CSVFileData>();
            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetTransferNoteReportRequest>._)).Returns(csvFile);

            //act
            await controller.DownloadEvidenceTransferNoteReport(TestFixture.Create<int>());

            //assert
            breadcrumbService.InternalActivity.Should().Be(InternalUserActivity.ViewReports);
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
