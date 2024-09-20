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

    public class AddSmallProducerSubmissionHandlerTests : IntegrationTestBase
    {
        [Component]
        public class WhenICreateANewProducerSubmission : AddSmallProducerSubmissionHandlerTestsBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                request = new AddSmallProducerSubmission(directRegistrant.Id);
            };

            private readonly Because of = () =>
            {
                result = AsyncHelper.RunSync(() => handler.HandleAsync(request));
            };

            private readonly It shouldHaveCompletedTheTransaction = () =>
            {
                var submission = Query.GetDirectProducerSubmissionById(result);
                submission.RegisteredProducer.Should().NotBeNull();
                submission.RegisteredProducer.ProducerRegistrationNumber.Should().NotBeNullOrWhiteSpace();
                submission.RegisteredProducer.ComplianceYear.Should().Be(SystemTime.UtcNow.Year);
                submission.DirectRegistrantId.Should().Be(directRegistrant.Id);
                submission.PaymentReference.Should().BeNull();
                submission.ComplianceYear.Should().Be(SystemTime.UtcNow.Year);
                submission.CurrentSubmission.Should().NotBeNull();
                submission.SubmissionHistory.Count.Should().Be(1);
            };
        }

        [Component]
        public class WhenICreateANewProducerSubmissionWhereRegisteredProducerExists : AddSmallProducerSubmissionHandlerTestsBase
        {
            private static RegisteredProducer registeredProducer;
            private static string registrationNumber;

            private readonly Establish context = () =>
            {
                LocalSetup();

                registrationNumber = "NEWREG";
                registeredProducer = new RegisteredProducer(registrationNumber, 2000);

                DirectRegistrantSubmissionDbSetup.Init()
                    .WithDirectRegistrant(directRegistrant)
                    .WithRegisteredProducer(registeredProducer)
                    .Create();

                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, directRegistrant.OrganisationId).Create();

                request = new AddSmallProducerSubmission(directRegistrant.Id);
            };

            private readonly Because of = () =>
            {
                result = AsyncHelper.RunSync(() => handler.HandleAsync(request));
            };

            private readonly It shouldHaveCompletedTheTransaction = () =>
            {
                var submission = Query.GetDirectProducerSubmissionById(result);
                submission.RegisteredProducer.Should().NotBeNull();
                submission.RegisteredProducer.ProducerRegistrationNumber.Should().Be(registrationNumber);
                submission.RegisteredProducer.ComplianceYear.Should().Be(SystemTime.UtcNow.Year);
                submission.DirectRegistrantId.Should().Be(directRegistrant.Id);
                submission.PaymentReference.Should().BeNull();
                submission.ComplianceYear.Should().Be(SystemTime.UtcNow.Year);
                submission.CurrentSubmission.Should().NotBeNull();
                submission.SubmissionHistory.Count.Should().Be(1);
            };
        }

        [Component]
        public class WhenUserIsNotAuthorised : AddSmallProducerSubmissionHandlerTestsBase
        {
            protected static IRequestHandler<AddSmallProducerSubmission, Guid> authHandler;

            private readonly Establish context = () =>
            {
                SetupTest(IocApplication.RequestHandler)
                    .WithDefaultSettings();

                authHandler = Container.Resolve<IRequestHandler<AddSmallProducerSubmission, Guid>>();
            };

            private readonly Because of = () =>
            {
                CatchExceptionAsync(() => authHandler.HandleAsync(new AddSmallProducerSubmission(Guid.NewGuid())));
            };

            private readonly It shouldHaveCaughtArgumentException = ShouldThrowException<SecurityException>;
        }

        public class AddSmallProducerSubmissionHandlerTestsBase : WeeeContextSpecification
        {
            protected static IRequestHandler<AddSmallProducerSubmission, Guid> handler;
            protected static Fixture fixture;
            protected static AddSmallProducerSubmission request;
            protected static Guid result;
            protected static Domain.Producer.DirectRegistrant directRegistrant;

            public static IntegrationTestSetupBuilder LocalSetup()
            {
                var setup = SetupTest(IocApplication.RequestHandler)
                    .WithIoC()
                    .WithTestData()
                    .WithExternalUserAccess();

                handler = Container.Resolve<IRequestHandler<AddSmallProducerSubmission, Guid>>();

                directRegistrant = DirectRegistrantDbSetup.Init().Create();

                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, directRegistrant.OrganisationId).Create();

                return setup;
            }
        }
    }
}
