namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Attributes
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using System.Web.Routing;
    using FakeItEasy;
    using FluentAssertions;
    using Services;
    using Web.Areas.AatfReturn.Attributes;
    using Xunit;

    public class ValidateOrganisationActionFilterAttributeTests
    {
        private readonly ValidateOrganisationActionFilterAttribute attribute;
        private readonly ActionExecutingContext context;

        public ValidateOrganisationActionFilterAttributeTests()
        {
            attribute = new ValidateOrganisationActionFilterAttribute { ConfigService = A.Fake<ConfigurationService>() };
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
    }
}
