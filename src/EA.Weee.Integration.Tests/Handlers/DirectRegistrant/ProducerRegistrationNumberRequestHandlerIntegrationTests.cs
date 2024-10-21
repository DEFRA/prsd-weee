namespace EA.Weee.Integration.Tests.Handlers.DirectRegistrant
{
    using Autofac;
    using EA.Weee.Integration.Tests.Base;
    using EA.Prsd.Core.Autofac;
    using EA.Weee.Integration.Tests.Builders;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using FluentAssertions;
    using NUnit.Specifications.Categories;
    using EA.Prsd.Core.Mediator;
    using System.Security;
    using EA.Weee.Domain.Producer;

    public class ProducerRegistrationNumberRequestHandlerIntegrationTests : IntegrationTestBase
    {
        [Component]
        public class WhenProducerRegistrationNumberExists : ProducerRegistrationNumberRequestHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                // Create a PRN that will be marked as existing in the database
                var existingPrn = "12345";

                // Directly add the PRN by creating a RegisteredProducer
                var registeredProducer = new RegisteredProducer(existingPrn, SystemTime.UtcNow.Year);
                // Assuming you have a way to persist the registeredProducer, e.g. through DbContext
                RegisteredProducerDbSetup.Init()
                    .WithProducer(registeredProducer) // Use an existing setup if you have one
                    .Create();

                request = new ProducerRegistrationNumberRequest(existingPrn);
            };

            private readonly Because of = () =>
            {
                result = AsyncHelper.RunSync(() => handler.HandleAsync(request));
            };

            private readonly It shouldReturnTrue = () =>
            {
                result.Should().BeTrue();
            };
        }

        [Component]
        public class WhenProducerRegistrationNumberDoesNotExist : ProducerRegistrationNumberRequestHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup(false); // Simulate non-existence
            };

            private readonly Because of = () =>
            {
                result = AsyncHelper.RunSync(() => handler.HandleAsync(request));
            };

            private readonly It shouldReturnFalse = () =>
            {
                result.Should().BeFalse();
            };
        }

        [Component]
        public class WhenUserIsNotAuthorized : ProducerRegistrationNumberRequestHandlerIntegrationTestBase
        {
            protected static IRequestHandler<ProducerRegistrationNumberRequest, bool> authHandler;

            private readonly Establish context = () =>
            {
                SetupTest(IocApplication.RequestHandler)
                    .WithDefaultSettings();

                authHandler = Container.Resolve<IRequestHandler<ProducerRegistrationNumberRequest, bool>>();
            };

            private readonly Because of = () =>
            {
                CatchExceptionAsync(() => authHandler.HandleAsync(request));
            };

            private readonly It shouldHaveCaughtSecurityException = ShouldThrowException<SecurityException>;
        }

        public class ProducerRegistrationNumberRequestHandlerIntegrationTestBase : WeeeContextSpecification
        {
            protected static IRequestHandler<ProducerRegistrationNumberRequest, bool> handler;
            protected static ProducerRegistrationNumberRequest request;
            protected static bool result;
            protected static string producerRegistrationNumber = "12345";

            public static IntegrationTestSetupBuilder LocalSetup(bool exists = true)
            {
                var setup = SetupTest(IocApplication.RequestHandler)
                    .WithIoC()
                    .WithTestData()
                    .WithExternalUserAccess();

                var organisation = OrganisationDbSetup.Init().Create();

                var directRegistrant = DirectRegistrantDbSetup.Init()
                    .WithOrganisation(organisation.Id)
                    .WithPrn(producerRegistrationNumber) // Set the PRN
                    .Create();

                if (!exists)
                {
                    producerRegistrationNumber = "NONEXISTENTPRN"; // Set PRN to a non-existent one for testing
                }

                handler = Container.Resolve<IRequestHandler<ProducerRegistrationNumberRequest, bool>>();

                request = new ProducerRegistrationNumberRequest(producerRegistrationNumber);

                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, directRegistrant.OrganisationId).Create();

                return setup;
            }
        }
    }
}