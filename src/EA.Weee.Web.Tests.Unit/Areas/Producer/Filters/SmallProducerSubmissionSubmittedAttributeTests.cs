namespace EA.Weee.Web.Tests.Unit.Areas.Producer.Filters
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using EA.Weee.Web.Areas.Producer.Controllers;
    using EA.Weee.Web.Areas.Producer.Filters;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Infrastructure.PDF;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using Xunit;

    public class SmallProducerSubmissionSubmittedAttributeTests
    {
        private readonly IWeeeClient fakeClient;
        private readonly ActionExecutingContext actionExecutingContextProducer;
        private readonly ActionExecutingContext actionExecutingContextProducerSubmission;
        private readonly SmallProducerSubmissionSubmittedAttribute filter;

        public SmallProducerSubmissionSubmittedAttributeTests()
        {
            fakeClient = A.Fake<IWeeeClient>();
            var fakeHttpContext = A.Fake<HttpContextBase>();

            var breadcrumbService = A.Fake<BreadcrumbService>();
            var weeeCache = A.Fake<IWeeeCache>();
            var mapper = A.Fake<IMapper>();
            var templateExecutor = A.Fake<IMvcTemplateExecutor>();
            var pdfDocumentProvider = A.Fake<IPdfDocumentProvider>();

            actionExecutingContextProducer = new ActionExecutingContext
            {
                ActionParameters = new System.Web.Routing.RouteValueDictionary(),
                Controller = new ProducerController(breadcrumbService, weeeCache, mapper, templateExecutor, pdfDocumentProvider),
                HttpContext = fakeHttpContext,
                RouteData = new System.Web.Routing.RouteData()
            };

            filter = new SmallProducerSubmissionSubmittedAttribute();
        }

        [Theory]
        [InlineData(true, SubmissionStatus.Submitted)]
        [InlineData(true, SubmissionStatus.InComplete)]
        [InlineData(false, SubmissionStatus.Submitted)]
        public void OnActionExecuting_WithStatusParamsProducerController_Redirects(bool hasPaid, SubmissionStatus status)
        {
            // Arrange
            var controller = (ProducerController)actionExecutingContextProducer.Controller;

            controller.SmallProducerSubmissionData = new Core.DirectRegistrant.SmallProducerSubmissionData
            {
                OrganisationData = new OrganisationData
                {
                    Id = Guid.Empty
                },
                CurrentSubmission = new Core.DirectRegistrant.SmallProducerSubmissionHistoryData
                {
                    HasPaid = hasPaid,
                    Status = status,
                    ComplianceYear = 2005,
                    ContactDetailsComplete = true,
                    EEEDetailsComplete = true,
                    OrganisationDetailsComplete = true,
                    RepresentingCompanyDetailsComplete = false,
                    ServiceOfNoticeComplete = true
                }
            };

            // Act
            filter.OnActionExecuting(actionExecutingContextProducer);

            if (hasPaid && status == SubmissionStatus.Submitted)
            {
                actionExecutingContextProducer.Result.Should().BeEquivalentTo(new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary
                {
                    { "action", "AlreadySubmittedAndPaid" },
                    { "controller", "Producer" }
                }));
            }
            else
            {
                actionExecutingContextProducer.Result.Should().BeNull();
            }
        }

        [Fact]
        public void OnActionExecuting_WithUnsupportedController_ThrowsInvalidOperationException()
        {
            // Arrange
            // Create a concrete controller that's not supported by the filter
            actionExecutingContextProducer.Controller = new HomeController();

            // Act & Assert
            filter.Invoking(f => f.OnActionExecuting(actionExecutingContextProducer))
                .Should().Throw<InvalidOperationException>()
                .WithMessage("Unsupported controller type");
        }

        // Dummy controller for testing unsupported controller type
        private class HomeController : Controller
        {
        }
    }
}