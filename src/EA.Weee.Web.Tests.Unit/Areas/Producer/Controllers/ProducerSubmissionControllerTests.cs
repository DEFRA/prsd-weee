namespace EA.Weee.Web.Tests.Unit.Areas.Producer.Controllers
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using EA.Weee.Tests.Core;
    using EA.Weee.Web.Areas.Producer.Controllers;
    using EA.Weee.Web.Areas.Producer.Filters;
    using EA.Weee.Web.Areas.Producer.ViewModels;
    using EA.Weee.Web.Controllers.Base;
    using EA.Weee.Web.Requests.Base;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Linq;
    using System.Reflection;
    using Xunit;

    public class ProducerSubmissionControllerTests : SimpleUnitTestBase
    {
        private readonly ProducerSubmissionController controller;
        private readonly IMapper mapper;

        private readonly IRequestCreator<EditOrganisationDetailsViewModel, EditProducerSubmissionAddressRequest>
            editOrganisationDetailsRequestCreator;

        public ProducerSubmissionControllerTests()
        {
            mapper = A.Fake<IMapper>();
            editOrganisationDetailsRequestCreator =
                A.Fake<IRequestCreator<EditOrganisationDetailsViewModel, EditProducerSubmissionAddressRequest>>();

            controller = new ProducerSubmissionController(mapper, editOrganisationDetailsRequestCreator);
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
    }
}
