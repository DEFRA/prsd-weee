namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Attributes
{
    using Api.Client;
    using Core.AatfReturn;
    using EA.Prsd.Core;
    using EA.Weee.Web.Infrastructure;
    using FakeItEasy;
    using FluentAssertions;
    using Services;
    using System;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Core.DataReturns;
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
            var returnStatusData = new ReturnStatusData()
            {
                OrganisationId = Guid.NewGuid(),
                ReturnStatus = ReturnStatus.Submitted
            };

            SystemTime.Freeze(new DateTime(2019, 04, 01));

            var returnData = new ReturnData()
            {
                QuarterWindow = new QuarterWindow(new DateTime(2019, 01, 01), new DateTime(2019, 03, 31), (int)QuarterType.Q1)
            };

            A.CallTo(() => client.SendAsync(A<string>._,
                A<GetReturnStatus>.That.Matches(r => r.ReturnId.Equals((Guid)context.RouteData.Values["returnId"])))).Returns(returnStatusData);

            A.CallTo(() => client.SendAsync(A<string>._,
               A<GetReturn>.That.Matches(r => r.ReturnId.Equals((Guid)context.RouteData.Values["returnId"])))).Returns(returnData);

            await attribute.OnAuthorizationAsync(context, (Guid)context.RouteData.Values["returnId"]);

            SystemTime.Unfreeze();

            var result = context.Result as RedirectToRouteResult;

            result.RouteName.Should().Be(AatfRedirect.ReturnsRouteName);
            result.RouteValues["controller"].Should().Be("Returns");
            result.RouteValues["action"].Should().Be("Index");
            result.RouteValues["organisationId"].Should().Be(returnStatusData.OrganisationId);
        }

        [Fact]
        public async void OnActionExecuting_GivenReturnStatusIsCreated_ContextResultShouldBeNull()
        {
            var returnStatusData = new ReturnStatusData()
            {
                OrganisationId = Guid.NewGuid(),
                ReturnStatus = ReturnStatus.Created
            };

            SystemTime.Freeze(new DateTime(2019, 04, 01));

            var returnData = new ReturnData()
            {
                QuarterWindow = new QuarterWindow(new DateTime(2019, 01, 01), new DateTime(2019, 03, 31), (int)QuarterType.Q1)
            };

            A.CallTo(() => client.SendAsync(A<string>._,
                A<GetReturnStatus>.That.Matches(r => r.ReturnId.Equals((Guid)context.RouteData.Values["returnId"])))).Returns(returnStatusData);

            A.CallTo(() => client.SendAsync(A<string>._,
                A<GetReturn>.That.Matches(r => r.ReturnId.Equals((Guid)context.RouteData.Values["returnId"])))).Returns(returnData);

            await attribute.OnAuthorizationAsync(context, (Guid)context.RouteData.Values["returnId"]);

            SystemTime.Unfreeze();

            context.Result.Should().BeNull();
        }

        [Fact]
        public async void OnActionExecuting_GivenReturnStatusIsCreatedAndQuaterWindowForReturnIsClosed_ContextResultReturnsErrorPage()
        {
            var returnStatusData = new ReturnStatusData()
            {
                OrganisationId = Guid.NewGuid(),
                ReturnStatus = ReturnStatus.Created
            };

            SystemTime.Freeze(new DateTime(2019, 04, 01));

            var returnData = new ReturnData()
            {
                QuarterWindow = new QuarterWindow(new DateTime(2018, 01, 01), new DateTime(2018, 03, 31), (int)QuarterType.Q1)
            };

            A.CallTo(() => client.SendAsync(A<string>._,
                A<GetReturnStatus>.That.Matches(r => r.ReturnId.Equals((Guid)context.RouteData.Values["returnId"])))).Returns(returnStatusData);

            A.CallTo(() => client.SendAsync(A<string>._,
                A<GetReturn>.That.Matches(r => r.ReturnId.Equals((Guid)context.RouteData.Values["returnId"])))).Returns(returnData);

            await attribute.OnAuthorizationAsync(context, (Guid)context.RouteData.Values["returnId"]);

            SystemTime.Unfreeze();

            RedirectResult result = context.Result as RedirectResult;

            Assert.Equal("~/errors/QuarterClosed", result.Url);
        }

        [Fact]
        public async void OnActionExecuting_GivenReturnStatusIsCreatedAndQuaterWindowForReturnIsOpen_ContextResultShouldBeNull()
        {
            var returnStatusData = new ReturnStatusData()
            {
                OrganisationId = Guid.NewGuid(),
                ReturnStatus = ReturnStatus.Created
            };

            SystemTime.Freeze(new DateTime(2019, 04, 01));

            var returnData = new ReturnData()
            {
                QuarterWindow = new QuarterWindow(new DateTime(2019, 01, 01), new DateTime(2019, 03, 31), (int)QuarterType.Q1)
            };

            A.CallTo(() => client.SendAsync(A<string>._,
                A<GetReturnStatus>.That.Matches(r => r.ReturnId.Equals((Guid)context.RouteData.Values["returnId"])))).Returns(returnStatusData);

            A.CallTo(() => client.SendAsync(A<string>._,
                A<GetReturn>.That.Matches(r => r.ReturnId.Equals((Guid)context.RouteData.Values["returnId"])))).Returns(returnData);

            await attribute.OnAuthorizationAsync(context, (Guid)context.RouteData.Values["returnId"]);

            SystemTime.Unfreeze();

            context.Result.Should().BeNull();
        }

        [Fact]
        public void ValidateReturnCreatedActionFilterAttribute_ShouldInheritFromValidateReturnBaseActionFilterAttribute()
        {
            typeof(ValidateReturnCreatedActionFilterAttribute).BaseType.Name.Should().Be(typeof(ValidateReturnBaseActionFilterAttribute).Name);
        }
    }
}
