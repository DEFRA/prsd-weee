namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Attributes
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using System.Web.Routing;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Requests.AatfReturn;
    using FakeItEasy;
    using FluentAssertions;
    using Services;
    using Web.Areas.AatfReturn.Attributes;
    using Xunit;

    public class ValidateOrganisationActionFilterAttributeTests
    {
        private readonly ValidateOrganisationActionFilterAttribute attribute;
        private readonly ActionExecutingContext context;
        private readonly IWeeeClient client;

        public ValidateOrganisationActionFilterAttributeTests()
        {
            client = A.Fake<IWeeeClient>();
            attribute = new ValidateOrganisationActionFilterAttribute { ConfigService = A.Fake<ConfigurationService>(), Client = () => client, FacilityType = FacilityType.Aatf};
            context = A.Fake<ActionExecutingContext>();
            
            var routeData = new RouteData();
            routeData.Values.Add("organisationId", Guid.NewGuid());
            A.CallTo(() => context.RouteData).Returns(routeData);

            A.CallTo(() => attribute.ConfigService.CurrentConfiguration.EnableAATFReturns).Returns(true);
        }

        [Fact]
        public void OnActionExecuting_GivenEnableAatfReturnIsFalse_InvalidOperationExceptionExpected()
        {
            A.CallTo(() => attribute.ConfigService.CurrentConfiguration.EnableAATFReturns).Returns(false);

            Action action = () => attribute.OnActionExecuting(context);

            action.Should().Throw<InvalidOperationException>().WithMessage("AATF returns are not enabled.");
        }

        [Fact]
        public void OnActionExecuting_GivenNoOrganisationId_ArgumentExceptionExpected()
        {
            Action action = () => attribute.OnActionExecuting(context);

            A.CallTo(() => context.RouteData).Returns(new RouteData());

            action.Should().Throw<ArgumentException>().WithMessage("No organisation ID was specified.");
        }

        [Fact]
        public void OnActionExecuting_GivenOrganisationIdIsNotGuid_ArgumentExceptionExpected()
        {
            Action action = () => attribute.OnActionExecuting(context);

            var routeData = new RouteData();
            routeData.Values.Add("organisationId", 1);

            A.CallTo(() => context.RouteData).Returns(routeData);

            action.Should().Throw<ArgumentException>().WithMessage("The specified organisation ID is not valid.");
        }

        [Fact]
        public void OnActionExecuting_NoAATFsOrAE_ArgumentExceptionExpected()
        {
            Action action = () => attribute.OnActionExecuting(context);

            Guid orgId = Guid.NewGuid();

            var routeData = new RouteData();
            routeData.Values.Add("organisationId", orgId);

            A.CallTo(() => context.RouteData).Returns(routeData);
            A.CallTo(() => client.SendAsync(A<string>._, new GetAatfByOrganisation(orgId))).Returns(new List<AatfData>());

            action.Should().Throw<InvalidOperationException>().WithMessage("No AATF found for this organisation.");
        }
    }
}
