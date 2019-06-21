namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Attributes
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Api.Client;
    using Core.AatfReturn;
    using FakeItEasy;
    using FluentAssertions;
    using Infrastructure;
    using Services;
    using Web.Areas.AatfReturn.Attributes;
    using Weee.Requests.AatfReturn;
    using Xunit;

    public class ValidateReturnBaseActionFilterAttributeTests
    {
        private readonly ValidateReturnTestActionFilterAttribute attribute;
        private readonly ActionExecutingContext context;
        private readonly IWeeeClient client;

        public ValidateReturnBaseActionFilterAttributeTests()
        {
            client = A.Fake<IWeeeClient>();
            attribute = new ValidateReturnTestActionFilterAttribute { ConfigService = A.Fake<ConfigurationService>(), Client = () => client };
            context = A.Fake<ActionExecutingContext>();
            
            var routeData = new RouteData();
            routeData.Values.Add("returnId", Guid.NewGuid());
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
        public void OnActionExecuting_GivenNoReturnIdId_ArgumentExceptionExpected()
        {
            Action action = () => attribute.OnActionExecuting(context);

            A.CallTo(() => context.RouteData).Returns(new RouteData());

            action.Should().Throw<ArgumentException>().WithMessage("No return ID was specified.");
        }

        [Fact]
        public void OnActionExecuting_GivenReturnIdIsNotGuid_ArgumentExceptionExpected()
        {
            Action action = () => attribute.OnActionExecuting(context);

            var routeData = new RouteData();
            routeData.Values.Add("returnId", 1);

            A.CallTo(() => context.RouteData).Returns(routeData);

            action.Should().Throw<ArgumentException>().WithMessage("The specified return ID is not valid.");
        }
    }
}
