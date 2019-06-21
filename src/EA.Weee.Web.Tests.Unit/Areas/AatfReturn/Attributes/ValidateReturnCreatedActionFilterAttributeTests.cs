namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Attributes
{
    using Api.Client;
    using Core.AatfReturn;
    using EA.Weee.Web.Infrastructure;
    using FakeItEasy;
    using FluentAssertions;
    using Services;
    using System;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Web.Areas.AatfReturn.Attributes;
    using Weee.Requests.AatfReturn;
    using Xunit;

    public class ValidateReturnCreatedActionFilterAttributeTests
    {
        private readonly ValidateReturnCreatedActionFilterAttribute attribute;
        private readonly ActionExecutingContext context;
        private readonly IWeeeClient client;

        public ValidateReturnCreatedActionFilterAttributeTests()
        {
            client = A.Fake<IWeeeClient>();
            attribute = new ValidateReturnCreatedActionFilterAttribute { ConfigService = A.Fake<ConfigurationService>(), Client = () => client };
            context = A.Fake<ActionExecutingContext>();
            
            var routeData = new RouteData();
            routeData.Values.Add("returnId", Guid.NewGuid());
            A.CallTo(() => context.RouteData).Returns(routeData);
            A.CallTo(() => attribute.ConfigService.CurrentConfiguration.EnableAATFReturns).Returns(true);
        }

        [Fact]
        public async void OnActionExecuting_GivenReturnStatusIsNotCreated_ShouldBeRedirectedToTaskList()
        {
            var returnData = new ReturnStatusData()
            {
                OrganisationId = Guid.NewGuid(),
                ReturnStatus = ReturnStatus.Submitted
            };

            A.CallTo(() => client.SendAsync(A<string>._,
                A<GetReturnStatus>.That.Matches(r => r.ReturnId.Equals((Guid)context.RouteData.Values["returnId"])))).Returns(returnData);

            await attribute.OnAuthorizationAsync(context, (Guid)context.RouteData.Values["returnId"]);

            var result = context.Result as RedirectToRouteResult;

            result.RouteName.Should().Be(AatfRedirect.ReturnsRouteName);
            result.RouteValues["controller"].Should().Be("Returns");
            result.RouteValues["action"].Should().Be("Index");
            result.RouteValues["organisationId"].Should().Be(returnData.OrganisationId);
        }

        [Fact]
        public async void OnActionExecuting_GivenReturnStatusIsCreated_ContextResultShouldBeNull()
        {
            var returnData = new ReturnStatusData()
            {
                OrganisationId = Guid.NewGuid(),
                ReturnStatus = ReturnStatus.Created
            };

            A.CallTo(() => client.SendAsync(A<string>._,
                A<GetReturnStatus>.That.Matches(r => r.ReturnId.Equals((Guid)context.RouteData.Values["returnId"])))).Returns(returnData);

            await attribute.OnAuthorizationAsync(context, (Guid)context.RouteData.Values["returnId"]);

            context.Result.Should().BeNull();
        }

        [Fact]
        public void ValidateReturnCreatedActionFilterAttribute_ShouldInheritFromValidateReturnBaseActionFilterAttribute()
        {
            typeof(ValidateReturnCreatedActionFilterAttribute).BaseType.Name.Should().Be(typeof(ValidateReturnBaseActionFilterAttribute).Name);
        }
    }
}
