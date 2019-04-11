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
    using Web.Controllers;
    using Web.ViewModels.OrganisationRegistration;
    using Web.ViewModels.OrganisationRegistration.Details;
    using Web.ViewModels.OrganisationRegistration.Type;
    using Web.ViewModels.Shared;
    using Weee.Requests.Organisations;
    using Xunit;

    public class OrganisationRegistrationControllerTests
    {
        [Fact]
        public async Task GetRegisteredOfficeAddress_ApiThrowsException_ExceptionShouldNotBeCaught()
        {
            // Arrange
            var weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._))
                .Throws<Exception>();

            var organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            var controller = new OrganisationRegistrationController(
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
            var addressData = new AddressData();
            addressData.Address1 = "Address Line 1";

            var organisationData = new OrganisationData();
            organisationData.Id = new Guid("1B7329B9-DC7F-4621-8E97-FD97CDDDBA10");
            organisationData.OrganisationType = OrganisationType.RegisteredCompany;
            organisationData.HasBusinessAddress = true;
            organisationData.BusinessAddress = addressData;

            var weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._))
                .Returns(organisationData);

            var organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            var controller = new OrganisationRegistrationController(
                () => weeeClient,
                organisationSearcher);

            // Act
            var result = await controller.RegisteredOfficeAddress(new Guid("1B7329B9-DC7F-4621-8E97-FD97CDDDBA10"));

            // Assert
            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);

            var viewModel = viewResult.Model as AddressViewModel;
            Assert.NotNull(viewModel);

            Assert.Equal(new Guid("1B7329B9-DC7F-4621-8E97-FD97CDDDBA10"), viewModel.OrganisationId);
            Assert.Equal(OrganisationType.RegisteredCompany, viewModel.OrganisationType);
            Assert.Equal("Address Line 1", viewModel.Address.Address1);
        }

        [Fact]
        public async Task PostRegisteredOfficeAddress_ModelStateIsInvalid_ReturnsViewModel()
        {
            // Arrange
            var weeeClient = A.Dummy<IWeeeClient>();
            var organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            var controller = new OrganisationRegistrationController(
                () => weeeClient,
                organisationSearcher);

            var model = new AddressViewModel();
            controller.ModelState.AddModelError("Key", "Error"); // To make the model state invalid

            // Act
            var result = await controller.RegisteredOfficeAddress(model);

            // Assert
            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);

            Assert.Equal(model, viewResult.Model);
        }

        [Fact]
        public async void PostRegisteredOfficeAddress_PrincipalPlaceOfBusinessIsValid_CallsApiToAddAddressToOrganisation()
        {
            // Arrange
            var weeeClient = A.Fake<IWeeeClient>();
            var organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            var controller = new OrganisationRegistrationController(
                () => weeeClient,
                organisationSearcher);

            var model = new AddressViewModel();

            // Act
            var result = await controller.RegisteredOfficeAddress(model);

            // Assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<AddAddressToOrganisation>._))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task PostRegisteredOfficeAddress_PrincipalPlaceOfBusinessIsValid_RedirectsToReviewOrganisationDetails()
        {
            // Arrange
            var weeeClient = A.Fake<IWeeeClient>();
            var organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            var controller = new OrganisationRegistrationController(
                () => weeeClient,
                organisationSearcher);

            var model = new AddressViewModel();

            // Act
            var result = await controller.RegisteredOfficeAddress(model);

            // Assert
            var redirectToRouteResult = result as RedirectToRouteResult;
            Assert.NotNull(redirectToRouteResult);

            Assert.Equal("ReviewOrganisationDetails", redirectToRouteResult.RouteValues["action"]);
        }

        [Fact]
        public async Task GetOrganisationAddress_ApiReturnsOrganisationData_ReturnsViewWithModel()
        {
            // Arrange
            var weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._))
                .Returns(new OrganisationData());

            var organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            var controller = new OrganisationRegistrationController(
                () => weeeClient,
                organisationSearcher);

            // Act
            var result = await controller.OrganisationAddress(A.Dummy<Guid>(), A.Dummy<Guid>(), A.Dummy<Guid>());

            // Assert
            var model = ((ViewResult)result).Model;

            Assert.NotNull(model);
            Assert.IsType<AddressViewModel>(model);
        }

        [Fact]
        public async Task PostOrganisationAddress_IrrespectiveOfCountry_RedirectsToRegisteredOfficeAddressPrepopulate()
        {
            // Arrange
            var weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<AddAddressToOrganisation>._))
               .Returns(Guid.NewGuid());

            var organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            var controller = new OrganisationRegistrationController(
                () => weeeClient,
                organisationSearcher);

            // Act
            var result = await controller.OrganisationAddress(new AddressViewModel());

            // Assert
            var redirectToRouteResult = ((RedirectToRouteResult)result);

            Assert.Equal("RegisteredOfficeAddressPrepopulate", redirectToRouteResult.RouteValues["action"]);
        }

        [Fact]
        public async Task GetMainContactPerson_OrganisationIdIsInvalid_ThrowsArgumentException()
        {
            // Arrange
            var weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExists>._))
                .Returns(false);

            var organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            var controller = new OrganisationRegistrationController(
                () => weeeClient,
                organisationSearcher);

            // Act
            Func<Task<ActionResult>> action = async () => await controller.MainContactPerson(A.Dummy<Guid>(), A.Dummy<Guid>(), A.Dummy<Guid>());

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(action);
        }

        [Fact]
        public async Task GetMainContactPerson_GivenContactId_ReturnsContactPersonViewModelWithOrganisationId()
        {
            // Arrange
            var weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExists>._)).Returns(true);

            var organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            var controller = new OrganisationRegistrationController(
                () => weeeClient,
                organisationSearcher);

            var organisationId = Guid.NewGuid();
            var contactId = Guid.NewGuid();
            var addressId = Guid.NewGuid();
            var contact = A.Fake<ContactData>();

            A.CallTo(() => contact.FirstName).Returns("first");
            A.CallTo(() => contact.LastName).Returns("last");
            A.CallTo(() => contact.Position).Returns("position");

            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetContact>.That.Matches(c => c.ContactId.Equals(contactId) && c.OrganisationId.Equals(organisationId)))).Returns(contact);

            // Act
            var result = await controller.MainContactPerson(organisationId, contactId, addressId) as ViewResult;

            // Assert
            var model = (ContactPersonViewModel)result.Model;

            Assert.Equal(model.FirstName, contact.FirstName);
            Assert.Equal(model.LastName, contact.LastName);
            Assert.Equal(model.Position, contact.Position);
            Assert.Equal(model.AddressId, addressId);
            Assert.Equal(model.ContactId, contactId);
            Assert.Equal(organisationId, model.OrganisationId);
        }

        [Fact]
        public async Task GetMainContactPerson_GivenNoContactId_ReturnsContactPersonViewModelWithOrganisationId()
        {
            // Arrange
            var weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExists>._)).Returns(true);

            var organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            var controller = new OrganisationRegistrationController(
                () => weeeClient,
                organisationSearcher);

            var organisationId = Guid.NewGuid();
            var addressId = Guid.NewGuid();
            var contact = A.Fake<ContactData>();

            // Act
            var result = await controller.MainContactPerson(organisationId, null, addressId) as ViewResult;

            // Assert
            var model = (ContactPersonViewModel)result.Model;

            Assert.Null(model.FirstName);
            Assert.Null(model.LastName);
            Assert.Null(model.Position);
            Assert.Equal(model.AddressId, addressId);
            Assert.Equal(model.ContactId, null);
            Assert.Equal(organisationId, model.OrganisationId);
        }

        [Fact]
        public async Task PostRegisteredOfficeAddressPrepopulate_WithYesSelection_RedirectsToSummaryPage()
        {
            // Arrange
            var weeeClient = A.Dummy<IWeeeClient>();
            var organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            var controller = new OrganisationRegistrationController(
                () => weeeClient,
                organisationSearcher);

            var model = new AddressPrepopulateViewModel { SelectedValue = "Yes", AddressId = Guid.NewGuid() };

            // Act
            var result = await controller.RegisteredOfficeAddressPrepopulate(model);

            // Assert
            var redirectToRouteResult = ((RedirectToRouteResult)result);

            Assert.Equal("ReviewOrganisationDetails", redirectToRouteResult.RouteValues["action"]);
        }

        [Fact]
        public async Task PostRegisteredOfficeAddressPrepopulate_WithYesSelection_CopyOrganisationAddressIntoRegisteredOfficeRequestShouldHappen()
        {
            // Arrange
            var weeeClient = A.Dummy<IWeeeClient>();
            var organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            var controller = new OrganisationRegistrationController(
                () => weeeClient,
                organisationSearcher);

            var model = new AddressPrepopulateViewModel { SelectedValue = "Yes", AddressId = Guid.NewGuid() };

            // Act
            var result = await controller.RegisteredOfficeAddressPrepopulate(model);

            // Assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<CopyOrganisationAddressIntoRegisteredOffice>.That.Matches(c =>
                    c.AddressId.Equals(model.AddressId) && c.OrganisationId.Equals(model.OrganisationId)))).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task PostRegisteredOfficeAddressPrepopulate_WithNoSelection_RedirectsToRegisteredOfficeAddress()
        {
            // Arrange
            var weeeClient = A.Dummy<IWeeeClient>();
            var organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            var controller = new OrganisationRegistrationController(
                () => weeeClient,
                organisationSearcher);

            var model = new AddressPrepopulateViewModel();
            model.SelectedValue = "No";

            // Act
            var result = await controller.RegisteredOfficeAddressPrepopulate(model);

            // Assert
            var redirectToRouteResult = ((RedirectToRouteResult)result);

            Assert.Equal("RegisteredOfficeAddress", redirectToRouteResult.RouteValues["action"]);
        }

        [Fact]
        public async Task GetType_OrganisationIdIsInvalid_ThrowsArgumentException()
        {
            // Arrange
            var weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExistsAndIncomplete>._))
                .Returns(false);

            var organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            var controller = new OrganisationRegistrationController(
                () => weeeClient,
                organisationSearcher);

            // Act
            Func<Task<ActionResult>> action = async () => await controller.Type(A.Dummy<string>(), A.Dummy<Guid>(), A.Dummy<Guid>(), A.Dummy<Guid>());

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(action);
        }

        [Fact]
        public async Task GetType_OrganisationIdIsValid_ReturnsOrganisationTypeViewModel()
        {
            // Arrange
            var weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExistsAndIncomplete>._))
                .Returns(true);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._))
                .Returns(new OrganisationData());

            var organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            var controller = new OrganisationRegistrationController(
                () => weeeClient,
                organisationSearcher);

            // Act
            var result = await controller.Type(A.Dummy<string>(), A.Dummy<Guid>(), A.Dummy<Guid>(), A.Dummy<Guid>());

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
            var weeeClient = A.Dummy<IWeeeClient>();
            var organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            var controller = new OrganisationRegistrationController(
                () => weeeClient,
                organisationSearcher);

            var model = new OrganisationTypeViewModel();
            model.SelectedValue = selection;
            model.OrganisationId = null;

            // Act
            var result = await controller.Type(model);

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
            var orgData = new OrganisationData
            {
                OrganisationType = type,
                Id = Guid.NewGuid()
            };

            var weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExistsAndIncomplete>._))
                .Returns(true);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._))
                .Returns(orgData);

            var organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            var controller = new OrganisationRegistrationController(
                () => weeeClient,
                organisationSearcher);

            var model = new OrganisationTypeViewModel(
                type,
                new Guid("35EFE82E-0706-4E80-8AFA-D81C4B58102A"));

            // Act
            var result = await controller.Type(model);

            // Assert
            var redirectToRouteResult = ((RedirectToRouteResult)result);

            Assert.Equal(action, redirectToRouteResult.RouteValues["action"]);
        }

        [Fact]
        public async Task GetSoleTraderDetails_WithoutOrganisationId_ReturnsSoleTraderDetailsView()
        {
            // Arrange
            var weeeClient = A.Dummy<IWeeeClient>();
            var organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            var controller = new OrganisationRegistrationController(
                () => weeeClient,
                organisationSearcher);

            // Act
            var result = await controller.SoleTraderDetails();

            // Assert
            var model = ((ViewResult)result).Model;

            Assert.NotNull(model);
            Assert.IsType<SoleTraderDetailsViewModel>(model);
        }

        [Fact]
        public async Task GetSoleTraderDetails_InvalidOrganisationId_ThrowsArgumentException()
        {
            // Arrange
            var weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExistsAndIncomplete>._))
               .Returns(false);

            var organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            var controller = new OrganisationRegistrationController(
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
            var orgData = new OrganisationData
            {
                OrganisationType = OrganisationType.SoleTraderOrIndividual,
                Id = Guid.NewGuid(),
                TradingName = "TEST Ltd."
            };

            var weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExistsAndIncomplete>._))
                .Returns(true);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._))
                .Returns(orgData);

            var organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            var controller = new OrganisationRegistrationController(
                () => weeeClient,
                organisationSearcher);

            // Act
            var result = await controller.SoleTraderDetails(orgData.Id);

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
            var weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExistsAndIncomplete>._))
               .Returns(false);

            var organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            var controller = new OrganisationRegistrationController(
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
            var weeeClient = A.Dummy<IWeeeClient>();
            var organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            var controller = new OrganisationRegistrationController(
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
            var orgData = new OrganisationData
            {
                OrganisationType = OrganisationType.RegisteredCompany,
                Id = Guid.NewGuid(),
                TradingName = "TEST Ltd.",
                CompanyRegistrationNumber = "12345678",
                Name = "TEST"
            };

            var weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExistsAndIncomplete>._))
                .Returns(true);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._))
                .Returns(orgData);

            var organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            var controller = new OrganisationRegistrationController(
                () => weeeClient,
                organisationSearcher);

            // Act
            var result = await controller.RegisteredCompanyDetails(orgData.Id);

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
            var weeeClient = A.Dummy<IWeeeClient>();
            var organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            var controller = new OrganisationRegistrationController(
                () => weeeClient,
                organisationSearcher);

            controller.ModelState.AddModelError("Key", "Error");

            var model = new RegisteredCompanyDetailsViewModel();

            // Act
            var result = await controller.RegisteredCompanyDetails(model);

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

            var weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetPublicOrganisationInfo>._))
                .Returns(orgData);

            var organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            var controller = new OrganisationRegistrationController(
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
            var weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetPublicOrganisationInfo>._))
                .Returns(new PublicOrganisationData());

            var organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            var controller = new OrganisationRegistrationController(
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

            var controller = new OrganisationRegistrationController(
                () => weeeClient,
                organisationSearcher);

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
        }

        [Fact]
        public async Task PostJoinOrganisation_NoSearchAnotherOrganisationSelected_RedirectsToType()
        {
            // Arrange
            var weeeClient = A.Dummy<IWeeeClient>();
            var organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            var controller = new OrganisationRegistrationController(
                () => weeeClient,
                organisationSearcher);

            var model = new JoinOrganisationViewModel();
            model.SelectedValue = "No";

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
                organisationSearcher);

            var model = new JoinOrganisationViewModel();
            model.SelectedValue = "Yes - join xyz";

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
                organisationSearcher);

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
                organisationSearcher);

            // Act
            var result = await controller.Search();

            // Assert
            var model = ((ViewResult)result).Model as SearchViewModel;

            Assert.NotNull(model);
            Assert.True(model.ShowPerformAnotherActivityLink);
        }

        [Fact]
        public async Task PostConfirmOrganisationDetails_WithInvalidModel_ReturnsView()
        {
            // Arrange
            var weeeClient = A.Dummy<IWeeeClient>();
            var organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            var controller = new OrganisationRegistrationController(
                () => weeeClient,
                organisationSearcher);

            controller.ModelState.AddModelError("Key", "Error");

            var model = new OrganisationSummaryViewModel();

            // Act
            var result = await controller.ConfirmOrganisationDetails(model, Guid.NewGuid());

            // Assert
            var viewResult = result as ViewResult;
            Assert.False(viewResult.ViewData.ModelState.IsValid);
        }

        [Fact]
        public async Task PostConfirmOrganisationDetails_ValidModel_ReturnsConfirmationView()
        {
            // Arrange
            var weeeClient = A.Fake<IWeeeClient>();
            
            var organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            var controller = new OrganisationRegistrationController(
                () => weeeClient,
                organisationSearcher);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<CompleteRegistration>._))
                .Returns(Guid.NewGuid());

            var model = new OrganisationSummaryViewModel() { AddressId = Guid.NewGuid(), ContactId = Guid.NewGuid(), OrganisationData = new OrganisationData() { Contact = new ContactData() {Id = Guid.NewGuid() }, OrganisationAddress = new AddressData() { Id = Guid.NewGuid() }}};

            // Act
            var result = await controller.ConfirmOrganisationDetails(model, Guid.NewGuid());

            // Assert
            var redirectToRouteResult = ((RedirectToRouteResult)result);

            Assert.Equal("Confirmation", redirectToRouteResult.RouteValues["action"]);
        }

        [Fact]
        public async Task PostConfirmOrganisationDetails_ValidViewModel_CompleteOrganisationRequestShouldHappen()
        {
            // Arrange
            var weeeClient = A.Fake<IWeeeClient>();

            var organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            var controller = new OrganisationRegistrationController(
                () => weeeClient,
                organisationSearcher);

            var model = new OrganisationSummaryViewModel() { AddressId = Guid.NewGuid(), ContactId = Guid.NewGuid(), OrganisationData = new OrganisationData() { Contact = new ContactData() { Id = Guid.NewGuid() }, OrganisationAddress = new AddressData() { Id = Guid.NewGuid() } } };

            // Act
            var result = await controller.ConfirmOrganisationDetails(model, model.OrganisationData.Id);

            // Assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<CompleteRegistration>.That.Matches(c => c.OrganisationId.Equals(model.OrganisationData.Id) && c.AddressId.Equals(model.OrganisationData.OrganisationAddress.Id) && c.ContactId.Equals(model.OrganisationData.Contact.Id)))).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task GetSearch_ReturnsSearchView()
        {
            // Arrange
            var organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();
            var weeeClient = A.Dummy<Func<IWeeeClient>>();

            var controller = new OrganisationRegistrationController(
                weeeClient,
                organisationSearcher);

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
                organisationSearcher);

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
                organisationSearcher);

            var viewModel = new SearchViewModel();
            viewModel.SearchTerm = "testSearchTerm";
            viewModel.SelectedOrganisationId = null;

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
                organisationSearcher);

            var viewModel = new SearchViewModel();
            viewModel.SearchTerm = "Test Company";
            viewModel.SelectedOrganisationId = new Guid("05DF9AE8-DACE-4173-A227-16933EB5D5F8");

            // Act
            var result = await controller.Search(viewModel);

            // Assert
            var redirectResult = result as RedirectToRouteResult;
            Assert.NotNull(redirectResult);

            Assert.Equal("JoinOrganisation", redirectResult.RouteValues["action"]);
            Assert.Equal(new Guid("05DF9AE8-DACE-4173-A227-16933EB5D5F8"), redirectResult.RouteValues["OrganisationId"]);
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
                organisationSearcher);

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
                organisationSearcher);

            var viewModel = new SearchResultsViewModel();
            viewModel.SearchTerm = "testSearchTerm";
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
                organisationSearcher);

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
            Assert.Equal(new Guid("05DF9AE8-DACE-4173-A227-16933EB5D5F8"), redirectResult.RouteValues["OrganisationId"]);
        }

        [Fact]
        public void GetCreateGuidance_ReturnsView()
        {
            // Arrange
            var organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();
            var weeeClient = A.Dummy<Func<IWeeeClient>>();

            var controller = new OrganisationRegistrationController(
                weeeClient,
                organisationSearcher);

            // Act
            var result = controller.CreateGuidance("test");

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void GetConfirmation_ReturnsView()
        {
            // Arrange
            var organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();
            var weeeClient = A.Dummy<Func<IWeeeClient>>();

            var controller = new OrganisationRegistrationController(
                weeeClient,
                organisationSearcher);

            // Act
            var result = controller.Confirmation("test");

            // Assert
            Assert.IsType<ViewResult>(result);
        }
    }
}
