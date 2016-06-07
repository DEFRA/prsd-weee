namespace EA.Weee.Web.Tests.Unit.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using Core.Organisations;
    using Core.Shared;
    using EA.Weee.Core.Search;
    using FakeItEasy;
    using ViewModels.OrganisationRegistration;
    using ViewModels.OrganisationRegistration.Details;
    using ViewModels.OrganisationRegistration.Type;
    using ViewModels.Shared;
    using Web.Controllers;
    using Weee.Requests.Organisations;
    using Xunit;

    public class OrganisationRegistrationControllerTests
    {
        [Fact]
        public async Task GetRegisteredOfficeAddress_ApiThrowsException_ExceptionShouldNotBeCaught()
        {
            // Arrange
            IWeeeClient weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._))
                .Throws<Exception>();

            ISearcher<OrganisationSearchResult> organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            OrganisationRegistrationController controller = new OrganisationRegistrationController(
                () => weeeClient,
                organisationSearcher);

            // Act
            Func<Task<ActionResult>> action = async () => await controller.RegisteredOfficeAddress(A.Dummy<Guid>());

            // Assert
            await Assert.ThrowsAnyAsync<Exception>(action);
        }

        [Fact]
        public async Task GetRegisteredOfficeAddress_ApiReturnsOrganisationData_ReturnsViewWithModel()
        {
            // Arrange
            AddressData addressData = new AddressData();
            addressData.Address1 = "Address Line 1";

            OrganisationData organisationData = new OrganisationData();
            organisationData.Id = new Guid("1B7329B9-DC7F-4621-8E97-FD97CDDDBA10");
            organisationData.OrganisationType = OrganisationType.RegisteredCompany;
            organisationData.HasBusinessAddress = true;
            organisationData.BusinessAddress = addressData;

            IWeeeClient weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._))
                .Returns(organisationData);

            ISearcher<OrganisationSearchResult> organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            OrganisationRegistrationController controller = new OrganisationRegistrationController(
                () => weeeClient,
                organisationSearcher);

            // Act
            ActionResult result = await controller.RegisteredOfficeAddress(new Guid("1B7329B9-DC7F-4621-8E97-FD97CDDDBA10"));

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);

            AddressViewModel viewModel = viewResult.Model as AddressViewModel;
            Assert.NotNull(viewModel);

            Assert.Equal(new Guid("1B7329B9-DC7F-4621-8E97-FD97CDDDBA10"), viewModel.OrganisationId);
            Assert.Equal(OrganisationType.RegisteredCompany, viewModel.OrganisationType);
            Assert.Equal("Address Line 1", viewModel.Address.Address1);
        }

        [Fact]
        public async Task PostRegisteredOfficeAddress_ModelStateIsInvalid_ReturnsViewModel()
        {
            // Arrange
            IWeeeClient weeeClient = A.Dummy<IWeeeClient>();
            ISearcher<OrganisationSearchResult> organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            OrganisationRegistrationController controller = new OrganisationRegistrationController(
                () => weeeClient,
                organisationSearcher);

            var model = new AddressViewModel();
            controller.ModelState.AddModelError("Key", "Error"); // To make the model state invalid

            // Act
            ActionResult result = await controller.RegisteredOfficeAddress(model);

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);

            Assert.Equal(model, viewResult.Model);
        }

        [Fact]
        public async void PostRegisteredOfficeAddress_PrincipalPlaceOfBusinessIsValid_CallsApiToAddAddressToOrganisation()
        {
            // Arrange
            IWeeeClient weeeClient = A.Fake<IWeeeClient>();
            ISearcher<OrganisationSearchResult> organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            OrganisationRegistrationController controller = new OrganisationRegistrationController(
                () => weeeClient,
                organisationSearcher);

            AddressViewModel model = new AddressViewModel();

            // Act
            ActionResult result = await controller.RegisteredOfficeAddress(model);

            // Assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<AddAddressToOrganisation>._))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task PostRegisteredOfficeAddress_PrincipalPlaceOfBusinessIsValid_RedirectsToReviewOrganisationDetails()
        {
            // Arrange
            IWeeeClient weeeClient = A.Fake<IWeeeClient>();
            ISearcher<OrganisationSearchResult> organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            OrganisationRegistrationController controller = new OrganisationRegistrationController(
                () => weeeClient,
                organisationSearcher);

            AddressViewModel model = new AddressViewModel();

            // Act
            ActionResult result = await controller.RegisteredOfficeAddress(model);

            // Assert
            RedirectToRouteResult redirectToRouteResult = result as RedirectToRouteResult;
            Assert.NotNull(redirectToRouteResult);

            Assert.Equal("ReviewOrganisationDetails", redirectToRouteResult.RouteValues["action"]);
        }

        [Fact]
        public async Task GetOrganisationAddress_ApiReturnsOrganisationData_ReturnsViewWithModel()
        {
            // Arrange
            IWeeeClient weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._))
                .Returns(new OrganisationData());

            ISearcher<OrganisationSearchResult> organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            OrganisationRegistrationController controller = new OrganisationRegistrationController(
                () => weeeClient,
                organisationSearcher);

            // Act
            ActionResult result = await controller.OrganisationAddress(A.Dummy<Guid>());

            // Assert
            var model = ((ViewResult)result).Model;

            Assert.NotNull(model);
            Assert.IsType<AddressViewModel>(model);
        }

        [Fact]
        public async Task PostOrganisationAddress_IrrespectiveOfCountry_RedirectsToRegisteredOfficeAddressPrepopulate()
        {
            // Arrange
            IWeeeClient weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<AddAddressToOrganisation>._))
               .Returns(Guid.NewGuid());

            ISearcher<OrganisationSearchResult> organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            OrganisationRegistrationController controller = new OrganisationRegistrationController(
                () => weeeClient,
                organisationSearcher);

            // Act
            ActionResult result = await controller.OrganisationAddress(new AddressViewModel());

            // Assert
            var redirectToRouteResult = ((RedirectToRouteResult)result);

            Assert.Equal("RegisteredOfficeAddressPrepopulate", redirectToRouteResult.RouteValues["action"]);
        }

        [Fact]
        public async Task GetMainContactPerson_OrganisationIdIsInvalid_ThrowsArgumentException()
        {
            // Arrange
            IWeeeClient weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExists>._))
                .Returns(false);

            ISearcher<OrganisationSearchResult> organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            OrganisationRegistrationController controller = new OrganisationRegistrationController(
                () => weeeClient,
                organisationSearcher);

            // Act
            Func<Task<ActionResult>> action = async () => await controller.MainContactPerson(A.Dummy<Guid>());

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(action);
        }

        [Fact]
        public async Task GetMainContactPerson_OrganisationIdIsValid_ReturnsContactPersonViewModelWithOrganisationId()
        {
            // Arrange
            IWeeeClient weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExists>._))
                .Returns(true);

            ISearcher<OrganisationSearchResult> organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            OrganisationRegistrationController controller = new OrganisationRegistrationController(
                () => weeeClient,
                organisationSearcher);

            var organisationId = new Guid("B387D62D-8615-4F67-999E-41F97F14638D");

            // Act
            var result = await controller.MainContactPerson(organisationId);

            // Assert
            var model = ((ViewResult)result).Model;

            Assert.IsType<ContactPersonViewModel>(model);
            Assert.Equal(organisationId, ((ContactPersonViewModel)model).OrganisationId);
        }

        [Fact]
        public async Task PostRegisteredOfficeAddressPrepopulate_WithYesSelection_RedirectsToSummaryPage()
        {
            // Arrange
            IWeeeClient weeeClient = A.Dummy<IWeeeClient>();
            ISearcher<OrganisationSearchResult> organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            OrganisationRegistrationController controller = new OrganisationRegistrationController(
                () => weeeClient,
                organisationSearcher);

            var model = new AddressPrepopulateViewModel();
            model.SelectedValue = "Yes";

            // Act
            ActionResult result = await controller.RegisteredOfficeAddressPrepopulate(model);

            // Assert
            var redirectToRouteResult = ((RedirectToRouteResult)result);

            Assert.Equal("ReviewOrganisationDetails", redirectToRouteResult.RouteValues["action"]);
        }

        [Fact]
        public async Task PostRegisteredOfficeAddressPrepopulate_WithNoSelection_RedirectsToRegisteredOfficeAddress()
        {
            // Arrange
            IWeeeClient weeeClient = A.Dummy<IWeeeClient>();
            ISearcher<OrganisationSearchResult> organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            OrganisationRegistrationController controller = new OrganisationRegistrationController(
                () => weeeClient,
                organisationSearcher);

            var model = new AddressPrepopulateViewModel();
            model.SelectedValue = "No";

            // Act
            ActionResult result = await controller.RegisteredOfficeAddressPrepopulate(model);

            // Assert
            var redirectToRouteResult = ((RedirectToRouteResult)result);

            Assert.Equal("RegisteredOfficeAddress", redirectToRouteResult.RouteValues["action"]);
        }

        [Fact]
        public async Task GetType_OrganisationIdIsInvalid_ThrowsArgumentException()
        {
            // Arrange
            IWeeeClient weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExistsAndIncomplete>._))
                .Returns(false);

            ISearcher<OrganisationSearchResult> organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            OrganisationRegistrationController controller = new OrganisationRegistrationController(
                () => weeeClient,
                organisationSearcher);

            // Act
            Func<Task<ActionResult>> action = async () => await controller.Type(A.Dummy<string>(), A.Dummy<Guid>());

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(action);
        }

        [Fact]
        public async Task GetType_OrganisationIdIsValid_ReturnsOrganisationTypeViewModel()
        {
            // Arrange
            IWeeeClient weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExistsAndIncomplete>._))
                .Returns(true);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._))
                .Returns(new OrganisationData());

            ISearcher<OrganisationSearchResult> organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            OrganisationRegistrationController controller = new OrganisationRegistrationController(
                () => weeeClient,
                organisationSearcher);

            // Act
            ActionResult result = await controller.Type(A.Dummy<string>(), A.Dummy<Guid>());

            // Assert
            var model = ((ViewResult)result).Model;
            Assert.IsType<OrganisationTypeViewModel>(model);
        }

        [Theory]
        [InlineData("Sole trader or individual", "SoleTraderDetails")]
        [InlineData("Partnership", "PartnershipDetails")]
        [InlineData("Registered company", "RegisteredCompanyDetails")]
        public async Task PostType_TypeDetailsSelectionWithoutOrganisationId_RedirectsToCorrectControllerAction(string selection, string action)
        {
            // Arrange
            IWeeeClient weeeClient = A.Dummy<IWeeeClient>();
            ISearcher<OrganisationSearchResult> organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            OrganisationRegistrationController controller = new OrganisationRegistrationController(
                () => weeeClient,
                organisationSearcher);

            OrganisationTypeViewModel model = new OrganisationTypeViewModel();
            model.SelectedValue = selection;
            model.OrganisationId = null;

            // Act
            ActionResult result = await controller.Type(model);

            // Assert
            var redirectToRouteResult = ((RedirectToRouteResult)result);

            Assert.Equal(action, redirectToRouteResult.RouteValues["action"]);
        }

        [Theory]
        [InlineData(OrganisationType.SoleTraderOrIndividual, "SoleTraderDetails")]
        [InlineData(OrganisationType.Partnership, "PartnershipDetails")]
        [InlineData(OrganisationType.RegisteredCompany, "RegisteredCompanyDetails")]
        public async Task PostType_TypeDetailsSelectionWithOrganisationId_RedirectsToCorrectControllerAction(OrganisationType type, string action)
        {
            // Arrange
            OrganisationData orgData = new OrganisationData
            {
                OrganisationType = type,
                Id = Guid.NewGuid()
            };

            IWeeeClient weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExistsAndIncomplete>._))
                .Returns(true);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._))
                .Returns(orgData);

            ISearcher<OrganisationSearchResult> organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            OrganisationRegistrationController controller = new OrganisationRegistrationController(
                () => weeeClient,
                organisationSearcher);

            OrganisationTypeViewModel model = new OrganisationTypeViewModel(
                type,
                new Guid("35EFE82E-0706-4E80-8AFA-D81C4B58102A"));

            // Act
            ActionResult result = await controller.Type(model);

            // Assert
            var redirectToRouteResult = ((RedirectToRouteResult)result);

            Assert.Equal(action, redirectToRouteResult.RouteValues["action"]);
        }

        [Fact]
        public async Task GetSoleTraderDetails_WithoutOrganisationId_ReturnsSoleTraderDetailsView()
        {
            // Arrange
            IWeeeClient weeeClient = A.Dummy<IWeeeClient>();
            ISearcher<OrganisationSearchResult> organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            OrganisationRegistrationController controller = new OrganisationRegistrationController(
                () => weeeClient,
                organisationSearcher);

            // Act
            ActionResult result = await controller.SoleTraderDetails();

            // Assert
            var model = ((ViewResult)result).Model;

            Assert.NotNull(model);
            Assert.IsType<SoleTraderDetailsViewModel>(model);
        }

        [Fact]
        public async Task GetSoleTraderDetails_InvalidOrganisationId_ThrowsArgumentException()
        {
            // Arrange
            IWeeeClient weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExistsAndIncomplete>._))
               .Returns(false);

            ISearcher<OrganisationSearchResult> organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            OrganisationRegistrationController controller = new OrganisationRegistrationController(
                () => weeeClient,
                organisationSearcher);

            // Act
            Func<Task<ActionResult>> action = async () => await controller.SoleTraderDetails(A.Dummy<Guid>());

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(action);
        }

        [Fact]
        public async Task GetSoleTraderDetails_WithValidOrganisationId_ReturnsSoleTraderDetailsViewWithModel()
        {
            // Arrange
            OrganisationData orgData = new OrganisationData
            {
                OrganisationType = OrganisationType.SoleTraderOrIndividual,
                Id = Guid.NewGuid(),
                TradingName = "TEST Ltd."
            };

            IWeeeClient weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExistsAndIncomplete>._))
                .Returns(true);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._))
                .Returns(orgData);

            ISearcher<OrganisationSearchResult> organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            OrganisationRegistrationController controller = new OrganisationRegistrationController(
                () => weeeClient,
                organisationSearcher);

            // Act
            ActionResult result = await controller.SoleTraderDetails(orgData.Id);

            // Assert
            var model = ((ViewResult)result).Model;

            Assert.NotNull(model);
            Assert.IsType<SoleTraderDetailsViewModel>(model);
            Assert.Equal(orgData.TradingName, ((SoleTraderDetailsViewModel)model).BusinessTradingName);
        }

        [Fact]
        public async Task GetRegisteredCompanyDetails_InvalidOrganisationId_ThrowsArgumentException()
        {
            // Arrange
            IWeeeClient weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExistsAndIncomplete>._))
               .Returns(false);

            ISearcher<OrganisationSearchResult> organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            OrganisationRegistrationController controller = new OrganisationRegistrationController(
                () => weeeClient,
                organisationSearcher);

            // Act
            Func<Task<ActionResult>> action = async () => await controller.RegisteredCompanyDetails(A.Dummy<Guid>());

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(action);
        }

        [Fact]
        public async Task GetRegisteredCompanyDetails_WithoutOrganisationId_ReturnsRegisteredCompanyDetailsView()
        {
            // Arrange
            IWeeeClient weeeClient = A.Dummy<IWeeeClient>();
            ISearcher<OrganisationSearchResult> organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            OrganisationRegistrationController controller = new OrganisationRegistrationController(
                () => weeeClient,
                organisationSearcher);

            // Act
            var result = await controller.RegisteredCompanyDetails();

            // Assert
            var model = ((ViewResult)result).Model;

            Assert.NotNull(model);
            Assert.IsType<RegisteredCompanyDetailsViewModel>(model);
        }

        [Fact]
        public async Task GetRegisteredCompanyDetails_WithValidOrganisationId_ReturnsRegisteredCompanyDetailsViewWithModel()
        {
            // Arrange
            OrganisationData orgData = new OrganisationData
            {
                OrganisationType = OrganisationType.RegisteredCompany,
                Id = Guid.NewGuid(),
                TradingName = "TEST Ltd.",
                CompanyRegistrationNumber = "12345678",
                Name = "TEST"
            };

            IWeeeClient weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExistsAndIncomplete>._))
                .Returns(true);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._))
                .Returns(orgData);

            ISearcher<OrganisationSearchResult> organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            OrganisationRegistrationController controller = new OrganisationRegistrationController(
                () => weeeClient,
                organisationSearcher);

            // Act
            ActionResult result = await controller.RegisteredCompanyDetails(orgData.Id);

            // Assert
            var model = ((ViewResult)result).Model;

            Assert.NotNull(model);
            Assert.IsType<RegisteredCompanyDetailsViewModel>(model);
            Assert.Equal(orgData.TradingName, ((RegisteredCompanyDetailsViewModel)model).BusinessTradingName);
            Assert.Equal(orgData.CompanyRegistrationNumber, ((RegisteredCompanyDetailsViewModel)model).CompaniesRegistrationNumber);
            Assert.Equal(orgData.Name, ((RegisteredCompanyDetailsViewModel)model).CompanyName);
        }

        [Fact]
        public async Task PostRegisteredCompanyDetails_WithInvalidModel_ReturnsView()
        {
            // Arrange
            IWeeeClient weeeClient = A.Dummy<IWeeeClient>();
            ISearcher<OrganisationSearchResult> organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            OrganisationRegistrationController controller = new OrganisationRegistrationController(
                () => weeeClient,
                organisationSearcher);

            controller.ModelState.AddModelError("Key", "Error");

            RegisteredCompanyDetailsViewModel model = new RegisteredCompanyDetailsViewModel();

            // Act
            ActionResult result = await controller.RegisteredCompanyDetails(model);

            // Assert
            var viewmodel = ((ViewResult)result).Model;

            Assert.NotNull(viewmodel);
            Assert.False(((ViewResult)result).ViewData.ModelState.IsValid);
        }

        [Fact]
        public async Task GetJoinOrganisationConfirmation_ReturnsJoinOrganisationConfirmationView()
        {
            // Arrange
            var orgData = new PublicOrganisationData
            {
                Id = Guid.NewGuid(),
                Address = new AddressData(),
                DisplayName = "Test"
            };

            IWeeeClient weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetPublicOrganisationInfo>._))
                .Returns(orgData);

            ISearcher<OrganisationSearchResult> organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            OrganisationRegistrationController controller = new OrganisationRegistrationController(
                () => weeeClient,
                organisationSearcher);

            // Act
            ActionResult result = await controller.JoinOrganisationConfirmation(orgData.Id);

            // Assert
            var model = ((ViewResult)result).Model;

            Assert.NotNull(model);
            Assert.IsType<JoinOrganisationConfirmationViewModel>(model);
        }

        [Fact]
        public async Task GetJoinOrganisation_ReturnsView()
        {
            // Arrange
            IWeeeClient weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetPublicOrganisationInfo>._))
                .Returns(new PublicOrganisationData());

            ISearcher<OrganisationSearchResult> organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            OrganisationRegistrationController controller = new OrganisationRegistrationController(
                () => weeeClient,
                organisationSearcher);

            // Act
            ActionResult result = await controller.JoinOrganisation(A.Dummy<Guid>());

            // Assert
            var model = ((ViewResult)result).Model;

            Assert.NotNull(model);
            Assert.IsType<JoinOrganisationViewModel>(model);
        }

        [Fact]
        public async Task GetJoinOrganisation_UserAlreadyAssociated_ReturnsUserAlreadyAssociatedWithOrgansiationView()
        {
            // Arrange
            Guid organisationId = new Guid("101F5E58-FEA3-4F59-9281-E543EDE5699F");

            IWeeeClient weeeClient = A.Fake<IWeeeClient>();

            PublicOrganisationData organisation = new PublicOrganisationData()
            {
                Id = organisationId,
                DisplayName = "Test Company"
            };

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetPublicOrganisationInfo>._))
                .Returns(organisation);

            OrganisationUserData association = new OrganisationUserData()
            {
                OrganisationId = organisationId,
                UserStatus = UserStatus.Active,
            };

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetUserOrganisationsByStatus>._))
                .Returns(new List<OrganisationUserData>() { association });

            ISearcher<OrganisationSearchResult> organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            OrganisationRegistrationController controller = new OrganisationRegistrationController(
                () => weeeClient,
                organisationSearcher);

            // Act
            ActionResult result = await controller.JoinOrganisation(organisationId);

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);

            Assert.Equal("UserAlreadyAssociatedWithOrganisation", viewResult.ViewName);

            UserAlreadyAssociatedWithOrganisationViewModel viewModel = viewResult.Model as UserAlreadyAssociatedWithOrganisationViewModel;
            Assert.NotNull(viewModel);

            Assert.Equal(organisationId, viewModel.OrganisationId);
            Assert.Equal(UserStatus.Active, viewModel.Status);
            Assert.Equal("Test Company", viewModel.OrganisationName);
        }

        [Fact]
        public async Task PostJoinOrganisation_NoSearchAnotherOrganisationSelected_RedirectsToType()
        {
            // Arrange
            IWeeeClient weeeClient = A.Dummy<IWeeeClient>();
            ISearcher<OrganisationSearchResult> organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            OrganisationRegistrationController controller = new OrganisationRegistrationController(
                () => weeeClient,
                organisationSearcher);

            JoinOrganisationViewModel model = new JoinOrganisationViewModel();
            model.SelectedValue = "No";

            // Act
            ActionResult result = await controller.JoinOrganisation(model);

            // Assert
            var redirectToRouteResult = ((RedirectToRouteResult)result);

            Assert.Equal("Search", redirectToRouteResult.RouteValues["action"]);
        }

        [Fact]
        public async Task PostJoinOrganisation_YesJoinOrganisationSelected_RedirectsToJoinOrganisationConfirmation()
        {
            // Arrange
            IWeeeClient weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<JoinOrganisation>._))
                .Returns(Guid.NewGuid());

            ISearcher<OrganisationSearchResult> organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            OrganisationRegistrationController controller = new OrganisationRegistrationController(
                () => weeeClient,
                organisationSearcher);

            JoinOrganisationViewModel model = new JoinOrganisationViewModel();
            model.SelectedValue = "Yes - join xyz";

            // Act
            ActionResult result = await controller.JoinOrganisation(model);

            // Assert
            var redirectToRouteResult = ((RedirectToRouteResult)result);

            Assert.Equal("JoinOrganisationConfirmation", redirectToRouteResult.RouteValues["action"]);
        }

        [Fact]
        public async Task GetSearch_UserHasNotOrganisation_ShowPerformAnotherActivityLinkIsFalse()
        {
            // Arrange
            IWeeeClient weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetUserOrganisationsByStatus>._))
               .Returns(new List<OrganisationUserData>());

            ISearcher<OrganisationSearchResult> organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            OrganisationRegistrationController controller = new OrganisationRegistrationController(
                () => weeeClient,
                organisationSearcher);

            // Act
            ActionResult result = await controller.Search();

            // Assert
            var model = ((ViewResult)result).Model as SearchViewModel;

            Assert.NotNull(model);
            Assert.False(model.ShowPerformAnotherActivityLink);
        }

        [Fact]
        public async Task GetSearch_UserHasOrganisations_ShowPerformAnotherActivityLinkIsTrue()
        {
            // Arrange
            IWeeeClient weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetUserOrganisationsByStatus>._))
               .Returns(new List<OrganisationUserData>
               {
                   new OrganisationUserData(),
                   new OrganisationUserData()
               });

            ISearcher<OrganisationSearchResult> organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            OrganisationRegistrationController controller = new OrganisationRegistrationController(
                () => weeeClient,
                organisationSearcher);

            // Act
            ActionResult result = await controller.Search();

            // Assert
            var model = ((ViewResult)result).Model as SearchViewModel;

            Assert.NotNull(model);
            Assert.True(model.ShowPerformAnotherActivityLink);
        }

        [Fact]
        public async Task PostConfirmOrganisationDetails_WithInvalidModel_ReturnsView()
        {
            // Arrange
            IWeeeClient weeeClient = A.Dummy<IWeeeClient>();
            ISearcher<OrganisationSearchResult> organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            OrganisationRegistrationController controller = new OrganisationRegistrationController(
                () => weeeClient,
                organisationSearcher);

            controller.ModelState.AddModelError("Key", "Error");

            OrganisationSummaryViewModel model = new OrganisationSummaryViewModel();

            // Act
            ActionResult result = await controller.ConfirmOrganisationDetails(model, Guid.NewGuid());

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.False(viewResult.ViewData.ModelState.IsValid);
        }

        [Fact]
        public async Task PostConfirmOrganisationDetails_ValidModel_ReturnsConfirmationView()
        {
            // Arrange
            IWeeeClient weeeClient = A.Fake<IWeeeClient>();
            
            ISearcher<OrganisationSearchResult> organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            OrganisationRegistrationController controller = new OrganisationRegistrationController(
                () => weeeClient,
                organisationSearcher);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<CompleteRegistration>._))
                .Returns(Guid.NewGuid());

            OrganisationSummaryViewModel model = new OrganisationSummaryViewModel();

            // Act
            ActionResult result = await controller.ConfirmOrganisationDetails(model, Guid.NewGuid());

            // Assert
            var redirectToRouteResult = ((RedirectToRouteResult)result);

            Assert.Equal("Confirmation", redirectToRouteResult.RouteValues["action"]);
        }

        [Fact]
        public async Task GetSearch_ReturnsSearchView()
        {
            // Arrange
            ISearcher<OrganisationSearchResult> organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();
            Func<IWeeeClient> weeeClient = A.Dummy<Func<IWeeeClient>>();

            OrganisationRegistrationController controller = new OrganisationRegistrationController(
                weeeClient,
                organisationSearcher);

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
            ISearcher<OrganisationSearchResult> organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();
            Func<IWeeeClient> weeeClient = A.Dummy<Func<IWeeeClient>>();

            OrganisationRegistrationController controller = new OrganisationRegistrationController(
                weeeClient,
                organisationSearcher);

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
        public async Task PostSearch_WithSearchTermAndNoSelectedOrganisationId_RedirectsToSearchResultsAction()
        {
            // Arrange
            ISearcher<OrganisationSearchResult> organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();
            Func<IWeeeClient> weeeClient = A.Dummy<Func<IWeeeClient>>();

            OrganisationRegistrationController controller = new OrganisationRegistrationController(
                weeeClient,
                organisationSearcher);

            SearchViewModel viewModel = new SearchViewModel();
            viewModel.SearchTerm = "testSearchTerm";
            viewModel.SelectedOrganisationId = null;

            // Act
            ActionResult result = await controller.Search(viewModel);

            // Assert
            RedirectToRouteResult redirectResult = result as RedirectToRouteResult;
            Assert.NotNull(redirectResult);

            Assert.Equal("SearchResults", redirectResult.RouteValues["action"]);
            Assert.Equal("testSearchTerm", redirectResult.RouteValues["SearchTerm"]);
        }

        [Fact]
        public async Task PostSearch_WithSelectedOrganisationId_RedirectsToJoinOrganisationAction()
        {
            // Arrange
            ISearcher<OrganisationSearchResult> organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();
            Func<IWeeeClient> weeeClient = A.Dummy<Func<IWeeeClient>>();

            OrganisationRegistrationController controller = new OrganisationRegistrationController(
                weeeClient,
                organisationSearcher);

            SearchViewModel viewModel = new SearchViewModel();
            viewModel.SearchTerm = "Test Company";
            viewModel.SelectedOrganisationId = new Guid("05DF9AE8-DACE-4173-A227-16933EB5D5F8");

            // Act
            ActionResult result = await controller.Search(viewModel);

            // Assert
            RedirectToRouteResult redirectResult = result as RedirectToRouteResult;
            Assert.NotNull(redirectResult);

            Assert.Equal("JoinOrganisation", redirectResult.RouteValues["action"]);
            Assert.Equal(new Guid("05DF9AE8-DACE-4173-A227-16933EB5D5F8"), redirectResult.RouteValues["OrganisationId"]);
        }

        [Fact]
        public async Task GetSearchResults_DoesSearchForFiveResultsAndReturnsSearchReturnsView()
        {
            // Arrange
            List<OrganisationSearchResult> fakeResults = new List<OrganisationSearchResult>()
            {
                new OrganisationSearchResult()
                {
                    OrganisationId = new Guid("05DF9AE8-DACE-4173-A227-16933EB5D5F8"),
                    Name = "Test Company",
                }
            };

            ISearcher<OrganisationSearchResult> organisationSearcher = A.Fake<ISearcher<OrganisationSearchResult>>();
            A.CallTo(() => organisationSearcher.Search("testSearchTerm", 5, false))
                .Returns(fakeResults);

            Func<IWeeeClient> weeeClient = A.Dummy<Func<IWeeeClient>>();

            OrganisationRegistrationController controller = new OrganisationRegistrationController(
                weeeClient,
                organisationSearcher);

            // Act
            ActionResult result = await controller.SearchResults("testSearchTerm");

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);

            Assert.True(string.IsNullOrEmpty(viewResult.ViewName) || viewResult.ViewName.ToLowerInvariant() == "searchresults");

            SearchResultsViewModel viewModel = viewResult.Model as SearchResultsViewModel;
            Assert.NotNull(viewModel);

            Assert.Contains(viewModel.Results, r => r.OrganisationId == new Guid("05DF9AE8-DACE-4173-A227-16933EB5D5F8"));
        }

        [Fact]
        public async Task PostSearchResults_WithInvalidModel_DoesSearchForFiveResultsAndReturnsSearchReturnsView()
        {
            // Arrange
            List<OrganisationSearchResult> fakeResults = new List<OrganisationSearchResult>()
            {
                new OrganisationSearchResult()
                {
                    OrganisationId = new Guid("05DF9AE8-DACE-4173-A227-16933EB5D5F8"),
                    Name = "Test Company",
                }
            };

            ISearcher<OrganisationSearchResult> organisationSearcher = A.Fake<ISearcher<OrganisationSearchResult>>();
            A.CallTo(() => organisationSearcher.Search("testSearchTerm", 5, false))
                .Returns(fakeResults);

            Func<IWeeeClient> weeeClient = A.Dummy<Func<IWeeeClient>>();

            OrganisationRegistrationController controller = new OrganisationRegistrationController(
                weeeClient,
                organisationSearcher);

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

            Assert.Contains(resultsViewModel.Results, r => r.OrganisationId == new Guid("05DF9AE8-DACE-4173-A227-16933EB5D5F8"));
        }

        [Fact]
        public async Task PostSearchResults_WithSelectedOrganisationId_RedirectsToJoinOrganisationAction()
        {
            // Arrange
            ISearcher<OrganisationSearchResult> organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            Func<IWeeeClient> weeeClient = A.Dummy<Func<IWeeeClient>>();

            OrganisationRegistrationController controller = new OrganisationRegistrationController(
                weeeClient,
                organisationSearcher);

            SearchResultsViewModel viewModel = new SearchResultsViewModel()
            {
                SelectedOrganisationId = new Guid("05DF9AE8-DACE-4173-A227-16933EB5D5F8")
            };

            // Act
            ActionResult result = await controller.SearchResults(viewModel);

            // Assert
            RedirectToRouteResult redirectResult = result as RedirectToRouteResult;
            Assert.NotNull(redirectResult);

            Assert.Equal("JoinOrganisation", redirectResult.RouteValues["action"]);
            Assert.Equal(new Guid("05DF9AE8-DACE-4173-A227-16933EB5D5F8"), redirectResult.RouteValues["OrganisationId"]);
        }

        [Fact]
        public void GetCreateGuidance_ReturnsView()
        {
            // Arrange
            ISearcher<OrganisationSearchResult> organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();
            Func<IWeeeClient> weeeClient = A.Dummy<Func<IWeeeClient>>();

            OrganisationRegistrationController controller = new OrganisationRegistrationController(
                weeeClient,
                organisationSearcher);

            // Act
            ActionResult result = controller.CreateGuidance("test");

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void GetConfirmation_ReturnsView()
        {
            // Arrange
            ISearcher<OrganisationSearchResult> organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();
            Func<IWeeeClient> weeeClient = A.Dummy<Func<IWeeeClient>>();

            OrganisationRegistrationController controller = new OrganisationRegistrationController(
                weeeClient,
                organisationSearcher);

            // Act
            ActionResult result = controller.Confirmation("test");

            // Assert
            Assert.IsType<ViewResult>(result);
        }
    }
}
