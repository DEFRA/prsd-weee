namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using Core.Admin;
    using Core.Shared;
    using FakeItEasy;
    using Services;
    using Services.Caching;
    using TestHelpers;
    using ViewModels.Shared.Submission;
    using Web.Areas.Admin.Controllers;
    using Web.Areas.Admin.ViewModels.Submissions;
    using Web.Areas.Scheme.ViewModels;
    using Weee.Requests.Admin.GetDataReturnSubmissionChanges;
    using Weee.Requests.Shared;
    using Xunit;

    public class SubmissionsControllerTests
    {
        private readonly IWeeeClient weeeClient;

        public SubmissionsControllerTests()
        {
            weeeClient = A.Fake<IWeeeClient>();
        }
        
        [Fact]
        public async void GetChooseSubmissionType_ReturnsView()
        {
            var result = await SubmissionsController().ChooseSubmissionType();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async void PostChooseSubmissionType_ModelIsInvalid_ShouldRedirectViewWithModel()
        {
            var controller = SubmissionsController();
            controller.ModelState.AddModelError("Key", "Any error");

            var model = new ChooseSubmissionTypeViewModel
            {
                SelectedValue = SubmissionType.EeeOrWeeeDataReturns
            };
            var result = await controller.ChooseSubmissionType(model);

            Assert.IsType<ViewResult>(result);
            Assert.Equal(model, ((ViewResult)(result)).Model);
            Assert.False(controller.ModelState.IsValid);
        }

        [Fact]
        public async void PostChooseSubmissionType_MemberRegistrationsSelected_RedirectsToMemberRegistrationSubmissionHistory()
        {
            var result = await SubmissionsController().ChooseSubmissionType(new ChooseSubmissionTypeViewModel
            {
                SelectedValue = SubmissionType.MemberRegistrations
            });

            Assert.IsType<RedirectToRouteResult>(result);

            var routeValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("SubmissionsHistory", routeValues["action"]);
        }

        [Fact]
        public async void PostChooseSubmissionType_DataReturnsSelected_RedirectsToDataReturnsSubmissionHistory()
        {
            var result = await SubmissionsController().ChooseSubmissionType(new ChooseSubmissionTypeViewModel
            {
                SelectedValue = SubmissionType.EeeOrWeeeDataReturns
            });

            Assert.IsType<RedirectToRouteResult>(result);

            var routeValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("DataReturnSubmissionHistory", routeValues["action"]);
        }

        [Fact]
        public async Task FetchSubmissionResults_RequestsForDataSortedBySubmissionDateDescending()
        {
            await SubmissionsController().FetchSubmissionResults(A.Dummy<int>(), A.Dummy<Guid>());

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSubmissionsHistoryResults>._))
                .WhenArgumentsMatch(a => ((GetSubmissionsHistoryResults)a[1]).Ordering == SubmissionsHistoryOrderBy.SubmissionDateDescending)
                .MustHaveHappened();
        }

        [Fact]
        public async Task FetchSubmissionResults_ReturnsModelWithSubmissionDateDescendingSort()
        {
            var result = await SubmissionsController().FetchSubmissionResults(A.Dummy<int>(), A.Dummy<Guid>());

            Assert.IsType<PartialViewResult>(result);

            var model = ((PartialViewResult)result).Model as SubmissionsResultsViewModel;

            Assert.NotNull(model);
            Assert.Equal(SubmissionsHistoryOrderBy.SubmissionDateDescending, model.OrderBy);
        }

        [Fact]
        public async Task FetchSubmissionResults_RequestsForSummaryData()
        {
            await SubmissionsController().FetchSubmissionResults(A.Dummy<int>(), A.Dummy<Guid>());

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSubmissionsHistoryResults>._))
                .WhenArgumentsMatch(a => ((GetSubmissionsHistoryResults)a[1]).IncludeSummaryData == true)
                .MustHaveHappened();
        }

        [Fact]
        public async Task GetSubmissionResults_RequestsForDataSortedBySpecifiedValue()
        {
            await SubmissionsController().GetSubmissionResults(A.Dummy<int>(), A.Dummy<Guid>(), SubmissionsHistoryOrderBy.SubmissionDateAscending);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSubmissionsHistoryResults>._))
                .WhenArgumentsMatch(a => ((GetSubmissionsHistoryResults)a[1]).Ordering == SubmissionsHistoryOrderBy.SubmissionDateAscending)
                .MustHaveHappened();
        }

        [Fact]
        public async Task GetSubmissionResults_ReturnsModelWithSpecifiedSortValue()
        {
            var result = await SubmissionsController().GetSubmissionResults(A.Dummy<int>(), A.Dummy<Guid>(), SubmissionsHistoryOrderBy.SubmissionDateAscending);

            Assert.IsType<PartialViewResult>(result);

            var model = ((PartialViewResult)result).Model as SubmissionsResultsViewModel;

            Assert.NotNull(model);
            Assert.Equal(SubmissionsHistoryOrderBy.SubmissionDateAscending, model.OrderBy);
        }

        [Fact]
        public async Task GetSubmissionResults_RequestsForSummaryData()
        {
            await SubmissionsController().GetSubmissionResults(A.Dummy<int>(), A.Dummy<Guid>(), SubmissionsHistoryOrderBy.SubmissionDateAscending);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSubmissionsHistoryResults>._))
                .WhenArgumentsMatch(a => ((GetSubmissionsHistoryResults)a[1]).IncludeSummaryData == true)
                .MustHaveHappened();
        }

        [Fact]
        public async Task FetchDataReturnSubmissionResults_RequestsForSummaryData()
        {
            await SubmissionsController().FetchDataReturnSubmissionResults(A.Dummy<int>(), A.Dummy<Guid>());

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetDataReturnSubmissionsHistoryResults>._))
                .WhenArgumentsMatch(a => ((GetDataReturnSubmissionsHistoryResults)a[1]).IncludeSummaryData == true)
                .MustHaveHappened();
        }

        [Fact]
        public async Task FetchDataReturnSubmissionResults_RequestsForEeeOutputDataComparison()
        {
            await SubmissionsController().FetchDataReturnSubmissionResults(A.Dummy<int>(), A.Dummy<Guid>());

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetDataReturnSubmissionsHistoryResults>._))
                .WhenArgumentsMatch(a => ((GetDataReturnSubmissionsHistoryResults)a[1]).CompareEeeOutputData == true)
                .MustHaveHappened();
        }

        [Fact]
        public async Task FetchDataReturnSubmissionResults_RequestsForDataSortedBySubmissionDateDescending()
        {
            await SubmissionsController().FetchDataReturnSubmissionResults(A.Dummy<int>(), A.Dummy<Guid>());

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetDataReturnSubmissionsHistoryResults>._))
                .WhenArgumentsMatch(a => ((GetDataReturnSubmissionsHistoryResults)a[1]).Ordering == DataReturnSubmissionsHistoryOrderBy.SubmissionDateDescending)
                .MustHaveHappened();
        }

        [Fact]
        public async Task FetchDataReturnSubmissionResults_ReturnsModelWithSubmissionDateDescendingSort()
        {
            var result = await SubmissionsController().FetchDataReturnSubmissionResults(A.Dummy<int>(), A.Dummy<Guid>());

            Assert.IsType<PartialViewResult>(result);

            var model = ((PartialViewResult)result).Model as DataReturnSubmissionsResultsViewModel;

            Assert.NotNull(model);
            Assert.Equal(DataReturnSubmissionsHistoryOrderBy.SubmissionDateDescending, model.OrderBy);
        }

        [Fact]
        public async Task GetDataReturnSubmissionResults_RequestsForSummaryData()
        {
            await SubmissionsController().GetDataReturnSubmissionResults(A.Dummy<int>(), A.Dummy<Guid>(), 
                A.Dummy<DataReturnSubmissionsHistoryOrderBy>());

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetDataReturnSubmissionsHistoryResults>._))
                .WhenArgumentsMatch(a => ((GetDataReturnSubmissionsHistoryResults)a[1]).IncludeSummaryData == true)
                .MustHaveHappened();
        }

        [Fact]
        public async Task GetDataReturnSubmissionResults_RequestsForEeeOutputDataComparison()
        {
            await SubmissionsController().GetDataReturnSubmissionResults(A.Dummy<int>(), A.Dummy<Guid>(), 
                A.Dummy<DataReturnSubmissionsHistoryOrderBy>());

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetDataReturnSubmissionsHistoryResults>._))
                .WhenArgumentsMatch(a => ((GetDataReturnSubmissionsHistoryResults)a[1]).CompareEeeOutputData == true)
                .MustHaveHappened();
        }

        [Fact]
        public async Task GetDataReturnSubmissionResults_RequestsForDataSortedBySpecifiedValue()
        {
            await SubmissionsController().GetDataReturnSubmissionResults(A.Dummy<int>(), A.Dummy<Guid>(), 
                DataReturnSubmissionsHistoryOrderBy.QuarterDescending);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetDataReturnSubmissionsHistoryResults>._))
                .WhenArgumentsMatch(a => ((GetDataReturnSubmissionsHistoryResults)a[1]).Ordering == DataReturnSubmissionsHistoryOrderBy.QuarterDescending)
                .MustHaveHappened();
        }

        [Fact]
        public async Task GetDataReturnSubmissionResults_ReturnsModelWithSpecifiedSortValue()
        {
            var result = await SubmissionsController().GetDataReturnSubmissionResults(A.Dummy<int>(), A.Dummy<Guid>(), 
                DataReturnSubmissionsHistoryOrderBy.QuarterDescending);

            Assert.IsType<PartialViewResult>(result);

            var model = ((PartialViewResult)result).Model as DataReturnSubmissionsResultsViewModel;

            Assert.NotNull(model);
            Assert.Equal(DataReturnSubmissionsHistoryOrderBy.QuarterDescending, model.OrderBy);
        }

        [Fact]
        public async Task DownloadDataReturnSubmissionEeeChanges_CallsApiAndReturnsFileResult()
        {
            // Arrange
            var csvFileData = new CSVFileData { FileName = "Test file.csv", FileContent = "CSV content" };

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetDataReturnSubmissionEeeChangesCsv>._))
               .Returns(csvFileData);

            // Act
            var result = await SubmissionsController().DownloadDataReturnSubmissionEeeChanges(A.Dummy<Guid>(), A.Dummy<Guid>());

            // Assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetDataReturnSubmissionEeeChangesCsv>._))
                .MustHaveHappened();

            FileResult fileResult = result as FileResult;
            Assert.NotNull(fileResult);

            Assert.Equal("Test file.csv", fileResult.FileDownloadName);
            Assert.Equal("text/csv", fileResult.ContentType);
        }

        private SubmissionsController SubmissionsController()
        {
            ConfigurationService configService = A.Fake<ConfigurationService>();
            var controller = new SubmissionsController(A.Fake<BreadcrumbService>(), () => weeeClient, A.Fake<IWeeeCache>(), A.Fake<CsvWriterFactory>());
            new HttpContextMocker().AttachToController(controller);
            return controller;
        }
    }
}
