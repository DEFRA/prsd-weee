namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers
{
    using EA.Weee.Api.Client;
    using EA.Weee.Core.Shared.Paging;
    using EA.Weee.DataAccess.StoredProcedure;
    using EA.Weee.Requests;
    using EA.Weee.Web.Areas.Admin.Controllers;
    using EA.Weee.Web.Areas.Admin.Controllers.Base;
    using EA.Weee.Web.Areas.Admin.ViewModels.ManageEvidenceNotes;
    using EA.Weee.Web.Areas.Admin.ViewModels.RemovePCSRecords;
    using EA.Weee.Web.Services;
    using FakeItEasy;
    using FluentAssertions;
    using iText.Layout.Element;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Xunit;

    public class RemovePCSRecordsControllerTests
    {
        private readonly IWeeeClient weeeClient;
        private readonly BreadcrumbService breadcrumbService;
        private readonly RemovePCSRecordsController controller;

        public RemovePCSRecordsControllerTests()
        {
            weeeClient = A.Fake<IWeeeClient>();
            breadcrumbService = A.Fake<BreadcrumbService>();

            controller = new RemovePCSRecordsController(() => weeeClient, breadcrumbService);
        }

        [Fact]
        public void Controller_ShouldInheritFromAdminBaseController()
        {
            typeof(RemovePCSRecordsController).Should().BeDerivedFrom<AdminController>();
        }

        [Fact]
        public async Task GetIndex_RemovePCSRecords_ComplianceYears_List()
        {
            // Arrange
            BreadcrumbService breadcrumb = A.Dummy<BreadcrumbService>();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSchemeComplianceYearsExceedingRetentionPeriod>._))
                                     .Returns(new List<int> { 2018, 2017, 2016 });

            Func<IWeeeClient> weeeClientFunc = A.Fake<Func<IWeeeClient>>();
            A.CallTo(() => weeeClientFunc()).Returns(weeeClient);

            // Act
            ActionResult result = await controller.Index();

            // Assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSchemeComplianceYearsExceedingRetentionPeriod>._))
                                     .MustHaveHappened();

            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);

            Assert.True(string.IsNullOrEmpty(viewResult.ViewName) ||
                        viewResult.ViewName.ToLowerInvariant() == "index");

            RemovePCSRecordsViewModel resultsViewModel = viewResult.Model as RemovePCSRecordsViewModel;
            Assert.NotNull(resultsViewModel);

            Assert.Equal("0", resultsViewModel.SelectedYear);
            Assert.Equal(4, resultsViewModel.ComplianceYears.Count());
        }

        [Fact]
        public async Task GetIndex_RemovePCSRecords_GetSchemesForComplianceYear_List()
        {
            // Arrange
            BreadcrumbService breadcrumb = A.Dummy<BreadcrumbService>();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSchemesForComplianceYear>._))
                                     .Returns(new List<string> { "Test Scheme 1", "Test Scheme 2", "Test Scheme 3", "Test Scheme 4" });

            Func<IWeeeClient> weeeClientFunc = A.Fake<Func<IWeeeClient>>();
            A.CallTo(() => weeeClientFunc()).Returns(weeeClient);

            // Act
            ActionResult result = await controller.Index();

            // Assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSchemesForComplianceYear>._))
                                     .MustHaveHappened();

            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);

            Assert.True(string.IsNullOrEmpty(viewResult.ViewName) ||
                        viewResult.ViewName.ToLowerInvariant() == "index");

            RemovePCSRecordsViewModel resultsViewModel = viewResult.Model as RemovePCSRecordsViewModel;
            Assert.NotNull(resultsViewModel);

            Assert.Equal("All PCSs", resultsViewModel.SelectedScheme);
            Assert.Equal(5, resultsViewModel.SchemeNames.Count());
        }

        [Fact]
        public async Task GetIndex_RemovePCSRecords_GetSchemeData_List()
        {
            // Arrange
            BreadcrumbService breadcrumb = A.Dummy<BreadcrumbService>();

            var pcsRow1 = new SchemeDataExceedingRetentionPeriod() { ApprovalNumber = "WEE/AB123CD/ATF", ComplianceYear = 2018, SchemeName = "Test Scheme 1", SchemeId = Guid.NewGuid() };
            var pcsRow2 = new SchemeDataExceedingRetentionPeriod() { ApprovalNumber = "WEE/AB456CD/ATF", ComplianceYear = 2017, SchemeName = "Test Scheme 2", SchemeId = Guid.NewGuid() };
            var pcsRow3 = new SchemeDataExceedingRetentionPeriod() { ApprovalNumber = "WEE/AB678CD/ATF", ComplianceYear = 2017, SchemeName = "Test Scheme 3", SchemeId = Guid.NewGuid() };
            var pcsRow4 = new SchemeDataExceedingRetentionPeriod() { ApprovalNumber = "WEE/AB910CD/ATF", ComplianceYear = 2016, SchemeName = "Test Scheme 4", SchemeId = Guid.NewGuid() };

            var pcsList = new List<SchemeDataExceedingRetentionPeriod>
            {
                pcsRow1,
                pcsRow2,
                pcsRow3,
                pcsRow4
            };

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSchemeDataExceedingRetentionPeriodRequest>._))
                                     .Returns(pcsList);

            Func<IWeeeClient> weeeClientFunc = A.Fake<Func<IWeeeClient>>();
            A.CallTo(() => weeeClientFunc()).Returns(weeeClient);

            // Act
            ActionResult result = await controller.Index();

            // Assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSchemeDataExceedingRetentionPeriodRequest>._))
                                     .MustHaveHappened();

            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);

            Assert.True(string.IsNullOrEmpty(viewResult.ViewName) ||
                        viewResult.ViewName.ToLowerInvariant() == "index");

            RemovePCSRecordsViewModel resultsViewModel = viewResult.Model as RemovePCSRecordsViewModel;
            Assert.NotNull(resultsViewModel);

            Assert.Equal(4, resultsViewModel.PCSSchemeData.Count());
            Assert.Equal(1, resultsViewModel.PCSSchemeData.Count(x => x.ComplianceYear == 2016));
            Assert.Equal(2, resultsViewModel.PCSSchemeData.Count(x => x.ComplianceYear == 2017));
            Assert.Equal(1, resultsViewModel.PCSSchemeData.Count(x => x.ComplianceYear == 2018));
        }

        [Fact]
        public async Task PostIndex_RemovePCSRecords_GetSchemeData_List()
        {
            // Arrange
            BreadcrumbService breadcrumb = A.Dummy<BreadcrumbService>();

            var pcsRow1 = new SchemeDataExceedingRetentionPeriod() { ApprovalNumber = "WEE/AB123CD/ATF", ComplianceYear = 2018, SchemeName = "Test Scheme 1", SchemeId = Guid.NewGuid() };
            var pcsRow2 = new SchemeDataExceedingRetentionPeriod() { ApprovalNumber = "WEE/AB456CD/ATF", ComplianceYear = 2017, SchemeName = "Test Scheme 2", SchemeId = Guid.NewGuid() };
            var pcsRow3 = new SchemeDataExceedingRetentionPeriod() { ApprovalNumber = "WEE/AB678CD/ATF", ComplianceYear = 2017, SchemeName = "Test Scheme 3", SchemeId = Guid.NewGuid() };
            var pcsRow4 = new SchemeDataExceedingRetentionPeriod() { ApprovalNumber = "WEE/AB910CD/ATF", ComplianceYear = 2016, SchemeName = "Test Scheme 4", SchemeId = Guid.NewGuid() };

            var pcsList = new List<SchemeDataExceedingRetentionPeriod>
            {
                pcsRow1,
                pcsRow2,
                pcsRow3,
                pcsRow4
            };

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSchemeDataExceedingRetentionPeriodRequest>._))
                                     .Returns(pcsList);

            Func<IWeeeClient> weeeClientFunc = A.Fake<Func<IWeeeClient>>();
            A.CallTo(() => weeeClientFunc()).Returns(weeeClient);

            RemovePCSRecordsViewModel model = new RemovePCSRecordsViewModel()
            {
                SelectedYear = "0",
                SelectedScheme = "All PCS",
            };

            // Act
            var result = await controller.Index(model, 1) as ViewResult;
            var convertedModel = result.Model as RemovePCSRecordsViewModel;

            // Assert
            convertedModel.SelectedYear.Should().BeEquivalentTo(model.SelectedYear);
            convertedModel.SelectedScheme.Should().BeEquivalentTo(model.SelectedScheme);
        }
    }
}
