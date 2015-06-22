namespace EA.Weee.Web.Tests.Unit.Controllers
{
    using System;
    using System.Web.Mvc;
    using Api.Client;
    using FakeItEasy;
    using ViewModels.OrganisationRegistration;
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

            Assert.Equal("OrganisationRegistration", redirectToRouteResult.RouteValues["controller"]);
            Assert.Equal("ReviewOrganisationDetails", redirectToRouteResult.RouteValues["action"]);
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

        private OrganisationRegistrationController OrganisationRegistrationController()
        {
            return new OrganisationRegistrationController(() => apiClient);
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
