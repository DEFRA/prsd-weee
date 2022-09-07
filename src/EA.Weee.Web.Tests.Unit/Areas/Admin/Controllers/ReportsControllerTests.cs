namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using AutoFixture;
    using Core.Admin;
    using Core.Shared;
    using EA.Weee.Core.Admin.AatfReports;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core.Mediator;
    using Services;
    using Web.Areas.Admin.Controllers;
    using Web.Areas.Admin.ViewModels.AatfReports;
    using Web.Areas.Admin.ViewModels.Reports;
    using Web.Infrastructure;
    using Weee.Requests.Admin;
    using Weee.Requests.Admin.AatfReports;
    using Weee.Requests.Admin.GetActiveComplianceYears;
    using Weee.Requests.Shared;
    using Xunit;

    public class ReportsControllerTests
    {
        private readonly Fixture fixture;
        private readonly ReportsController controller;
        private readonly IWeeeClient weeeClient;
        private readonly BreadcrumbService breadcrumb;

        public ReportsControllerTests()
        {
            fixture = new Fixture();
            weeeClient = A.Fake<IWeeeClient>();
            breadcrumb = A.Fake<BreadcrumbService>();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<IRequest<UserStatus>>._)).Returns(UserStatus.Active);
            
            controller = new ReportsController(() => weeeClient, breadcrumb);
        }

        [Fact]
        public void ReportsController_ShouldInheritFromReportBaseController()
        {
            typeof(ReportsController).Should().BeDerivedFrom<ReportsBaseController>();
        }

        /// <summary>
        /// These tests ensure that the GET "Index" action prevents users who are inactive, pending or rejected
        /// from accessing the index action by redirecting them to the "InternalUserAuthorizationRequired"
        /// action of tyhe account controller.
        /// </summary>
        /// <param name="userStatus"></param>
        /// <returns></returns>
        [Theory]
        [InlineData(UserStatus.Inactive)]
        [InlineData(UserStatus.Pending)]
        [InlineData(UserStatus.Rejected)]
        public async Task GetIndex_WhenUserIsNotActive_RedirectsToInternalUserAuthorizationRequired(UserStatus userStatus)
        {
            // Arrange
            var client = A.Fake<IWeeeClient>();
            A.CallTo(() => client.SendAsync(A<string>._, A<IRequest<UserStatus>>._)).Returns(userStatus);

            var controller = new ReportsController(
                () => client,
                A.Dummy<BreadcrumbService>());

            // Act
            var result = await controller.Index();

            // Assert
            var redirectResult = result as RedirectToRouteResult;
            Assert.NotNull(redirectResult);

            Assert.Equal("InternalUserAuthorisationRequired", redirectResult.RouteValues["action"]);
            Assert.Equal("Account", redirectResult.RouteValues["controller"]);
            Assert.Equal(userStatus, redirectResult.RouteValues["userStatus"]);
        }

        /// <summary>
        /// This test ensures that the GET "Index" action will redirect an active user to the "ChooseReport" action.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetIndex_WhenUserIsActive_RedirectsToChooseReport()
        {
            // Arrange
            var client = A.Fake<IWeeeClient>();
            A.CallTo(() => client.SendAsync(A<string>._, A<IRequest<UserStatus>>._)).Returns(UserStatus.Active);

            var controller = new ReportsController(
                () => client,
                A.Dummy<BreadcrumbService>());

            // Act
            var result = await controller.Index();

            // Assert
            var redirectResult = result as RedirectToRouteResult;
            Assert.NotNull(redirectResult);

            Assert.Equal("ChooseReport", redirectResult.RouteValues["action"]);
            Assert.Equal("Reports", redirectResult.RouteValues["controller"]);
        }

        [Fact]
        public void GetChooseReport_Always_ReturnsChooseReportTypeViewWithValidModel()
        {
            var result = controller.ChooseReport();

            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);

            Assert.True(string.IsNullOrEmpty(viewResult.ViewName));

            var viewModel = viewResult.Model as ChooseReportTypeModel;
            Assert.NotNull(viewModel);
        }

        /// <summary>
        /// This test ensures that the POST "ChooseReport" action will return the "ChooseReport"
        /// view when an invalid model is provided.
        /// </summary>
        [Fact]
        public void PostChooseReport_ModelIsInvalid_ReturnsChooseReportView()
        {
            // Arrange
            var controller = new ReportsController(
                () => A.Dummy<IWeeeClient>(),
                A.Dummy<BreadcrumbService>());

            controller.ModelState.AddModelError("Key", "Any error");

            // Act
            var result = controller.ChooseReport(A.Dummy<ChooseReportTypeModel>());

            // Assert
            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);

            Assert.True(string.IsNullOrEmpty(viewResult.ViewName));
        }

        [Theory]
        [InlineData(Reports.PcsReports, "Index", "SchemeReports")]
        [InlineData(Reports.AatfReports, "Index", "AatfReports")]
        [InlineData(Reports.EvidenceNotesReports, "Index", "EvidenceReports")]
        public void PostChooseReport_WithSelectedValue_RedirectsToExpectedAction(string selectedValue, string expectedAction, string expectedController)
        {
            // Arrange
            var controller = new ReportsController(
               () => A.Dummy<IWeeeClient>(),
               A.Dummy<BreadcrumbService>());

            // Act
            var model = new ChooseReportTypeModel { SelectedValue = selectedValue };
            var result = controller.ChooseReport(model);

            // Assert
            var redirectResult = result as RedirectToRouteResult;
            Assert.NotNull(redirectResult);

            Assert.Equal(expectedAction, redirectResult.RouteValues["action"]);
            Assert.Equal(expectedController, redirectResult.RouteValues["controller"]);
        }

        /// <summary>
        /// This test ensures that the POST "ChooseReport" action will throw a
        /// NotSupportedException when the selected value is not one of the
        /// allowed values.
        /// </summary>
        [Fact]
        public void PostChooseReport_WithInvalidSelectedValue_ThrowsNotSupportedException()
        {
            // Arrange
            var controller = new ReportsController(
               () => A.Dummy<IWeeeClient>(),
               A.Dummy<BreadcrumbService>());

            // Act
            var model = new ChooseReportTypeModel { SelectedValue = "SOME INVALID VALUE" };
            Func<ActionResult> testCode = () => controller.ChooseReport(model);

            // Assert
            Assert.Throws<NotSupportedException>(testCode);
        }

        [Fact]
        public async Task GetAatfAeDetails_Always_ReturnsAatfAeDetailsViewModel()
        {
            var competentAuthorities = fixture.CreateMany<UKCompetentAuthorityData>().ToList();
            var panAreas = fixture.CreateMany<PanAreaData>().ToList();
            var localAreas = fixture.CreateMany<LocalAreaData>().ToList();
            var years = fixture.CreateMany<int>().ToList();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetAatfAeActiveComplianceYears>._)).Returns(years);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetPanAreas>._)).Returns(panAreas);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetLocalAreas>._)).Returns(localAreas);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetUKCompetentAuthorities>._)).Returns(competentAuthorities);

            // Act
            var result = await controller.AatfAeDetails();

            // Assert
            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.True(string.IsNullOrEmpty(viewResult.ViewName) || viewResult.ViewName == "AatfAeDetails");

            var model = viewResult.Model as AatfAeDetailsViewModel;
            Assert.NotNull(model);

            model.ComplianceYears.Select(c => c.Text).Should().BeEquivalentTo(years.Select(y => y.ToString()));
            model.PanAreaList.Select(c => c.Text).Should().BeEquivalentTo(panAreas.Select(y => y.Name.ToString()));
            model.PanAreaList.Select(c => c.Value).Should().BeEquivalentTo(panAreas.Select(y => y.Id.ToString()));
            model.CompetentAuthoritiesList.Select(c => c.Text).Should().BeEquivalentTo(competentAuthorities.Select(y => y.Abbreviation.ToString()));
            model.CompetentAuthoritiesList.Select(c => c.Value).Should().BeEquivalentTo(competentAuthorities.Select(y => y.Id.ToString()));

            Assert.Collection(model.FacilityTypes,
               s1 => Assert.Equal("AATF", s1.Text),
               s2 => Assert.Equal("AE", s2.Text),
               s3 => Assert.Equal("PCS", s3.Text));

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetAatfAeActiveComplianceYears>._)).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetUKCompetentAuthorities>._)).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetPanAreas>._)).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetLocalAreas>._)).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task GetAatfAeDetails_Always_SetsTriggerDownloadToFalse()
        {
            // Act
            var result = await controller.AatfAeDetails();

            // Assert
            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.Equal(false, viewResult.ViewBag.TriggerDownload);
        }

        [Fact]
        public async Task GetAatfAeDetails_Always_SetsInternalBreadcrumbToViewReports()
        {
            // Act
            await controller.AatfAeDetails();

            // Assert
            Assert.Equal("View reports", breadcrumb.InternalActivity);
        }

        [Fact]
        public async Task PostAatfAeDetails_WithInvalidViewModel_ReturnsAatfAeDetailsViewModel()
        {
            var competentAuthorities = fixture.CreateMany<UKCompetentAuthorityData>().ToList();
            var panAreas = fixture.CreateMany<PanAreaData>().ToList();
            var localAreas = fixture.CreateMany<LocalAreaData>().ToList();
            var years = fixture.CreateMany<int>().ToList();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetAatfAeActiveComplianceYears>._)).Returns(years);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetPanAreas>._)).Returns(panAreas);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetLocalAreas>._)).Returns(localAreas);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetUKCompetentAuthorities>._)).Returns(competentAuthorities);

            // Act
            controller.ModelState.AddModelError("Key", "Error");
            var result = await controller.AatfAeDetails(new AatfAeDetailsViewModel());

            // Assert
            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.True(string.IsNullOrEmpty(viewResult.ViewName) || viewResult.ViewName == "AatfAeDetails");

            var model = viewResult.Model as AatfAeDetailsViewModel;
            Assert.NotNull(model);

            model.ComplianceYears.Select(c => c.Text).Should().BeEquivalentTo(years.Select(y => y.ToString()));
            model.PanAreaList.Select(c => c.Text).Should().BeEquivalentTo(panAreas.Select(y => y.Name.ToString()));
            model.PanAreaList.Select(c => c.Value).Should().BeEquivalentTo(panAreas.Select(y => y.Id.ToString()));
            model.CompetentAuthoritiesList.Select(c => c.Text).Should().BeEquivalentTo(competentAuthorities.Select(y => y.Abbreviation.ToString()));
            model.CompetentAuthoritiesList.Select(c => c.Value).Should().BeEquivalentTo(competentAuthorities.Select(y => y.Id.ToString()));
        }

        [Fact]
        public async Task PostAatfAeDetails_WithInvalidViewModel_SetsTriggerDownloadToFalse()
        {
            // Act
            controller.ModelState.AddModelError("Key", "Error");
            var result = await controller.AatfAeDetails(new AatfAeDetailsViewModel());

            // Assert
            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.Equal(false, viewResult.ViewBag.TriggerDownload);
        }

        [Fact]
        public async Task PostAatfAeDetails_WithViewModel_SetsTriggerDownloadToTrue()
        {
            // Act
            var result = await controller.AatfAeDetails(new AatfAeDetailsViewModel());

            // Assert
            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.Equal(true, viewResult.ViewBag.TriggerDownload);
        }

        [Fact]
        public async Task PostAatfAeDetails_Always_SetsInternalBreadcrumbToViewReports()
        {
            // Act
            await controller.AatfAeDetails(A.Dummy<AatfAeDetailsViewModel>());

            // Assert
            Assert.Equal("View reports", breadcrumb.InternalActivity);
        }

        [Fact]
        public async Task GetDownloadAatfAeDetailsCsv_GivenActionParameters_CsvShouldBeReturned()
        {
            var complianceYear = fixture.Create<int>();
            var facilityType = fixture.Create<ReportFacilityType>();
            var authority = fixture.Create<Guid?>();
            var pat = fixture.Create<Guid?>();
            var area = fixture.Create<Guid?>();

            var fileData = fixture.Create<CSVFileData>();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetAatfAeDetailsCsv>.That.Matches(g =>
                g.ComplianceYear.Equals(complianceYear) && g.FacilityType.Equals(facilityType)
                                                        && g.AuthorityId.Equals(authority)))).Returns(fileData);

            var result = await controller.DownloadAatfAeDetailsCsv(complianceYear, facilityType, authority, pat, area) as FileContentResult;

            result.FileContents.Should().Contain(new UTF8Encoding().GetBytes(fileData.FileContent));
            result.FileDownloadName.Should().Be(CsvFilenameFormat.FormatFileName(fileData.FileName));
            result.ContentType.Should().Be("text/csv");
        }

        [Fact]
        public async Task GetPcsAatfDataDifference_Always_ReturnsPcsAatfDataDifferenceViewModel()
        {
            var years = fixture.CreateMany<int>().ToList();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetAatfReturnsActiveComplianceYears>._)).Returns(years);

            var result = await controller.PcsAatfDataDifference();

            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.True(string.IsNullOrEmpty(viewResult.ViewName) || viewResult.ViewName == "PcsAatfDataDifference");

            var model = viewResult.Model as PcsAatfDataDifferenceViewModel;
            Assert.NotNull(model);

            model.ComplianceYears.Select(c => c.Text).Should().BeEquivalentTo(years.Select(y => y.ToString()));

            Assert.Collection(model.Quarters,
               s1 => Assert.Equal("Q1", s1.Text),
               s2 => Assert.Equal("Q2", s2.Text),
               s3 => Assert.Equal("Q3", s3.Text),
               s4 => Assert.Equal("Q4", s4.Text));

            Assert.Collection(model.ObligationTypes,
                s1 => Assert.Equal("B2B", s1.Text),
                s2 => Assert.Equal("B2C", s2.Text));

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetAatfReturnsActiveComplianceYears>._)).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task GetPcsAatfDataDifference_Always_SetsTriggerDownloadToFalse()
        {
            var result = await controller.PcsAatfDataDifference();

            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.Equal(false, viewResult.ViewBag.TriggerDownload);
        }

        [Fact]
        public async Task GetPcsAatfDataDifference_Always_SetsInternalBreadcrumbToViewReports()
        {
            await controller.PcsAatfDataDifference();

            Assert.Equal("View reports", breadcrumb.InternalActivity);
        }

        [Fact]
        public async Task PostPcsAatfDataDifference_WithInvalidViewModel_ReturnsPcsAatfDataDifferenceViewModel()
        {
            var years = fixture.CreateMany<int>().ToList();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetAatfReturnsActiveComplianceYears>._)).Returns(years);

            controller.ModelState.AddModelError("Key", "Error");
            var result = await controller.PcsAatfDataDifference(new PcsAatfDataDifferenceViewModel());

            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.True(string.IsNullOrEmpty(viewResult.ViewName) || viewResult.ViewName == "PcsAatfDataDifference");

            var model = viewResult.Model as PcsAatfDataDifferenceViewModel;
            Assert.NotNull(model);

            model.ComplianceYears.Select(c => c.Text).Should().BeEquivalentTo(years.Select(y => y.ToString()));

            Assert.Collection(model.Quarters,
              s1 => Assert.Equal("Q1", s1.Text),
              s2 => Assert.Equal("Q2", s2.Text),
              s3 => Assert.Equal("Q3", s3.Text),
              s4 => Assert.Equal("Q4", s4.Text));

            Assert.Collection(model.ObligationTypes,
                s1 => Assert.Equal("B2B", s1.Text),
                s2 => Assert.Equal("B2C", s2.Text));
        }

        [Fact]
        public async Task PostPcsAatfDataDifference_WithInvalidViewModel_SetsTriggerDownloadToFalse()
        {
            controller.ModelState.AddModelError("Key", "Error");
            var result = await controller.PcsAatfDataDifference(new PcsAatfDataDifferenceViewModel());

            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.Equal(false, viewResult.ViewBag.TriggerDownload);
        }

        [Fact]
        public async Task PostPcsAatfDataDifference_WithViewModel_SetsTriggerDownloadToTrue()
        {
            var result = await controller.PcsAatfDataDifference(new PcsAatfDataDifferenceViewModel());

            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.Equal(true, viewResult.ViewBag.TriggerDownload);
        }

        [Fact]
        public async Task PostPcsAatfDataDifference_Always_SetsInternalBreadcrumbToViewReports()
        {
            await controller.PcsAatfDataDifference(A.Dummy<PcsAatfDataDifferenceViewModel>());

            Assert.Equal("View reports", breadcrumb.InternalActivity);
        }

        [Fact]
        public async Task GetDownloadPcsAatfDataDifferenceCsv_GivenActionParameters_CsvShouldBeReturned()
        {
            var complianceYear = fixture.Create<int>();
            var quarter = fixture.Create<int>();
            var obligation = fixture.Create<string>();

            var fileData = fixture.Create<CSVFileData>();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetPcsAatfComparisonData>.That.Matches(g =>
                g.ComplianceYear.Equals(complianceYear) && g.Quarter.Equals(quarter)
                && g.ObligationType.Equals(obligation)))).Returns(fileData);

            var result = await controller.DownloadPcsAatfDataDifference(complianceYear, quarter, obligation) as FileContentResult;

            result.FileContents.Should().Contain(new UTF8Encoding().GetBytes(fileData.FileContent));
            result.FileDownloadName.Should().Be(CsvFilenameFormat.FormatFileName(fileData.FileName));
            result.ContentType.Should().Be("text/csv");
        }
    }
}
