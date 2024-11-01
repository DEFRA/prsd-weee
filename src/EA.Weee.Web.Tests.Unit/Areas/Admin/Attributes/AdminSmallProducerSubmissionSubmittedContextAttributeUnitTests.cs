namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Attributes
{
    using EA.Weee.Api.Client;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Helpers;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Shared;
    using EA.Weee.Requests.Admin.DirectRegistrants;
    using EA.Weee.Web.Areas.Admin.Controllers;
    using EA.Weee.Web.Areas.Admin.Filters;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using EA.Weee.Web.Services.SubmissionService;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Security.Principal;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Xunit;

    public class AdminSmallProducerSubmissionSubmittedContextAttributeUnitTests
    {
        private readonly IWeeeClient client;
        private readonly HttpContextBase httpContext;
        private readonly IPrincipal principal;
        private readonly ActionExecutingContext actionContext;
        private readonly ProducerSubmissionController controller;
        private readonly RequestContext requestContext;
        private readonly HttpRequestBase request;

        public AdminSmallProducerSubmissionSubmittedContextAttributeUnitTests()
        {
            client = A.Fake<IWeeeClient>();
            httpContext = A.Fake<HttpContextBase>();
            principal = A.Fake<IPrincipal>();
            request = A.Fake<HttpRequestBase>();
            controller = new ProducerSubmissionController(A.Fake<IWeeeClient>, A.Fake<IWeeeCache>(), A.Fake<ISubmissionService>(), A.Fake<BreadcrumbService>());

            requestContext = new RequestContext(httpContext, new RouteData());
            controller.ControllerContext = new ControllerContext(requestContext, controller);

            actionContext = new ActionExecutingContext(
                controller.ControllerContext,
                A.Fake<ActionDescriptor>(),
                new RouteValueDictionary());

            A.CallTo(() => httpContext.Request).Returns(request);
            A.CallTo(() => httpContext.User).Returns(principal);
        }

        [Fact]
        public void OnActionExecuting_GivenNoSubmissions_RedirectsToNoSubmissionsAction()
        {
            // Arrange
            var attribute = new AdminSmallProducerSubmissionSubmittedContextAttribute { Client = () => client };

            var organisationId = Guid.NewGuid();

            var submissionData = CreateSubmissionData(
                organisationId,
                new Dictionary<int, SmallProducerSubmissionHistoryData>());

            controller.SmallProducerSubmissionData = submissionData;

            // Act
            attribute.OnActionExecuting(actionContext);

            // Assert
            var result = actionContext.Result.Should().BeOfType<RedirectToRouteResult>().Subject;
            result.RouteValues["action"].Should().Be(nameof(ProducerSubmissionController.OrganisationHasNoSubmissions));
            result.RouteValues["controller"].Should().Be(typeof(ProducerSubmissionController).GetControllerName());
            result.RouteValues["organisationId"].Should().Be(organisationId);
        }

        [Fact]
        public void OnActionExecuting_GivenSomeSubmissions_DoesNotRedirectToNoSubmissionsAction()
        {
            // Arrange
            var attribute = new AdminSmallProducerSubmissionSubmittedContextAttribute { Client = () => client };

            var organisationId = Guid.NewGuid();

            var submissionData = CreateSubmissionData(
                organisationId,
                new Dictionary<int, SmallProducerSubmissionHistoryData>()
                {
                    { 2024, new SmallProducerSubmissionHistoryData() },
                });

            controller.SmallProducerSubmissionData = submissionData;

            // Act
            attribute.OnActionExecuting(actionContext);

            // Assert
            var result = actionContext.Result.Should().BeNull();
        }

        private static SmallProducerSubmissionData CreateSubmissionData(Guid organisationId, IDictionary<int, SmallProducerSubmissionHistoryData> submissionHistory)
        {
            return new SmallProducerSubmissionData
            {
                OrganisationData = new OrganisationData { Id = organisationId },
                ContactData = new ContactData { FirstName = "Test", LastName = "User" },
                ContactAddressData = new AddressData(),
                SubmissionHistory = submissionHistory,
                DirectRegistrantId = Guid.NewGuid(),
                HasAuthorisedRepresentitive = false,
                AuthorisedRepresentitiveData = null,
                ServiceOfNoticeData = new AddressData(),
                ProducerRegistrationNumber = "TEST1234"
            };
        }
    }
}