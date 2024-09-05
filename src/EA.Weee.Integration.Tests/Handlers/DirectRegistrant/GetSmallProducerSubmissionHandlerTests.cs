namespace EA.Weee.Integration.Tests.Handlers.DirectRegistrant
{
    using Autofac;
    using AutoFixture;
    using Base;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Integration.Tests.Builders;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using NUnit.Specifications.Categories;
    using Prsd.Core.Autofac;
    using Prsd.Core.Mediator;
    using System;
    using System.Security;

    public class GetSmallProducerSubmissionHandlerTests : IntegrationTestBase
    {
        [Component]
        public class WhenICreateANewProducerSubmission : GetSmallProducerSubmissionHandlerTestsBase
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

            private readonly It shouldHaveCompletedTheTransaction = () =>
            {
            };
        }

        [Component]
        public class WhenUserIsNotAuthorised : GetSmallProducerSubmissionHandlerTestsBase
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

        public class GetSmallProducerSubmissionHandlerTestsBase : WeeeContextSpecification
        {
            protected static IRequestHandler<GetSmallProducerSubmission, SmallProducerSubmissionData> handler;
            protected static Fixture fixture;
            protected static GetSmallProducerSubmission request;
            protected static SmallProducerSubmissionData result;
            protected static Domain.Producer.DirectRegistrant directRegistrant;

            public static IntegrationTestSetupBuilder LocalSetup()
            {
                var setup = SetupTest(IocApplication.RequestHandler)
                    .WithIoC()
                    .WithTestData()
                    .WithExternalUserAccess();

                handler = Container.Resolve<IRequestHandler<GetSmallProducerSubmission, SmallProducerSubmissionData>>();

                var organisation = OrganisationDbSetup.Init().Create();
                directRegistrant = DirectRegistrantDbSetup.Init().Create();

                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, directRegistrant.OrganisationId).Create();

                return setup;
            }
        }
    }
}
