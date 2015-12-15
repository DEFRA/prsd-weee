﻿namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.Admin;
    using EA.Weee.Core.Search;
    using EA.Weee.Requests.Admin;
    using EA.Weee.Web.Areas.Admin.Controllers;
    using EA.Weee.Web.Areas.Admin.ViewModels.Producers;
    using EA.Weee.Web.Services;
    using FakeItEasy;
    using Xunit;

    public class ProducersControllerTests
    {
        [Fact]
        public async Task GetSearch_ReturnsSearchView()
        {
            // Arrange
            BreadcrumbService breadcrumb = A.Dummy<BreadcrumbService>();
            ISearcher<ProducerSearchResult> producerSearcher = A.Dummy<ISearcher<ProducerSearchResult>>();
            Func<IWeeeClient> weeeClient = A.Dummy<Func<IWeeeClient>>();

            ProducersController controller = new ProducersController(breadcrumb, producerSearcher, weeeClient);

            // Act
            ActionResult result = await controller.Search();

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);

            Assert.True(string.IsNullOrEmpty(viewResult.ViewName) || viewResult.ViewName.ToLowerInvariant() == "search");
        }

        [Fact]
        public async Task PostSearch_WithInvalidModel_ReturnsSearchView()
        {
            // Arrange
            BreadcrumbService breadcrumb = A.Dummy<BreadcrumbService>();
            ISearcher<ProducerSearchResult> producerSearcher = A.Dummy<ISearcher<ProducerSearchResult>>();
            Func<IWeeeClient> weeeClient = A.Dummy<Func<IWeeeClient>>();

            ProducersController controller = new ProducersController(breadcrumb, producerSearcher, weeeClient);

            SearchViewModel viewModel = new SearchViewModel();
            controller.ModelState.AddModelError("SomeProperty", "Exception");

            // Act
            ActionResult result = await controller.Search(viewModel);

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);

            Assert.True(string.IsNullOrEmpty(viewResult.ViewName) || viewResult.ViewName.ToLowerInvariant() == "search");
        }

        [Fact]
        public async Task PostSearch_WithSearchTermAndNoSelectedPRN_RedirectsToSearchResultsAction()
        {
            // Arrange
            BreadcrumbService breadcrumb = A.Dummy<BreadcrumbService>();
            ISearcher<ProducerSearchResult> producerSearcher = A.Dummy<ISearcher<ProducerSearchResult>>();
            Func<IWeeeClient> weeeClient = A.Dummy<Func<IWeeeClient>>();

            ProducersController controller = new ProducersController(breadcrumb, producerSearcher, weeeClient);

            SearchViewModel viewModel = new SearchViewModel();
            viewModel.SearchTerm = "testSearchTerm";
            viewModel.SelectedRegistrationNumber = null;

            // Act
            ActionResult result = await controller.Search(viewModel);

            // Assert
            RedirectToRouteResult redirectResult = result as RedirectToRouteResult;
            Assert.NotNull(redirectResult);

            Assert.Equal("SearchResults", redirectResult.RouteValues["action"]);
            Assert.Equal("testSearchTerm", redirectResult.RouteValues["SearchTerm"]);
        }

        [Fact]
        public async Task PostSearch_WithSelectedPRN_RedirectsToDetailsAction()
        {
            // Arrange
            BreadcrumbService breadcrumb = A.Dummy<BreadcrumbService>();
            ISearcher<ProducerSearchResult> producerSearcher = A.Dummy<ISearcher<ProducerSearchResult>>();
            Func<IWeeeClient> weeeClient = A.Dummy<Func<IWeeeClient>>();

            ProducersController controller = new ProducersController(breadcrumb, producerSearcher, weeeClient);

            SearchViewModel viewModel = new SearchViewModel();
            viewModel.SearchTerm = "testSearchTerm, WEE/AA1111AA";
            viewModel.SelectedRegistrationNumber = "WEE/AA1111AA";

            // Act
            ActionResult result = await controller.Search(viewModel);

            // Assert
            RedirectToRouteResult redirectResult = result as RedirectToRouteResult;
            Assert.NotNull(redirectResult);

            Assert.Equal("Details", redirectResult.RouteValues["action"]);
            Assert.Equal("WEE/AA1111AA", redirectResult.RouteValues["RegistrationNumber"]);
        }

        [Fact]
        public async Task GetSearchResults_DoesSearchForTenResultsAndReturnsSearchReturnsView()
        {
            // Arrange
            BreadcrumbService breadcrumb = A.Dummy<BreadcrumbService>();

            List<ProducerSearchResult> fakeResults = new List<ProducerSearchResult>()
            {
                new ProducerSearchResult()
                {
                    Name = "Producer1",
                    RegistrationNumber = "WEE/AA1111AA"
                }
            };

            ISearcher<ProducerSearchResult> producerSearcher = A.Fake<ISearcher<ProducerSearchResult>>();
            A.CallTo(() => producerSearcher.Search("testSearchTerm", 10, false))
                .Returns(fakeResults);

            Func<IWeeeClient> weeeClient = A.Dummy<Func<IWeeeClient>>();

            ProducersController controller = new ProducersController(breadcrumb, producerSearcher, weeeClient);

            // Act
            ActionResult result = await controller.SearchResults("testSearchTerm");

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);

            Assert.True(string.IsNullOrEmpty(viewResult.ViewName) || viewResult.ViewName.ToLowerInvariant() == "searchresults");
            
            SearchResultsViewModel viewModel = viewResult.Model as SearchResultsViewModel;
            Assert.NotNull(viewModel);

            Assert.Contains(viewModel.Results, r => r.RegistrationNumber == "WEE/AA1111AA");
        }

        [Fact]
        public async Task PostSearchResults_WithInvalidModel_DoesSearchForTenResultsAndReturnsSearchReturnsView()
        {
            // Arrange
            BreadcrumbService breadcrumb = A.Dummy<BreadcrumbService>();

            List<ProducerSearchResult> fakeResults = new List<ProducerSearchResult>()
            {
                new ProducerSearchResult()
                {
                    Name = "Producer1",
                    RegistrationNumber = "WEE/AA1111AA"
                }
            };

            ISearcher<ProducerSearchResult> producerSearcher = A.Fake<ISearcher<ProducerSearchResult>>();
            A.CallTo(() => producerSearcher.Search("testSearchTerm", 10, false))
                .Returns(fakeResults);

            Func<IWeeeClient> weeeClient = A.Dummy<Func<IWeeeClient>>();

            ProducersController controller = new ProducersController(breadcrumb, producerSearcher, weeeClient);

            SearchResultsViewModel viewModel = new SearchResultsViewModel();
            viewModel.SearchTerm = "testSearchTerm";
            controller.ModelState.AddModelError("SomeProperty", "Exception");

            // Act
            ActionResult result = await controller.SearchResults(viewModel);

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);

            Assert.True(string.IsNullOrEmpty(viewResult.ViewName) || viewResult.ViewName.ToLowerInvariant() == "searchresults");

            SearchResultsViewModel resultsViewModel = viewResult.Model as SearchResultsViewModel;
            Assert.NotNull(resultsViewModel);

            Assert.Contains(resultsViewModel.Results, r => r.RegistrationNumber == "WEE/AA1111AA");
        }

        [Fact]
        public async Task PostSearchResults_WithSelectedPRN_RedirectsToDetailsAction()
        {
            // Arrange
            BreadcrumbService breadcrumb = A.Dummy<BreadcrumbService>();
            ISearcher<ProducerSearchResult> producerSearcher = A.Dummy<ISearcher<ProducerSearchResult>>();

            Func<IWeeeClient> weeeClient = A.Dummy<Func<IWeeeClient>>();

            ProducersController controller = new ProducersController(breadcrumb, producerSearcher, weeeClient);

            SearchResultsViewModel viewModel = new SearchResultsViewModel()
            {
                SelectedRegistrationNumber = "WEE/AA1111AA"
            };

            // Act
            ActionResult result = await controller.SearchResults(viewModel);

            // Assert
            RedirectToRouteResult redirectResult = result as RedirectToRouteResult;
            Assert.NotNull(redirectResult);

            Assert.Equal("Details", redirectResult.RouteValues["action"]);
            Assert.Equal("WEE/AA1111AA", redirectResult.RouteValues["RegistrationNumber"]);
        }

        [Fact]
        public async Task GetDetails_FetchesDetailsFromApiAndReturnsDetailsView()
        {
            // Arrange
            BreadcrumbService breadcrumb = A.Dummy<BreadcrumbService>();
            ISearcher<ProducerSearchResult> producerSearcher = A.Dummy<ISearcher<ProducerSearchResult>>();

            ProducerDetails producerDetails = A.Dummy<ProducerDetails>();

            IWeeeClient weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetProducerDetails>._))
                .WhenArgumentsMatch(a => ((GetProducerDetails)a[1]).RegistrationNumber == "WEE/AA1111AA")
                .Returns(producerDetails);
            
            Func<IWeeeClient> weeeClientFunc = A.Fake<Func<IWeeeClient>>();
            A.CallTo(() => weeeClientFunc())
                .Returns(weeeClient);

            ProducersController controller = new ProducersController(breadcrumb, producerSearcher, weeeClientFunc);

            // Act
            ActionResult result = await controller.Details("WEE/AA1111AA");

            // Assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetProducerDetails>._))
                .WhenArgumentsMatch(a => ((GetProducerDetails)a[1]).RegistrationNumber == "WEE/AA1111AA")
                .MustHaveHappened();
            
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);

            Assert.True(string.IsNullOrEmpty(viewResult.ViewName) || viewResult.ViewName.ToLowerInvariant() == "details");

            DetailsViewModel resultsViewModel = viewResult.Model as DetailsViewModel;
            Assert.NotNull(resultsViewModel);

            Assert.Equal(producerDetails, resultsViewModel.Details);
        }

        [Fact]
        public async void HttpGet_DownloadProducerAmendmentsCsv_ShouldReturnFileContentType()
        {
            // Arrange
            BreadcrumbService breadcrumb = A.Dummy<BreadcrumbService>();
            ISearcher<ProducerSearchResult> producerSearcher = A.Dummy<ISearcher<ProducerSearchResult>>();
            IWeeeClient weeeClient = A.Fake<IWeeeClient>();
            CSVFileData csvData = A.Dummy<CSVFileData>();
            
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetProducerAmendmentsHistoryCSV>._))
                .Returns(new CSVFileData
                {
                    FileName = "test.csv",
                    FileContent = "123,abc"
                });

            Func<IWeeeClient> weeeClientFunc = A.Fake<Func<IWeeeClient>>();
            A.CallTo(() => weeeClientFunc())
                .Returns(weeeClient);

            ProducersController controller = new ProducersController(breadcrumb, producerSearcher, weeeClientFunc);

            //Act
            var result = await controller.DownloadProducerAmendmentsCsv("WEE/AA1111AA");

            //Assert
            Assert.IsType<FileContentResult>(result);
        }

        [Fact]
        public async void HttpGet_ConfirmRemoveProducer_ShouldReturnConfirmRemoveProducerView()
        {
            // Arrange
            BreadcrumbService breadcrumb = A.Dummy<BreadcrumbService>();
            ISearcher<ProducerSearchResult> producerSearcher = A.Dummy<ISearcher<ProducerSearchResult>>();

            ProducerDetailsScheme producerDetailsScheme = A.Dummy<ProducerDetailsScheme>();

            var registeredProducerId = Guid.NewGuid();

            IWeeeClient weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetProducerDetailsByRegisteredProducerId>._))
                .WhenArgumentsMatch(a => ((GetProducerDetailsByRegisteredProducerId)a[1]).RegisteredProducerId == registeredProducerId)
                .Returns(producerDetailsScheme);

            Func<IWeeeClient> weeeClientFunc = A.Fake<Func<IWeeeClient>>();
            A.CallTo(() => weeeClientFunc())
                .Returns(weeeClient);

            ProducersController controller = new ProducersController(breadcrumb, producerSearcher, weeeClientFunc);

            // Act
            ActionResult result = await controller.ConfirmRemoveProducer(registeredProducerId);

            // Assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetProducerDetailsByRegisteredProducerId>._))
                .WhenArgumentsMatch(a => ((GetProducerDetailsByRegisteredProducerId)a[1]).RegisteredProducerId == registeredProducerId)
                .MustHaveHappened();

            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);

            Assert.True(string.IsNullOrEmpty(viewResult.ViewName) || viewResult.ViewName.ToLowerInvariant() == "confirmremoveproducer");

            ConfirmRemoveProducerViewModel resultsViewModel = viewResult.Model as ConfirmRemoveProducerViewModel;
            Assert.NotNull(resultsViewModel);
        }

        [Fact]
        public async Task HttpPost_WithInvalidModel_ReturnsConfirmRemoveProducerView()
        {
            // Arrange
            BreadcrumbService breadcrumb = A.Dummy<BreadcrumbService>();
            ISearcher<ProducerSearchResult> producerSearcher = A.Dummy<ISearcher<ProducerSearchResult>>();
            Func<IWeeeClient> weeeClient = A.Dummy<Func<IWeeeClient>>();

            ProducersController controller = new ProducersController(breadcrumb, producerSearcher, weeeClient);

            ConfirmRemoveProducerViewModel viewModel = new ConfirmRemoveProducerViewModel();
            controller.ModelState.AddModelError("SomeProperty", "Exception");

            // Act
            ActionResult result = await controller.ConfirmRemoveProducer(viewModel);

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);

            Assert.True(string.IsNullOrEmpty(viewResult.ViewName) || viewResult.ViewName.ToLowerInvariant() == "confirmremoveproducer");
        }

        [Fact]
        public async Task HttpPost_WithYesOptionSelected_RedirectToRemovedProducerAction()
        {
            // Arrange
            BreadcrumbService breadcrumb = A.Dummy<BreadcrumbService>();
            ISearcher<ProducerSearchResult> producerSearcher = A.Dummy<ISearcher<ProducerSearchResult>>();
            IWeeeClient weeeClient = A.Fake<IWeeeClient>();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<RemoveProducer>._))
                .Returns(true);

            Func<IWeeeClient> weeeClientFunc = A.Fake<Func<IWeeeClient>>();
            A.CallTo(() => weeeClientFunc())
                .Returns(weeeClient);

            ProducersController controller = new ProducersController(breadcrumb, producerSearcher, weeeClientFunc);

            ConfirmRemoveProducerViewModel viewModel = new ConfirmRemoveProducerViewModel();
            viewModel.SelectedValue = "Yes";

            // Act
            ActionResult result = await controller.ConfirmRemoveProducer(viewModel);

            // Assert
            var redirectToRouteResult = ((RedirectToRouteResult)result);

            Assert.Equal("RemovedProducer", redirectToRouteResult.RouteValues["action"]);
        }

        [Fact]
        public async Task HttpPost_WithNoOptionSelected_RedirectToDetailsAction()
        {
            // Arrange
            BreadcrumbService breadcrumb = A.Dummy<BreadcrumbService>();
            ISearcher<ProducerSearchResult> producerSearcher = A.Dummy<ISearcher<ProducerSearchResult>>();
            Func<IWeeeClient> weeeClient = A.Dummy<Func<IWeeeClient>>();

            ProducersController controller = new ProducersController(breadcrumb, producerSearcher, weeeClient);

            ConfirmRemoveProducerViewModel viewModel = new ConfirmRemoveProducerViewModel();
            viewModel.SelectedValue = "No";

            // Act
            ActionResult result = await controller.ConfirmRemoveProducer(viewModel);

            // Assert
            var redirectToRouteResult = ((RedirectToRouteResult)result);

            Assert.Equal("Details", redirectToRouteResult.RouteValues["action"]);
        }

        [Fact]
        public async void HttpGet_RemovedProducer_ShouldReturnRemovedProducerView()
        {
            // Arrange
            BreadcrumbService breadcrumb = A.Dummy<BreadcrumbService>();
            ISearcher<ProducerSearchResult> producerSearcher = A.Dummy<ISearcher<ProducerSearchResult>>();
            Func<IWeeeClient> weeeClientFunc = A.Fake<Func<IWeeeClient>>();
            
            ProducersController controller = new ProducersController(breadcrumb, producerSearcher, weeeClientFunc);

            // Act
            ActionResult result = await controller.RemovedProducer("WEE/MM0001AA", 2016, "SchemeName");

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);

            Assert.True(string.IsNullOrEmpty(viewResult.ViewName) || viewResult.ViewName.ToLowerInvariant() == "removedproducer");
        }

        [Fact]
        public async void HttpPost_RemovedProducer_ProducerAssociateWithOtherScheme_ShouldRedirectToDetailsAction()
        {
            // Arrange
            BreadcrumbService breadcrumb = A.Dummy<BreadcrumbService>();
            ISearcher<ProducerSearchResult> producerSearcher = A.Dummy<ISearcher<ProducerSearchResult>>();
            
            var registeredNumber = "WEE/MM0001AA";

            IWeeeClient weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<IsProducerAssociateWithAnotherScheme>._))
                .WhenArgumentsMatch(a => ((IsProducerAssociateWithAnotherScheme)a[1]).RegistrationNumber == registeredNumber)
                .Returns(true);

            Func<IWeeeClient> weeeClientFunc = A.Fake<Func<IWeeeClient>>();
            A.CallTo(() => weeeClientFunc()).Returns(weeeClient);

            ProducersController controller = new ProducersController(breadcrumb, producerSearcher, weeeClientFunc);

            // Act
            RemovedProducerViewModel model = new RemovedProducerViewModel
            {
                RegistrationNumber = registeredNumber
            };
            ActionResult result = await controller.RemovedProducer(model);

            // Assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<IsProducerAssociateWithAnotherScheme>._))
                .WhenArgumentsMatch(a => ((IsProducerAssociateWithAnotherScheme)a[1]).RegistrationNumber == registeredNumber)
                .MustHaveHappened();
            
            var redirectToRouteResult = ((RedirectToRouteResult)result);

            Assert.Equal("Details", redirectToRouteResult.RouteValues["action"]);
        }

        [Fact]
        public async void HttpPost_RemovedProducer_ProducerNotAssociateWithOtherScheme_ShouldRedirectToSearchAction()
        {
            // Arrange
            BreadcrumbService breadcrumb = A.Dummy<BreadcrumbService>();
            ISearcher<ProducerSearchResult> producerSearcher = A.Dummy<ISearcher<ProducerSearchResult>>();

            var registeredNumber = "WEE/MM0001AA";

            IWeeeClient weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<IsProducerAssociateWithAnotherScheme>._))
                .WhenArgumentsMatch(a => ((IsProducerAssociateWithAnotherScheme)a[1]).RegistrationNumber == registeredNumber)
                .Returns(false);

            Func<IWeeeClient> weeeClientFunc = A.Fake<Func<IWeeeClient>>();
            A.CallTo(() => weeeClientFunc()).Returns(weeeClient);

            ProducersController controller = new ProducersController(breadcrumb, producerSearcher, weeeClientFunc);

            // Act
            RemovedProducerViewModel model = new RemovedProducerViewModel
            {
                RegistrationNumber = registeredNumber
            };
            ActionResult result = await controller.RemovedProducer(model);

            // Assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<IsProducerAssociateWithAnotherScheme>._))
                .WhenArgumentsMatch(a => ((IsProducerAssociateWithAnotherScheme)a[1]).RegistrationNumber == registeredNumber)
                .MustHaveHappened();

            var redirectToRouteResult = ((RedirectToRouteResult)result);

            Assert.Equal("Search", redirectToRouteResult.RouteValues["action"]);
        }
    }
}
