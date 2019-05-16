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

    public class ValidateReturnActionFilterAttributeTests
    {
        private readonly ValidateReturnActionFilterAttribute attribute;
        private readonly ActionExecutingContext context;
        private readonly IWeeeClient client;

        public ValidateReturnActionFilterAttributeTests()
        {
            client = A.Fake<IWeeeClient>();
            attribute = new ValidateReturnActionFilterAttribute { ConfigService = A.Fake<ConfigurationService>(), Client = () => client };
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

        [Fact]
        public void OnActionExecuting_GivenReturnStatusIsNotCreated_ShouldBeRedirectedToTaskList()
        {
            var returnData = new ReturnStatusData()
            {
                OrganisationId = Guid.NewGuid(),
                ReturnStatus = ReturnStatus.Submitted
            };

            A.CallTo(() => client.SendAsync(A<string>._,
                A<GetReturnStatus>.That.Matches(r => r.ReturnId.Equals((Guid)context.RouteData.Values["returnId"])))).Returns(returnData);

            attribute.OnActionExecuting(context);

            var result = context.Result as RedirectToRouteResult;

            result.RouteName.Should().Be(AatfRedirect.ReturnsRouteName);
            result.RouteValues["controller"].Should().Be("Returns");
            result.RouteValues["action"].Should().Be("Index");
            result.RouteValues["organisationId"].Should().Be(returnData.OrganisationId);
        }
    }
}
