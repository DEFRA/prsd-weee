namespace EA.Weee.Web.Tests.Integration.SpecifyWhichOrganisationIWorkFor
{
    using System;
    using TechTalk.SpecFlow;
    using System.Configuration;
    using System.Web.Mvc;
    using Api.Client;
    using Controllers;
    using Requests;
    using ViewModels.OrganisationRegistration.Type;
    using ViewModels.Shared;
    using Xunit;

    [Binding]
    public class SpecifyWhichOrganisationIWorkForSteps
    {
        [Given(@"I selected the sole trader or individual option")]
        public void GivenISelectedTheSoleTraderOrIndividualOption()
        {
            ScenarioContext.Current[typeof(OrganisationTypeViewModel).Name] =
                OrganisationType(OrganisationTypeEnum.SoleTrader);
        }

        [Given(@"I selected the partnership option")]
        public void GivenISelectedThePartnershipOption()
        {
            ScenarioContext.Current[typeof(OrganisationTypeViewModel).Name] =
                OrganisationType(OrganisationTypeEnum.Partnership);
        }

        [Given(@"I selected the registered company option")]
        public void GivenISelectedTheRegisteredCompanyOption()
        {
            ScenarioContext.Current[typeof(OrganisationTypeViewModel).Name] =
                OrganisationType(OrganisationTypeEnum.RegisteredCompany);
        }

        [When(@"I continue")]
        public void WhenISelectContinue()
        {
            var controller = OrganisationRegistrationController();

            var model = ScenarioContext.Current.Get<OrganisationTypeViewModel>(typeof(OrganisationTypeViewModel).Name);

            ScenarioContext.Current["Result"] = controller.Type(model);
        }

        [Then(@"I should be redirected to the sole trader or individual page")]
        public void ThenIShouldBeRedirectedToTheSoleTraderOrIndividualPage()
        {
            var result = ScenarioContext.Current.Get<RedirectToRouteResult>("Result");

            Assert.Equal("OrganisationRegistration", result.RouteValues["controller"]);
            Assert.Equal("SoleTraderDetails", result.RouteValues["action"]);
        }

        [Then(@"I should be redirected to the partnership details page")]
        public void ThenIShouldBeRedirectedToThePartnershipDetailsPage()
        {
            var result = ScenarioContext.Current.Get<RedirectToRouteResult>("Result");

            Assert.Equal("OrganisationRegistration", result.RouteValues["controller"]);
            Assert.Equal("PartnershipDetails", result.RouteValues["action"]);
        }

        [Then(@"I should be redirected to the registered company details page")]
        public void ThenIShouldBeRedirectedToTheRegisteredCompanyDetailsPage()
        {
            var result = ScenarioContext.Current.Get<RedirectToRouteResult>("Result");

            Assert.Equal("OrganisationRegistration", result.RouteValues["controller"]);
            Assert.Equal("RegisteredCompanyDetails", result.RouteValues["action"]);
        }

        [AfterScenario]
        public void Cleanup()
        {
            ScenarioContext.Current.Clear();
        }

        private OrganisationRegistrationController OrganisationRegistrationController()
        {
            return new OrganisationRegistrationController(
                    () => new WeeeClient(ConfigurationManager.AppSettings["Weee.ApiUrl"]),
                    new SoleTraderDetailsRequestCreator());
        }

        private OrganisationTypeViewModel OrganisationType(OrganisationTypeEnum selectedOption)
        {
            return new OrganisationTypeViewModel
            {
                OrganisationTypes = RadioButtonStringCollectionViewModel.CreateFromEnum(selectedOption)
            };
        }
    }
}
