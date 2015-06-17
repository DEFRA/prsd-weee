namespace EA.Weee.Web.Tests.Integration.SpecifyWhichOrganisationIWorkFor
{
    using System.Configuration;
    using System.Linq;
    using System.Web.Mvc;
    using Api.Client;
    using Controllers;
    using Prsd.Core.Extensions;
    using Requests;
    using TechTalk.SpecFlow;
    using ViewModels.OrganisationRegistration.Details;
    using ViewModels.OrganisationRegistration.Type;
    using ViewModels.Shared;
    using Xunit;

    [Binding]
    public class SpecifyOrganisationDetailsSteps
    {
        [Given(@"I am a sole trader or individual")]
        public void GivenIAmASoleTraderOrIndividual()
        {
            ScenarioContext.Current[typeof(OrganisationTypeEnum).Name] =
                OrganisationType(OrganisationTypeEnum.SoleTrader);
        }

        [Given(@"I am a partnership")]
        public void GivenIAmAPartnership()
        {
            ScenarioContext.Current[typeof(OrganisationTypeEnum).Name] =
                OrganisationType(OrganisationTypeEnum.Partnership);
        }

        [Given(@"I am a registered company")]
        public void GivenIAmARegisteredCompany()
        {
            ScenarioContext.Current[typeof(OrganisationTypeEnum).Name] =
                OrganisationType(OrganisationTypeEnum.RegisteredCompany);
        }

        [When(@"I submit details about my sole trader organisation")]
        public void WhenISubmitDetailsAboutMySoleTraderOrganisation()
        {
            var controller = OrganisationRegistrationController();
            ScenarioContext.Current["controller"] = controller;

            ScenarioContext.Current["result"] = controller.SoleTraderDetails(SoleTrader());
        }

        [When(@"I submit details about my partnership organisation")]
        public void WhenISubmitDetailsAboutMyPartnershipOrganisation()
        {
            var controller = OrganisationRegistrationController();
            ScenarioContext.Current["controller"] = controller;

            ScenarioContext.Current["result"] = controller.PartnershipDetails(Partnership());
        }

        [When(@"I submit details about my registered company organisation")]
        public void WhenISubmitDetailsAboutMyRegisteredCompanyOrganisation()
        {
            var controller = OrganisationRegistrationController();
            ScenarioContext.Current["controller"] = controller;

            ScenarioContext.Current["result"] = controller.RegisteredCompanyDetails(RegisteredCompany());
        }

        [Then(@"the details should be stored")]
        public void ThenTheDetailsShouldBeStored()
        {
            var controller = (OrganisationRegistrationController)ScenarioContext.Current["controller"];
            var tempData = controller.TempData.Values.Single();

            var organisationTypeViewModel =
                ScenarioContext.Current.Get<OrganisationTypeViewModel>(typeof(OrganisationTypeEnum).Name);

            var selectedOrganisationType = organisationTypeViewModel.OrganisationTypes.SelectedValue
                .GetValueFromDisplayName<OrganisationTypeEnum>();

            switch (selectedOrganisationType)
            {
                case OrganisationTypeEnum.Partnership:
                    Assert.IsType<PartnershipDetailsViewModel>(tempData);
                    break;
                case OrganisationTypeEnum.SoleTrader:
                    Assert.IsType<SoleTraderDetailsViewModel>(tempData);
                    break;
                case OrganisationTypeEnum.RegisteredCompany:
                    Assert.IsType<RegisteredCompanyDetailsViewModel>(tempData);
                    break;
            }
        }

        [Then(@"I should be redirected to the select organisation page")]
        public void ThenIShouldBeRedirectedToTheSelectOrganisationPage()
        {
            var result = ScenarioContext.Current.Get<RedirectToRouteResult>("result");

            Assert.Equal("OrganisationRegistration", result.RouteValues["controller"]);
            Assert.Equal("SelectOrganisation", result.RouteValues["action"]);
        }

        [AfterScenario]
        public void Cleanup()
        {
            ScenarioContext.Current.Clear();
        }

        private OrganisationRegistrationController OrganisationRegistrationController()
        {
            return new OrganisationRegistrationController(
                    () => new WeeeClient(ConfigurationManager.AppSettings["Weee.ApiUrl"]));
        }

        private OrganisationTypeViewModel OrganisationType(OrganisationTypeEnum selectedOption)
        {
            return new OrganisationTypeViewModel
            {
                OrganisationTypes = RadioButtonStringCollectionViewModel.CreateFromEnum(selectedOption)
            };
        }

        private RegisteredCompanyDetailsViewModel RegisteredCompany()
        {
            return new RegisteredCompanyDetailsViewModel
            {
                BusinessTradingName = "Test business trading name",
                CompaniesRegistrationNumber = "AB123456",
                CompanyName = "Test company name"
            };
        }

        private PartnershipDetailsViewModel Partnership()
        {
            return new PartnershipDetailsViewModel
            {
                BusinessTradingName = "Test business trading name"
            };
        }

        private SoleTraderDetailsViewModel SoleTrader()
        {
            return new SoleTraderDetailsViewModel
            {
                BusinessTradingName = "Test business trading name"
            };
        }
    }
}
