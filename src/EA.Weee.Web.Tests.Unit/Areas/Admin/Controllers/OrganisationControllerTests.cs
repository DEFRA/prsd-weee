namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Shared;
    using EA.Weee.Requests.Organisations;
    using EA.Weee.Requests.Shared;
    using EA.Weee.Web.Areas.Admin.Controllers;
    using EA.Weee.Web.Areas.Admin.Controllers.Base;
    using EA.Weee.Web.Areas.Admin.ViewModels.Organisation;
    using EA.Weee.Web.Areas.Admin.ViewModels.Scheme.Overview;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using EA.Weee.Web.Tests.Unit.TestHelpers;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class OrganisationControllerTests
    {
        private readonly IWeeeClient weeeClient;
        private readonly IWeeeCache cache;
        private readonly BreadcrumbService breadcrumb;
        private readonly IMapper mapper;
        private readonly OrganisationController controller;

        public OrganisationControllerTests()
        {
            weeeClient = A.Fake<IWeeeClient>();
            cache = A.Fake<IWeeeCache>();
            breadcrumb = A.Fake<BreadcrumbService>();
            mapper = A.Fake<IMapper>();

            controller = new OrganisationController(() => weeeClient, cache, breadcrumb, mapper);
        }

        [Fact]
        public void OrganisationControllerInheritsAdminController()
        {
            typeof(OrganisationController).BaseType.Name.Should().Be(typeof(AdminController).Name);
        }

        [Fact]
        public async void GetEditRegisteredCompanyOrganisationDetails_CanEditOrganisationIsTrue_ReturnsView()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetCountries>._))
                .Returns(new List<CountryData>());

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<OrganisationBySchemeId>._))
                .Returns(new OrganisationData
                {
                    OrganisationType = OrganisationType.SoleTraderOrIndividual,
                    TradingName = "TradingName",
                    Name = "CompanyName",
                    CompanyRegistrationNumber = "123456789",
                    BusinessAddress = new AddressData(),
                    CanEditOrganisation = true
                });

            var result = await controller.EditRegisteredCompanyOrganisationDetails(Guid.NewGuid(), Guid.NewGuid());

            result.Should().BeOfType<ViewResult>();
            ((ViewResult)result).ViewName.Should().Be("EditRegisteredCompanyOrganisationDetails");
        }

        [Fact]
        public async void GetEditSoleTraderOrIndividualOrganisationDetails_CanEditOrganisationIsTrue_ReturnsView()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetCountries>._))
                .Returns(new List<CountryData>());

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<OrganisationBySchemeId>._))
                .Returns(new OrganisationData
                {
                    OrganisationType = OrganisationType.SoleTraderOrIndividual,
                    TradingName = "TradingName",
                    BusinessAddress = new AddressData(),
                    CanEditOrganisation = true
                });

            var result = await controller.EditSoleTraderOrIndividualOrganisationDetails(Guid.NewGuid(), Guid.NewGuid());

            result.Should().BeOfType<ViewResult>();
            ((ViewResult)result).ViewName.Should().Be("EditSoleTraderOrIndividualOrganisationDetails");
        }

        [Fact]
        public async void GetEditRegisteredCompanyOrganisationDetails_CanEditOrganisationIsFalse_ReturnsHttpForbiddenResult()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<OrganisationBySchemeId>._))
                .Returns(new OrganisationData
                {
                    OrganisationType = OrganisationType.SoleTraderOrIndividual,
                    TradingName = "TradingName",
                    Name = "CompanyName",
                    CompanyRegistrationNumber = "123456789",
                    BusinessAddress = new AddressData(),
                    CanEditOrganisation = false
                });

            var result = await controller.EditRegisteredCompanyOrganisationDetails(Guid.NewGuid(), Guid.NewGuid());

            result.Should().NotBeNull();
            result.Should().BeOfType<HttpForbiddenResult>();
        }

        [Fact]
        public async void PostEditRegisteredCompanyOrganisationDetails_ModelIsInvalid_GetsCountriesAndReturnsDefaultView()
        {
            var countries = new List<CountryData>();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetCountries>._))
                .Returns(countries);

            new HttpContextMocker().AttachToController(controller);

            controller.ModelState.AddModelError("SomeProperty", "IsInvalid");

            var viewModel = new EditRegisteredCompanyOrganisationDetailsViewModel
            {
                OrganisationType = OrganisationType.SoleTraderOrIndividual,
                BusinessAddress = new AddressData(),
                CompanyName = "CompanyName",
                CompaniesRegistrationNumber = "123456789",
                BusinessTradingName = "TradingName",
                OrgId = Guid.NewGuid(),
                SchemeId = Guid.NewGuid()
            };
            var result = await controller.EditRegisteredCompanyOrganisationDetails(viewModel);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetCountries>._))
                .MustHaveHappened(Repeated.Exactly.Once);

            countries.Should().BeSameAs(viewModel.BusinessAddress.Countries);

            result.Should().NotBeNull();
            result.Should().BeOfType<ViewResult>();

            var viewResult = (ViewResult)result;

            viewResult.ViewName.Should().BeEmpty();
            viewModel.Should().BeSameAs(viewResult.Model);
        }

        [Fact]
        public async Task PostEditRegisteredCompanyOrganisationDetails_ModelIsValid_UpdatesDetailsAndRedirectsToSchemeOverview()
        {
            var viewModel = new EditRegisteredCompanyOrganisationDetailsViewModel
            {
                OrganisationType = OrganisationType.SoleTraderOrIndividual,
                BusinessAddress = new AddressData(),
                CompanyName = "CompanyName",
                CompaniesRegistrationNumber = "123456789",
                BusinessTradingName = "TradingName",
                OrgId = Guid.NewGuid(),
                SchemeId = Guid.NewGuid()
            };
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<UpdateOrganisationDetails>._))
                .Returns(true);
            
            new HttpContextMocker().AttachToController(controller);

            var result = await controller.EditRegisteredCompanyOrganisationDetails(viewModel);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<UpdateOrganisationDetails>._))
                .MustHaveHappened(Repeated.Exactly.Once);

            result.Should().NotBeNull();
            result.Should().BeOfType<RedirectToRouteResult>();

            var redirectResult = (RedirectToRouteResult)result;
            redirectResult.RouteValues["Action"].Should().Be("Overview");
            redirectResult.RouteValues["overviewDisplayOption"].Should().Be(OverviewDisplayOption.OrganisationDetails);
        }

        [Fact]
        public async void GetEditSoleTraderOrIndividualOrganisationDetails_CanEditOrganisationIsFalse_ReturnsHttpForbiddenResult()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<OrganisationBySchemeId>._))
                .Returns(new OrganisationData
                {
                    OrganisationType = OrganisationType.SoleTraderOrIndividual,
                    TradingName = "TradingName",
                    BusinessAddress = new AddressData(),
                    CanEditOrganisation = false
                });

            var result = await controller.EditSoleTraderOrIndividualOrganisationDetails(Guid.NewGuid(), Guid.NewGuid());

            result.Should().NotBeNull();
            result.Should().BeOfType<HttpForbiddenResult>();
        }

        [Fact]
        public async void PostEditSoleTraderOrIndividualOrganisationDetails_ModelIsInvalid_GetsCountriesAndReturnsDefaultView()
        {
            var countries = new List<CountryData>();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetCountries>._))
                .Returns(countries);

            new HttpContextMocker().AttachToController(controller);

            controller.ModelState.AddModelError("SomeProperty", "IsInvalid");

            var viewModel = new EditSoleTraderOrIndividualOrganisationDetailsViewModel
            {
                OrganisationType = OrganisationType.SoleTraderOrIndividual,
                BusinessAddress = new AddressData(),
                BusinessTradingName = "TradingName",
                OrgId = Guid.NewGuid(),
                SchemeId = Guid.NewGuid()
            };
            var result = await controller.EditSoleTraderOrIndividualOrganisationDetails(viewModel);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetCountries>._))
                .MustHaveHappened(Repeated.Exactly.Once);

            countries.Should().BeSameAs(viewModel.BusinessAddress.Countries);

            result.Should().NotBeNull();
            result.Should().BeOfType<ViewResult>();

            var viewResult = (ViewResult)result;

            viewResult.ViewName.Should().BeEmpty();
            viewResult.Model.Should().Be(viewModel);
        }

        [Fact]
        public async Task PostEditSoleTraderOrIndividualOrganisationDetails_ModelIsValid_UpdatesDetailsAndRedirectsToSchemeOverview()
        {
            var viewModel = new EditSoleTraderOrIndividualOrganisationDetailsViewModel
            {
                OrganisationType = OrganisationType.SoleTraderOrIndividual,
                BusinessAddress = new AddressData(),
                BusinessTradingName = "TradingName",
                OrgId = Guid.NewGuid(),
                SchemeId = Guid.NewGuid()
            };
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<UpdateOrganisationDetails>._))
                .Returns(true);
            
            new HttpContextMocker().AttachToController(controller);

            var result = await controller.EditSoleTraderOrIndividualOrganisationDetails(viewModel);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<UpdateOrganisationDetails>._))
                .MustHaveHappened(Repeated.Exactly.Once);

            result.Should().NotBeNull();
            result.Should().BeOfType<RedirectToRouteResult>();

            var redirectResult = (RedirectToRouteResult)result;
            redirectResult.RouteValues["Action"].Should().Be("Overview");
            redirectResult.RouteValues["overviewDisplayOption"].Should().Be(OverviewDisplayOption.OrganisationDetails);
        }
    }
}
