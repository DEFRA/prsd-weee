﻿namespace EA.Weee.Web.Tests.Unit.Areas.Producer.Filters
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using EA.Weee.Web.Areas.Producer.Controllers;
    using EA.Weee.Web.Areas.Producer.Filters;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Infrastructure.PDF;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using EA.Weee.Web.Services.SubmissionService;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using Xunit;

    public class SmallProducerSubmissionContextAttributeTests
    {
        // Dummy controller for testing unsupported controller type
        public class HomeController : Controller
        {
        }

        private readonly IWeeeClient fakeClient;
        private readonly IWeeeCache weeeCache;
        private readonly ActionExecutingContext actionExecutingContext;
        private readonly SmallProducerSubmissionContextAttribute filter;
        private readonly ISubmissionService submissionService;

        public SmallProducerSubmissionContextAttributeTests()
        {
            fakeClient = A.Fake<IWeeeClient>();
            this.weeeCache = A.Fake<IWeeeCache>();

            var fakeHttpContext = A.Fake<HttpContextBase>();

            var breadcrumbService = A.Fake<BreadcrumbService>();
            var mapper = A.Fake<IMapper>();
            var templateExecutor = A.Fake<IMvcTemplateExecutor>();
            var pdfDocumentProvider = A.Fake<IPdfDocumentProvider>();
            submissionService = A.Fake<ISubmissionService>();

            actionExecutingContext = new ActionExecutingContext
            {
                ActionParameters = new System.Web.Routing.RouteValueDictionary(),
                Controller = new ProducerController(breadcrumbService, weeeCache, mapper, templateExecutor, pdfDocumentProvider, submissionService),
                HttpContext = fakeHttpContext,
                RouteData = new System.Web.Routing.RouteData()
            };

            filter = new SmallProducerSubmissionContextAttribute
            {
                Client = () => fakeClient,
                Cache = weeeCache
            };

            var fakePrincipal = A.Fake<ClaimsPrincipal>();
            A.CallTo(() => fakePrincipal.Identity.IsAuthenticated).Returns(true);
            A.CallTo(() => fakeHttpContext.User).Returns(fakePrincipal);
        }

        [Fact]
        public void OnActionExecuting_WithValidDirectRegistrantId_SetsSmallProducerSubmissionData()
        {
            // Arrange
            var directRegistrantId = Guid.NewGuid();
            actionExecutingContext.RouteData.Values["directRegistrantId"] = directRegistrantId.ToString();

            var expectedData = new SmallProducerSubmissionData();
            A.CallTo(() => fakeClient.SendAsync(A<string>._, A<GetSmallProducerSubmission>.That.Matches(g => g.DirectRegistrantId == directRegistrantId)))
                .Returns(Task.FromResult(expectedData));

            // Act
            filter.OnActionExecuting(actionExecutingContext);

            // Assert
            var controller = (ProducerController)actionExecutingContext.Controller;
            controller.SmallProducerSubmissionData.Should().Be(expectedData);
        }

        [Fact]
        public void OnActionExecuting_WithMissingDirectRegistrantId_ThrowsArgumentException()
        {
            // Act & Assert
            filter.Invoking(f => f.OnActionExecuting(actionExecutingContext))
                .Should().Throw<ArgumentException>()
                .WithMessage("No direct registrant ID was specified.");
        }

        [Fact]
        public void OnActionExecuting_WithInvalidDirectRegistrantId_ThrowsArgumentException()
        {
            // Arrange
            actionExecutingContext.RouteData.Values["directRegistrantId"] = "invalid-guid";

            // Act & Assert
            filter.Invoking(f => f.OnActionExecuting(actionExecutingContext))
                .Should().Throw<ArgumentException>()
                .WithMessage("The specified direct registrant ID is not valid.");
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void OnActionExecuting_WhenDataIsNull_CreatesNewSubmission(bool invalidateCache)
        {
            // Arrange
            var directRegistrantId = Guid.NewGuid();
            actionExecutingContext.RouteData.Values["directRegistrantId"] = directRegistrantId.ToString();

            A.CallTo(() => fakeClient.SendAsync(A<string>._, A<GetSmallProducerSubmission>._))
                .Returns(Task.FromResult(new SmallProducerSubmissionData()))
                .Once()
                .Then
                .Returns(Task.FromResult(new SmallProducerSubmissionData()));

            A.CallTo(() => fakeClient.SendAsync(A<string>._, A<AddSmallProducerSubmission>._))
                .Returns(new AddSmallProducerSubmissionResult(invalidateCache, A.Dummy<Guid>()));

            // Act
            filter.OnActionExecuting(actionExecutingContext);

            // Assert
            A.CallTo(() => fakeClient.SendAsync(A<string>._, A<AddSmallProducerSubmission>.That.Matches(a => a.DirectRegistrantId == directRegistrantId)))
                .MustHaveHappenedOnceExactly();
            if (invalidateCache)
            {
                A.CallTo(() => weeeCache.InvalidateSmallProducerSearch()).MustHaveHappenedOnceExactly();
            }
            else
            {
                A.CallTo(() => weeeCache.InvalidateSmallProducerSearch()).MustNotHaveHappened();
            }
        }

        [Fact]
        public void OnActionExecuting_WithUnsupportedController_ThrowsInvalidOperationException()
        {
            // Arrange
            var directRegistrantId = Guid.NewGuid();
            actionExecutingContext.RouteData.Values["directRegistrantId"] = directRegistrantId.ToString();

            // Create a concrete controller that's not supported by the filter
            actionExecutingContext.Controller = new HomeController();

            A.CallTo(() => fakeClient.SendAsync(A<string>._, A<GetSmallProducerSubmission>._))
                .Returns(Task.FromResult(new SmallProducerSubmissionData()));

            // Act & Assert
            filter.Invoking(f => f.OnActionExecuting(actionExecutingContext))
                .Should().Throw<InvalidOperationException>()
                .WithMessage("Unsupported controller type");
        }
    }
}