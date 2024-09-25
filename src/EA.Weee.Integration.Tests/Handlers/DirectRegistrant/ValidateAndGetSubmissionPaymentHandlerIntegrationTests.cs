namespace EA.Weee.Integration.Tests.Handlers.DirectRegistrant
{
    using Autofac;
    using AutoFixture;
    using Base;
    using EA.Prsd.Core;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Domain.Producer;
    using EA.Weee.Integration.Tests.Builders;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using FluentAssertions;
    using NUnit.Specifications.Categories;
    using Prsd.Core.Autofac;
    using Prsd.Core.Mediator;
    using System.Security;

    public class ValidateAndGetSubmissionPaymentHandlerIntegrationTests : IntegrationTestBase
    {
        [Component]
        public class WhenThePaymentReturnTokenDoesNotExist : GetInProgressPaymentSessionRequestHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();
            };

            private readonly Because of = () =>
            {
                result = AsyncHelper.RunSync(() => handler.HandleAsync(request));
            };

            private readonly It shouldUpdateTheData = () =>
            {
                result.Should().NotBeNull();
                result.ErrorMessage.Should().Contain("No payment request exists ");
            };
        }

        [Component]
        public class WhenThePaymentDoesNotExistForSubmission : GetInProgressPaymentSessionRequestHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                var directRegistrantAnother = DirectRegistrantDbSetup.Init()
                    .Create();

                var directProducerSubmission = DirectRegistrantSubmissionDbSetup.Init()
                    .WithDefaultRegisteredProducer()
                    .WithComplianceYear(SystemTime.UtcNow.Year)
                    .WithDirectRegistrant(directRegistrantAnother)
                    .Create();

                directProducerSubmissionHistory = DirectRegistrantSubmissionHistoryDbSetup.Init()
                    .WithDirectProducerSubmission(directProducerSubmission).Create();

                Query.UpdateCurrentProducerSubmission(directProducerSubmission.Id, directProducerSubmissionHistory.Id);

                var submission = Query.GetDirectProducerSubmissionById(directProducerSubmissionHistory.DirectProducerSubmissionId);

                paymentSession = PaymentSessionDbSetup.Init()
                    .WithUser(UserId.ToString())
                    .WithStatus(PaymentState.New)
                    .WithPaymentTokenUrl(request.PaymentReturnToken)
                    .WithDirectRegistrantSubmission(submission).Create();
            };

            private readonly Because of = () =>
            {
                result = AsyncHelper.RunSync(() => handler.HandleAsync(request));
            };

            private readonly It shouldUpdateTheData = () =>
            {
                result.Should().NotBeNull();
                result.ErrorMessage.Should().Contain($"No payment request {request.PaymentReturnToken} exists");
            };
        }

        [Component]
        public class WhenThePaymentExistForSubmission : GetInProgressPaymentSessionRequestHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                paymentSession = PaymentSessionDbSetup.Init()
                    .WithUser(UserId.ToString())
                    .WithStatus(PaymentState.New)
                    .WithPaymentTokenUrl(request.PaymentReturnToken)
                    .WithDirectRegistrantSubmission(directProducerSubmission).Create();
            };

            private readonly Because of = () =>
            {
                result = AsyncHelper.RunSync(() => handler.HandleAsync(request));
            };

            private readonly It shouldUpdateTheData = () =>
            {
                result.Should().NotBeNull();
                result.ErrorMessage.Should().BeNullOrEmpty();
                result.DirectRegistrantId.Should().Be(directRegistrant.Id);
                result.PaymentId.Should().Be(paymentSession.PaymentId);
                result.PaymentReference.Should().Be(paymentSession.PaymentReference);
                result.PaymentSessionId.Should().Be(paymentSession.Id);
            };
        }

        [Component]
        public class WhenUserIsNotAuthorised : GetInProgressPaymentSessionRequestHandlerIntegrationTestBase
        {
            protected static IRequestHandler<ValidateAndGetSubmissionPayment, SubmissionPaymentDetails> authHandler;

            private readonly Establish context = () =>
            {
                SetupTest(IocApplication.RequestHandler)
                    .WithDefaultSettings();

                authHandler = Container.Resolve<IRequestHandler<ValidateAndGetSubmissionPayment, SubmissionPaymentDetails>>();
            };

            private readonly Because of = () =>
            {
                CatchExceptionAsync(() => authHandler.HandleAsync(request));
            };

            private readonly It shouldHaveCaughtArgumentException = ShouldThrowException<SecurityException>;
        }

        public class GetInProgressPaymentSessionRequestHandlerIntegrationTestBase : WeeeContextSpecification
        {
            protected static IRequestHandler<ValidateAndGetSubmissionPayment, SubmissionPaymentDetails> handler;
            protected static Fixture fixture;
            protected static Domain.Producer.DirectRegistrant directRegistrant;
            protected static ValidateAndGetSubmissionPayment request;
            protected static Domain.Producer.DirectProducerSubmissionHistory directProducerSubmissionHistory;
            protected static Domain.Producer.DirectProducerSubmission directProducerSubmission;
            protected static SubmissionPaymentDetails result;
            protected static PaymentSession paymentSession;

            public static IntegrationTestSetupBuilder LocalSetup()
            {
                var setup = SetupTest(IocApplication.RequestHandler)
                    .WithIoC()
                    .WithTestData()
                .WithExternalUserAccess();

                directRegistrant = DirectRegistrantDbSetup.Init()
                    .Create();

                directProducerSubmission = DirectRegistrantSubmissionDbSetup.Init()
                    .WithDefaultRegisteredProducer()
                    .WithComplianceYear(SystemTime.UtcNow.Year)
                    .WithDirectRegistrant(directRegistrant)
                    .Create();

                directProducerSubmissionHistory = DirectRegistrantSubmissionHistoryDbSetup.Init()
                    .WithDirectProducerSubmission(directProducerSubmission).Create();

                Query.UpdateCurrentProducerSubmission(directProducerSubmission.Id, directProducerSubmissionHistory.Id);

                handler = Container.Resolve<IRequestHandler<ValidateAndGetSubmissionPayment, SubmissionPaymentDetails>>();

                fixture = new Fixture();

                request = new ValidateAndGetSubmissionPayment(fixture.Create<string>(), directRegistrant.Id);

                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, directRegistrant.OrganisationId).Create();

                return setup;
            }
        }
    }
}
