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

    public class GetInProgressPaymentSessionRequestHandlerIntegrationTests : IntegrationTestBase
    {
        [Component]
        public class WhenIGetAnInProgressPaymentSession : GetInProgressPaymentSessionRequestHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                var submission = Query.GetDirectProducerSubmissionById(directProducerSubmissionHistory.DirectProducerSubmissionId);

                paymentSession = PaymentSessionDbSetup.Init()
                    .WithUser(UserId.ToString())
                    .WithStatus(PaymentState.New)
                    .WithDirectRegistrantSubmission(submission).Create();
            };

            private readonly Because of = () =>
            {
                result = AsyncHelper.RunSync(() => handler.HandleAsync(request));
            };

            private readonly It shouldUpdateTheData = () =>
            {
                result.Should().NotBeNull();
                result.PaymentId.Should().Be(paymentSession.PaymentId);
            };
        }

        [Component]
        public class WhenIGetAnInProgressPaymentSessionWhereNoneExist : GetInProgressPaymentSessionRequestHandlerIntegrationTestBase
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
                result.Should().BeNull();
            };
        }

        [Component]
        public class WhenUserIsNotAuthorised : GetInProgressPaymentSessionRequestHandlerIntegrationTestBase
        {
            protected static IRequestHandler<GetInProgressPaymentSessionRequest, SubmissionPaymentDetails> authHandler;

            private readonly Establish context = () =>
            {
                SetupTest(IocApplication.RequestHandler)
                    .WithDefaultSettings();

                authHandler = Container.Resolve<IRequestHandler<GetInProgressPaymentSessionRequest, SubmissionPaymentDetails>>();
            };

            private readonly Because of = () =>
            {
                CatchExceptionAsync(() => authHandler.HandleAsync(request));
            };

            private readonly It shouldHaveCaughtArgumentException = ShouldThrowException<SecurityException>;
        }

        public class GetInProgressPaymentSessionRequestHandlerIntegrationTestBase : WeeeContextSpecification
        {
            protected static IRequestHandler<GetInProgressPaymentSessionRequest, SubmissionPaymentDetails> handler;
            protected static Fixture fixture;
            protected static Domain.Producer.DirectRegistrant directRegistrant;
            protected static GetInProgressPaymentSessionRequest request;
            protected static Domain.Producer.DirectProducerSubmissionHistory directProducerSubmissionHistory;
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

                var directProducerSubmission = DirectRegistrantSubmissionDbSetup.Init()
                    .WithDefaultRegisteredProducer()
                    .WithComplianceYear(SystemTime.UtcNow.Year)
                    .WithDirectRegistrant(directRegistrant)
                    .Create();

                directProducerSubmissionHistory = DirectRegistrantSubmissionHistoryDbSetup.Init()
                    .WithDirectProducerSubmission(directProducerSubmission).Create();

                Query.UpdateCurrentProducerSubmission(directProducerSubmission.Id, directProducerSubmissionHistory.Id);

                handler = Container.Resolve<IRequestHandler<GetInProgressPaymentSessionRequest, SubmissionPaymentDetails>>();

                fixture = new Fixture();

                request = new GetInProgressPaymentSessionRequest(directRegistrant.Id);

                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, directRegistrant.OrganisationId).Create();

                return setup;
            }
        }
    }
}
