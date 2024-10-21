//namespace EA.Weee.Integration.Tests.Handlers.DirectRegistrant
//{
//    using Autofac;
//    using EA.Prsd.Core;
//    using EA.Prsd.Core.Autofac;
//    using EA.Prsd.Core.Mediator;
//    using EA.Weee.Domain.Organisation;
//    using EA.Weee.Domain.Producer;
//    using EA.Weee.Integration.Tests.Base;
//    using EA.Weee.Integration.Tests.Builders;
//    using EA.Weee.Requests.Organisations.DirectRegistrant;
//    using EA.Weee.Tests.Core.Model;
//    using FluentAssertions;
//    using NUnit.Specifications.Categories;
//    using System;
//    using System.Security;

//    public class ProducerRegistrationNumberRequestHandlerIntegrationTests : IntegrationTestBase
//    {
//        [Component]
//        public class WhenProducerRegistrationNumberExists : ProducerRegistrationNumberRequestHandlerIntegrationTestBase
//        {
//            private static Domain.Producer.RegisteredProducer registeredProducer;
//            private static string registrationNumber;

//            private readonly Establish context = () =>
//            {
//                // Setup with an existing PRN
//                LocalSetup(exists: true);

//                registrationNumber = "ValidRegistrationNumber"; // Simulating an existing PRN
//                directRegistrant = DirectRegistrantDbSetup.Init()
//                    .WithOrganisation(directRegistrant.OrganisationId)
//                    .WithPrn(registrationNumber)
//                    .Create();

//                // Create a RegisteredProducer associated with the DirectRegistrant
//                registeredProducer = new Domain.Producer.RegisteredProducer(registrationNumber, SystemTime.UtcNow.Year);

//                // Create DirectProducerSubmission with a valid submission history
//                var directProducerSubmission = new DirectProducerSubmission(directRegistrant, registeredProducer, SystemTime.UtcNow.Year);
//                directProducerSubmission.SetCurrentSubmission(new DirectProducerSubmissionHistory(directProducerSubmission));

//                // Setup the DirectRegistrantSubmission linking the necessary entities
//                DirectRegistrantSubmissionDbSetup.Init()
//                    .WithDirectRegistrant(directRegistrant)
//                    .WithRegisteredProducer(registeredProducer)
//                    .WithSubmission(directProducerSubmission.CurrentSubmission)
//                    .Create();

//                // Create the handler and request
//                handler = Container.Resolve<IRequestHandler<ProducerRegistrationNumberRequest, bool>>();
//                request = new ProducerRegistrationNumberRequest(registrationNumber);

//                // Associate the organisation user with the direct registrant
//                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, directRegistrant.OrganisationId).Create();
//            };

//            private readonly Because of = () =>
//            {
//                // Act
//                result = AsyncHelper.RunSync(() => handler.HandleAsync(request));
//            };

//            private readonly It shouldReturnTrue = () =>
//            {
//                // Assert
//                result.Should().BeTrue();
//            };
//        }

//        [Component]
//        public class WhenProducerRegistrationNumberDoesNotExist : ProducerRegistrationNumberRequestHandlerIntegrationTestBase
//        {
//            private readonly Establish context = () =>
//            {
//                LocalSetup(false);
//            };

//            private readonly Because of = () =>
//            {
//                result = AsyncHelper.RunSync(() => handler.HandleAsync(request));
//            };

//            private readonly It shouldReturnFalse = () =>
//            {
//                result.Should().BeFalse();
//            };
//        }

//        [Component]
//        public class WhenUserIsNotAuthorized : ProducerRegistrationNumberRequestHandlerIntegrationTestBase
//        {
//            protected static IRequestHandler<ProducerRegistrationNumberRequest, bool> authHandler;

//            private readonly Establish context = () =>
//            {
//                SetupTest(IocApplication.RequestHandler)
//                    .WithDefaultSettings();

//                authHandler = Container.Resolve<IRequestHandler<ProducerRegistrationNumberRequest, bool>>();
//            };

//            private readonly Because of = () =>
//            {
//                CatchExceptionAsync(() => authHandler.HandleAsync(request));
//            };

//            private readonly It shouldHaveCaughtSecurityException = ShouldThrowException<SecurityException>;
//        }

//        public class ProducerRegistrationNumberRequestHandlerIntegrationTestBase : WeeeContextSpecification
//        {
//            protected static IRequestHandler<ProducerRegistrationNumberRequest, bool> handler;
//            protected static ProducerRegistrationNumberRequest request;
//            protected static bool result;
//            protected static string producerRegistrationNumber = "12345";
//            protected static DirectRegistrant directRegistrant;
//            protected static Domain.Organisation.Organisation organisation;

//            public static IntegrationTestSetupBuilder LocalSetup(bool exists = true)
//            {
//                var setup = SetupTest(IocApplication.RequestHandler)
//                    .WithIoC()
//                    .WithTestData()
//                    .WithExternalUserAccess();

//                organisation = OrganisationDbSetup.Init().Create();

//                directRegistrant = DirectRegistrantDbSetup.Init()
//                    .WithOrganisation(organisation.Id)
//                    .WithPrn(producerRegistrationNumber)
//                    .Create();

//                // If the PRN does not exist, set to a non-existent value
//                if (!exists)
//                {
//                    producerRegistrationNumber = "NONEXISTENTPRN";
//                }

//                handler = Container.Resolve<IRequestHandler<ProducerRegistrationNumberRequest, bool>>();
//                request = new ProducerRegistrationNumberRequest(producerRegistrationNumber);

//                OrganisationUserDbSetup.Init()
//                    .WithUserIdAndOrganisationId(UserId, directRegistrant.OrganisationId)
//                    .Create();

//                return setup;
//            }
//        }
//    }
//}