namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers
{
    using Api.Client;
    using AutoFixture;
    using Core.Admin;
    using Core.Scheme;
    using Core.Shared;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core;
    using Prsd.Core.Mediator;
    using Services;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Web.Areas.Admin.Controllers;
    using Web.Areas.Admin.Controllers.Base;
    using Web.Areas.Admin.ViewModels.Reports;
    using Web.Areas.Admin.ViewModels.SchemeReports;
    using Web.Infrastructure;
    using Weee.Requests.AatfEvidence.Reports;
    using Weee.Requests.Admin;
    using Weee.Requests.Admin.AatfReports;
    using Weee.Requests.Admin.GetActiveComplianceYears;
    using Weee.Requests.Admin.Reports;
    using Weee.Requests.Scheme;
    using Weee.Requests.Shared;
    using Weee.Tests.Core;
    using Xunit;
    using GetSchemes = Weee.Requests.Admin.GetSchemes;

    public class SchemeReportsControllerTests : SimpleUnitTestBase
    {
        private readonly SchemeReportsController controller;
        private readonly IWeeeClient client;
        private readonly BreadcrumbService breadcrumbService;
        private readonly ConfigurationService configurationService;

        public SchemeReportsControllerTests()
        {
            client = A.Fake<IWeeeClient>();
            breadcrumbService = A.Fake<BreadcrumbService>();
            configurationService = A.Fake<ConfigurationService>();

            A.CallTo(() => client.SendAsync(A<string>._, A<IRequest<UserStatus>>._)).Returns(UserStatus.Active);

            controller = new SchemeReportsController(() => client, breadcrumbService, configurationService);
        }

        [Fact]
        public void SchemeReportsController_ShouldInheritFromReportBaseController()
        {
            typeof(SchemeReportsController).Should().BeDerivedFrom<ReportsBaseController>();
        }

        [Theory]
        [InlineData(UserStatus.Inactive)]
        [InlineData(UserStatus.Pending)]
        [InlineData(UserStatus.Rejected)]
        public async Task GetIndex_WhenUserIsNotActive_RedirectsToInternalUserAuthorizationRequired(UserStatus userStatus)
        {
            A.CallTo(() => client.SendAsync(A<string>._, A<IRequest<UserStatus>>._)).Returns(userStatus);

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
            var result = await controller.Index();

            var redirectResult = result as RedirectToRouteResult;
            Assert.NotNull(redirectResult);

            Assert.Equal("ChooseReport", redirectResult.RouteValues["action"]);
            Assert.Equal("SchemeReports", redirectResult.RouteValues["controller"]);
        }

        /// <summary>
        /// This test ensures that the GET "ChooseReport" action returns the "ChooseReport" view with
        /// a populated view model.
        /// </summary>
        [Fact]
        public void GetChooseReport_Always_ReturnsChooseReportView()
        {
            var result = controller.ChooseReport();

            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);

            Assert.True(string.IsNullOrEmpty(viewResult.ViewName));

            var viewModel = viewResult.Model as ChooseSchemeReportViewModel;
            Assert.NotNull(viewModel);
        }

        /// <summary>
        /// This test ensures that the POST "ChooseReport" action will return the "ChooseReport"
        /// view when an invalid model is provided.
        /// </summary>
        [Fact]
        public void PostChooseReport_ModelIsInvalid_ReturnsChooseReportView()
        {
            controller.ModelState.AddModelError("Key", "Any error");

            // Act
            var result = controller.ChooseReport(A.Dummy<ChooseSchemeReportViewModel>());

            // Assert
            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);

            Assert.True(string.IsNullOrEmpty(viewResult.ViewName));
        }

        /// <summary>
        /// This test ensures that the POST "ChooseReport" action will redirect to the
        /// action represented by each of the possible valid options.
        /// </summary>
        /// <param name="selectedValue"></param>
        /// <param name="expectedAction"></param>
        [Theory]
        [InlineData(Reports.ProducerDetails, "ProducerDetails")]
        [InlineData(Reports.ProducerPublicRegister, "ProducerPublicRegister")]
        [InlineData(Reports.ProducerEeeData, "ProducerEeeData")]
        [InlineData(Reports.SchemeWeeeData, "SchemeWeeeData")]
        [InlineData(Reports.UkEeeData, "UkEeeData")]
        [InlineData(Reports.SchemeObligationData, "SchemeObligationData")]
        [InlineData(Reports.UkWeeeData, "UkWeeeData")]
        [InlineData(Reports.PcsEvidenceAndObligationProgressData, "EvidenceAndObligationProgress")]
        public void PostChooseReport_WithSelectedValue_RedirectsToExpectedAction(string selectedValue, string expectedAction)
        {
            var model = new ChooseSchemeReportViewModel { SelectedValue = selectedValue };
            var result = controller.ChooseReport(model);

            var redirectResult = result as RedirectToRouteResult;
            Assert.NotNull(redirectResult);

            Assert.Equal(expectedAction, redirectResult.RouteValues["action"]);
            Assert.Equal("SchemeReports", redirectResult.RouteValues["controller"]);
        }

        [Fact]
        public void GetChooseReport_Always_ReturnsCorrectList()
        {
            var result = controller.ChooseReport();

            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);

            Assert.True(string.IsNullOrEmpty(viewResult.ViewName) || viewResult.ViewName.ToLowerInvariant() == "choosereport");

            var viewModel = viewResult.Model as ChooseSchemeReportViewModel;
            viewModel.Should().NotBeNull();
        }

        /// <summary>
        /// This test ensures that the POST "ChooseReport" action will throw a
        /// NotSupportedException when the selected value is not one of the
        /// allowed values.
        /// </summary>
        [Fact]
        public void PostChooseReport_WithInvalidSelectedValue_ThrowsNotSupportedException()
        {
            var model = new ChooseSchemeReportViewModel { SelectedValue = "SOME INVALID VALUE" };
            Func<ActionResult> testCode = () => controller.ChooseReport(model);

            Assert.Throws<NotSupportedException>(testCode);
        }

        /// <summary>
        /// This test ensures that the GET "ProducerDetails" action calls the API to populate
        /// the report filters and returns the "ProducerDetails" view.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetProducerDetails_Always_PopulatesFiltersAndReturnsProducerDetailsView()
        {
            var years = new List<int>() { 2001, 2002 };
            A.CallTo(() => client.SendAsync(A<string>._, A<GetMemberRegistrationsActiveComplianceYears>._)).Returns(years);

            var authority1 = new UKCompetentAuthorityData()
            {
                Id = new Guid("EB44DAD0-6B47-47C1-9124-4BE91042E563"),
                Abbreviation = "AA1"
            };
            var authorities = new List<UKCompetentAuthorityData>() { authority1 };
            A.CallTo(() => client.SendAsync(A<string>._, A<GetUKCompetentAuthorities>._)).Returns(authorities);

            var scheme1 = new SchemeData()
            {
                Id = new Guid("0F638399-226F-4942-AEF1-E6BC7EB447D6"),
                SchemeName = "Test Scheme"
            };
            var schemes = new List<SchemeData>() { scheme1 };
            A.CallTo(() => client.SendAsync(A<string>._, A<Weee.Requests.Admin.GetSchemes>._)).Returns(schemes);

            var result = await controller.ProducerDetails();

            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.True(string.IsNullOrEmpty(viewResult.ViewName));

            var model = viewResult.Model as ReportsFilterViewModel;
            Assert.NotNull(model);

            Assert.Collection(model.ComplianceYears,
                y1 => Assert.Equal("2001", y1.Text),
                y2 => Assert.Equal("2002", y2.Text));

            Assert.Collection(model.AppropriateAuthorities,
                a1 => { Assert.Equal("EB44DAD0-6B47-47C1-9124-4BE91042E563", a1.Value, true); Assert.Equal("AA1", a1.Text); });

            Assert.Collection(model.SchemeNames,
                s1 => { Assert.Equal("0F638399-226F-4942-AEF1-E6BC7EB447D6", s1.Value, true); Assert.Equal("Test Scheme", s1.Text); });
        }

        /// <summary>
        /// This test ensures that the GET "ProducerDetails" action returns
        /// a view with the ViewBag property "TriggerDownload" set to false.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetProducerDetails_Always_SetsTriggerDownloadToFalse()
        {
            var result = await controller.ProducerDetails();

            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.Equal(false, viewResult.ViewBag.TriggerDownload);
        }

        /// <summary>
        /// This test ensures that the GET "ProducerDetails" action sets
        /// the breadcrumb's internal activity to "View reports".
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetProducerDetails_Always_SetsInternalBreadcrumbToViewReports()
        {
            await controller.ProducerDetails();

            Assert.Equal("View reports", breadcrumbService.InternalActivity);
        }

        /// <summary>
        /// This test ensures that the POST "ProducerDetails" action with an invalid view model
        /// calls the API to populate the report filters and returns the "ProducerDetails" view.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task PostProducerDetails_WithInvalidViewModel_ReturnsSchemeWeeeDataProducerDataViewModel()
        {
            var years = new List<int>() { 2001, 2002 };
            A.CallTo(() => client.SendAsync(A<string>._, A<GetMemberRegistrationsActiveComplianceYears>._)).Returns(years);

            var authority1 = new UKCompetentAuthorityData()
            {
                Id = new Guid("EB44DAD0-6B47-47C1-9124-4BE91042E563"),
                Abbreviation = "AA1"
            };
            var authorities = new List<UKCompetentAuthorityData>() { authority1 };
            A.CallTo(() => client.SendAsync(A<string>._, A<GetUKCompetentAuthorities>._)).Returns(authorities);

            var scheme1 = new SchemeData()
            {
                Id = new Guid("0F638399-226F-4942-AEF1-E6BC7EB447D6"),
                SchemeName = "Test Scheme"
            };
            var schemes = new List<SchemeData>() { scheme1 };
            A.CallTo(() => client.SendAsync(A<string>._, A<Weee.Requests.Admin.GetSchemes>._)).Returns(schemes);

            controller.ModelState.AddModelError("Key", "Error");
            var result = await controller.ProducerDetails(A.Dummy<ReportsFilterViewModel>());

            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.True(string.IsNullOrEmpty(viewResult.ViewName));

            var model = viewResult.Model as ReportsFilterViewModel;
            Assert.NotNull(model);

            Assert.Collection(model.ComplianceYears,
                y1 => Assert.Equal("2001", y1.Text),
                y2 => Assert.Equal("2002", y2.Text));

            Assert.Collection(model.AppropriateAuthorities,
                a1 => { Assert.Equal("EB44DAD0-6B47-47C1-9124-4BE91042E563", a1.Value, true); Assert.Equal("AA1", a1.Text); });

            Assert.Collection(model.SchemeNames,
                s1 => { Assert.Equal("0F638399-226F-4942-AEF1-E6BC7EB447D6", s1.Value, true); Assert.Equal("Test Scheme", s1.Text); });
        }

        /// <summary>
        /// This test ensures that the POST "ProducerDetails" action with an invalid view model
        /// returns a view with the ViewBag property "TriggerDownload" set to false.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task PostProducerDetails_WithInvalidViewModel_SetsTriggerDownloadToFalse()
        {
            controller.ModelState.AddModelError("Key", "Error");
            var result = await controller.ProducerDetails(A.Dummy<ReportsFilterViewModel>());

            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.Equal(false, viewResult.ViewBag.TriggerDownload);
        }

        /// <summary>
        /// This test ensures that the POST "ProducerDetails" action with a valid view model
        /// returns a view with the ViewBag property "TriggerDownload" set to true.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task PostProducerDetails_WithViewModel_SetsTriggerDownloadToTrue()
        {
            var result = await controller.ProducerDetails(A.Dummy<ReportsFilterViewModel>());

            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.Equal(true, viewResult.ViewBag.TriggerDownload);
        }

        /// <summary>
        /// This test ensures that the POST "ProducerDetails" action sets
        /// the breadcrumb's internal activity to "View reports".
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task PostProducerDetails_Always_SetsInternalBreadcrumbToViewReports()
        {
            await controller.ProducerDetails(A.Dummy<ReportsFilterViewModel>());

            Assert.Equal("View reports", breadcrumbService.InternalActivity);
        }

        /// <summary>
        /// This test ensures that the GET "DownloadProducerDetailsCsv" action when called with no scheme ID and no
        /// authority ID will return a file with a name in the format "2015_producerdetails_31122016_2359" containing
        /// the specified compliance year and the current time.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetDownloadProducerDetailsCsv_WithNoSchemeIdAndNoAuthorityId_ReturnsFileNameWithComplianceYearAndCurrentTime()
        {
            var file = new CSVFileData() { FileContent = "Content" };
            A.CallTo(() => client.SendAsync(A<string>._, A<GetMemberDetailsCsv>._))
                .Returns(file);

            SystemTime.Freeze(new DateTime(2016, 12, 31, 23, 59, 58));
            var result = await controller.DownloadProducerDetailsCsv(2015, null, null, false, false);
            SystemTime.Unfreeze();

            var fileResult = result as FileResult;
            Assert.NotNull(fileResult);

            Assert.Equal("2015_producerdetails_31122016_2359.csv", fileResult.FileDownloadName);
        }

        /// <summary>
        /// This test ensures that the GET "DownloadProducerDetailsCsv" action when called with a scheme ID and no
        /// authority ID will return a file with a name in the format "2015_WEEAA1111AASCH_producerdetails_31122016_2359"
        /// containing the specified compliance year, the specified scheme's approval number and the current time.
        /// The forward slashes in the scheme approval number must be removed.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetDownloadProducerDetailsCsv_WithSchemeIdAndNoAuthorityId_ReturnsFileNameWithComplianceYearSchemeApprovalNumberAndCurrentTime()
        {
            var file = new CSVFileData() { FileContent = "Content" };
            A.CallTo(() => client.SendAsync(A<string>._, A<GetMemberDetailsCsv>._))
                .Returns(file);

            var schemeData = new SchemeData() { ApprovalName = "WEE/AA1111AA/SCH" };
            A.CallTo(() => client.SendAsync(A<string>._, A<GetSchemeById>._))
                .WhenArgumentsMatch(a => a.Get<GetSchemeById>("request").SchemeId == new Guid("88DF333E-6B2B-4D72-B411-7D7024EAA5F5"))
                .Returns(schemeData);

            SystemTime.Freeze(new DateTime(2016, 12, 31, 23, 59, 58));
            var result = await controller.DownloadProducerDetailsCsv(2015, new Guid("88DF333E-6B2B-4D72-B411-7D7024EAA5F5"), null, false, false);
            SystemTime.Unfreeze();

            var fileResult = result as FileResult;
            Assert.NotNull(fileResult);

            Assert.Equal("2015_WEEAA1111AASCH_producerdetails_31122016_2359.csv", fileResult.FileDownloadName);
        }

        /// <summary>
        /// This test ensures that the GET "DownloadProducerDetailsCsv" action when called with a scheme ID and an
        /// authority ID will return a file with a name in the format "2015_WEEAA1111AASCH_AA_producerdetails_31122016_2359"
        /// containing the specified compliance year, the specified scheme's approval number, the authority abbreviation and the current time.
        /// The forward slashes in the scheme approval number must be removed.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetDownloadProducerDetailsCsv_WithSchemeIdAndAuthorityId_ReturnsFileNameWithComplianceYearSchemeApprovalNumberAuthorityAbbreviationAndCurrentTime()
        {
            var file = new CSVFileData() { FileContent = "Content" };
            A.CallTo(() => client.SendAsync(A<string>._, A<GetMemberDetailsCsv>._))
                .Returns(file);

            var schemeData = new SchemeData() { ApprovalName = "WEE/AA1111AA/SCH" };
            A.CallTo(() => client.SendAsync(A<string>._, A<GetSchemeById>._))
                .WhenArgumentsMatch(a => a.Get<GetSchemeById>("request").SchemeId == new Guid("88DF333E-6B2B-4D72-B411-7D7024EAA5F5"))
                .Returns(schemeData);

            var authorityData = new UKCompetentAuthorityData() { Abbreviation = "AA" };
            A.CallTo(() => client.SendAsync(A<string>._, A<GetUKCompetentAuthorityById>._))
                .WhenArgumentsMatch(a => a.Get<GetUKCompetentAuthorityById>("request").Id == new Guid("703839B3-A081-4491-92B7-FCF969067EA3"))
                .Returns(authorityData);

            SystemTime.Freeze(new DateTime(2016, 12, 31, 23, 59, 58));
            var result = await controller.DownloadProducerDetailsCsv(
                2015,
                new Guid("88DF333E-6B2B-4D72-B411-7D7024EAA5F5"),
                new Guid("703839B3-A081-4491-92B7-FCF969067EA3"),
                false,
                false);
            SystemTime.Unfreeze();

            var fileResult = result as FileResult;
            Assert.NotNull(fileResult);

            Assert.Equal("2015_WEEAA1111AASCH_AA_producerdetails_31122016_2359.csv", fileResult.FileDownloadName);
        }

        /// <summary>
        /// This test ensures that the GET "DownloadProducerDetailsCsv" action when called without a scheme ID and with an
        /// authority ID will return a file with a name in the format "2015_AA_producerdetails_31122016_2359"
        /// containing the specified compliance year, the authority abbreviation and the current time.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetDownloadProducerDetailsCsv_WithNoSchemeIdAndWithAnAuthorityId_ReturnsFileNameWithComplianceYearAuthorityAbbreviationAndCurrentTime()
        {
            var file = new CSVFileData() { FileContent = "Content" };
            A.CallTo(() => client.SendAsync(A<string>._, A<GetMemberDetailsCsv>._))
                .Returns(file);

            var authorityData = new UKCompetentAuthorityData() { Abbreviation = "AA" };
            A.CallTo(() => client.SendAsync(A<string>._, A<GetUKCompetentAuthorityById>._))
                .WhenArgumentsMatch(a => a.Get<GetUKCompetentAuthorityById>("request").Id == new Guid("703839B3-A081-4491-92B7-FCF969067EA3"))
                .Returns(authorityData);

            SystemTime.Freeze(new DateTime(2016, 12, 31, 23, 59, 58));
            var result = await controller.DownloadProducerDetailsCsv(
                2015,
                null,
                new Guid("703839B3-A081-4491-92B7-FCF969067EA3"),
                false,
                false);
            SystemTime.Unfreeze();

            var fileResult = result as FileResult;
            Assert.NotNull(fileResult);

            Assert.Equal("2015_AA_producerdetails_31122016_2359.csv", fileResult.FileDownloadName);
        }

        /// <summary>
        /// This test ensures that the GET "DownloadProducerDetailsCsv" action will
        /// return a FileResult with a content type of "text/csv".
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetDownloadProducerDetailsCsv_Always_ReturnsFileResultWithContentTypeOfTextCsv()
        {
            var file = new CSVFileData() { FileContent = "Content" };
            A.CallTo(() => client.SendAsync(A<string>._, A<GetMemberDetailsCsv>._))
                .Returns(file);

            var result = await controller.DownloadProducerDetailsCsv(2015, null, null, false, false);

            var fileResult = result as FileResult;
            Assert.NotNull(fileResult);

            Assert.Equal("text/csv", fileResult.ContentType);
        }

        [Fact]
        public async void HttpGet_UkEeeData_ShouldReturnsUkEeeDataView()
        {
            A.CallTo(() => client.SendAsync(A<string>._, A<GetDataReturnsActiveComplianceYears>._))
                .Returns(new List<int> { 2015, 2016 });

            var result = await controller.UkEeeData();

            var viewResult = ((ViewResult)result);
            Assert.Equal("UkEeeData", viewResult.ViewName);

            A.CallTo(() => client.SendAsync(A<string>._, A<GetDataReturnsActiveComplianceYears>._))
                .MustHaveHappened();
        }

        [Fact]
        public async Task GetUKEeeData_Always_SetsTriggerDownloadToFalse()
        {
            var result = await controller.UkEeeData();

            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.Equal(false, viewResult.ViewBag.TriggerDownload);
        }

        [Fact]
        public async void HttpPost_UkEeeData_ModelIsInvalid_ShouldRedirectViewWithError()
        {
            controller.ModelState.AddModelError("Key", "Any error");

            var result = await controller.UkEeeData(new UkEeeDataViewModel());

            Assert.IsType<ViewResult>(result);
            Assert.False(controller.ModelState.IsValid);
        }

        [Fact]
        public async Task PostUkEeeData_ModelIsInvalid_SetsTriggerDownloadToFalse()
        {
            var viewModel = new UkEeeDataViewModel();

            controller.ModelState.AddModelError("Key", "Error");
            var result = await controller.UkEeeData(viewModel);

            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.Equal(false, viewResult.ViewBag.TriggerDownload);
        }

        [Fact]
        public async Task PostUKEeeData_ValidModel_SetsTriggerDownloadToTrue()
        {
            var viewModel = new UkEeeDataViewModel();

            var result = await controller.UkEeeData(viewModel);

            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.Equal(true, viewResult.ViewBag.TriggerDownload);
        }

        [Fact]
        public async Task GetDownloadUkEeeDataCsv_ReturnsFileResultWithContentTypeOfTextCsv()
        {
            A.CallTo(() => client.SendAsync(A<string>._, A<GetUkEeeDataCsv>._)).Returns(new CSVFileData
            {
                FileContent = "UK EEE DATA REPORT",
                FileName = "test.csv"
            });

            var result = await controller.DownloadUkEeeDataCsv(2015);

            var fileResult = result as FileResult;
            Assert.NotNull(fileResult);
            Assert.Equal("text/csv", fileResult.ContentType);
        }

        /// <summary>
        /// This test ensures that the GET "SchemeWeeeData" action calls the API to retrieve
        /// the list of compliance years and returns the "SchemeWeeeData" view with 
        /// a ProducerDataViewModel that has be populated with the list of years.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetSchemeWeeeData_Always_ReturnsSchemeWeeeDataProducerDataViewModel()
        {
            var years = new List<int>() { 2001, 2002 };
            A.CallTo(() => client.SendAsync(A<string>._, A<GetDataReturnsActiveComplianceYears>._)).Returns(years);

            var schemes = new List<SchemeData>
            {
                new SchemeData() {Id = new Guid("F0D0B242-7656-46FA-AF15-204E710E9850"), SchemeName = "Scheme 1"},
                new SchemeData() {Id = new Guid("2FE842AD-E122-4C40-9C39-EC183CFCD9F3"), SchemeName = "Scheme 2"}
            };
            A.CallTo(() => client.SendAsync(A<string>._, A<Weee.Requests.Admin.GetSchemes>._)).Returns(schemes);

            var result = await controller.SchemeWeeeData();
   
            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.True(string.IsNullOrEmpty(viewResult.ViewName));

            var model = viewResult.Model as ProducersDataViewModel;
            Assert.NotNull(model);

            Assert.Collection(model.ComplianceYears,
                y1 => Assert.Equal("2001", y1.Text),
                y2 => Assert.Equal("2002", y2.Text));

            Assert.Collection(model.Schemes,
                s1 => Assert.Equal("Scheme 1", s1.Text),
                s2 => Assert.Equal("Scheme 2", s2.Text));
        }

        /// <summary>
        /// This test ensures that the GET "SchemeWeeeData" action returns
        /// a view with the ViewBag property "TriggerDownload" set to false.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetSchemeWeeeData_Always_SetsTriggerDownloadToFalse()
        {
            var result = await controller.SchemeWeeeData();

            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.Equal(false, viewResult.ViewBag.TriggerDownload);
        }

        /// <summary>
        /// This test ensures that the GET "SchemeWeeeData" action sets
        /// the breadcrumb's internal activity to "View reports".
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetSchemeWeeeData_Always_SetsInternalBreadcrumbToViewReports()
        {
            await controller.SchemeWeeeData();

            // Assert
            Assert.Equal("View reports", breadcrumbService.InternalActivity);
        }

        /// <summary>
        /// This test ensures that the POST "SchemeWeeeData" action with an invalid view model
        /// calls the API to retrieve the list of compliance years and returns the "SchemeWeeeData"
        /// view with a ProducerDataViewModel that has be populated with the list of years.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task PostSchemeWeeeData_WithInvalidViewModel_ReturnsSchemeWeeeDataProducerDataViewModel()
        {
            var years = new List<int>() { 2001, 2002 };
            A.CallTo(() => client.SendAsync(A<string>._, A<GetDataReturnsActiveComplianceYears>._)).Returns(years);

            var schemes = new List<SchemeData>
            {
                new SchemeData() {Id = new Guid("F0D0B242-7656-46FA-AF15-204E710E9850"), SchemeName = "Scheme 1"},
                new SchemeData() {Id = new Guid("2FE842AD-E122-4C40-9C39-EC183CFCD9F3"), SchemeName = "Scheme 2"}
            };
            A.CallTo(() => client.SendAsync(A<string>._, A<Weee.Requests.Admin.GetSchemes>._)).Returns(schemes);

            var viewModel = new ProducersDataViewModel();

            controller.ModelState.AddModelError("Key", "Error");
            var result = await controller.SchemeWeeeData(viewModel);

            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.True(string.IsNullOrEmpty(viewResult.ViewName));

            var model = viewResult.Model as ProducersDataViewModel;
            Assert.NotNull(model);

            Assert.Collection(model.ComplianceYears,
                y1 => Assert.Equal("2001", y1.Text),
                y2 => Assert.Equal("2002", y2.Text));

            Assert.Collection(model.Schemes,
                s1 => Assert.Equal("Scheme 1", s1.Text),
                s2 => Assert.Equal("Scheme 2", s2.Text));
        }

        /// <summary>
        /// This test ensures that the POST "SchemeWeeeData" action with an invalid view model
        /// returns a view with the ViewBag property "TriggerDownload" set to false.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task PostSchemeWeeeData_WithInvalidViewModel_SetsTriggerDownloadToFalse()
        {
            var viewModel = new ProducersDataViewModel();

            controller.ModelState.AddModelError("Key", "Error");
            var result = await controller.SchemeWeeeData(viewModel);

            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.Equal(false, viewResult.ViewBag.TriggerDownload);
        }

        /// <summary>
        /// This test ensures that the POST "SchemeWeeeData" action with a valid view model
        /// returns a view with the ViewBag property "TriggerDownload" set to true.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task PostSchemeWeeeData_WithViewModel_SetsTriggerDownloadToTrue()
        {
            var viewModel = new ProducersDataViewModel();

            var result = await controller.SchemeWeeeData(viewModel);

            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.Equal(true, viewResult.ViewBag.TriggerDownload);
        }

        /// <summary>
        /// This test ensures that the POST "SchemeWeeeData" action sets
        /// the breadcrumb's internal activity to "View reports".
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task PostSchemeWeeeData_Always_SetsInternalBreadcrumbToViewReports()
        {
            await controller.SchemeWeeeData(A.Dummy<ProducersDataViewModel>());

            Assert.Equal("View reports", breadcrumbService.InternalActivity);
        }

        /// <summary>
        /// This test ensures that the GET "DownloadSchemeWeeeDataCsv" action will
        /// call the API to generate a CSV file which is returned with the correct file name.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetDownloadSchemeWeeeDataCsv_Always_CallsApiAndReturnsFileResultWithCorrectFileName()
        {
            // Arrange
            var file = new FileInfo("TEST FILE.csv", new byte[] { 1, 2, 3 });

            A.CallTo(() => client.SendAsync(A<string>._, A<GetSchemeWeeeCsv>._))
                .WhenArgumentsMatch(a =>
                    a.Get<GetSchemeWeeeCsv>("request").ComplianceYear == 2015 &&
                    a.Get<GetSchemeWeeeCsv>("request").SchemeId == new Guid("B6826CC7-5043-47D2-96C9-7B559BB7DEA1") &&
                    a.Get<GetSchemeWeeeCsv>("request").ObligationType == ObligationType.B2C)
                .Returns(file);

            var viewModel = new ProducersDataViewModel();

            var result = await controller.DownloadSchemeWeeeDataCsv(
                2015,
                new Guid("B6826CC7-5043-47D2-96C9-7B559BB7DEA1"),
                ObligationType.B2C);

            var fileResult = result as FileResult;
            Assert.NotNull(fileResult);

            Assert.Equal("TEST FILE.csv", fileResult.FileDownloadName);
        }

        /// <summary>
        /// This test ensures that the GET "DownloadSchemeWeeeDataCsv" action will
        /// returned with a file with a content type of "text/csv".
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetDownloadSchemeWeeeDataCsv_Always_ReturnsFileResultWithContentTypeOfTextCsv()
        {
            var result = await controller.DownloadSchemeWeeeDataCsv(
                A.Dummy<int>(),
                A.Dummy<Guid>(),
                A.Dummy<ObligationType>());

            var fileResult = result as FileResult;
            Assert.NotNull(fileResult);

            Assert.Equal("text/csv", fileResult.ContentType);
        }

        /// <summary>
        /// This test ensures that the GET "UKWeeeData" action calls the API to retrieve
        /// the list of compliance years and returns the "UKWeeeData" view with 
        /// a ProducerDataViewModel that has be populated with the list of years.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetUkWeeeData_Always_ReturnsUKWeeeDataProducerDataViewModel()
        {
            // Arrange
            var years = new List<int>() { 2001, 2002 };

            A.CallTo(() => client.SendAsync(A<string>._, A<GetDataReturnsActiveComplianceYears>._)).Returns(years);

            var result = await controller.UkWeeeData();

            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.True(string.IsNullOrEmpty(viewResult.ViewName));

            var model = viewResult.Model as ProducersDataViewModel;
            Assert.NotNull(model);
            Assert.Collection(model.ComplianceYears,
                y1 => Assert.Equal("2001", y1.Text),
                y2 => Assert.Equal("2002", y2.Text));
        }

        /// <summary>
        /// This test ensures that the GET "UKWeeeData" action returns
        /// a view with the ViewBag property "TriggerDownload" set to false.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetUkWeeeData_Always_SetsTriggerDownloadToFalse()
        {
            var result = await controller.UkWeeeData();

            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.Equal(false, viewResult.ViewBag.TriggerDownload);
        }

        /// <summary>
        /// This test ensures that the GET "UKWeeeData" action sets
        /// the breadcrumb's internal activity to "View reports".
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetUkWeeeData_Always_SetsInternalBreadcrumbToViewReports()
        {
            await controller.UkWeeeData();

            Assert.Equal("View reports", breadcrumbService.InternalActivity);
        }

        /// <summary>
        /// This test ensures that the POST "UkWeeeData" action with an invalid view model
        /// calls the API to retrieve the list of compliance years and returns the "UkWeeeData"
        /// view with a ProducerDataViewModel that has be populated with the list of years.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task PostUkWeeeData_WithInvalidViewModel_ReturnsUkWeeeDataProducerDataViewModel()
        {
            // Arrange
            var years = new List<int>() { 2001, 2002 };

            A.CallTo(() => client.SendAsync(A<string>._, A<GetDataReturnsActiveComplianceYears>._)).Returns(years);

            var viewModel = new ProducersDataViewModel();

            controller.ModelState.AddModelError("Key", "Error");
            var result = await controller.UkWeeeData(viewModel);

            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.True(string.IsNullOrEmpty(viewResult.ViewName));

            var model = viewResult.Model as ProducersDataViewModel;
            Assert.NotNull(model);
            Assert.Collection(model.ComplianceYears,
                y1 => Assert.Equal("2001", y1.Text),
                y2 => Assert.Equal("2002", y2.Text));
        }

        /// <summary>
        /// This test ensures that the POST "UkWeeeData" action with an invalid view model
        /// returns a view with the ViewBag property "TriggerDownload" set to false.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task PostUkWeeeData_WithInvalidViewModel_SetsTriggerDownloadToFalse()
        {
            var viewModel = new ProducersDataViewModel();

            controller.ModelState.AddModelError("Key", "Error");
            var result = await controller.UkWeeeData(viewModel);

            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.Equal(false, viewResult.ViewBag.TriggerDownload);
        }

        /// <summary>
        /// This test ensures that the POST "UkWeeeData" action with a valid view model
        /// returns a view with the ViewBag property "TriggerDownload" set to true.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task PostUkWeeeData_WithViewModel_SetsTriggerDownloadToTrue()
        {
            var viewModel = new ProducersDataViewModel();

            var result = await controller.UkWeeeData(viewModel);

            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.Equal(true, viewResult.ViewBag.TriggerDownload);
        }

        /// <summary>
        /// This test ensures that the POST "UkWeeeData" action sets
        /// the breadcrumb's internal activity to "View reports".
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task PostUkWeeeData_Always_SetsInternalBreadcrumbToViewReports()
        {
            await controller.UkWeeeData(A.Dummy<ProducersDataViewModel>());

            Assert.Equal("View reports", breadcrumbService.InternalActivity);
        }

        /// <summary>
        /// This test ensures that the GET "DownloadUkWeeeDataCsv" action will
        /// call the API to generate a CSV file which is returned with the correct file name.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetDownloadUkWeeeDataCsv_Always_CallsApiAndReturnsFileResultWithCorrectFileName()
        {
            // Arrange
            var file = new FileInfo("TEST FILE.csv", new byte[] { 1, 2, 3 });

            A.CallTo(() => client.SendAsync(A<string>._, A<GetUkWeeeCsv>._))
                .WhenArgumentsMatch(a => a.Get<GetUkWeeeCsv>("request").ComplianceYear == 2015)
                .Returns(file);

            var viewModel = new ProducersDataViewModel();

            var result = await controller.DownloadUkWeeeDataCsv(2015);

            var fileResult = result as FileResult;
            Assert.NotNull(fileResult);

            Assert.Equal("TEST FILE.csv", fileResult.FileDownloadName);
        }

        /// <summary>
        /// This test ensures that the GET "DownloadUkWeeeDataCsv" action will
        /// call the API to generate a CSV file which is returned with a content
        /// type of "text/csv"
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetDownloadUkWeeeDataCsv_Always_CallsApiAndReturnsFileResultWithContentTypeOfTextCsv()
        {
            A.CallTo(() => client.SendAsync(A<GetUkWeeeCsv>._))
                .WhenArgumentsMatch(a => a.Get<int>("complianceYear") == 2015)
                .Returns(A.Dummy<FileInfo>());

            var viewModel = new ProducersDataViewModel();

            // Act
            var result = await controller.DownloadUkWeeeDataCsv(2015);

            // Assert
            var fileResult = result as FileResult;
            Assert.NotNull(fileResult);

            Assert.Equal("text/csv", fileResult.ContentType);
        }

        /// <summary>
        /// This test ensures that the GET "SchemeObligationData" action returns the
        /// "SchemeObligationData" view with a ComplianceYearReportViewModel that has be populated
        /// with a list of years.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public void GetSchemeObligationData_Always_ReturnsComplianceYearReportViewModel()
        {
            var result = controller.SchemeObligationData();

            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.Equal("SchemeObligationData", viewResult.ViewName);

            var model = viewResult.Model as SchemeObligationDataViewModel;
            Assert.NotNull(model);

            A.CallTo(() => client.SendAsync(A<string>._, A<GetDataReturnsActiveComplianceYears>._))
                .MustNotHaveHappened();
            Assert.NotNull(model.ComplianceYears);
        }

        /// <summary>
        /// This test ensures that the GET "SchemeObligationData" action returns
        /// a view with the ViewBag property "TriggerDownload" set to false.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public void GetSchemeObligationData_Always_SetsTriggerDownloadToFalse()
        {
            var result = controller.SchemeObligationData();

            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.Equal(false, viewResult.ViewBag.TriggerDownload);
        }

        /// <summary>
        /// This test ensures that the GET "SchemeObligationData" action sets
        /// the breadcrumb's internal activity to "View reports".
        /// </summary>
        /// <returns></returns>
        [Fact]
        public void GetSchemeObligationData_Always_SetsInternalBreadcrumbToViewReports()
        {
            // Act
            controller.SchemeObligationData();

            // Assert
            Assert.Equal("View reports", breadcrumbService.InternalActivity);
        }

        /// <summary>
        /// This test ensures that the POST "SchemeObligationData" action returns the
        /// "SchemeObligationData" view with a ComplianceYearReportViewModel that has be populated
        /// with a list of years.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public void PostSchemeObligationData_WithInvalidViewModel_ReturnsUkWeeeDataProducerDataViewModel()
        {
            controller.ModelState.AddModelError("Key", "Error");
            var result = controller.SchemeObligationData(new SchemeObligationDataViewModel());

            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.True(string.IsNullOrEmpty(viewResult.ViewName));

            var model = viewResult.Model as SchemeObligationDataViewModel;
            Assert.NotNull(model);

            A.CallTo(() => client.SendAsync(A<string>._, A<GetDataReturnsActiveComplianceYears>._))
                .MustNotHaveHappened();
            Assert.NotNull(model.ComplianceYears);
        }

        /// <summary>
        /// This test ensures that the POST "SchemeObligationData" action with an invalid view model
        /// returns a view with the ViewBag property "TriggerDownload" set to false.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public void PostSchemeObligationData_WithInvalidViewModel_SetsTriggerDownloadToFalse()
        {
            controller.ModelState.AddModelError("Key", "Error");
            var result = controller.SchemeObligationData(new SchemeObligationDataViewModel());

            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.Equal(false, viewResult.ViewBag.TriggerDownload);
        }

        /// <summary>
        /// This test ensures that the POST "SchemeObligationData" action with a valid view model
        /// returns a view with the ViewBag property "TriggerDownload" set to true.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public void PostSchemeObligationData_WithViewModel_SetsTriggerDownloadToTrue()
        {
            var result = controller.SchemeObligationData(new SchemeObligationDataViewModel());

            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.Equal(true, viewResult.ViewBag.TriggerDownload);
        }

        /// <summary>
        /// This test ensures that the POST "SchemeObligationData" action sets
        /// the breadcrumb's internal activity to "View reports".
        /// </summary>
        /// <returns></returns>
        [Fact]
        public void PostSchemeObligationData_Always_SetsInternalBreadcrumbToViewReports()
        {
            controller.SchemeObligationData(A.Dummy<SchemeObligationDataViewModel>());

            Assert.Equal("View reports", breadcrumbService.InternalActivity);
        }

        /// <summary>
        /// This test ensures that the GET "MissingProducerData" action calls the API to retrieve
        /// the list of compliance years + schemes and returns the "MissingProducerData" view with 
        /// a MissingProducerDataViewModel that has be populated.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetMissingProducerData_Always_ReturnsMissingProducerDataViewModel()
        {
            // Arrange
            var years = new List<int>() { 2001, 2002 };

            A.CallTo(() => client.SendAsync(A<string>._, A<GetDataReturnsActiveComplianceYears>._)).Returns(years);

            var schemes = new List<SchemeData>
            {
                new SchemeData() {Id = new Guid("F0D0B242-7656-46FA-AF15-204E710E9850"), SchemeName = "Scheme 1"},
                new SchemeData() {Id = new Guid("2FE842AD-E122-4C40-9C39-EC183CFCD9F3"), SchemeName = "Scheme 2"}
            };
            A.CallTo(() => client.SendAsync(A<string>._, A<Weee.Requests.Admin.GetSchemes>._)).Returns(schemes);

            var result = await controller.MissingProducerData();

            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.Equal("MissingProducerData", viewResult.ViewName);

            var model = viewResult.Model as MissingProducerDataViewModel;
            Assert.NotNull(model);

            Assert.Collection(model.ComplianceYears,
                y1 => Assert.Equal("2001", y1.Text),
                y2 => Assert.Equal("2002", y2.Text));

            Assert.Collection(model.Schemes,
                s1 => Assert.Equal("Scheme 1", s1.Text),
                s2 => Assert.Equal("Scheme 2", s2.Text));
        }

        /// <summary>
        /// This test ensures that the GET "MissingProducerData" action returns
        /// a view with the ViewBag property "TriggerDownload" set to false.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetMissingProducerData_Always_SetsTriggerDownloadToFalse()
        {
            var result = await controller.MissingProducerData();

            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.Equal(false, viewResult.ViewBag.TriggerDownload);
        }

        /// <summary>
        /// This test ensures that the GET "MissingProducerData" action sets
        /// the breadcrumb's internal activity to "View reports".
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetMissingProducerData_Always_SetsInternalBreadcrumbToViewReports()
        {
            await controller.MissingProducerData();

            Assert.Equal("View reports", breadcrumbService.InternalActivity);
        }

        /// <summary>
        /// This test ensures that the POST "MissingProducerData" action with an invalid view model
        /// calls the API to retrieve the list of compliance years and returns the "MissingProducerData"
        /// view with a MissingProducerDataViewModel that has be populated with the list of years.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task PostMissingProducerData_WithInvalidViewModel_ReturnsMissingProducerDataViewModel()
        {
            // Arrange
            var years = new List<int>() { 2001, 2002 };

            A.CallTo(() => client.SendAsync(A<string>._, A<GetDataReturnsActiveComplianceYears>._)).Returns(years);

            var schemes = new List<SchemeData>
            {
                new SchemeData() {Id = new Guid("F0D0B242-7656-46FA-AF15-204E710E9850"), SchemeName = "Scheme 1"},
                new SchemeData() {Id = new Guid("2FE842AD-E122-4C40-9C39-EC183CFCD9F3"), SchemeName = "Scheme 2"}
            };
            A.CallTo(() => client.SendAsync(A<string>._, A<Weee.Requests.Admin.GetSchemes>._)).Returns(schemes);

            controller.ModelState.AddModelError("Key", "Error");
            var result = await controller.MissingProducerData(new MissingProducerDataViewModel());

            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.True(string.IsNullOrEmpty(viewResult.ViewName));

            var model = viewResult.Model as MissingProducerDataViewModel;
            Assert.NotNull(model);
            Assert.Collection(model.ComplianceYears,
                y1 => Assert.Equal("2001", y1.Text),
                y2 => Assert.Equal("2002", y2.Text));
        }

        /// <summary>
        /// This test ensures that the POST "MissingProducerData" action with an invalid view model
        /// returns a view with the ViewBag property "TriggerDownload" set to false.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task PostMissingProducerData_WithInvalidViewModel_SetsTriggerDownloadToFalse()
        {
            controller.ModelState.AddModelError("Key", "Error");
            var result = await controller.MissingProducerData(new MissingProducerDataViewModel());

            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.Equal(false, viewResult.ViewBag.TriggerDownload);
        }

        /// <summary>
        /// This test ensures that the POST "MissingProducerData" action with a valid view model
        /// returns a view with the ViewBag property "TriggerDownload" set to true.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task PostMissingProducerData_WithViewModel_SetsTriggerDownloadToTrue()
        {
            var result = await controller.MissingProducerData(new MissingProducerDataViewModel());

            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.Equal(true, viewResult.ViewBag.TriggerDownload);
        }

        /// <summary>
        /// This test ensures that the POST "MissingProducerData" action sets
        /// the breadcrumb's internal activity to "View reports".
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task PostMissingProducerData_Always_SetsInternalBreadcrumbToViewReports()
        {
            await controller.MissingProducerData(A.Dummy<MissingProducerDataViewModel>());

            Assert.Equal("View reports", breadcrumbService.InternalActivity);
        }

        [Fact]
        public void EvidenceAndObligationProgressGet_ShouldHaveHttpGetAttribute()
        {
            typeof(SchemeReportsController).GetMethod("EvidenceAndObligationProgress", types: new[] { typeof(int?) })
                .Should().BeDecoratedWith<HttpGetAttribute>();
        }

        [Fact]
        public async Task EvidenceAndObligationProgressGet_ShouldSetBreadCrumb()
        {
            //act
            await controller.EvidenceAndObligationProgress(TestFixture.Create<int?>());

            //assert
            breadcrumbService.InternalActivity.Should().Be("View reports");
        }

        [Fact]
        public async Task EvidenceAndObligationProgressGet_SystemDateShouldBeRetrieved()
        {
            //act
            await controller.EvidenceAndObligationProgress(TestFixture.Create<int?>());

            //assert
            A.CallTo(() => client.SendAsync(A<string>._, A<GetApiUtcDate>._)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EvidenceAndObligationProgressGet_GivenSelectedYear_SchemesShouldBeRetrieved()
        {
            //arrange
            var selectedYear = TestFixture.Create<int>();

            //act
            await controller.EvidenceAndObligationProgress(selectedYear);

            //assert

            A.CallTo(() => client.SendAsync(A<string>._, A<GetSchemesWithObligationOrEvidence>.That.Matches(g => g.ComplianceYear == selectedYear))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EvidenceAndObligationProgressGet_GivenNoSelectedYear_SchemesShouldNotBeRetrieved()
        {
            //act
            await controller.EvidenceAndObligationProgress((int?)null);

            //assert

            A.CallTo(() => client.SendAsync(A<string>._, A<GetSchemesWithObligationOrEvidence>._)).MustNotHaveHappened();
        }

        [Fact]
        public async Task EvidenceAndObligationProgressGet_GivenNoSelectedYear_ModelShouldBeBuilt()
        {
            //arrange
            var currentDate = new DateTime(2023, 1, 1);
            var evidenceStartDate = new DateTime(2020, 1, 1);

            A.CallTo(() => configurationService.CurrentConfiguration.EvidenceNotesSiteSelectionDateFrom)
                .Returns(evidenceStartDate);
            A.CallTo(() => client.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(currentDate);
            //act
            var result = await controller.EvidenceAndObligationProgress((int?)null) as ViewResult;

            //assert
            var convertedModel = result.Model as EvidenceAndObligationProgressViewModel;
            convertedModel.SelectedSchemeId.Should().BeNull();
            convertedModel.SelectedYear.Should().Be(0);
            convertedModel.ComplianceYears.Should().BeEquivalentTo(new SelectList(new List<int>() { 2023, 2022, 2021, 2020 }));
            convertedModel.Schemes.Should().BeEmpty();
        }

        [Fact]
        public async Task EvidenceAndObligationProgressGet_GivenSelectedYear_ModelShouldBeBuilt()
        {
            //arrange
            var currentDate = new DateTime(2023, 1, 1);
            var evidenceStartDate = new DateTime(2020, 1, 1);
            var schemeData = TestFixture.CreateMany<SchemeData>().ToList();
            const int selectedYear = 2023;

            A.CallTo(() => configurationService.CurrentConfiguration.EvidenceNotesSiteSelectionDateFrom)
                .Returns(evidenceStartDate);
            A.CallTo(() => client.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(currentDate);
            A.CallTo(() => client.SendAsync(A<string>._, A<GetSchemesWithObligationOrEvidence>._)).Returns(schemeData);

            //act
            var result = await controller.EvidenceAndObligationProgress(selectedYear) as ViewResult;

            //assert
            var convertedModel = result.Model as EvidenceAndObligationProgressViewModel;
            convertedModel.SelectedSchemeId.Should().BeNull();
            convertedModel.SelectedYear.Should().Be(selectedYear);
            convertedModel.ComplianceYears.Should().BeEquivalentTo(new SelectList(new List<int>() { 2023, 2022, 2021, 2020 }));
            convertedModel.Schemes.Should().BeEquivalentTo(new SelectList(schemeData, "Id", "SchemeName"));
        }

        [Fact]
        public async Task EvidenceAndObligationProgressGet_DefaultViewShouldBeReturned()
        {
            //arrange
            var result = await controller.EvidenceAndObligationProgress(TestFixture.Create<int?>()) as ViewResult;

            //assert
            result.ViewName.Should().BeNullOrWhiteSpace();
        }

        [Fact]
        public void EvidenceAndObligationProgressPost_ShouldHaveHttpPostAttribute()
        {
            typeof(SchemeReportsController).GetMethod("EvidenceAndObligationProgress", types: new[] { typeof(EvidenceAndObligationProgressViewModel) }).Should()
                .BeDecoratedWith<HttpPostAttribute>();
        }

        [Fact]
        public void EvidenceAndObligationProgressPost_ShouldHaveValidateAntiforgeryAttribute()
        {
            typeof(SchemeReportsController).GetMethod("EvidenceAndObligationProgress", types: new[] { typeof(EvidenceAndObligationProgressViewModel) }).Should()
                .BeDecoratedWith<ValidateAntiForgeryTokenAttribute>();
        }

        [Fact]
        public async Task EvidenceAndObligationProgressPost_ShouldSetBreadCrumb()
        {
            //arrange
            var model = TestFixture.Create<EvidenceAndObligationProgressViewModel>();
            var csv = TestFixture.Create<CSVFileData>();

            A.CallTo(() => client.SendAsync(A<string>._, A<GetSchemeObligationAndEvidenceTotalsReportRequest>._))
                .Returns(csv);

            //act
            await controller.EvidenceAndObligationProgress(model);

            //assert
            breadcrumbService.InternalActivity.Should().Be("View reports");
        }

        [Fact]
        public async Task EvidenceAndObligationProgressPost_GivenInvalidModel_SystemDateShouldBeRetrieved()
        {
            //arrange
            var model = TestFixture.Create<EvidenceAndObligationProgressViewModel>();
            controller.ModelState.AddModelError("error", "error");

            //act
            await controller.EvidenceAndObligationProgress(model);

            //assert
            A.CallTo(() => client.SendAsync(A<string>._, A<GetApiUtcDate>._)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EvidenceAndObligationProgressPost_GivenInvalidModel_SchemesShouldBeRetrieved()
        {
            //arrange
            var model = TestFixture.Create<EvidenceAndObligationProgressViewModel>();
            controller.ModelState.AddModelError("error", "error");

            //act
            await controller.EvidenceAndObligationProgress(model);

            //assert

            A.CallTo(() => client.SendAsync(A<string>._, A<GetSchemesWithObligationOrEvidence>
                .That.Matches(g => g.ComplianceYear == model.SelectedYear))).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task EvidenceAndObligationProgressPost_GivenModel_ModelShouldBeBuilt(bool valid)
        {
            //arrange
            var model = TestFixture.Create<EvidenceAndObligationProgressViewModel>();
            if (!valid)
            {
                controller.ModelState.AddModelError("error", "error");
            }
            
            var currentDate = new DateTime(2023, 1, 1);
            var evidenceStartDate = new DateTime(2020, 1, 1);
            var schemeData = TestFixture.CreateMany<SchemeData>().ToList();

            A.CallTo(() => configurationService.CurrentConfiguration.EvidenceNotesSiteSelectionDateFrom)
                .Returns(evidenceStartDate);
            A.CallTo(() => client.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(currentDate);
            A.CallTo(() => client.SendAsync(A<string>._, A<GetSchemesWithObligationOrEvidence>._)).Returns(schemeData);

            //act
            var result = await controller.EvidenceAndObligationProgress(model) as ViewResult;

            //assert
            var convertedModel = result.Model as EvidenceAndObligationProgressViewModel;
            convertedModel.SelectedSchemeId.Should().Be(model.SelectedSchemeId);
            convertedModel.SelectedYear.Should().Be(model.SelectedYear);
            convertedModel.ComplianceYears.Should().BeEquivalentTo(new SelectList(new List<int>() { 2023, 2022, 2021, 2020 }));
            convertedModel.Schemes.Should().BeEquivalentTo(new SelectList(schemeData, "Id", "SchemeName"));
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task EvidenceAndObligationProgressPost_GivenModel_DefaultViewShouldBeReturned(bool valid)
        {
            //arrange
            var model = TestFixture.Create<EvidenceAndObligationProgressViewModel>();
            if (!valid)
            {
                controller.ModelState.AddModelError("error", "error");
            }

            //act
            var result = await controller.EvidenceAndObligationProgress(model) as ViewResult;

            //assert
            result.ViewName.Should().BeNullOrWhiteSpace();
        }

        [Fact]
        public async Task EvidenceAndObligationProgressPost_GivenValidModel_ViewBagTriggerDownloadShouldBeTrue()
        {
            //arrange
            var model = TestFixture.Create<EvidenceAndObligationProgressViewModel>();

            //act
            var result = await controller.EvidenceAndObligationProgress(model) as ViewResult;

            //assert
            ((bool)result.ViewBag.TriggerDownload).Should().BeTrue();
        }

        [Fact]
        public async Task EvidenceAndObligationProgressPost_GivenInValidModel_ViewBagTriggerDownloadShouldBeFalse()
        {
            //arrange
            var model = TestFixture.Create<EvidenceAndObligationProgressViewModel>();
            controller.ModelState.AddModelError("error", "error");

            //act
            var result = await controller.EvidenceAndObligationProgress(model) as ViewResult;

            //assert
            ((bool)result.ViewBag.TriggerDownload).Should().BeFalse();
        }

        [Fact]
        public async Task EvidenceAndObligationProgressGet_ViewBagTriggerDownloadShouldBeFalse()
        {
            //act
            var result = await controller.EvidenceAndObligationProgress(TestFixture.Create<int?>()) as ViewResult;

            //assert
            ((bool)result.ViewBag.TriggerDownload).Should().BeFalse();
        }

        [Fact]
        public void DownloadEvidenceAndObligationProgressCsv_ShouldHaveHttpGetAttribute()
        {
            typeof(SchemeReportsController).GetMethod("DownloadEvidenceAndObligationProgressCsv", new Type[] {typeof(int), typeof(Guid?) }).Should()
                .BeDecoratedWith<HttpGetAttribute>();
        }

        [Fact]
        public async Task DownloadEvidenceAndObligationProgressCsv_GivenComplianceYearAndSchemeId_ReportShouldBeRetrieved()
        {
            //arrange
            var schemeId = TestFixture.Create<Guid?>();
            var complianceYear = TestFixture.Create<int>();
            A.CallTo(() => client.SendAsync(A<string>._,
                A<GetSchemeObligationAndEvidenceTotalsReportRequest>._)).Returns(TestFixture.Create<CSVFileData>());

            //act
            await controller.DownloadEvidenceAndObligationProgressCsv(complianceYear, schemeId);

            //assert
            A.CallTo(() => client.SendAsync(A<string>._,
                A<GetSchemeObligationAndEvidenceTotalsReportRequest>.That.Matches(g =>
                    g.ComplianceYear  == complianceYear &&
                    g.AppropriateAuthorityId == null &&
                    g.SchemeId == schemeId &&
                    g.OrganisationId == null))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task DownloadEvidenceAndObligationProgressCsv_GivenCsvData_FileResultShouldBeReturned()
        {
            //arrange
            var schemeId = TestFixture.Create<Guid?>();
            var complianceYear = TestFixture.Create<int>();
            var csvData = TestFixture.Create<CSVFileData>();

            A.CallTo(() => client.SendAsync(A<string>._,
                A<GetSchemeObligationAndEvidenceTotalsReportRequest>._)).Returns(csvData);

            //act
            var result = await controller.DownloadEvidenceAndObligationProgressCsv(complianceYear, schemeId) as FileContentResult;

            //assert
            result.FileContents.Should().BeEquivalentTo(new UTF8Encoding().GetBytes(csvData.FileContent));
            result.FileDownloadName.Should().BeEquivalentTo(CsvFilenameFormat.FormatFileName(csvData.FileName));
        }
    }
}
