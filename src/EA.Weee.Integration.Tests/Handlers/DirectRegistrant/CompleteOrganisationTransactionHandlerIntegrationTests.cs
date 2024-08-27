namespace EA.Weee.Integration.Tests.Handlers.DirectRegistrant
{
    using Autofac;
    using AutoFixture;
    using Base;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Domain.Obligation;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using FluentAssertions;
    using Newtonsoft.Json;
    using NUnit.Specifications.Categories;
    using Prsd.Core.Autofac;
    using Prsd.Core.Mediator;
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.Integration.Tests.Builders;

    public class CompleteOrganisationTransactionHandlerIntegrationTests : IntegrationTestBase
    {
        [Component]
        public class WhenICompleteAnOrganisationTransaction : CompleteOrganisationTransactionIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                OrganisationTransactionDbSetup.Init().WithModel(organisationTransactionData).Create();

                request = new CompleteOrganisationTransaction(organisationTransactionData);
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(request)).Result;
            };

            private readonly It shouldHaveCompletedTheTransaction = () =>
            {
                var entity = Query.GetOrganisationTransactionForUser(UserId.ToString());

                entity.OrganisationJson.Should().Be(JsonConvert.SerializeObject(organisationTransactionData));
                entity.UserId.Should().Be(UserId.ToString());
                entity.CreatedDateTime.Should().BeAfter(date);
                entity.CompletedDateTime.Should().BeAfter(date);
                entity.CompletionStatus.Should().Be(CompletionStatus.Complete);
            };

            private readonly It shouldHaveReturnedTrue = () =>
            {
                result.Should().BeTrue();
            };
        }

        [Component]
        public class WhenICompleteAnOrganisationTransactionThatIsNotIncomplete : CompleteOrganisationTransactionIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                request = new CompleteOrganisationTransaction(organisationTransactionData);
            };

            private readonly Because of = () =>
            {
                CatchExceptionAsync(() => handler.HandleAsync(request));
            };

            private readonly It shouldHaveCaughtArgumentException = ShouldThrowException<InvalidOperationException>;
        }

        [Component]
        public class WhenUserIsNotAuthorised : CompleteOrganisationTransactionIntegrationTestBase
        {
            protected static IRequestHandler<AddUpdateOrganisationTransaction, OrganisationTransactionData> authHandler;

            private readonly Establish context = () =>
            {
                SetupTest(IocApplication.RequestHandler)
                    .WithDefaultSettings();

                authHandler = Container.Resolve<IRequestHandler<AddUpdateOrganisationTransaction, OrganisationTransactionData>>();
            };

            private readonly Because of = () =>
            {
                CatchExceptionAsync(() => authHandler.HandleAsync(new AddUpdateOrganisationTransaction(organisationTransactionData)));
            };

            private readonly It shouldHaveCaughtArgumentException = ShouldThrowException<SecurityException>;
        }

        public class CompleteOrganisationTransactionIntegrationTestBase : WeeeContextSpecification
        {
            protected static IRequestHandler<CompleteOrganisationTransaction, bool> handler;
            protected static Fixture fixture;
            protected static OrganisationTransactionData organisationTransactionData;
            protected static CompleteOrganisationTransaction request;
            protected static bool result;
            protected static DateTime date;

            public static IntegrationTestSetupBuilder LocalSetup()
            {
                var setup = SetupTest(IocApplication.RequestHandler)
                    .WithIoC()
                    .WithTestData()
                    .WithExternalUserAccess();

                Query.DeleteAllOrganisationTransactions();

                date = DateTime.UtcNow;
                fixture = new Fixture();
                handler = Container.Resolve<IRequestHandler<CompleteOrganisationTransaction, bool>>();

                organisationTransactionData = fixture.Create<OrganisationTransactionData>();

                return setup;
            }
        }
    }
}
