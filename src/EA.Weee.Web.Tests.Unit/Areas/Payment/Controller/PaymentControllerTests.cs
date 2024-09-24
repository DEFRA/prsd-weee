namespace EA.Weee.Web.Tests.Unit.Areas.Payment.Controller
{
    using AutoFixture;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Tests.Core;
    using EA.Weee.Web.Areas.Payment.Controllers;
    using EA.Weee.Web.Areas.Producer.Controllers;
    using EA.Weee.Web.Controllers.Base;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Services;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Xunit;

    public class PaymentControllerTests : SimpleUnitTestBase
    {
        private readonly IPaymentService paymentService;
        private readonly PaymentController controller;

        public PaymentControllerTests()
        {
            paymentService = A.Fake<IPaymentService>();
            controller = new PaymentController(paymentService);

            var identity = A.Fake<System.Security.Principal.IIdentity>();
            var principal = A.Fake<System.Security.Principal.IPrincipal>();
            A.CallTo(() => principal.Identity).Returns(identity);
            A.CallTo(() => identity.IsAuthenticated).Returns(true);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = A.Fake<System.Web.HttpContextBase>(),
                Controller = controller
            };
            A.CallTo(() => controller.ControllerContext.HttpContext.User).Returns(principal);
        }

        [Fact]
        public void PaymentController_ShouldInheritFromExternalSiteController()
        {
            // Assert
            typeof(PaymentController).Should().BeDerivedFrom<ExternalSiteController>();
        }

        [Fact]
        public async Task PaymentResult_WithValidPaymentReference_RedirectsToPaymentSuccess()
        {
            // Arrange
            var token = TestFixture.Create<string>();
            var paymentReference = TestFixture.Create<string>();
            var directRegistrantId = Guid.NewGuid();

            A.CallTo(() => paymentService.HandlePaymentReturnAsync(A<string>._, token))
                .Returns(new PaymentResult
                {
                    PaymentReference = paymentReference,
                    DirectRegistrantId = directRegistrantId
                });

            // Act
            var result = await controller.PaymentResult(token) as RedirectToRouteResult;

            // Assert
            result.Should().NotBeNull();
            result.RouteValues["action"].Should().Be(nameof(ProducerSubmissionController.PaymentSuccess));
            result.RouteValues["controller"].Should().Be("ProducerSubmission");
            result.RouteValues["area"].Should().Be("Producer");
            result.RouteValues["directRegistrantId"].Should().Be(directRegistrantId);
            result.RouteValues["reference"].Should().Be(paymentReference);
        }

        [Fact]
        public async Task PaymentResult_WithInvalidPaymentReference_RedirectsToPaymentFailure()
        {
            // Arrange
            var token = TestFixture.Create<string>();
            var directRegistrantId = Guid.NewGuid();

            A.CallTo(() => paymentService.HandlePaymentReturnAsync(A<string>._, token))
                .Returns(new PaymentResult
                {
                    PaymentReference = null,
                    DirectRegistrantId = directRegistrantId
                });

            // Act
            var result = await controller.PaymentResult(token) as RedirectToRouteResult;

            // Assert
            result.Should().NotBeNull();
            result.RouteValues["action"].Should().Be(nameof(ProducerSubmissionController.PaymentFailure));
            result.RouteValues["controller"].Should().Be("ProducerSubmission");
            result.RouteValues["area"].Should().Be("Producer");
            result.RouteValues["directRegistrantId"].Should().Be(directRegistrantId);
            result.RouteValues.Should().NotContainKey("reference");
        }

        [Fact]
        public async Task PaymentResult_CallsHandlePaymentReturnAsyncWithCorrectParameters()
        {
            // Arrange
            var token = TestFixture.Create<string>();

            // Act
            await controller.PaymentResult(token);

            // Assert
            A.CallTo(() => paymentService.HandlePaymentReturnAsync(A<string>._, token))
                .MustHaveHappenedOnceExactly();
        }
    }
}