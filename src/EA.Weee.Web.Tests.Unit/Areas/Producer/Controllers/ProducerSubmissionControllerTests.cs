namespace EA.Weee.Web.Tests.Unit.Areas.Producer.Controllers
{
    using AutoFixture;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Shared;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using EA.Weee.Requests.Shared;
    using EA.Weee.Tests.Core;
    using EA.Weee.Web.Areas.Producer.Controllers;
    using EA.Weee.Web.Areas.Producer.Filters;
    using EA.Weee.Web.Areas.Producer.ViewModels;
    using EA.Weee.Web.Controllers.Base;
    using EA.Weee.Web.Requests.Base;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Xunit;

    public class ProducerSubmissionControllerTests : SimpleUnitTestBase
    {
        private readonly ProducerSubmissionController controller;
        private readonly IMapper mapper;
        private readonly IWeeeClient weeeClient;
        private readonly IRequestCreator<EditOrganisationDetailsViewModel, EditOrganisationDetailsRequest>
            editOrganisationDetailsRequestCreator;

        public ProducerSubmissionControllerTests()
        {
            mapper = A.Fake<IMapper>();
            weeeClient = A.Fake<IWeeeClient>();
            editOrganisationDetailsRequestCreator =
                A.Fake<IRequestCreator<EditOrganisationDetailsViewModel, EditOrganisationDetailsRequest>>();

            controller = new ProducerSubmissionController(mapper, editOrganisationDetailsRequestCreator, () => weeeClient);
        }

        [Fact]
        public void Controller_Should_Have_ExternalSiteController_As_Base()
        {
            typeof(ProducerSubmissionController).BaseType.Name.Should().Be(nameof(ExternalSiteController));
        }

        [Fact]
        public void Controller_Should_Have_AuthorizeRouteClaims_Attribute()
        {
            // Arrange
            var controllerType = typeof(ProducerSubmissionController);

            // Act
            var attribute = controllerType.GetCustomAttributes(typeof(AuthorizeRouteClaimsAttribute), true)
                .FirstOrDefault() as AuthorizeRouteClaimsAttribute;

            // Assert
            attribute.Should().NotBeNull();

            var attributeType = typeof(AuthorizeRouteClaimsAttribute);
            var flags = BindingFlags.NonPublic | BindingFlags.Instance;

            var routeIdParamField = attributeType.GetField("routeIdParam", flags);
            var routeIdParam = routeIdParamField.GetValue(attribute) as string;

            var claimsField = attributeType.GetField("claims", flags);
            var claims = claimsField.GetValue(attribute) as string[];

            routeIdParam.Should().Be("directRegistrantId");
            claims.Should().ContainSingle().Which.Should().Be(WeeeClaimTypes.DirectRegistrantAccess);

            var usage = attribute.GetType().GetCustomAttribute<AttributeUsageAttribute>();
            usage.Should().NotBeNull();
            usage.ValidOn.Should().Be(AttributeTargets.Class | AttributeTargets.Method);
            usage.AllowMultiple.Should().BeTrue();
        }

        [Fact]
        public async Task EditOrganisationDetails_Get_ShouldReturnViewWithMappedModel()
        {
            // Arrange
            var submissionData = new SmallProducerSubmissionData();
            controller.SmallProducerSubmissionData = submissionData;

            var viewModel = TestFixture.Create<EditOrganisationDetailsViewModel>();
            A.CallTo(() => mapper.Map<SmallProducerSubmissionData, EditOrganisationDetailsViewModel>(submissionData)).Returns(viewModel);
            
            var countries = TestFixture.CreateMany<CountryData>().ToList();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetCountries>._)).Returns(Task.FromResult<IList<CountryData>>(countries));

            // Act
            var result = await controller.EditOrganisationDetails() as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result.ViewName.Should().BeEmpty();
            result.Model.Should().Be(viewModel);
            viewModel.Organisation.Address.Countries.Should().BeSameAs(countries);
        }

        [Fact]
        public async Task EditOrganisationDetails_Post_ValidModel_ShouldRedirectToTaskList()
        {
            // Arrange
            var model = TestFixture.Create<EditOrganisationDetailsViewModel>();
            var request = TestFixture.Create<EditOrganisationDetailsRequest>();
            A.CallTo(() => editOrganisationDetailsRequestCreator.ViewModelToRequest(model)).Returns(request);

            // Act
            var result = await controller.EditOrganisationDetails(model) as RedirectToRouteResult;

            // Assert
            result.Should().NotBeNull();
            result.RouteValues["action"].Should().Be("TaskList");
            result.RouteValues["controller"].Should().Be("Producer");
            A.CallTo(() => weeeClient.SendAsync(A<string>._, request)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditOrganisationDetails_Post_InvalidModel_ShouldReturnViewWithModel()
        {
            // Arrange
            var model = TestFixture.Create<EditOrganisationDetailsViewModel>();
            controller.ModelState.AddModelError("Test", "Test error");

            var countries = TestFixture.CreateMany<CountryData>().ToList();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetCountries>._)).Returns(Task.FromResult<IList<CountryData>>(countries));

            // Act
            var result = await controller.EditOrganisationDetails(model) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result.ViewName.Should().BeEmpty();
            result.Model.Should().Be(model);
            ((EditOrganisationDetailsViewModel)result.Model).Organisation.Address.Countries.Should().BeSameAs(countries);
        }

        [Fact]
        public void EditOrganisationDetails_Get_ShouldHaveSmallProducerSubmissionContextAttribute()
        {
            // Arrange
            var methodInfo = typeof(ProducerSubmissionController).GetMethod("EditOrganisationDetails", new Type[0]);

            // Act & Assert
            methodInfo.Should().BeDecoratedWith<SmallProducerSubmissionContextAttribute>();
            methodInfo.Should().BeDecoratedWith<HttpGetAttribute>();
        }
    }
}
