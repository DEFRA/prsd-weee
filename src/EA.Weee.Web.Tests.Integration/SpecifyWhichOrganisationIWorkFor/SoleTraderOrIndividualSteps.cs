using System;
using TechTalk.SpecFlow;

namespace EA.Weee.Web.Tests.Integration.SpecifyWhichOrganisationIWorkFor
{
    using System.Configuration;
    using System.Web.Mvc;
    using Api.Client;
    using Controllers;
    using Requests;
    using ViewModels.Organisation.Type;
    using ViewModels.Shared;
    using Xunit;

    [Binding]
    public class SpecifyWhichOrganisationIWorkForSteps
    {
        [Given(@"I select the sole trader or indivdual option")]
        public void GivenISelectTheSoleTraderOrIndivdualOption()
        {
            ScenarioContext.Current[typeof(OrganisationTypeViewModel).Name] =
                OrganisationType(OrganisationTypeEnum.SoleTrader);
        }

        [When(@"I select continue")]
        public void WhenISelectContinue()
        {
            var controller =
                new OrganisationRegistrationController(
                    () => new WeeeClient(ConfigurationManager.AppSettings["Weee.ApiUrl"]),
                    new SoleTraderDetailsRequestCreator());

            var model = (OrganisationTypeViewModel)ScenarioContext.Current[typeof(OrganisationTypeViewModel).Name];

            ScenarioContext.Current["Result"] = controller.Type(model);
        }

        [Then(@"I should by redirected to the sole trader or individual page")]
        public void ThenIShouldByRedirectedToTheSoleTraderOrIndividualPage()
        {
            var result = (RedirectToRouteResult)ScenarioContext.Current["Result"];

            Assert.Equal("OrganisationRegistration", result.RouteValues["controller"]);
            Assert.Equal("SoleTraderDetails", result.RouteValues["action"]);
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
