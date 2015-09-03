namespace EA.Weee.Web.Tests.Unit.Controllers
{
    using System;
    using System.Web.Mvc;
    using Api.Client;
    using Core.Organisations;
    using FakeItEasy;
    using ViewModels.JoinOrganisation;
    using ViewModels.OrganisationRegistration;
    using ViewModels.OrganisationRegistration.Details;
    using ViewModels.OrganisationRegistration.Type;
    using ViewModels.Shared;
    using Web.Controllers;
    using Weee.Requests.Organisations;
    using Xunit;

    public class OrganisationRegistrationControllerTests
    {
        private readonly IWeeeClient apiClient;

        public OrganisationRegistrationControllerTests()
        {
            apiClient = A.Fake<IWeeeClient>();
        }

        [Fact]
        public async void GetRegisteredOfficeAddress_ApiThrowsException_ExceptionShouldNotBeCaught()
        {
            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetOrganisationInfo>._))
                .Throws<Exception>();

            await Assert.ThrowsAnyAsync<Exception>(() => OrganisationRegistrationController().RegisteredOfficeAddress(A<Guid>._));
        }

        [Theory]
        [InlineData("Sole trader or individual")]
        [InlineData("Partnership")]
        [InlineData("Registered Company")]
        public async void GetRegisteredOfficeAddress_ApiReturnsOrganisationData_ShouldMapOrganisationTypeToModel(string organisationType)
        {
            var orgType = CastOrganisationType(organisationType);
            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetOrganisationInfo>._))
                .Returns(new OrganisationData
                {
                    OrganisationType = orgType
                });

            var result = await OrganisationRegistrationController().RegisteredOfficeAddress(A<Guid>._);
            var model = (AddressViewModel)((ViewResult)result).Model;

            Assert.Equal(orgType, model.OrganisationType);
        }

        [Fact]
        public async void GetRegisteredOfficeAddress_ApiReturnsOrganisationData_ShouldReturnViewWithModel()
        {
            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetOrganisationInfo>._))
                .Returns(new OrganisationData());

            var result = await OrganisationRegistrationController().RegisteredOfficeAddress(A<Guid>._);
            var model = ((ViewResult)result).Model;

            Assert.NotNull(model);
            Assert.IsType<AddressViewModel>(model);
        }

        [Fact]
        public async void PostRegisteredOfficeAddress_ModelStateIsInvalid_ShouldReturnViewWithInvalidModel()
        {
            var model = new AddressViewModel();
            var controller = OrganisationRegistrationController();
            controller.ModelState.AddModelError("Key", "Error"); // To make the model state invalid

            var result = await controller.RegisteredOfficeAddress(model);
            var returnedModel = ((ViewResult)result).Model;

            Assert.Equal(model, returnedModel);
        }

        [Fact]
        public async void PostRegisteredOfficeAddress_PrincipalPlaceOfBusinessIsValid_ShouldSubmitDetailsToApi()
        {
            var model = new AddressViewModel();

            await OrganisationRegistrationController().RegisteredOfficeAddress(model);

            A.CallTo(() => apiClient.SendAsync(A<string>._, A<AddAddressToOrganisation>._))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void PostRegisteredOfficeAddress_PrincipalPlaceOfBusinessIsValid_ShouldRedirectToSummaryPage()
        {
            var model = new AddressViewModel();

            var result = await OrganisationRegistrationController().RegisteredOfficeAddress(model);
            var redirectToRouteResult = ((RedirectToRouteResult)result);

            Assert.Equal("ReviewOrganisationDetails", redirectToRouteResult.RouteValues["action"]);
        }

        [Fact]
        public async void GetOrganisationAddress_ApiReturnsOrganisationData_ShouldReturnViewWithModel()
        {
            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetOrganisationInfo>._))
                .Returns(new OrganisationData());

            var result = await OrganisationRegistrationController().OrganisationAddress(A<Guid>._);
            var model = ((ViewResult)result).Model;

            Assert.NotNull(model);
            Assert.IsType<AddressViewModel>(model);
        }

        [Fact]
        public async void GetMainContactPerson_OrganisationIdIsInvalid_ShouldThrowArgumentException()
        {
            A.CallTo(() => apiClient.SendAsync(A<string>._, A<VerifyOrganisationExists>._))
                .Returns(false);

            await Assert.ThrowsAsync<ArgumentException>(() => OrganisationRegistrationController().MainContactPerson(A<Guid>._));
        }

        [Fact]
        public async void
            GetMainContactPerson_OrganisationIdIsValid_ShouldReturnContactPersonViewModelWithOrganisationId()
        {
            var organisationId = Guid.NewGuid();
            A.CallTo(() => apiClient.SendAsync(A<string>._, A<VerifyOrganisationExists>._))
                .Returns(true);

            var result = await OrganisationRegistrationController().MainContactPerson(organisationId);
            var model = ((ViewResult)result).Model;

            Assert.IsType<ContactPersonViewModel>(model);
            Assert.Equal(organisationId, ((ContactPersonViewModel)model).OrganisationId);
        }

        [Fact]
        public async void PostRegisteredOfficeAddressPrepopulate_WithYesSelection_ShouldRedirectToSummaryPage()
        {
            var model = GetMockAddressPrepopulateViewModel();

            var result = await OrganisationRegistrationController().RegisteredOfficeAddressPrepopulate(model);
            var redirectToRouteResult = ((RedirectToRouteResult)result);

            Assert.Equal("ReviewOrganisationDetails", redirectToRouteResult.RouteValues["action"]);
        }

        [Fact]
        public async void PostRegisteredOfficeAddressPrepopulate_WithNoSelection_ShouldRedirectToRegisteredOfficeAddress()
        {
            var model = GetMockAddressPrepopulateViewModel();
            model.ContactDetailsSameAs.Choices.SelectedValue = "No";

            var result = await OrganisationRegistrationController().RegisteredOfficeAddressPrepopulate(model);
            var redirectToRouteResult = ((RedirectToRouteResult)result);

            Assert.Equal("RegisteredOfficeAddress", redirectToRouteResult.RouteValues["action"]);
        }

        [Fact]
        public async void GetType_OrganisationIdIsInvalid_ShouldThrowArgumentException()
        {
            A.CallTo(() => apiClient.SendAsync(A<string>._, A<VerifyOrganisationExistsAndIncomplete>._))
                .Returns(false);

            await Assert.ThrowsAsync<ArgumentException>(() => OrganisationRegistrationController().Type(A<Guid>._));
        }

        [Fact]
        public async void GetType_OrganisationIdIsValid_ShouldReturnOrganisationTypeViewModel()
        {
            var organisationId = Guid.NewGuid();
            A.CallTo(() => apiClient.SendAsync(A<string>._, A<VerifyOrganisationExistsAndIncomplete>._))
                .Returns(true);

            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetOrganisationInfo>._))
                .Returns(new OrganisationData());

            var result = await OrganisationRegistrationController().Type(organisationId);
            var model = ((ViewResult)result).Model;

            Assert.IsType<OrganisationTypeViewModel>(model);
        }

        [Fact]
        public async void PostType_SoleTraderDetailsSelectionWithoutOrganisationId_ShouldRedirectSoleTraderDetails()
        {
            var model = GetMockOrganisationTypeViewModel();

            var result = await OrganisationRegistrationController().Type(model);
            var redirectToRouteResult = ((RedirectToRouteResult)result);

            Assert.Equal("SoleTraderDetails", redirectToRouteResult.RouteValues["action"]);
        }

        [Fact]
        public async void PostType_SoleTraderDetailsSelectionWithOrganisationId_ShouldRedirectSoleTraderDetails()
        {
            var model = GetMockOrganisationTypeViewModel();
            model.OrganisationId = Guid.NewGuid();

            A.CallTo(() => apiClient.SendAsync(A<string>._, A<VerifyOrganisationExistsAndIncomplete>._))
               .Returns(true);

            var orgData = new OrganisationData
            {
                OrganisationType = OrganisationType.SoleTraderOrIndividual,
                Id = Guid.NewGuid()
            };

            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetOrganisationInfo>._))
                .Returns(orgData);

            var result = await OrganisationRegistrationController().Type(model);
            var redirectToRouteResult = ((RedirectToRouteResult)result);

            Assert.Equal("SoleTraderDetails", redirectToRouteResult.RouteValues["action"]);
        }

        [Fact]
        public async void GetSoleTraderDetails_WithoutOrganisationId_ShouldReturnsSoleTraderDetailsView()
        {
            var result = await OrganisationRegistrationController().SoleTraderDetails();
            var model = ((ViewResult)result).Model;

            Assert.NotNull(model);
            Assert.IsType<SoleTraderDetailsViewModel>(model);
        }

        [Fact]
        public async void GetSoleTraderDetails_InvalidOrganisationId_ShouldThrowArgumentException()
        {
            A.CallTo(() => apiClient.SendAsync(A<string>._, A<VerifyOrganisationExistsAndIncomplete>._))
               .Returns(false);

            await Assert.ThrowsAsync<ArgumentException>(() => OrganisationRegistrationController().SoleTraderDetails(A<Guid>._));
        }

        [Fact]
        public async void GetSoleTraderDetails_WithValidOrganisationId_ShouldReturnsSoleTraderDetailsViewWithModel()
        {
            A.CallTo(() => apiClient.SendAsync(A<string>._, A<VerifyOrganisationExistsAndIncomplete>._))
             .Returns(true);

            var orgData = new OrganisationData
            {
                OrganisationType = OrganisationType.SoleTraderOrIndividual,
                Id = Guid.NewGuid(),
                TradingName = "TEST Ltd."
            };

            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetOrganisationInfo>._))
                .Returns(orgData);

            var result = await OrganisationRegistrationController().SoleTraderDetails(orgData.Id);
            var model = ((ViewResult)result).Model;

            Assert.NotNull(model);
            Assert.IsType<SoleTraderDetailsViewModel>(model);
            Assert.Equal(orgData.TradingName, ((SoleTraderDetailsViewModel)model).BusinessTradingName);
        }

        [Fact]
        public async void GetRegisteredCompanyDetails_InvalidOrganisationId_ShouldThrowArgumentException()
        {
            A.CallTo(() => apiClient.SendAsync(A<string>._, A<VerifyOrganisationExistsAndIncomplete>._))
               .Returns(false);

            await Assert.ThrowsAsync<ArgumentException>(() => OrganisationRegistrationController().RegisteredCompanyDetails(A<Guid>._));
        }

        [Fact]
        public async void GetRegisteredCompanyDetails_WithoutOrganisationId_ShouldReturnsRegisteredCompanyDetailsView()
        {
            var result = await OrganisationRegistrationController().RegisteredCompanyDetails();
            var model = ((ViewResult)result).Model;

            Assert.NotNull(model);
            Assert.IsType<RegisteredCompanyDetailsViewModel>(model);
        }

        [Fact]
        public async void GetRegisteredCompanyDetails_WithValidOrganisationId_ShouldReturnsRegisteredCompanyDetailsViewWithModel()
        {
            A.CallTo(() => apiClient.SendAsync(A<string>._, A<VerifyOrganisationExistsAndIncomplete>._))
             .Returns(true);

            var orgData = new OrganisationData
            {
                OrganisationType = OrganisationType.RegisteredCompany,
                Id = Guid.NewGuid(),
                TradingName = "TEST Ltd.",
                CompanyRegistrationNumber = "12345678",
                Name = "TEST"
            };

            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetOrganisationInfo>._))
                .Returns(orgData);

            var result = await OrganisationRegistrationController().RegisteredCompanyDetails(orgData.Id);
            var model = ((ViewResult)result).Model;

            Assert.NotNull(model);
            Assert.IsType<RegisteredCompanyDetailsViewModel>(model);
            Assert.Equal(orgData.TradingName, ((RegisteredCompanyDetailsViewModel)model).BusinessTradingName);
            Assert.Equal(orgData.CompanyRegistrationNumber, ((RegisteredCompanyDetailsViewModel)model).CompaniesRegistrationNumber);
            Assert.Equal(orgData.Name, ((RegisteredCompanyDetailsViewModel)model).CompanyName);
        }

        [Fact]
        public void GetJoinOrganisation_ReturnsView()
        {
            var result = OrganisationRegistrationController().JoinOrganisation(A<Guid>._);
            var model = ((ViewResult)result).Model;

            Assert.NotNull(model);
            Assert.IsType<JoinOrganisationViewModel>(model);
        }

        private OrganisationRegistrationController OrganisationRegistrationController()
        {
            return new OrganisationRegistrationController(() => apiClient);
        }

        private AddressPrepopulateViewModel GetMockAddressPrepopulateViewModel()
        {
            var addressPrepopulateViewModel = new AddressPrepopulateViewModel
            {
                OrganisationType = OrganisationType.SoleTraderOrIndividual,
                OrganisationId = Guid.NewGuid(),
                ContactDetailsSameAs = new YesNoChoiceViewModel { Choices = new RadioButtonStringCollectionViewModel { PossibleValues = new[] { "Yes", "No" }, SelectedValue = "Yes" } }
            };

            return addressPrepopulateViewModel;
        }

        private OrganisationTypeViewModel GetMockOrganisationTypeViewModel()
        {
            var organisationTypeViewModel = new OrganisationTypeViewModel
            {
                OrganisationTypes = RadioButtonStringCollectionViewModel.CreateFromEnum<OrganisationType>(OrganisationType.SoleTraderOrIndividual)
            };

            return organisationTypeViewModel;
        }

        private OrganisationType CastOrganisationType(string organisationType)
        {
            switch (organisationType)
            {
                case "Sole trader or individual":
                    return OrganisationType.SoleTraderOrIndividual;
                case "Partnership":
                    return OrganisationType.Partnership;
                default:
                    return OrganisationType.RegisteredCompany;
            }
        }
    }
}
