﻿namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using Api.Client;
    using AutoFixture;
    using Core.Admin;
    using Core.Scheme;
    using Core.Shared;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Requests.Admin.Aatf;
    using EA.Weee.Web.Tests.Unit.TestHelpers;
    using FakeItEasy;
    using Prsd.Core;
    using Prsd.Core.Mediator;
    using Services;
    using Web.Areas.Admin.Controllers;
    using Web.Areas.Admin.ViewModels.Reports;
    using Weee.Requests.Admin;
    using Weee.Requests.Admin.GetActiveComplianceYears;
    using Weee.Requests.Admin.Reports;
    using Weee.Requests.Scheme;
    using Weee.Requests.Shared;
    using Xunit;

    public class ReportsControllerTests
    {
        private readonly Fixture fixture;

        public ReportsControllerTests()
        {
            fixture = new Fixture();
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
            IWeeeClient client = A.Fake<IWeeeClient>();
            A.CallTo(() => client.SendAsync(A<string>._, A<IRequest<UserStatus>>._)).Returns(userStatus);

            ReportsController controller = new ReportsController(
                () => client,
                A.Dummy<BreadcrumbService>());

            // Act
            ActionResult result = await controller.Index();

            // Assert
            RedirectToRouteResult redirectResult = result as RedirectToRouteResult;
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
            IWeeeClient client = A.Fake<IWeeeClient>();
            A.CallTo(() => client.SendAsync(A<string>._, A<IRequest<UserStatus>>._)).Returns(UserStatus.Active);

            ReportsController controller = new ReportsController(
                () => client,
                A.Dummy<BreadcrumbService>());

            // Act
            ActionResult result = await controller.Index();

            // Assert
            RedirectToRouteResult redirectResult = result as RedirectToRouteResult;
            Assert.NotNull(redirectResult);

            Assert.Equal("ChooseReport", redirectResult.RouteValues["action"]);
            Assert.Equal("Reports", redirectResult.RouteValues["controller"]);
        }

        /// <summary>
        /// This test ensures that the GET "ChooseReport" action returns the "ChooseReport" view with
        /// a populated view model.
        /// </summary>
        [Fact]
        public void GetChooseReport_Always_ReturnsChooseReportView()
        {
            // Arrange
            ReportsController controller = new ReportsController(
                () => A.Dummy<IWeeeClient>(),
                A.Dummy<BreadcrumbService>());

            // Act
            ActionResult result = controller.ChooseReport();

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);

            Assert.True(string.IsNullOrEmpty(viewResult.ViewName));

            ChooseReportViewModel viewModel = viewResult.Model as ChooseReportViewModel;
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
            ReportsController controller = new ReportsController(
                () => A.Dummy<IWeeeClient>(),
                A.Dummy<BreadcrumbService>());

            controller.ModelState.AddModelError("Key", "Any error");

            // Act
            ActionResult result = controller.ChooseReport(A.Dummy<ChooseReportViewModel>());

            // Assert
            ViewResult viewResult = result as ViewResult;
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
        [InlineData(Reports.UkWeeeData, "UkWeeeData")]
        [InlineData(Reports.ProducerEeeData, "ProducerEeeData")]
        [InlineData(Reports.SchemeWeeeData, "SchemeWeeeData")]
        [InlineData(Reports.UkEeeData, "UkEeeData")]
        [InlineData(Reports.SchemeObligationData, "SchemeObligationData")]
        [InlineData(Reports.AatfAeReturnData, "AatfAeReturnData")]
        [InlineData(Reports.AatfObligatedData, "AatfObligatedData")]
        public void PostChooseReport_WithSelectedValue_RedirectsToExpectedAction(string selectedValue, string expectedAction)
        {
            // Arrange
            ReportsController controller = new ReportsController(
               () => A.Dummy<IWeeeClient>(),
               A.Dummy<BreadcrumbService>());

            // Act
            ChooseReportViewModel model = new ChooseReportViewModel { SelectedValue = selectedValue };
            ActionResult result = controller.ChooseReport(model);

            // Assert
            RedirectToRouteResult redirectResult = result as RedirectToRouteResult;
            Assert.NotNull(redirectResult);

            Assert.Equal(expectedAction, redirectResult.RouteValues["action"]);
        }

        [Fact]
        public void GetChooseReport_Always_ReturnsCorrectList()
        {
            // Arrange
            ReportsController controller = new ReportsController(
                () => A.Dummy<IWeeeClient>(),
                A.Dummy<BreadcrumbService>());

            // Act
            ActionResult result = controller.ChooseReport();

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);

            Assert.True(string.IsNullOrEmpty(viewResult.ViewName) || viewResult.ViewName.ToLowerInvariant() == "choosereport");

            ChooseReportViewModel viewModel = viewResult.Model as ChooseReportViewModel;
            viewModel.PossibleValues.ElementAt(8).Contains(Reports.AatfAeReturnData);
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
            ReportsController controller = new ReportsController(
               () => A.Dummy<IWeeeClient>(),
               A.Dummy<BreadcrumbService>());

            // Act
            ChooseReportViewModel model = new ChooseReportViewModel { SelectedValue = "SOME INVALID VALUE" };
            Func<ActionResult> testCode = () => controller.ChooseReport(model);

            // Assert
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
            // Arrange
            IWeeeClient weeeClient = A.Fake<IWeeeClient>();

            List<int> years = new List<int>() { 2001, 2002 };
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetMemberRegistrationsActiveComplianceYears>._)).Returns(years);

            UKCompetentAuthorityData authority1 = new UKCompetentAuthorityData()
            {
                Id = new Guid("EB44DAD0-6B47-47C1-9124-4BE91042E563"),
                Abbreviation = "AA1"
            };
            List<UKCompetentAuthorityData> authorities = new List<UKCompetentAuthorityData>() { authority1 };
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetUKCompetentAuthorities>._)).Returns(authorities);

            SchemeData scheme1 = new SchemeData()
            {
                Id = new Guid("0F638399-226F-4942-AEF1-E6BC7EB447D6"),
                SchemeName = "Test Scheme"
            };
            List<SchemeData> schemes = new List<SchemeData>() { scheme1 };
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<Weee.Requests.Admin.GetSchemes>._)).Returns(schemes);

            ReportsController controller = new ReportsController(
                () => weeeClient,
                A.Dummy<BreadcrumbService>());

            // Act
            ActionResult result = await controller.ProducerDetails();

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.True(string.IsNullOrEmpty(viewResult.ViewName));

            ReportsFilterViewModel model = viewResult.Model as ReportsFilterViewModel;
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
            // Arrange
            ReportsController controller = new ReportsController(
                () => A.Dummy<IWeeeClient>(),
                A.Dummy<BreadcrumbService>());

            // Act
            ActionResult result = await controller.ProducerDetails();

            // Assert
            ViewResult viewResult = result as ViewResult;
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
            BreadcrumbService breadcrumb = new BreadcrumbService();

            // Arrange
            ReportsController controller = new ReportsController(
                () => A.Dummy<IWeeeClient>(),
                breadcrumb);

            // Act
            ActionResult result = await controller.ProducerDetails();

            // Assert
            Assert.Equal("View reports", breadcrumb.InternalActivity);
        }

        /// <summary>
        /// This test ensures that the POST "ProducerDetails" action with an invalid view model
        /// calls the API to populate the report filters and returns the "ProducerDetails" view.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task PostProducerDetails_WithInvalidViewModel_ReturnsSchemeWeeeDataProducerDataViewModel()
        {
            // Arrange
            IWeeeClient weeeClient = A.Fake<IWeeeClient>();

            List<int> years = new List<int>() { 2001, 2002 };
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetMemberRegistrationsActiveComplianceYears>._)).Returns(years);

            UKCompetentAuthorityData authority1 = new UKCompetentAuthorityData()
            {
                Id = new Guid("EB44DAD0-6B47-47C1-9124-4BE91042E563"),
                Abbreviation = "AA1"
            };
            List<UKCompetentAuthorityData> authorities = new List<UKCompetentAuthorityData>() { authority1 };
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetUKCompetentAuthorities>._)).Returns(authorities);

            SchemeData scheme1 = new SchemeData()
            {
                Id = new Guid("0F638399-226F-4942-AEF1-E6BC7EB447D6"),
                SchemeName = "Test Scheme"
            };
            List<SchemeData> schemes = new List<SchemeData>() { scheme1 };
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<Weee.Requests.Admin.GetSchemes>._)).Returns(schemes);

            ReportsController controller = new ReportsController(
                () => weeeClient,
                A.Dummy<BreadcrumbService>());

            // Act
            controller.ModelState.AddModelError("Key", "Error");
            ActionResult result = await controller.ProducerDetails(A.Dummy<ReportsFilterViewModel>());

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.True(string.IsNullOrEmpty(viewResult.ViewName));

            ReportsFilterViewModel model = viewResult.Model as ReportsFilterViewModel;
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
            // Arrange
            ReportsController controller = new ReportsController(
                () => A.Dummy<IWeeeClient>(),
                A.Dummy<BreadcrumbService>());

            ProducersDataViewModel viewModel = new ProducersDataViewModel();

            // Act
            controller.ModelState.AddModelError("Key", "Error");
            ActionResult result = await controller.ProducerDetails(A.Dummy<ReportsFilterViewModel>());

            // Assert
            ViewResult viewResult = result as ViewResult;
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
            // Arrange
            ReportsController controller = new ReportsController(
                () => A.Dummy<IWeeeClient>(),
                A.Dummy<BreadcrumbService>());

            // Act
            ActionResult result = await controller.ProducerDetails(A.Dummy<ReportsFilterViewModel>());

            // Assert
            ViewResult viewResult = result as ViewResult;
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
            BreadcrumbService breadcrumb = new BreadcrumbService();

            // Arrange
            ReportsController controller = new ReportsController(
                () => A.Dummy<IWeeeClient>(),
                breadcrumb);

            // Act
            ActionResult result = await controller.ProducerDetails(A.Dummy<ReportsFilterViewModel>());

            // Assert
            Assert.Equal("View reports", breadcrumb.InternalActivity);
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
            // Arrange
            IWeeeClient client = A.Fake<IWeeeClient>();

            CSVFileData file = new CSVFileData() { FileContent = "Content" };
            A.CallTo(() => client.SendAsync(A<string>._, A<GetMemberDetailsCsv>._))
                .Returns(file);

            ReportsController controller = new ReportsController(
                () => client,
                A.Dummy<BreadcrumbService>());

            // Act
            SystemTime.Freeze(new DateTime(2016, 12, 31, 23, 59, 58));
            ActionResult result = await controller.DownloadProducerDetailsCsv(2015, null, null, false, false);
            SystemTime.Unfreeze();

            // Assert
            FileResult fileResult = result as FileResult;
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
            // Arrange
            IWeeeClient client = A.Fake<IWeeeClient>();

            CSVFileData file = new CSVFileData() { FileContent = "Content" };
            A.CallTo(() => client.SendAsync(A<string>._, A<GetMemberDetailsCsv>._))
                .Returns(file);

            SchemeData schemeData = new SchemeData() { ApprovalName = "WEE/AA1111AA/SCH" };
            A.CallTo(() => client.SendAsync(A<string>._, A<GetSchemeById>._))
                .WhenArgumentsMatch(a => a.Get<GetSchemeById>("request").SchemeId == new Guid("88DF333E-6B2B-4D72-B411-7D7024EAA5F5"))
                .Returns(schemeData);

            ReportsController controller = new ReportsController(
                () => client,
                A.Dummy<BreadcrumbService>());

            // Act
            SystemTime.Freeze(new DateTime(2016, 12, 31, 23, 59, 58));
            ActionResult result = await controller.DownloadProducerDetailsCsv(2015, new Guid("88DF333E-6B2B-4D72-B411-7D7024EAA5F5"), null, false, false);
            SystemTime.Unfreeze();

            // Assert
            FileResult fileResult = result as FileResult;
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
            // Arrange
            IWeeeClient client = A.Fake<IWeeeClient>();

            CSVFileData file = new CSVFileData() { FileContent = "Content" };
            A.CallTo(() => client.SendAsync(A<string>._, A<GetMemberDetailsCsv>._))
                .Returns(file);

            SchemeData schemeData = new SchemeData() { ApprovalName = "WEE/AA1111AA/SCH" };
            A.CallTo(() => client.SendAsync(A<string>._, A<GetSchemeById>._))
                .WhenArgumentsMatch(a => a.Get<GetSchemeById>("request").SchemeId == new Guid("88DF333E-6B2B-4D72-B411-7D7024EAA5F5"))
                .Returns(schemeData);

            UKCompetentAuthorityData authorityData = new UKCompetentAuthorityData() { Abbreviation = "AA" };
            A.CallTo(() => client.SendAsync(A<string>._, A<GetUKCompetentAuthorityById>._))
                .WhenArgumentsMatch(a => a.Get<GetUKCompetentAuthorityById>("request").Id == new Guid("703839B3-A081-4491-92B7-FCF969067EA3"))
                .Returns(authorityData);

            ReportsController controller = new ReportsController(
                () => client,
                A.Dummy<BreadcrumbService>());

            // Act
            SystemTime.Freeze(new DateTime(2016, 12, 31, 23, 59, 58));
            ActionResult result = await controller.DownloadProducerDetailsCsv(
                2015,
                new Guid("88DF333E-6B2B-4D72-B411-7D7024EAA5F5"),
                new Guid("703839B3-A081-4491-92B7-FCF969067EA3"),
                false,
                false);
            SystemTime.Unfreeze();

            // Assert
            FileResult fileResult = result as FileResult;
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
            // Arrange
            IWeeeClient client = A.Fake<IWeeeClient>();

            CSVFileData file = new CSVFileData() { FileContent = "Content" };
            A.CallTo(() => client.SendAsync(A<string>._, A<GetMemberDetailsCsv>._))
                .Returns(file);

            UKCompetentAuthorityData authorityData = new UKCompetentAuthorityData() { Abbreviation = "AA" };
            A.CallTo(() => client.SendAsync(A<string>._, A<GetUKCompetentAuthorityById>._))
                .WhenArgumentsMatch(a => a.Get<GetUKCompetentAuthorityById>("request").Id == new Guid("703839B3-A081-4491-92B7-FCF969067EA3"))
                .Returns(authorityData);

            ReportsController controller = new ReportsController(
                () => client,
                A.Dummy<BreadcrumbService>());

            // Act
            SystemTime.Freeze(new DateTime(2016, 12, 31, 23, 59, 58));
            ActionResult result = await controller.DownloadProducerDetailsCsv(
                2015,
                null,
                new Guid("703839B3-A081-4491-92B7-FCF969067EA3"),
                false,
                false);
            SystemTime.Unfreeze();

            // Assert
            FileResult fileResult = result as FileResult;
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
            // Arrange
            IWeeeClient client = A.Fake<IWeeeClient>();

            CSVFileData file = new CSVFileData() { FileContent = "Content" };
            A.CallTo(() => client.SendAsync(A<string>._, A<GetMemberDetailsCsv>._))
                .Returns(file);

            ReportsController controller = new ReportsController(
                () => client,
                A.Dummy<BreadcrumbService>());

            // Act
            ActionResult result = await controller.DownloadProducerDetailsCsv(2015, null, null, false, false);

            // Assert
            FileResult fileResult = result as FileResult;
            Assert.NotNull(fileResult);

            Assert.Equal("text/csv", fileResult.ContentType);
        }

        [Fact]
        public async void HttpGet_UkEeeData_ShouldReturnsUkEeeDataView()
        {
            IWeeeClient client = A.Fake<IWeeeClient>();
            ReportsController controller = new ReportsController(
                () => client,
                A.Dummy<BreadcrumbService>());

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
            // Arrange
            ReportsController controller = new ReportsController(
                () => A.Dummy<IWeeeClient>(),
                A.Dummy<BreadcrumbService>());

            // Act
            ActionResult result = await controller.UkEeeData();

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.Equal(false, viewResult.ViewBag.TriggerDownload);
        }

        [Fact]
        public async void HttpPost_UkEeeData_ModelIsInvalid_ShouldRedirectViewWithError()
        {
            IWeeeClient client = A.Fake<IWeeeClient>();
            ReportsController controller = new ReportsController(
                () => client,
                A.Dummy<BreadcrumbService>());
            controller.ModelState.AddModelError("Key", "Any error");

            var result = await controller.UkEeeData(new UkEeeDataViewModel());

            Assert.IsType<ViewResult>(result);
            Assert.False(controller.ModelState.IsValid);
        }

        [Fact]
        public async Task PostUkEeeData_ModelIsInvalid_SetsTriggerDownloadToFalse()
        {
            // Arrange
            ReportsController controller = new ReportsController(
                () => A.Dummy<IWeeeClient>(),
                A.Dummy<BreadcrumbService>());

            UkEeeDataViewModel viewModel = new UkEeeDataViewModel();

            // Act
            controller.ModelState.AddModelError("Key", "Error");
            ActionResult result = await controller.UkEeeData(viewModel);

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.Equal(false, viewResult.ViewBag.TriggerDownload);
        }

        [Fact]
        public async Task PostUKEeeData_ValidModel_SetsTriggerDownloadToTrue()
        {
            // Arrange
            ReportsController controller = new ReportsController(
                () => A.Dummy<IWeeeClient>(),
                A.Dummy<BreadcrumbService>());

            UkEeeDataViewModel viewModel = new UkEeeDataViewModel();

            // Act
            ActionResult result = await controller.UkEeeData(viewModel);

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.Equal(true, viewResult.ViewBag.TriggerDownload);
        }

        [Fact]
        public async Task GetDownloadUkEeeDataCsv_ReturnsFileResultWithContentTypeOfTextCsv()
        {
            // Arrange
            IWeeeClient weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetUkEeeDataCsv>._)).Returns(new CSVFileData
            {
                FileContent = "UK EEE DATA REPORT",
                FileName = "test.csv"
            });
            
            ReportsController controller = new ReportsController(
                () => weeeClient,
                A.Dummy<BreadcrumbService>());

            // Act
            ActionResult result = await controller.DownloadUkEeeDataCsv(2015);

            // Assert
            FileResult fileResult = result as FileResult;
            Assert.NotNull(fileResult);
            Assert.Equal("text/csv", fileResult.ContentType);
        }

        [Fact]
        public async void Get_UkNonObligatedWeeeReceived_ShouldReturnsUkNonObligatedWeeeReceivedView()
        {
            IWeeeClient client = A.Fake<IWeeeClient>();
            ReportsController controller = new ReportsController(
                () => client,
                A.Dummy<BreadcrumbService>());

            A.CallTo(() => client.SendAsync(A<string>._, A<GetAatfReturnsActiveComplianceYears>._))
                .Returns(new List<int> { 2015, 2016 });

            ActionResult result = await controller.UkNonObligatedWeeeReceived();

            var viewResult = ((ViewResult)result);
            Assert.Equal("UkNonObligatedWeeeReceived", viewResult.ViewName);

            A.CallTo(() => client.SendAsync(A<string>._, A<GetAatfReturnsActiveComplianceYears>._))
                .MustHaveHappened();
        }

        [Fact]
        public async Task GetUkNonObligatedWeeeReceived_Always_SetsTriggerDownloadToFalse()
        {
            ReportsController controller = new ReportsController(
                () => A.Dummy<IWeeeClient>(),
                A.Dummy<BreadcrumbService>());

            ActionResult result = await controller.UkNonObligatedWeeeReceived();

            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.Equal(false, viewResult.ViewBag.TriggerDownload);
        }

        [Fact]
        public async void Post_UkNonObligatedWeeeReceived_ModelIsInvalid_ShouldRedirectViewWithError()
        {
            IWeeeClient client = A.Fake<IWeeeClient>();
            ReportsController controller = new ReportsController(
                () => client,
                A.Dummy<BreadcrumbService>());
            controller.ModelState.AddModelError("Key", "Any error");

            ActionResult result = await controller.UkNonObligatedWeeeReceived(new UkNonObligatedWeeeReceivedViewModel());

            Assert.IsType<ViewResult>(result);
            Assert.False(controller.ModelState.IsValid);
        }

        [Fact]
        public async Task PostUkNonObligatedWeeeReceived_ModelIsInvalid_SetsTriggerDownloadToFalse()
        {
            ReportsController controller = new ReportsController(
                () => A.Dummy<IWeeeClient>(),
                A.Dummy<BreadcrumbService>());

            UkNonObligatedWeeeReceivedViewModel viewModel = new UkNonObligatedWeeeReceivedViewModel();

            controller.ModelState.AddModelError("Key", "Error");
            ActionResult result = await controller.UkNonObligatedWeeeReceived(viewModel);

            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.Equal(false, viewResult.ViewBag.TriggerDownload);
        }

        [Fact]
        public async Task PostUkNonObligatedWeeeReceived_ValidModel_SetsTriggerDownloadToTrue()
        {
            ReportsController controller = new ReportsController(
                () => A.Dummy<IWeeeClient>(),
                A.Dummy<BreadcrumbService>());

            UkNonObligatedWeeeReceivedViewModel viewModel = new UkNonObligatedWeeeReceivedViewModel();

            ActionResult result = await controller.UkNonObligatedWeeeReceived(viewModel);

            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.Equal(true, viewResult.ViewBag.TriggerDownload);
        }
        
        [Fact]
        public async Task GetDownloadUkNonObligatedWeeeReceivedCsv_ReturnsFileResultWithContentTypeOfTextCsv()
        {
            IWeeeClient weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetUkNonObligatedWeeeReceivedDataCsv>._)).Returns(new CSVFileData
            {
                FileContent = "UK non-obligated WEEE REPORT",
                FileName = "test.csv"
            });

            ReportsController controller = new ReportsController(
                () => weeeClient,
                A.Dummy<BreadcrumbService>());

            ActionResult result = await controller.DownloadUkNonObligatedWeeeReceivedCsv(2015);

            FileResult fileResult = result as FileResult;
            Assert.NotNull(fileResult);
            Assert.Equal("text/csv", fileResult.ContentType);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetUkNonObligatedWeeeReceivedDataCsv>._)).MustHaveHappened(Repeated.Exactly.Once);
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
            // Arrange
            IWeeeClient weeeClient = A.Fake<IWeeeClient>();

            List<int> years = new List<int>() { 2001, 2002 };
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetDataReturnsActiveComplianceYears>._)).Returns(years);

            List<SchemeData> schemes = new List<SchemeData>
            {
                new SchemeData() {Id = new Guid("F0D0B242-7656-46FA-AF15-204E710E9850"), SchemeName = "Scheme 1"},
                new SchemeData() {Id = new Guid("2FE842AD-E122-4C40-9C39-EC183CFCD9F3"), SchemeName = "Scheme 2"}
            };
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<Weee.Requests.Admin.GetSchemes>._)).Returns(schemes);

            ReportsController controller = new ReportsController(
                () => weeeClient,
                A.Dummy<BreadcrumbService>());

            // Act
            ActionResult result = await controller.SchemeWeeeData();

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.True(string.IsNullOrEmpty(viewResult.ViewName));

            ProducersDataViewModel model = viewResult.Model as ProducersDataViewModel;
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
            // Arrange
            ReportsController controller = new ReportsController(
                () => A.Dummy<IWeeeClient>(),
                A.Dummy<BreadcrumbService>());

            // Act
            ActionResult result = await controller.SchemeWeeeData();

            // Assert
            ViewResult viewResult = result as ViewResult;
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
            BreadcrumbService breadcrumb = new BreadcrumbService();

            // Arrange
            ReportsController controller = new ReportsController(
                () => A.Dummy<IWeeeClient>(),
                breadcrumb);

            // Act
            ActionResult result = await controller.SchemeWeeeData();

            // Assert
            Assert.Equal("View reports", breadcrumb.InternalActivity);
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
            // Arrange
            IWeeeClient weeeClient = A.Fake<IWeeeClient>();

            List<int> years = new List<int>() { 2001, 2002 };
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetDataReturnsActiveComplianceYears>._)).Returns(years);

            List<SchemeData> schemes = new List<SchemeData>
            {
                new SchemeData() {Id = new Guid("F0D0B242-7656-46FA-AF15-204E710E9850"), SchemeName = "Scheme 1"},
                new SchemeData() {Id = new Guid("2FE842AD-E122-4C40-9C39-EC183CFCD9F3"), SchemeName = "Scheme 2"}
            };
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<Weee.Requests.Admin.GetSchemes>._)).Returns(schemes);

            ReportsController controller = new ReportsController(
                () => weeeClient,
                A.Dummy<BreadcrumbService>());

            ProducersDataViewModel viewModel = new ProducersDataViewModel();

            // Act
            controller.ModelState.AddModelError("Key", "Error");
            ActionResult result = await controller.SchemeWeeeData(viewModel);

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.True(string.IsNullOrEmpty(viewResult.ViewName));

            ProducersDataViewModel model = viewResult.Model as ProducersDataViewModel;
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
            // Arrange
            ReportsController controller = new ReportsController(
                () => A.Dummy<IWeeeClient>(),
                A.Dummy<BreadcrumbService>());

            ProducersDataViewModel viewModel = new ProducersDataViewModel();

            // Act
            controller.ModelState.AddModelError("Key", "Error");
            ActionResult result = await controller.SchemeWeeeData(viewModel);

            // Assert
            ViewResult viewResult = result as ViewResult;
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
            // Arrange
            ReportsController controller = new ReportsController(
                () => A.Dummy<IWeeeClient>(),
                A.Dummy<BreadcrumbService>());

            ProducersDataViewModel viewModel = new ProducersDataViewModel();

            // Act
            ActionResult result = await controller.SchemeWeeeData(viewModel);

            // Assert
            ViewResult viewResult = result as ViewResult;
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
            BreadcrumbService breadcrumb = new BreadcrumbService();

            // Arrange
            ReportsController controller = new ReportsController(
                () => A.Dummy<IWeeeClient>(),
                breadcrumb);

            // Act
            ActionResult result = await controller.SchemeWeeeData(A.Dummy<ProducersDataViewModel>());

            // Assert
            Assert.Equal("View reports", breadcrumb.InternalActivity);
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
            FileInfo file = new FileInfo("TEST FILE.csv", new byte[] { 1, 2, 3 });

            IWeeeClient weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSchemeWeeeCsv>._))
                .WhenArgumentsMatch(a =>
                    a.Get<GetSchemeWeeeCsv>("request").ComplianceYear == 2015 &&
                    a.Get<GetSchemeWeeeCsv>("request").SchemeId == new Guid("B6826CC7-5043-47D2-96C9-7B559BB7DEA1") &&
                    a.Get<GetSchemeWeeeCsv>("request").ObligationType == ObligationType.B2C)
                .Returns(file);

            ReportsController controller = new ReportsController(
                () => weeeClient,
                A.Dummy<BreadcrumbService>());

            ProducersDataViewModel viewModel = new ProducersDataViewModel();

            // Act
            ActionResult result = await controller.DownloadSchemeWeeeDataCsv(
                2015,
                new Guid("B6826CC7-5043-47D2-96C9-7B559BB7DEA1"),
                ObligationType.B2C);

            // Assert
            FileResult fileResult = result as FileResult;
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
            // Arrange
            ReportsController controller = new ReportsController(
                () => A.Fake<IWeeeClient>(),
                A.Dummy<BreadcrumbService>());

            ProducersDataViewModel viewModel = new ProducersDataViewModel();

            // Act
            ActionResult result = await controller.DownloadSchemeWeeeDataCsv(
                A.Dummy<int>(),
                A.Dummy<Guid>(),
                A.Dummy<ObligationType>());

            // Assert
            FileResult fileResult = result as FileResult;
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
            List<int> years = new List<int>() { 2001, 2002 };

            IWeeeClient weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetDataReturnsActiveComplianceYears>._)).Returns(years);

            ReportsController controller = new ReportsController(
                () => weeeClient,
                A.Dummy<BreadcrumbService>());

            // Act
            ActionResult result = await controller.UkWeeeData();

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.True(string.IsNullOrEmpty(viewResult.ViewName));

            ProducersDataViewModel model = viewResult.Model as ProducersDataViewModel;
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
            // Arrange
            ReportsController controller = new ReportsController(
                () => A.Dummy<IWeeeClient>(),
                A.Dummy<BreadcrumbService>());

            // Act
            ActionResult result = await controller.UkWeeeData();

            // Assert
            ViewResult viewResult = result as ViewResult;
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
            BreadcrumbService breadcrumb = new BreadcrumbService();

            // Arrange
            ReportsController controller = new ReportsController(
                () => A.Dummy<IWeeeClient>(),
                breadcrumb);

            // Act
            ActionResult result = await controller.UkWeeeData();

            // Assert
            Assert.Equal("View reports", breadcrumb.InternalActivity);
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
            List<int> years = new List<int>() { 2001, 2002 };

            IWeeeClient weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetDataReturnsActiveComplianceYears>._)).Returns(years);

            ReportsController controller = new ReportsController(
                () => weeeClient,
                A.Dummy<BreadcrumbService>());

            ProducersDataViewModel viewModel = new ProducersDataViewModel();

            // Act
            controller.ModelState.AddModelError("Key", "Error");
            ActionResult result = await controller.UkWeeeData(viewModel);

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.True(string.IsNullOrEmpty(viewResult.ViewName));

            ProducersDataViewModel model = viewResult.Model as ProducersDataViewModel;
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
            // Arrange
            ReportsController controller = new ReportsController(
                () => A.Dummy<IWeeeClient>(),
                A.Dummy<BreadcrumbService>());

            ProducersDataViewModel viewModel = new ProducersDataViewModel();

            // Act
            controller.ModelState.AddModelError("Key", "Error");
            ActionResult result = await controller.UkWeeeData(viewModel);

            // Assert
            ViewResult viewResult = result as ViewResult;
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
            // Arrange
            ReportsController controller = new ReportsController(
                () => A.Dummy<IWeeeClient>(),
                A.Dummy<BreadcrumbService>());

            ProducersDataViewModel viewModel = new ProducersDataViewModel();

            // Act
            ActionResult result = await controller.UkWeeeData(viewModel);

            // Assert
            ViewResult viewResult = result as ViewResult;
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
            BreadcrumbService breadcrumb = new BreadcrumbService();

            // Arrange
            ReportsController controller = new ReportsController(
                () => A.Dummy<IWeeeClient>(),
                breadcrumb);

            // Act
            ActionResult result = await controller.UkWeeeData(A.Dummy<ProducersDataViewModel>());

            // Assert
            Assert.Equal("View reports", breadcrumb.InternalActivity);
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
            FileInfo file = new FileInfo("TEST FILE.csv", new byte[] { 1, 2, 3 });

            IWeeeClient weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetUkWeeeCsv>._))
                .WhenArgumentsMatch(a => a.Get<GetUkWeeeCsv>("request").ComplianceYear == 2015)
                .Returns(file);

            ReportsController controller = new ReportsController(
                () => weeeClient,
                A.Dummy<BreadcrumbService>());

            ProducersDataViewModel viewModel = new ProducersDataViewModel();

            // Act
            ActionResult result = await controller.DownloadUkWeeeDataCsv(2015);

            // Assert
            FileResult fileResult = result as FileResult;
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
            // Arrange
            IWeeeClient weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<GetUkWeeeCsv>._))
                .WhenArgumentsMatch(a => a.Get<int>("complianceYear") == 2015)
                .Returns(A.Dummy<FileInfo>());

            ReportsController controller = new ReportsController(
                () => weeeClient,
                A.Dummy<BreadcrumbService>());

            ProducersDataViewModel viewModel = new ProducersDataViewModel();

            // Act
            ActionResult result = await controller.DownloadUkWeeeDataCsv(2015);

            // Assert
            FileResult fileResult = result as FileResult;
            Assert.NotNull(fileResult);

            Assert.Equal("text/csv", fileResult.ContentType);
        }

        /// <summary>
        /// This test ensures that the GET "UKWeeeDataAtAatfs" action calls the API to retrieve
        /// the list of compliance years and returns the "UKWeeeDataAtAatfs" view with 
        /// a ProducerDataViewModel that has be populated with the list of years.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetUkWeeeDataAtAatfs_Always_ReturnsUKWeeeDataAtAatfsProducerDataViewModel()
        {
            // Arrange
            var years = new List<int>() { 2001, 2002 };

            var weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<string>.Ignored, A<GetAatfReturnsActiveComplianceYears>.Ignored)).Returns(years);

            var controller = new ReportsController(
                () => weeeClient,
                A.Dummy<BreadcrumbService>());

            // Act
            var result = await controller.UkWeeeDataAtAatfs();

            // Assert
            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.True(string.IsNullOrEmpty(viewResult.ViewName));

            var model = viewResult.Model as UkWeeeDataAtAatfViewModel;
            Assert.NotNull(model);
            Assert.Collection(model.ComplianceYears,
                y1 => Assert.Equal("2001", y1.Text),
                y2 => Assert.Equal("2002", y2.Text));
        }

        /// <summary>
        /// This test ensures that the GET "UKWeeeDataAtAatfs" action returns
        /// a view with the ViewBag property "TriggerDownload" set to false.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetUkWeeeDataAtAatfs_Always_SetsTriggerDownloadToFalse()
        {
            // Arrange
            var controller = new ReportsController(
                () => A.Dummy<IWeeeClient>(),
                A.Dummy<BreadcrumbService>());

            // Act
            var result = await controller.UkWeeeDataAtAatfs();

            // Assert
            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.Equal(false, viewResult.ViewBag.TriggerDownload);
        }

        /// <summary>
        /// This test ensures that the GET "UKWeeeDataAtAatfs" action sets
        /// the breadcrumb's internal activity to "View reports".
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetUkWeeeDataAtAatfs_Always_SetsInternalBreadcrumbToViewReports()
        {
            var breadcrumb = new BreadcrumbService();

            // Arrange
            var controller = new ReportsController(
                () => A.Dummy<IWeeeClient>(),
                breadcrumb);

            // Act
            var result = await controller.UkWeeeDataAtAatfs();

            // Assert
            Assert.Equal("View reports", breadcrumb.InternalActivity);
        }

        /// <summary>
        /// This test ensures that the POST "UkWeeeDataAtAatfs" action with an invalid view model
        /// calls the API to retrieve the list of compliance years and returns the "UkWeeeDataAtAatfs"
        /// view with a ProducerDataViewModel that has be populated with the list of years.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task PostUkWeeeDataAtAatfs_WithInvalidViewModel_ReturnsUkWeeeDataAtAatfsProducerDataViewModel()
        {
            // Arrange
            var years = new List<int>() { 2001, 2002 };

            var weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<string>.Ignored, A<GetAatfReturnsActiveComplianceYears>.Ignored)).Returns(years);

            var controller = new ReportsController(
                () => weeeClient,
                A.Dummy<BreadcrumbService>());

            var viewModel = new UkWeeeDataAtAatfViewModel();

            // Act
            controller.ModelState.AddModelError("Key", "Error");
            var result = await controller.UkWeeeDataAtAatfs(viewModel);

            // Assert
            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.True(string.IsNullOrEmpty(viewResult.ViewName));

            var model = viewResult.Model as UkWeeeDataAtAatfViewModel;
            Assert.NotNull(model);
            Assert.Collection(model.ComplianceYears,
                y1 => Assert.Equal("2001", y1.Text),
                y2 => Assert.Equal("2002", y2.Text));
        }

        /// <summary>
        /// This test ensures that the POST "UkWeeeDataAtAatfs" action with an invalid view model
        /// returns a view with the ViewBag property "TriggerDownload" set to false.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task PostUkWeeeDataAtAatfs_WithInvalidViewModel_SetsTriggerDownloadToFalse()
        {
            // Arrange
            var controller = new ReportsController(
                () => A.Dummy<IWeeeClient>(),
                A.Dummy<BreadcrumbService>());

            var viewModel = new UkWeeeDataAtAatfViewModel();

            // Act
            controller.ModelState.AddModelError("Key", "Error");
            var result = await controller.UkWeeeDataAtAatfs(viewModel);

            // Assert
            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.Equal(false, viewResult.ViewBag.TriggerDownload);
        }

        /// <summary>
        /// This test ensures that the POST "UkWeeeDataAtAatfs" action with a valid view model
        /// returns a view with the ViewBag property "TriggerDownload" set to true.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task PostUkWeeeDataAtAatfs_WithViewModel_SetsTriggerDownloadToTrue()
        {
            // Arrange
            var controller = new ReportsController(
                () => A.Dummy<IWeeeClient>(),
                A.Dummy<BreadcrumbService>());

            var viewModel = new UkWeeeDataAtAatfViewModel();

            // Act
            var result = await controller.UkWeeeDataAtAatfs(viewModel);

            // Assert
            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.Equal(true, viewResult.ViewBag.TriggerDownload);
        }

        /// <summary>
        /// This test ensures that the POST "UkWeeeDataAtAatfs" action sets
        /// the breadcrumb's internal activity to "View reports".
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task PostUkWeeeDataAtAatfs_Always_SetsInternalBreadcrumbToViewReports()
        {
            var breadcrumb = new BreadcrumbService();

            // Arrange
            var controller = new ReportsController(
                () => A.Dummy<IWeeeClient>(),
                breadcrumb);

            // Act
            var result = await controller.UkWeeeDataAtAatfs(A.Dummy<UkWeeeDataAtAatfViewModel>());

            // Assert
            Assert.Equal("View reports", breadcrumb.InternalActivity);
        }

        /// <summary>
        /// This test ensures that the GET "DownloadUkWeeeDataAtAatfsCsv" action will
        /// call the API to generate a CSV file which is returned with the correct file name.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetDownloadUkWeeeDataAtAatfsCsv_Always_CallsApiAndReturnsFileResultWithCorrectFileName()
        {
            // Arrange
            var filename = fixture.Create<string>();
            var file = new FileInfo(filename, new byte[] { 1, 2, 3 });

            var weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<string>.Ignored, A<GetUkWeeeAtAatfsCsv>.Ignored))
                .WhenArgumentsMatch(a => a.Get<GetUkWeeeAtAatfsCsv>("request").ComplianceYear == 2015)
                .Returns(file);

            var controller = new ReportsController(
                () => weeeClient,
                A.Dummy<BreadcrumbService>());

            var viewModel = new ProducersDataViewModel();

            // Act
            var result = await controller.DownloadUkWeeeDataAtAatfsCsv(2015);

            // Assert
            var fileResult = result as FileResult;
            Assert.NotNull(fileResult);

            Assert.Equal(filename, fileResult.FileDownloadName);
        }

        /// <summary>
        /// This test ensures that the GET "DownloadUkWeeeDataAtAatfsCsv" action will
        /// call the API to generate a CSV file which is returned with a content
        /// type of "text/csv"
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetDownloadUkWeeeDataAtAatfsCsv_Always_CallsApiAndReturnsFileResultWithContentTypeOfTextCsv()
        {
            // Arrange
            var weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<GetUkWeeeCsv>.Ignored))
                .WhenArgumentsMatch(a => a.Get<int>("complianceYear") == 2015)
                .Returns(A.Dummy<FileInfo>());

            var controller = new ReportsController(
                () => weeeClient,
                A.Dummy<BreadcrumbService>());

            var viewModel = new ProducersDataViewModel();

            // Act
            var result = await controller.DownloadUkWeeeDataAtAatfsCsv(2015);

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
            // Arrange
            IWeeeClient weeeClient = A.Fake<IWeeeClient>();
            ReportsController controller = new ReportsController(
                () => weeeClient,
                A.Dummy<BreadcrumbService>());

            // Act
            ActionResult result = controller.SchemeObligationData();

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.Equal("SchemeObligationData", viewResult.ViewName);

            SchemeObligationDataViewModel model = viewResult.Model as SchemeObligationDataViewModel;
            Assert.NotNull(model);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetDataReturnsActiveComplianceYears>._))
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
            // Arrange
            ReportsController controller = new ReportsController(
                () => A.Dummy<IWeeeClient>(),
                A.Dummy<BreadcrumbService>());

            // Act
            ActionResult result = controller.SchemeObligationData();

            // Assert
            ViewResult viewResult = result as ViewResult;
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
            BreadcrumbService breadcrumb = new BreadcrumbService();

            // Arrange
            ReportsController controller = new ReportsController(
                () => A.Dummy<IWeeeClient>(),
                breadcrumb);

            // Act
            controller.SchemeObligationData();

            // Assert
            Assert.Equal("View reports", breadcrumb.InternalActivity);
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
            // Arrange
            IWeeeClient weeeClient = A.Fake<IWeeeClient>();
            ReportsController controller = new ReportsController(
                () => weeeClient,
                A.Dummy<BreadcrumbService>());

            // Act
            controller.ModelState.AddModelError("Key", "Error");
            ActionResult result = controller.SchemeObligationData(new SchemeObligationDataViewModel());

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.True(string.IsNullOrEmpty(viewResult.ViewName));

            SchemeObligationDataViewModel model = viewResult.Model as SchemeObligationDataViewModel;
            Assert.NotNull(model);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetDataReturnsActiveComplianceYears>._))
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
            // Arrange
            ReportsController controller = new ReportsController(
                () => A.Dummy<IWeeeClient>(),
                A.Dummy<BreadcrumbService>());

            // Act
            controller.ModelState.AddModelError("Key", "Error");
            ActionResult result = controller.SchemeObligationData(new SchemeObligationDataViewModel());

            // Assert
            ViewResult viewResult = result as ViewResult;
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
            // Arrange
            ReportsController controller = new ReportsController(
                () => A.Dummy<IWeeeClient>(),
                A.Dummy<BreadcrumbService>());

            // Act
            ActionResult result = controller.SchemeObligationData(new SchemeObligationDataViewModel());

            // Assert
            ViewResult viewResult = result as ViewResult;
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
            BreadcrumbService breadcrumb = new BreadcrumbService();

            // Arrange
            ReportsController controller = new ReportsController(
                () => A.Dummy<IWeeeClient>(),
                breadcrumb);

            // Act
            controller.SchemeObligationData(A.Dummy<SchemeObligationDataViewModel>());

            // Assert
            Assert.Equal("View reports", breadcrumb.InternalActivity);
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
            List<int> years = new List<int>() { 2001, 2002 };

            IWeeeClient weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetDataReturnsActiveComplianceYears>._)).Returns(years);

            List<SchemeData> schemes = new List<SchemeData>
            {
                new SchemeData() {Id = new Guid("F0D0B242-7656-46FA-AF15-204E710E9850"), SchemeName = "Scheme 1"},
                new SchemeData() {Id = new Guid("2FE842AD-E122-4C40-9C39-EC183CFCD9F3"), SchemeName = "Scheme 2"}
            };
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<Weee.Requests.Admin.GetSchemes>._)).Returns(schemes);

            ReportsController controller = new ReportsController(
                () => weeeClient,
                A.Dummy<BreadcrumbService>());

            // Act
            ActionResult result = await controller.MissingProducerData();

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.Equal("MissingProducerData", viewResult.ViewName);

            MissingProducerDataViewModel model = viewResult.Model as MissingProducerDataViewModel;
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
            // Arrange
            ReportsController controller = new ReportsController(
                () => A.Dummy<IWeeeClient>(),
                A.Dummy<BreadcrumbService>());

            // Act
            ActionResult result = await controller.MissingProducerData();

            // Assert
            ViewResult viewResult = result as ViewResult;
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
            BreadcrumbService breadcrumb = new BreadcrumbService();

            // Arrange
            ReportsController controller = new ReportsController(
                () => A.Dummy<IWeeeClient>(),
                breadcrumb);

            // Act
            await controller.MissingProducerData();

            // Assert
            Assert.Equal("View reports", breadcrumb.InternalActivity);
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
            List<int> years = new List<int>() { 2001, 2002 };

            IWeeeClient weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetDataReturnsActiveComplianceYears>._)).Returns(years);

            List<SchemeData> schemes = new List<SchemeData>
            {
                new SchemeData() {Id = new Guid("F0D0B242-7656-46FA-AF15-204E710E9850"), SchemeName = "Scheme 1"},
                new SchemeData() {Id = new Guid("2FE842AD-E122-4C40-9C39-EC183CFCD9F3"), SchemeName = "Scheme 2"}
            };
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<Weee.Requests.Admin.GetSchemes>._)).Returns(schemes);

            ReportsController controller = new ReportsController(
                () => weeeClient,
                A.Dummy<BreadcrumbService>());

            // Act
            controller.ModelState.AddModelError("Key", "Error");
            ActionResult result = await controller.MissingProducerData(new MissingProducerDataViewModel());

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.True(string.IsNullOrEmpty(viewResult.ViewName));

            MissingProducerDataViewModel model = viewResult.Model as MissingProducerDataViewModel;
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
            // Arrange
            ReportsController controller = new ReportsController(
                () => A.Dummy<IWeeeClient>(),
                A.Dummy<BreadcrumbService>());

            // Act
            controller.ModelState.AddModelError("Key", "Error");
            ActionResult result = await controller.MissingProducerData(new MissingProducerDataViewModel());

            // Assert
            ViewResult viewResult = result as ViewResult;
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
            // Arrange
            ReportsController controller = new ReportsController(
                () => A.Dummy<IWeeeClient>(),
                A.Dummy<BreadcrumbService>());

            // Act
            ActionResult result = await controller.MissingProducerData(new MissingProducerDataViewModel());

            // Assert
            ViewResult viewResult = result as ViewResult;
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
            BreadcrumbService breadcrumb = new BreadcrumbService();

            // Arrange
            ReportsController controller = new ReportsController(
                () => A.Dummy<IWeeeClient>(),
                breadcrumb);

            // Act
            await controller.MissingProducerData(A.Dummy<MissingProducerDataViewModel>());

            // Assert
            Assert.Equal("View reports", breadcrumb.InternalActivity);
        }

        [Fact]
        public async Task GetAatfAeReturnData_Always_ReturnsAatfAeReturnDataViewModel()
        {
            Fixture fixture = new Fixture();

            IWeeeClient weeeClient = A.Fake<IWeeeClient>();
            IList<UKCompetentAuthorityData> competentAuthorities = fixture.CreateMany<UKCompetentAuthorityData>().ToList();
            IList<PanAreaData> panAreas = fixture.CreateMany<PanAreaData>().ToList();
            IList<LocalAreaData> localAreas = fixture.CreateMany<LocalAreaData>().ToList();

            ReportsController controller = new ReportsController(
                () => weeeClient,
                A.Dummy<BreadcrumbService>());

            // Act
            ActionResult result = await controller.AatfAeReturnData();

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.True(string.IsNullOrEmpty(viewResult.ViewName) || viewResult.ViewName == "AatfAeReturnData");

            AatfAeReturnDataViewModel model = viewResult.Model as AatfAeReturnDataViewModel;
            Assert.NotNull(model);

            Assert.Collection(model.ComplianceYears,
                y1 => Assert.Equal("2019", y1.Text));

            Assert.Collection(model.Quarters,
                s1 => Assert.Equal("1", s1.Text),
                s2 => Assert.Equal("2", s2.Text),
                s3 => Assert.Equal("3", s3.Text),
                s4 => Assert.Equal("4", s4.Text));

            Assert.Collection(model.FacilityTypes,
               s1 => Assert.Equal("AATF", s1.Text),
               s2 => Assert.Equal("AE", s2.Text));

            Assert.Collection(model.SubmissionStatus,
               s1 => Assert.Equal("Submitted", s1.Text),
               s2 => Assert.Equal("Started", s2.Text),
               s3 => Assert.Equal("Not Started", s3.Text));

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetUKCompetentAuthorities>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetPanAreas>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetLocalAreas>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        /// <summary>
        /// This test ensures that the GET "AatfAeReturnData" action returns
        /// a view with the ViewBag property "TriggerDownload" set to false.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetAatfAeReturnData_Always_SetsTriggerDownloadToFalse()
        {
            // Arrange
            ReportsController controller = new ReportsController(
                () => A.Dummy<IWeeeClient>(),
                A.Dummy<BreadcrumbService>());

            // Act
            ActionResult result = await controller.AatfAeReturnData();

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.Equal(false, viewResult.ViewBag.TriggerDownload);
        }

        /// <summary>
        /// This test ensures that the GET "AatfAeReturnData" action sets
        /// the breadcrumb's internal activity to "View reports".
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetAatfAeReturnData_Always_SetsInternalBreadcrumbToViewReports()
        {
            BreadcrumbService breadcrumb = new BreadcrumbService();

            // Arrange
            ReportsController controller = new ReportsController(
                () => A.Dummy<IWeeeClient>(),
                breadcrumb);

            // Act
            await controller.AatfAeReturnData();

            // Assert
            Assert.Equal("View reports", breadcrumb.InternalActivity);
        }

        /// <summary>
        /// This test ensures that the POST "AatfAeReturnData" action with an invalid view model
        /// calls the API to retrieve the list of compliance years and returns the "AatfAeReturnData"
        /// view with a AatfAeReturnDataViewModel that has be populated with the list of years.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task PostAatfAeReturnData_WithInvalidViewModel_ReturnsAatfAeReturnDataViewModel()
        {
            IWeeeClient weeeClient = A.Fake<IWeeeClient>();

            ReportsController controller = new ReportsController(
                () => weeeClient,
                A.Dummy<BreadcrumbService>());

            // Act
            controller.ModelState.AddModelError("Key", "Error");
            ActionResult result = await controller.AatfAeReturnData(new AatfAeReturnDataViewModel());

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.True(string.IsNullOrEmpty(viewResult.ViewName) || viewResult.ViewName == "AatfAeReturnData");

            AatfAeReturnDataViewModel model = viewResult.Model as AatfAeReturnDataViewModel;
            Assert.NotNull(model);
            Assert.Collection(model.ComplianceYears,
                y1 => Assert.Equal("2019", y1.Text));
        }

        /// <summary>
        /// This test ensures that the POST "AatfAeReturnData" action with an invalid view model
        /// returns a view with the ViewBag property "TriggerDownload" set to false.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task PostAatfAeReturnData_WithInvalidViewModel_SetsTriggerDownloadToFalse()
        {
            // Arrange
            ReportsController controller = new ReportsController(
                () => A.Dummy<IWeeeClient>(),
                A.Dummy<BreadcrumbService>());

            // Act
            controller.ModelState.AddModelError("Key", "Error");
            ActionResult result = await controller.AatfAeReturnData(new AatfAeReturnDataViewModel());

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.Equal(false, viewResult.ViewBag.TriggerDownload);
        }

        /// <summary>
        /// This test ensures that the POST "AatfAeReturnData" action with a valid view model
        /// returns a view with the ViewBag property "TriggerDownload" set to true.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task PostAatfAeReturnData_WithViewModel_SetsTriggerDownloadToTrue()
        {
            // Arrange
            ReportsController controller = new ReportsController(
                () => A.Dummy<IWeeeClient>(),
                A.Dummy<BreadcrumbService>());

            // Act
            ActionResult result = await controller.AatfAeReturnData(new AatfAeReturnDataViewModel());

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.Equal(true, viewResult.ViewBag.TriggerDownload);
        }

        /// <summary>
        /// This test ensures that the POST "AatfAeReturnData" action sets
        /// the breadcrumb's internal activity to "View reports".
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task PostAatfAeReturnData_Always_SetsInternalBreadcrumbToViewReports()
        {
            BreadcrumbService breadcrumb = new BreadcrumbService();

            // Arrange
            ReportsController controller = new ReportsController(
                () => A.Dummy<IWeeeClient>(),
                breadcrumb);

            // Act
            await controller.AatfAeReturnData(A.Dummy<AatfAeReturnDataViewModel>());

            // Assert
            Assert.Equal("View reports", breadcrumb.InternalActivity);
        }

        [Fact]
        public async void PostAatfAeReturnData_OnDownload_SetsURL()
        {
            IWeeeClient apiClient = A.Fake<IWeeeClient>();
            ReportsController controller = new ReportsController(
                () => apiClient,
                A.Dummy<BreadcrumbService>());

            var httpContext = new HttpContextMocker();
            httpContext.AttachToController(controller);
            var httpRequest = A.Fake<HttpRequestBase>();

            CSVFileData file = new CSVFileData() { FileContent = "Content", FileName = "test.csv" };

            Uri uri = new Uri("https://localhost:44300");
            A.CallTo(() => controller.HttpContext.Request).Returns(httpRequest);
            A.CallTo(() => controller.HttpContext.Request.Url).Returns(uri);

            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetAatfAeReturnDataCsv>._)).Returns(file);

            ActionResult result = await controller.DownloadAatfAeDataCsv(2019, 1,
                FacilityType.Aatf, A.Dummy<int?>(), A.Dummy<Guid?>(), A.Dummy<Guid?>(), 
                A.Dummy<Guid?>());

            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetAatfAeReturnDataCsv>.That.Matches(x => x.AatfDataUrl.Equals("https://localhost:44300/admin/aatf/details/")))).MustHaveHappened();

            FileResult fileResult = result as FileResult;
            Assert.NotNull(fileResult);
            Assert.Equal("text/csv", fileResult.ContentType);
        }

        [Fact]
        public async void PostAatfAeReturnData_OnDownload_SetsURLWithVirtualDirectory()
        {
            var apiClient = A.Fake<IWeeeClient>();
            var controller = new ReportsController(
                () => apiClient,
                A.Dummy<BreadcrumbService>());

            var httpContext = new HttpContextMocker();
            httpContext.AttachToController(controller);
            var httpRequest = A.Fake<HttpRequestBase>();

            var file = new CSVFileData() { FileContent = "Content", FileName = "test.csv" };

            var uri = new Uri("https://localhost:44300/weeerelease");
            A.CallTo(() => controller.HttpContext.Request).Returns(httpRequest);
            A.CallTo(() => controller.HttpContext.Request.Url).Returns(uri);
            A.CallTo(() => controller.HttpContext.Request.ApplicationPath).Returns("weeerelease");

            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetAatfAeReturnDataCsv>._)).Returns(file);

            var result = await controller.DownloadAatfAeDataCsv(2019, 1,
                FacilityType.Aatf, A.Dummy<int?>(), A.Dummy<Guid?>(), A.Dummy<Guid?>(),
                A.Dummy<Guid?>());

            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetAatfAeReturnDataCsv>.That.Matches(x => x.AatfDataUrl.Equals("https://localhost:44300/weeerelease/admin/aatf/details/")))).MustHaveHappened();

            var fileResult = result as FileResult;
            Assert.NotNull(fileResult);
            Assert.Equal("text/csv", fileResult.ContentType);
        }

        [Fact]
        public async Task GetAatfObligatedData_Always_ReturnsAatfObligatedDataViewModel()
        {
            Fixture fixture = new Fixture();

            IWeeeClient weeeClient = A.Fake<IWeeeClient>();
            IList<UKCompetentAuthorityData> competentAuthorities = fixture.CreateMany<UKCompetentAuthorityData>().ToList();
            IList<PanAreaData> panAreas = fixture.CreateMany<PanAreaData>().ToList();
            IList<LocalAreaData> localAreas = fixture.CreateMany<LocalAreaData>().ToList();

            ReportsController controller = new ReportsController(
                () => weeeClient,
                A.Dummy<BreadcrumbService>());

            // Act
            ActionResult result = await controller.AatfObligatedData();

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.True(string.IsNullOrEmpty(viewResult.ViewName) || viewResult.ViewName == "AatfObligatedData");

            AatfObligatedDataViewModel model = viewResult.Model as AatfObligatedDataViewModel;
            Assert.NotNull(model);

            Assert.Collection(model.ComplianceYears,
                y1 => Assert.Equal("2019", y1.Text));

            Assert.Collection(model.ObligationTypes,
                s1 => Assert.Equal("B2B", s1.Text),
                s2 => Assert.Equal("B2C", s2.Text));

            Assert.Collection(model.SchemeColumnPossibleValues,
               s1 => Assert.Equal("PCS names", s1.Text),
               s2 => Assert.Equal("Approval numbers", s2.Text));

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetUKCompetentAuthorities>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetPanAreas>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        /// <summary>
        /// This test ensures that the GET "AatfObligatedData" action returns
        /// a view with the ViewBag property "TriggerDownload" set to false.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetAatfObligatedData_Always_SetsTriggerDownloadToFalse()
        {
            // Arrange
            ReportsController controller = new ReportsController(
                () => A.Dummy<IWeeeClient>(),
                A.Dummy<BreadcrumbService>());

            // Act
            ActionResult result = await controller.AatfObligatedData();

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.Equal(false, viewResult.ViewBag.TriggerDownload);
        }

        /// <summary>
        /// This test ensures that the GET "AatfObligatedData" action sets
        /// the breadcrumb's internal activity to "View reports".
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetAatfObligatedData_Always_SetsInternalBreadcrumbToViewReports()
        {
            BreadcrumbService breadcrumb = new BreadcrumbService();

            // Arrange
            ReportsController controller = new ReportsController(
                () => A.Dummy<IWeeeClient>(),
                breadcrumb);

            // Act
            await controller.AatfObligatedData();

            // Assert
            Assert.Equal("View reports", breadcrumb.InternalActivity);
        }

        /// <summary>
        /// This test ensures that the POST "AatfObligatedData" action with an invalid view model
        /// calls the API to retrieve the list of compliance years and returns the "AatfObligatedData"
        /// view with a AatfObligatedDataViewModel that has be populated with the list of years.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task PostAatfObligatedData_WithInvalidViewModel_ReturnsAatfObligatedDataViewModel()
        {
            IWeeeClient weeeClient = A.Fake<IWeeeClient>();

            ReportsController controller = new ReportsController(
                () => weeeClient,
                A.Dummy<BreadcrumbService>());

            // Act
            controller.ModelState.AddModelError("Key", "Error");
            ActionResult result = await controller.AatfObligatedData(new AatfObligatedDataViewModel());

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.True(string.IsNullOrEmpty(viewResult.ViewName) || viewResult.ViewName == "AatfObligatedData");

            AatfObligatedDataViewModel model = viewResult.Model as AatfObligatedDataViewModel;
            Assert.NotNull(model);
            Assert.Collection(model.ComplianceYears,
                y1 => Assert.Equal("2019", y1.Text));
        }

        /// <summary>
        /// This test ensures that the POST "AatfObligatedData" action with an invalid view model
        /// returns a view with the ViewBag property "TriggerDownload" set to false.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task PostAatfObligatedData_WithInvalidViewModel_SetsTriggerDownloadToFalse()
        {
            // Arrange
            ReportsController controller = new ReportsController(
                () => A.Dummy<IWeeeClient>(),
                A.Dummy<BreadcrumbService>());

            // Act
            controller.ModelState.AddModelError("Key", "Error");
            ActionResult result = await controller.AatfObligatedData(new AatfObligatedDataViewModel());

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.Equal(false, viewResult.ViewBag.TriggerDownload);
        }

        /// <summary>
        /// This test ensures that the POST "AatfObligatedData" action with a valid view model
        /// returns a view with the ViewBag property "TriggerDownload" set to true.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task PostAatfObligatedData_WithViewModel_SetsTriggerDownloadToTrue()
        {
            // Arrange
            ReportsController controller = new ReportsController(
                () => A.Dummy<IWeeeClient>(),
                A.Dummy<BreadcrumbService>());

            // Act
            ActionResult result = await controller.AatfObligatedData(new AatfObligatedDataViewModel());

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.Equal(true, viewResult.ViewBag.TriggerDownload);
        }

        /// <summary>
        /// This test ensures that the POST "AatfObligatedData" action sets
        /// the breadcrumb's internal activity to "View reports".
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task PostAatfObligatedData_Always_SetsInternalBreadcrumbToViewReports()
        {
            BreadcrumbService breadcrumb = new BreadcrumbService();

            // Arrange
            ReportsController controller = new ReportsController(
                () => A.Dummy<IWeeeClient>(),
                breadcrumb);

            // Act
            await controller.AatfObligatedData(A.Dummy<AatfObligatedDataViewModel>());

            // Assert
            Assert.Equal("View reports", breadcrumb.InternalActivity);
        }
    }
}
