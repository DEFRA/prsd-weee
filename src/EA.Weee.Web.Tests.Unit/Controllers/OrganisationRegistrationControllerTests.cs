namespace EA.Weee.Web.Tests.Unit.Controllers
{
    using Api.Client;
    using Core.Organisations;
    using Core.Shared;
    using EA.Prsd.Core.Extensions;
    using EA.Weee.Core.Search;
    using EA.Weee.Web.Areas.Admin.ViewModels.AddOrganisation.Details;
    using EA.Weee.Web.ViewModels.OrganisationRegistration.Type;
    using FakeItEasy;
    using FluentAssertions;
    using Services;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Web.Controllers;
    using Web.ViewModels.OrganisationRegistration;
    using Weee.Requests.Organisations;
    using Xunit;

    public class OrganisationRegistrationControllerTests
    {
        private readonly Func<IWeeeClient> weeeClient;
        private readonly ISearcher<OrganisationSearchResult> organisationSearcher;
        private readonly ConfigurationService configurationService;
        private readonly IOrganisationTransactionService transactionService;
        private readonly OrganisationRegistrationController controller;

        public OrganisationRegistrationControllerTests()
        {
            configurationService = A.Fake<ConfigurationService>();
            transactionService = A.Fake<IOrganisationTransactionService>();

            weeeClient = A.Fake<Func<IWeeeClient>>();
            organisationSearcher = A.Fake<ISearcher<OrganisationSearchResult>>();
            configurationService = A.Fake<ConfigurationService>();
            transactionService = A.Fake<IOrganisationTransactionService>();

            controller = new OrganisationRegistrationController(
                weeeClient,
                organisationSearcher,
                configurationService,
                transactionService);

            A.CallTo(() => configurationService.CurrentConfiguration.MaximumOrganisationSearchResults).Returns(5);
        }

        [Fact]
        public async Task GetJoinOrganisationConfirmation_ReturnsJoinOrganisationConfirmationView()
        {
            // Arrange
            var orgData = new PublicOrganisationData
            {
                Id = Guid.NewGuid(),
                DisplayName = "Test"
            };

            var weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetPublicOrganisationInfo>._))
                .Returns(orgData);

            var organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            var controller = new OrganisationRegistrationController(
                () => weeeClient,
                organisationSearcher,
                configurationService,
                transactionService);

            // Act
            ActionResult result = await controller.JoinOrganisationConfirmation(orgData.Id, true);

            // Assert
            var model = ((ViewResult)result).Model;

            Assert.NotNull(model);
            Assert.IsType<JoinOrganisationConfirmationViewModel>(model);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task GetJoinOrganisationConfirmation_GivenActiveUsers_ActiveUsersIsSetInViewModel(bool activeUsers)
        {
            // Arrange
            var orgData = new PublicOrganisationData
            {
                Id = Guid.NewGuid(),
                DisplayName = "Test"
            };

            var weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetPublicOrganisationInfo>._))
                .Returns(orgData);

            var organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            var controller = new OrganisationRegistrationController(
                () => weeeClient,
                organisationSearcher,
                configurationService,
                transactionService);

            // Act
            ActionResult result = await controller.JoinOrganisationConfirmation(orgData.Id, activeUsers);

            // Assert
            var model = ((ViewResult)result).Model;

            JoinOrganisationConfirmationViewModel viewModel = model as JoinOrganisationConfirmationViewModel;

            viewModel.AnyActiveUsers.Should().Be(activeUsers);
        }

        [Fact]
        public async Task GetJoinOrganisation_ReturnsView()
        {
            // Arrange
            var weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetPublicOrganisationInfo>._))
                .Returns(new PublicOrganisationData());

            var organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            var controller = new OrganisationRegistrationController(
                () => weeeClient,
                organisationSearcher,
                configurationService, transactionService);

            // Act
            ActionResult result = await controller.JoinOrganisation(A.Dummy<Guid>());

            // Assert
            var model = ((ViewResult)result).Model;

            Assert.NotNull(model);
            Assert.IsType<JoinOrganisationViewModel>(model);
        }

        [Fact]
        public async Task GetJoinOrganisation_GivenActiveUsers_ReturnsViewWithActiveUsersSet()
        {
            // Arrange
            var weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetPublicOrganisationInfo>._))
                .Returns(new PublicOrganisationData());

            var organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            var controller = new OrganisationRegistrationController(
                () => weeeClient,
                organisationSearcher,
                configurationService, transactionService);

            var activeUsers = new List<OrganisationUserData>()
            {
                A.Fake<OrganisationUserData>()
            };

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetActiveOrganisationUsers>._)).Returns(activeUsers);

            // Act
            ActionResult result = await controller.JoinOrganisation(A.Dummy<Guid>());

            // Assert
            var model = ((ViewResult)result).Model;

            JoinOrganisationViewModel viewModel = model as JoinOrganisationViewModel;

            viewModel.AnyActiveUsers.Should().Be(true);
        }

        [Fact]
        public async Task GetJoinOrganisation_GivenNoActiveUsers_ReturnsViewWithActiveUsersSet()
        {
            // Arrange
            var weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetPublicOrganisationInfo>._))
                .Returns(new PublicOrganisationData());

            var organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            var controller = new OrganisationRegistrationController(
                () => weeeClient,
                organisationSearcher,
                configurationService, transactionService);

            var activeUsers = new List<OrganisationUserData>();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetActiveOrganisationUsers>._)).Returns(activeUsers);

            // Act
            ActionResult result = await controller.JoinOrganisation(A.Dummy<Guid>());

            // Assert
            var model = ((ViewResult)result).Model;

            JoinOrganisationViewModel viewModel = model as JoinOrganisationViewModel;

            viewModel.AnyActiveUsers.Should().Be(false);
        }

        [Fact]
        public async Task GetJoinOrganisation_UserAlreadyAssociated_ReturnsUserAlreadyAssociatedWithOrgansiationView()
        {
            // Arrange
            var organisationId = new Guid("101F5E58-FEA3-4F59-9281-E543EDE5699F");

            var weeeClient = A.Fake<IWeeeClient>();

            var organisation = new PublicOrganisationData()
            {
                Id = organisationId,
                DisplayName = "Test Company"
            };

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetPublicOrganisationInfo>._))
                .Returns(organisation);

            var association = new OrganisationUserData()
            {
                OrganisationId = organisationId,
                UserStatus = UserStatus.Active,
            };

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetUserOrganisationsByStatus>._))
                .Returns(new List<OrganisationUserData>() { association });

            var organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            var activeUsers = new List<OrganisationUserData>()
            {
                A.Fake<OrganisationUserData>()
            };

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetActiveOrganisationUsers>._)).Returns(activeUsers);

            var controller = new OrganisationRegistrationController(
                () => weeeClient,
                organisationSearcher,
                configurationService, transactionService);

            // Act
            ActionResult result = await controller.JoinOrganisation(organisationId);

            // Assert
            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);

            Assert.Equal("UserAlreadyAssociatedWithOrganisation", viewResult.ViewName);

            var viewModel = viewResult.Model as UserAlreadyAssociatedWithOrganisationViewModel;
            Assert.NotNull(viewModel);

            Assert.Equal(organisationId, viewModel.OrganisationId);
            Assert.Equal(UserStatus.Active, viewModel.Status);
            Assert.Equal("Test Company", viewModel.OrganisationName);
            Assert.True(viewModel.AnyActiveUsers);
        }

        [Fact]
        public async Task PostJoinOrganisation_NoSearchAnotherOrganisationSelected_RedirectsToType()
        {
            // Arrange
            var weeeClient = A.Dummy<IWeeeClient>();
            var organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            var controller = new OrganisationRegistrationController(
                () => weeeClient,
                organisationSearcher,
                configurationService, transactionService);

            var model = new JoinOrganisationViewModel { SelectedValue = "No" };

            // Act
            var result = await controller.JoinOrganisation(model);

            // Assert
            var redirectToRouteResult = ((RedirectToRouteResult)result);

            Assert.Equal("Search", redirectToRouteResult.RouteValues["action"]);
        }

        [Fact]
        public async Task PostJoinOrganisation_YesJoinOrganisationSelected_RedirectsToJoinOrganisationConfirmation()
        {
            // Arrange
            var weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<JoinOrganisation>._))
                .Returns(Guid.NewGuid());

            var organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            var controller = new OrganisationRegistrationController(
                () => weeeClient,
                organisationSearcher,
                configurationService, transactionService);

            var model = new JoinOrganisationViewModel { SelectedValue = "Yes - join xyz" };

            // Act
            var result = await controller.JoinOrganisation(model);

            // Assert
            var redirectToRouteResult = ((RedirectToRouteResult)result);

            Assert.Equal("JoinOrganisationConfirmation", redirectToRouteResult.RouteValues["action"]);
        }

        [Fact]
        public async Task GetSearch_UserHasNotOrganisation_ShowPerformAnotherActivityLinkIsFalse()
        {
            // Arrange
            var weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetUserOrganisationsByStatus>._))
               .Returns(new List<OrganisationUserData>());

            var organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            var controller = new OrganisationRegistrationController(
                () => weeeClient,
                organisationSearcher,
                configurationService, transactionService);

            // Act
            var result = await controller.Search();

            // Assert
            var model = ((ViewResult)result).Model as SearchViewModel;

            Assert.NotNull(model);
            Assert.False(model.ShowPerformAnotherActivityLink);
        }

        [Fact]
        public async Task GetSearch_UserHasOrganisations_ShowPerformAnotherActivityLinkIsTrue()
        {
            // Arrange
            var weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetUserOrganisationsByStatus>._))
               .Returns(new List<OrganisationUserData>
               {
                   new OrganisationUserData(),
                   new OrganisationUserData()
               });

            var organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            var controller = new OrganisationRegistrationController(
                () => weeeClient,
                organisationSearcher,
                configurationService, transactionService);

            // Act
            var result = await controller.Search();

            // Assert
            var model = ((ViewResult)result).Model as SearchViewModel;

            Assert.NotNull(model);
            Assert.True(model.ShowPerformAnotherActivityLink);
        }
        
        [Fact]
        public async Task GetSearch_ReturnsSearchView()
        {
            // Arrange
            var organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();
            var weeeClient = A.Dummy<Func<IWeeeClient>>();

            var controller = new OrganisationRegistrationController(
                weeeClient,
                organisationSearcher,
                configurationService,
                transactionService);

            // Act
            var result = await controller.Search();

            // Assert
            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);

            Assert.True(string.IsNullOrEmpty(viewResult.ViewName) || viewResult.ViewName.ToLowerInvariant() == "search");
        }

        [Fact]
        public async Task PostSearch_WithInvalidModel_ReturnsSearchView()
        {
            // Arrange
            var organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();
            var weeeClient = A.Dummy<Func<IWeeeClient>>();

            var controller = new OrganisationRegistrationController(
                weeeClient,
                organisationSearcher,
                configurationService,
                transactionService);

            var viewModel = new SearchViewModel();
            controller.ModelState.AddModelError("SomeProperty", "Exception");

            // Act
            var result = await controller.Search(viewModel);

            // Assert
            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);

            Assert.True(string.IsNullOrEmpty(viewResult.ViewName) || viewResult.ViewName.ToLowerInvariant() == "search");
        }

        [Fact]
        public async Task PostSearch_WithSearchTermAndNoSelectedOrganisationId_RedirectsToSearchResultsAction()
        {
            // Arrange
            var organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();
            var weeeClient = A.Dummy<Func<IWeeeClient>>();

            var controller = new OrganisationRegistrationController(
                weeeClient,
                organisationSearcher,
                configurationService,
                transactionService);

            var viewModel = new SearchViewModel { SearchTerm = "testSearchTerm", SelectedOrganisationId = null };

            // Act
            var result = await controller.Search(viewModel);

            // Assert
            var redirectResult = result as RedirectToRouteResult;
            Assert.NotNull(redirectResult);

            Assert.Equal("SearchResults", redirectResult.RouteValues["action"]);
            Assert.Equal("testSearchTerm", redirectResult.RouteValues["SearchTerm"]);
        }

        [Fact]
        public async Task PostSearch_WithSelectedOrganisationId_RedirectsToJoinOrganisationAction()
        {
            // Arrange
            var organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();
            var weeeClient = A.Dummy<Func<IWeeeClient>>();

            var controller = new OrganisationRegistrationController(
                weeeClient,
                organisationSearcher,
                configurationService, transactionService);

            var viewModel = new SearchViewModel
            {
                SearchTerm = "Test Company",
                SelectedOrganisationId = new Guid("05DF9AE8-DACE-4173-A227-16933EB5D5F8")
            };

            // Act
            var result = await controller.Search(viewModel);

            // Assert
            var redirectResult = result as RedirectToRouteResult;
            Assert.NotNull(redirectResult);

            Assert.Equal("JoinOrganisation", redirectResult.RouteValues["action"]);
            Assert.Equal(redirectResult.RouteValues["OrganisationId"], new Guid("05DF9AE8-DACE-4173-A227-16933EB5D5F8"));
        }

        [Fact]
        public async Task GetSearchResults_DoesSearchForFiveResultsAndReturnsSearchReturnsView()
        {
            // Arrange
            var fakeResults = new List<OrganisationSearchResult>()
            {
                new OrganisationSearchResult()
                {
                    OrganisationId = new Guid("05DF9AE8-DACE-4173-A227-16933EB5D5F8"),
                    Name = "Test Company",
                }
            };

            var organisationSearcher = A.Fake<ISearcher<OrganisationSearchResult>>();
            A.CallTo(() => organisationSearcher.Search("testSearchTerm", 5, false))
                .Returns(fakeResults);

            var weeeClient = A.Dummy<Func<IWeeeClient>>();

            var controller = new OrganisationRegistrationController(
                weeeClient,
                organisationSearcher,
                configurationService,
                transactionService);

            // Act
            var result = await controller.SearchResults("testSearchTerm");

            // Assert
            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);

            Assert.True(string.IsNullOrEmpty(viewResult.ViewName) || viewResult.ViewName.ToLowerInvariant() == "searchresults");

            var viewModel = viewResult.Model as SearchResultsViewModel;
            Assert.NotNull(viewModel);

            Assert.Contains(viewModel.Results, r => r.OrganisationId == new Guid("05DF9AE8-DACE-4173-A227-16933EB5D5F8"));
        }

        [Fact]
        public async Task PostSearchResults_WithInvalidModel_DoesSearchForFiveResultsAndReturnsSearchReturnsView()
        {
            // Arrange
            var fakeResults = new List<OrganisationSearchResult>()
            {
                new OrganisationSearchResult()
                {
                    OrganisationId = new Guid("05DF9AE8-DACE-4173-A227-16933EB5D5F8"),
                    Name = "Test Company",
                }
            };

            var organisationSearcher = A.Fake<ISearcher<OrganisationSearchResult>>();
            A.CallTo(() => organisationSearcher.Search("testSearchTerm", 5, false))
                .Returns(fakeResults);

            var weeeClient = A.Dummy<Func<IWeeeClient>>();

            var controller = new OrganisationRegistrationController(
                weeeClient,
                organisationSearcher,
                configurationService,
                transactionService);

            var viewModel = new SearchResultsViewModel { SearchTerm = "testSearchTerm" };
            controller.ModelState.AddModelError("SomeProperty", "Exception");

            // Act
            var result = await controller.SearchResults(viewModel);

            // Assert
            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);

            Assert.True(string.IsNullOrEmpty(viewResult.ViewName) || viewResult.ViewName.ToLowerInvariant() == "searchresults");

            var resultsViewModel = viewResult.Model as SearchResultsViewModel;
            Assert.NotNull(resultsViewModel);

            Assert.Contains(resultsViewModel.Results, r => r.OrganisationId == new Guid("05DF9AE8-DACE-4173-A227-16933EB5D5F8"));
        }

        [Fact]
        public async Task PostSearchResults_WithSelectedOrganisationId_RedirectsToJoinOrganisationAction()
        {
            // Arrange
            var organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            var weeeClient = A.Dummy<Func<IWeeeClient>>();

            var controller = new OrganisationRegistrationController(
                weeeClient,
                organisationSearcher,
                configurationService,
                transactionService);

            var viewModel = new SearchResultsViewModel()
            {
                SelectedOrganisationId = new Guid("05DF9AE8-DACE-4173-A227-16933EB5D5F8")
            };

            // Act
            var result = await controller.SearchResults(viewModel);

            // Assert
            var redirectResult = result as RedirectToRouteResult;
            Assert.NotNull(redirectResult);

            Assert.Equal("JoinOrganisation", redirectResult.RouteValues["action"]);
            Assert.Equal(redirectResult.RouteValues["OrganisationId"], new Guid("05DF9AE8-DACE-4173-A227-16933EB5D5F8"));
        }

        [Fact]
        public void TypeGet_ReturnsViewWithViewModel()
        {
            var organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            var weeeClient = A.Dummy<Func<IWeeeClient>>();

            var controller = new OrganisationRegistrationController(
               weeeClient,
               organisationSearcher,
               configurationService,
               transactionService);

            var result = controller.Type() as ViewResult;

            var resultViewModel = result.Model as ExternalOrganisationTypeViewModel;

            Assert.True(string.IsNullOrEmpty(result.ViewName) || result.ViewName == "Type");
            Assert.NotNull(resultViewModel);
        }

        [Theory]
        [InlineData("Sole trader", "Index")]
        [InlineData("Partnership", "Index")]
        [InlineData("Registered company", "Index")]
        public async Task TypePost_ValidViewModel_ReturnsCorrectRedirect(string selectedValue, string action)
        {
            var organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            var weeeClient = A.Dummy<Func<IWeeeClient>>();

            var controller = new OrganisationRegistrationController(
               weeeClient,
               organisationSearcher,
               configurationService,
               transactionService);

            var viewModel = new ExternalOrganisationTypeViewModel()
            {
                SelectedValue = selectedValue,
            };

            var result = await controller.Type(viewModel) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be(action);
            result.RouteValues["controller"].Should().Be("Holding");
            result.RouteValues["organisationType"].Should().Be(viewModel.SelectedValue);
        }

        [Fact]
        public async Task SoleTraderDetailsGet_ReturnsViewWithViewModelPopulated()
        {
            var organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            var weeeClient = A.Dummy<Func<IWeeeClient>>();

            var controller = new OrganisationRegistrationController(
               weeeClient,
               organisationSearcher,
               configurationService,
               transactionService);

            const string searchText = "Company";
            const string organisationType = "Sole trader";

            var result = await controller.SoleTraderDetails(organisationType, searchText) as ViewResult;

            var resultViewModel = result.Model as SoleTraderDetailsViewModel;

            Assert.Equal(searchText, resultViewModel.CompanyName);
            Assert.Equal(organisationType, resultViewModel.OrganisationType);
            //Assert.Equal(countries, resultViewModel.Address.Countries);

            Assert.True(string.IsNullOrEmpty(result.ViewName) || result.ViewName == "SoleTraderOrPartnershipDetails");
        }

        [Fact]
        public async Task TonnageTypeGet_ReturnsViewWithViewModel_WithSearchText()
        {
            // Arrange
            var organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();
            var weeeClient = A.Dummy<Func<IWeeeClient>>();

            var controller = new OrganisationRegistrationController(
                weeeClient,
                organisationSearcher,
                configurationService,
                transactionService);

            const string searchText = "company";

            // Act
            var result = await controller.TonnageType(searchText);

            // Assert
            var resultViewModel = result.Model as TonnageTypeViewModel;
            Assert.NotNull(result);
            Assert.Equal(searchText, resultViewModel.SearchedText);
            Assert.True(string.IsNullOrEmpty(result.ViewName) || result.ViewName == "Tonnage");
        }

        [Fact]
        public async void TonnageTypePost_ModelNotValid_ReturnsView()
        {
            // Arrange
            var organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();
            var weeeClient = A.Dummy<Func<IWeeeClient>>();

            var controller = new OrganisationRegistrationController(
                weeeClient,
                organisationSearcher,
                configurationService,
                transactionService);

            controller.ModelState.AddModelError("error", "error");

            const string searchText = "company";
            var viewModel = new TonnageTypeViewModel()
            {
                SearchedText = searchText
            };

            // Act
            var result = await controller.TonnageType(viewModel) as ViewResult;

            // Assert
            var resultViewModel = result.Model as TonnageTypeViewModel;
            Assert.NotNull(result);
            Assert.Equal(searchText, resultViewModel.SearchedText);
            Assert.True(string.IsNullOrEmpty(result.ViewName) || result.ViewName == "Tonnage");
        }

        [Theory]
        [InlineData("5 tonnes or more", "FiveTonnesOrMore", "OrganisationRegistration")]
        [InlineData("Less than 5 tonnes", "PreviousRegistration", "OrganisationRegistration")]
        public async Task TonnageTypePost_ValidViewModel_ReturnsCorrectRedirect(string selectedValue, string action, string correctController)
        {
            // Arrange
            var organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            var weeeClient = A.Dummy<Func<IWeeeClient>>();

            var controller = new OrganisationRegistrationController(
                weeeClient,
                organisationSearcher,
                configurationService,
                transactionService);

            const string searchText = "company";
            var viewModel = new TonnageTypeViewModel()
            {
                SearchedText = searchText,
                SelectedValue = selectedValue
            };

            // Act
            var result = await controller.TonnageType(viewModel) as RedirectToRouteResult;

            // Assert
            Assert.NotNull(result);
            result.RouteValues["action"].Should().Be(action);
            result.RouteValues["controller"].Should().Be(correctController);

            var tonnageType = selectedValue.GetValueFromDisplayName<TonnageType>();

            if (tonnageType == Core.Organisations.TonnageType.FiveTonnesOrMore)
            {
                A.CallTo(() => transactionService.CaptureData(A<string>._, A<object>._)).MustNotHaveHappened();
            }
            else
            {
                A.CallTo(() =>
                        transactionService.CaptureData(A<string>._, A<TonnageTypeViewModel>.That.IsSameAs(viewModel)))
                    .MustHaveHappenedOnceExactly();
            }
        }

        [Fact]
        public async void FiveTonnesOrMoreGet_ReturnsView()
        {
            // Arrange
            var organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();
            var weeeClient = A.Dummy<Func<IWeeeClient>>();

            var controller = new OrganisationRegistrationController(
                weeeClient,
                organisationSearcher,
                configurationService,
                transactionService);

            // Act
            var result = await controller.FiveTonnesOrMore();

            // Assert
            Assert.True(string.IsNullOrEmpty(result.ViewName) || result.ViewName == "FiveTonnesOrMore");
        }

        [Fact]
        public async Task PreviousRegistration_Post_WhenModelValid_CapturesDataAndRedirectsToType()
        {
            // Arrange
            var model = new PreviousRegistrationViewModel { SelectedValue = "No" };

            // Act
            var result = await controller.PreviousRegistration(model) as RedirectToRouteResult;

            // Assert
            A.CallTo(() => transactionService.CaptureData(A<string>._, model))
                .MustHaveHappenedOnceExactly();
            result.Should().NotBeNull();
            result.RouteValues["action"].Should().Be("Type");
            result.RouteValues["controller"].Should().Be("OrganisationRegistration");
        }

        [Fact]
        public async Task PreviousRegistration_Get_RetrievesTransactionDataAndPopulatesViewModel()
        {
            // Arrange
            var organisationTransactionData = new OrganisationTransactionData()
            {
                PreviousRegistration = YesNoType.Yes,
                SearchTerm = "Test Company"
            };

            A.CallTo(() => transactionService.GetOrganisationTransactionData(A<string>._))
                .Returns(organisationTransactionData);

            // Act
            var result = await controller.PreviousRegistration() as ViewResult;

            // Assert
            result.Should().NotBeNull();
            var model = result.Model as PreviousRegistrationViewModel;
            model.Should().NotBeNull();
            model.SelectedValue.Should().Be("Yes");
            model.SearchText.Should().Be("Test Company");
        }

        [Fact]
        public async Task RegisterSmallProducer_ClearsTransactionDataAndRedirectsToTonnageType()
        {
            // Arrange
            const string searchTerm = "Test Company";

            // Act
            var result = await controller.RegisterSmallProducer(searchTerm) as RedirectToRouteResult;

            // Assert
            A.CallTo(() => transactionService.DeleteOrganisationTransactionData(A<string>._))
                .MustHaveHappenedOnceExactly();

            result.Should().NotBeNull();
            result.RouteValues["action"].Should().Be("TonnageType");
            result.RouteValues["searchTerm"].Should().Be(searchTerm);
            result.RouteValues["controller"].Should().BeNull();
        }
    }
}
