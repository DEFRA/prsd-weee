namespace EA.Weee.Integration.Tests.Handlers.DirectRegistrant
{
    using Autofac;
    using AutoFixture;
    using Base;
    using EA.Prsd.Core;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Integration.Tests.Builders;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using FluentAssertions;
    using NUnit.Specifications.Categories;
    using Prsd.Core.Autofac;
    using Prsd.Core.Mediator;
    using System;
    using System.Security;

    public class GetSmallProducerSubmissionHandlerTests : IntegrationTestBase
    {
        [Component]
        public class WhenIGetSmallProducerSubmission : GetSmallProducerSubmissionHandlerTestsBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                request = new GetSmallProducerSubmission(directRegistrant.Id);
            };

            private readonly Because of = () =>
            {
                result = AsyncHelper.RunSync(() => handler.HandleAsync(request));
            };

            private readonly It shouldHaveRetrievedTheData = () =>
            {
                var fullDirectRegistrant = Query.GetDirectRegistrantByOrganisationId(directRegistrant.OrganisationId);

                result.CurrentSubmission.Should().NotBeNull();
            };
        }

        [Component]
        public class WhenUserIsNotAuthorised : GetSmallProducerSubmissionHandlerTestsBase
        {
            protected static IRequestHandler<GetSmallProducerSubmission, SmallProducerSubmissionData> authHandler;

            private readonly Establish context = () =>
            {
                SetupTest(IocApplication.RequestHandler)
                    .WithDefaultSettings();

                authHandler = Container.Resolve<IRequestHandler<GetSmallProducerSubmission, SmallProducerSubmissionData>>();
            };

            private readonly Because of = () =>
            {
                CatchExceptionAsync(() => authHandler.HandleAsync(new GetSmallProducerSubmission(Guid.NewGuid())));
            };

            private readonly It shouldHaveCaughtArgumentException = ShouldThrowException<SecurityException>;
        }

        public class GetSmallProducerSubmissionHandlerTestsBase : WeeeContextSpecification
        {
            protected static IRequestHandler<GetSmallProducerSubmission, SmallProducerSubmissionData> handler;
            protected static Fixture fixture;
            protected static GetSmallProducerSubmission request;
            protected static SmallProducerSubmissionData result;
            protected static Domain.Producer.DirectRegistrant directRegistrant;
            protected static Domain.Producer.DirectProducerSubmissionHistory directProducerSubmissionHistory;
            protected static Domain.Producer.DirectProducerSubmission directProducerSubmission;

            public static IntegrationTestSetupBuilder LocalSetup()
            {
                var setup = SetupTest(IocApplication.RequestHandler)
                    .WithIoC()
                    .WithTestData()
                    .WithExternalUserAccess();

                handler = Container.Resolve<IRequestHandler<GetSmallProducerSubmission, SmallProducerSubmissionData>>();

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

                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, directRegistrant.OrganisationId).Create();

                return setup;
            }
        }
    }
}
