namespace EA.Weee.Integration.Tests.Handlers.DirectRegistrant
{
    using Autofac;
    using AutoFixture;
    using Base;
    using EA.Prsd.Core;
    using EA.Weee.Domain.Producer;
    using EA.Weee.Integration.Tests.Builders;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using FluentAssertions;
    using NUnit.Specifications.Categories;
    using Prsd.Core.Autofac;
    using Prsd.Core.Mediator;
    using System;
    using System.Security;

    public class AddPaymentSessionRequestHandlerIntegrationTests : IntegrationTestBase
    {
        [Component]
        public class WhenIAddAPaymentSession : AddPaymentSessionRequestHandlerIntegrationTestBase
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
                var payment = Query.GetPaymentSessionById(result);

                payment.UserId.Should().Be(UserId.ToString());
                payment.Amount.Should().Be(request.Amount);
                payment.DirectProducerSubmission.Should().BeEquivalentTo(directProducerSubmission);
                payment.CreatedAt.Should().BeBefore(SystemTime.UtcNow);
                payment.DirectRegistrant.Should().BeEquivalentTo(directRegistrant);
                payment.InFinalState.Should().BeFalse();
                payment.PaymentReference.Should().Be(request.PaymentReference);
                payment.PaymentId.Should().Be(request.PaymentId);
                payment.PaymentReturnToken.Should().Be(request.PaymentReturnToken);
                payment.Status.Should().Be(PaymentState.New);
            };
        }

        [Component]
        public class WhenUserIsNotAuthorised : AddPaymentSessionRequestHandlerIntegrationTestBase
        {
            protected static IRequestHandler<AddPaymentSessionRequest, Guid> authHandler;

            private readonly Establish context = () =>
            {
                SetupTest(IocApplication.RequestHandler)
                    .WithDefaultSettings();

                authHandler = Container.Resolve<IRequestHandler<AddPaymentSessionRequest, Guid>>();
            };

            private readonly Because of = () =>
            {
                CatchExceptionAsync(() => authHandler.HandleAsync(request));
            };

            private readonly It shouldHaveCaughtArgumentException = ShouldThrowException<SecurityException>;
        }

        public class AddPaymentSessionRequestHandlerIntegrationTestBase : WeeeContextSpecification
        {
            protected static IRequestHandler<AddPaymentSessionRequest, Guid> handler;
            protected static Fixture fixture;
            protected static Domain.Producer.DirectRegistrant directRegistrant;
            protected static AddPaymentSessionRequest request;
            protected static Domain.Producer.DirectProducerSubmission directProducerSubmission;
            protected static Guid result;

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

                var directProducerSubmissionHistory = DirectRegistrantSubmissionHistoryDbSetup.Init()
                    .WithDirectProducerSubmission(directProducerSubmission).Create();

                Query.UpdateCurrentProducerSubmission(directProducerSubmission.Id, directProducerSubmissionHistory.Id);

                handler = Container.Resolve<IRequestHandler<AddPaymentSessionRequest, Guid>>();

                fixture = new Fixture();

                request = new AddPaymentSessionRequest(directRegistrant.Id, 
                    "PaymentRef01",
                    "http://test/",
                    "PaymentId01",
                    fixture.Create<decimal>());

                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, directRegistrant.OrganisationId).Create();

                return setup;
            }
        }
    }
}
