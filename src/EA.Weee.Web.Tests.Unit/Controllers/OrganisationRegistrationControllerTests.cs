namespace EA.Weee.Web.Tests.Unit.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Api.Client;
    using Core.Organisations;
    using Core.Shared;
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
        public async void GetJoinOrganisationConfirmation_ShouldReturnsJoinOrganisationConfirmationView()
        {
            var orgData = new PublicOrganisationData
            {
                Id = Guid.NewGuid(),
                Address = new AddressData(),
                DisplayName = "Test"
            };

            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetPublicOrganisationInfo>._))
                .Returns(orgData);

            var result = await OrganisationRegistrationController().JoinOrganisationConfirmation(orgData.Id);
            var model = ((ViewResult)result).Model;

            Assert.NotNull(model);
            Assert.IsType<JoinOrganisationConfirmationViewModel>(model);
        }

        [Fact]
        public async void GetJoinOrganisation_ShouldReturnsView()
        {
            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetPublicOrganisationInfo>._))
                .Returns(new PublicOrganisationData());

            var result = await OrganisationRegistrationController().JoinOrganisation(A<Guid>._);
            var model = ((ViewResult)result).Model;

            Assert.NotNull(model);
            Assert.IsType<JoinOrganisationViewModel>(model);
        }

        [Fact]
        public async void PostJoinOrganisation_NoSearchAnotherOrganisationSelected_ShouldRedirectedToTypeView()
        {
            var model = GetTestJoinOrgViewModel();

            var result = await OrganisationRegistrationController().JoinOrganisation(model);

            var redirectToRouteResult = ((RedirectToRouteResult)result);

            Assert.Equal("Type", redirectToRouteResult.RouteValues["action"]);
        }

        [Fact]
        public async void PostJoinOrganisation_YesJoinOrganisationSelected_ShouldRedirectedToJoinOrganisationConfirmationView()
        {
            var model = GetTestJoinOrgViewModel();

            model.JoinOrganisationOptions.SelectedValue = "Yes - join xyz";

            A.CallTo(() => apiClient.SendAsync(A<string>._, A<JoinOrganisation>._))
                .Returns(Guid.NewGuid());

            var result = await OrganisationRegistrationController().JoinOrganisation(model);

            var redirectToRouteResult = ((RedirectToRouteResult)result);

            Assert.Equal("JoinOrganisationConfirmation", redirectToRouteResult.RouteValues["action"]);
        }

        private JoinOrganisationViewModel GetTestJoinOrgViewModel()
        {
            const string NoSearchAnotherOrganisation = "No - search for another organisation";
            return new JoinOrganisationViewModel
            {
                OrganisationId = Guid.NewGuid(),
                JoinOrganisationOptions = new RadioButtonStringCollectionViewModel
                {
                    PossibleValues = new[] { "Yes - join xyz", NoSearchAnotherOrganisation },
                    SelectedValue = NoSearchAnotherOrganisation
                }
            };
        }

        [Fact]
        public void GetNotFoundOrganisation_ShouldReturnsView()
        {
            var result = OrganisationRegistrationController().NotFoundOrganisation("name", "trading name", "12345678", OrganisationType.RegisteredCompany);

            var model = ((ViewResult)result).Model;

            Assert.NotNull(model);
            Assert.IsType<NotFoundOrganisationViewModel>(model);
        }

        [Fact]
        public async void PostNotFoundOrganisation_TryAnotherSearchActionSelected_ShouldRedirectToTypeView()
        {
            var model = new NotFoundOrganisationViewModel
            {
                SearchedText = "Test",
                ActivityOptions = new RadioButtonStringCollectionViewModel
                {
                    PossibleValues = new[] { NotFoundOrganisationAction.TryAnotherSearch, NotFoundOrganisationAction.CreateNewOrg },
                    SelectedValue = NotFoundOrganisationAction.TryAnotherSearch
                }
            };

            var result = await OrganisationRegistrationController().NotFoundOrganisation(model);

            var redirectRouteResult = (RedirectToRouteResult)result;

            Assert.Equal(redirectRouteResult.RouteValues["action"], "Type");
        }

        [Fact]
        public async void PostNotFoundOrganisation_CreateNewOrgSelected_ShouldRedirectToContactPersonView()
        {
            var model = new NotFoundOrganisationViewModel
            {
                SearchedText = "xyz ltd.",
                Name = "xyz ltd.",
                TradingName = "xyz",
                Type = OrganisationType.RegisteredCompany,
                CompaniesRegistrationNumber = "12345678",
                ActivityOptions = new RadioButtonStringCollectionViewModel
                {
                    PossibleValues = new[] { NotFoundOrganisationAction.TryAnotherSearch, NotFoundOrganisationAction.CreateNewOrg },
                    SelectedValue = NotFoundOrganisationAction.CreateNewOrg
                }
            };

            var result = await OrganisationRegistrationController().NotFoundOrganisation(model);

            var redirectRouteResult = (RedirectToRouteResult)result;

            Assert.Equal(redirectRouteResult.RouteValues["action"], "MainContactPerson");
        }

        [Fact]
        public async void GetSelectOrganisation_NoMatchingOrganisation_ShouldRedirectToNotFoundOrganisationView()
        {
            A.CallTo(() => apiClient.SendAsync(A<string>._, A<FindMatchingOrganisations>._))
                .Returns(new OrganisationSearchDataResult(new List<PublicOrganisationData>(), 0));

            var result =
                await
                    OrganisationRegistrationController()
                        .SelectOrganisation("xyz ltd.", "xyz", "12345678", OrganisationType.RegisteredCompany);

            var redirectRouteResult = (RedirectToRouteResult)result;

            Assert.Equal(redirectRouteResult.RouteValues["action"], "NotFoundOrganisation");
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
