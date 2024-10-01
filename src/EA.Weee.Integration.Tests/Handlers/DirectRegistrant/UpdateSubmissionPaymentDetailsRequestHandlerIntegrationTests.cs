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

    public class UpdateSubmissionPaymentDetailsRequestHandlerIntegrationTests : IntegrationTestBase
    {
        [Component]
        public class WhenUpdatingAPaymentSession : UpdateSubmissionPaymentDetailsRequestHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                paymentSession = PaymentSessionDbSetup.Init()
                    .WithDirectRegistrantSubmission(directProducerSubmission)
                    .WithUser(UserId.ToString())
                    .WithStatus(PaymentState.New)
                    .Create();

                request = new UpdateSubmissionPaymentDetailsRequest(directRegistrant.Id, PaymentStatus.Failed, paymentSession.Id, false);
            };

            private readonly Because of = () =>
            {
                result = AsyncHelper.RunSync(() => handler.HandleAsync(request));
            };

            private readonly It shouldUpdateTheData = () =>
            {
                var updatedPaymentSession = Query.GetPaymentSessionById(paymentSession.Id);

                updatedPaymentSession.Status.Should().Be(PaymentState.Failed);
                updatedPaymentSession.InFinalState.Should().BeFalse();
            };
        }

        [Component]
        public class WhenUpdatingAPaymentSessionWithFinalState : UpdateSubmissionPaymentDetailsRequestHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                paymentSession = PaymentSessionDbSetup.Init()
                    .WithDirectRegistrantSubmission(directProducerSubmission)
                    .WithUser(UserId.ToString())
                    .WithStatus(PaymentState.New)
                    .Create();

                request = new UpdateSubmissionPaymentDetailsRequest(directRegistrant.Id, PaymentStatus.Success, paymentSession.Id, true);
            };

            private readonly Because of = () =>
            {
                result = AsyncHelper.RunSync(() => handler.HandleAsync(request));
            };

            private readonly It shouldUpdateTheData = () =>
            {
                var updatedPaymentSession = Query.GetPaymentSessionById(paymentSession.Id);
                var updatedSubmission = Query.GetDirectProducerSubmissionById(directProducerSubmission.Id);

                updatedPaymentSession.Status.Should().Be(PaymentState.Success);
                updatedPaymentSession.InFinalState.Should().BeTrue();

                updatedSubmission.FinalPaymentSessionId.Should().Be(updatedPaymentSession.Id);
            };
        }

        [Component]
        public class WhenUserIsNotAuthorised : UpdateSubmissionPaymentDetailsRequestHandlerIntegrationTestBase
        {
            protected static IRequestHandler<UpdateSubmissionPaymentDetailsRequest, bool> authHandler;

            private readonly Establish context = () =>
            {
                SetupTest(IocApplication.RequestHandler)
                    .WithDefaultSettings();

                authHandler = Container.Resolve<IRequestHandler<UpdateSubmissionPaymentDetailsRequest, bool>>();
            };

            private readonly Because of = () =>
            {
                CatchExceptionAsync(() => authHandler.HandleAsync(request));
            };

            private readonly It shouldHaveCaughtArgumentException = ShouldThrowException<SecurityException>;
        }

        public class UpdateSubmissionPaymentDetailsRequestHandlerIntegrationTestBase : WeeeContextSpecification
        {
            protected static IRequestHandler<UpdateSubmissionPaymentDetailsRequest, bool> handler;
            protected static Fixture fixture;
            protected static Domain.Producer.DirectRegistrant directRegistrant;
            protected static UpdateSubmissionPaymentDetailsRequest request;
            protected static Domain.Producer.DirectProducerSubmissionHistory directProducerSubmissionHistory;
            protected static Domain.Producer.DirectProducerSubmission directProducerSubmission;
            protected static bool result;
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

                handler = Container.Resolve<IRequestHandler<UpdateSubmissionPaymentDetailsRequest, bool>>();

                fixture = new Fixture();

                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, directRegistrant.OrganisationId).Create();

                return setup;
            }
        }
    }
}
