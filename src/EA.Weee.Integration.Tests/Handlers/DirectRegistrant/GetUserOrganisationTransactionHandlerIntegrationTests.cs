namespace EA.Weee.Integration.Tests.Handlers.DirectRegistrant
{
    using Autofac;
    using AutoFixture;
    using Base;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Integration.Tests.Builders;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using FluentAssertions;
    using Newtonsoft.Json;
    using NUnit.Specifications.Categories;
    using Prsd.Core.Autofac;
    using Prsd.Core.Mediator;
    using System;
    using System.Security;
    using System.Threading.Tasks;

    public class GetUserOrganisationTransactionHandlerIntegrationTests : IntegrationTestBase
    {
        [Component]
        public class WhenIGetUserOrganisationTransaction : GetOrganisationTransactionIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                OrganisationTransactionDbSetup.Init().WithModel(organisationTransactionData).Create();

                request = new GetUserOrganisationTransaction();
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(request)).Result;
            };

            private readonly It shouldHaveRetrievedTheTransaction = () =>
            {
                var entity = Query.GetOrganisationTransactionForUser(UserId.ToString());

                entity.OrganisationJson.Should().Be(JsonConvert.SerializeObject(result));
                entity.UserId.Should().Be(UserId.ToString());
                entity.CompletedDateTime.Should().BeNull();
                entity.CreatedDateTime.Should().BeAfter(date);
            };

            private readonly It shouldHaveReturnedTheTransaction = () =>
            {
                result.Should().BeEquivalentTo(organisationTransactionData);
            };
        }

        [Component]
        public class WhenIGetUserOrganisationTransactionWhereNoneExist : GetOrganisationTransactionIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                Query.DeleteAllOrganisationTransactions();

                request = new GetUserOrganisationTransaction();
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(request)).Result;
            };

            private readonly It shouldHaveReturnedNull = () =>
            {
                result.Should().BeNull();
            };
        }

        [Component]
        public class WhenUserIsNotAuthorised : GetOrganisationTransactionIntegrationTestBase
        {
            protected static IRequestHandler<GetUserOrganisationTransaction, OrganisationTransactionData> authHandler;

            private readonly Establish context = () =>
            {
                SetupTest(IocApplication.RequestHandler)
                    .WithDefaultSettings();

                authHandler = Container.Resolve<IRequestHandler<GetUserOrganisationTransaction, OrganisationTransactionData>>();
            };

            private readonly Because of = () =>
            {
                CatchExceptionAsync(() => authHandler.HandleAsync(new GetUserOrganisationTransaction()));
            };

            private readonly It shouldHaveCaughtArgumentException = ShouldThrowException<SecurityException>;
        }

        public class GetOrganisationTransactionIntegrationTestBase : WeeeContextSpecification
        {
            protected static IRequestHandler<GetUserOrganisationTransaction, OrganisationTransactionData> handler;
            protected static Fixture fixture;
            protected static OrganisationTransactionData organisationTransactionData;
            protected static GetUserOrganisationTransaction request;
            protected static OrganisationTransactionData result;
            protected static DateTime date;

            public static IntegrationTestSetupBuilder LocalSetup()
            {
                var setup = SetupTest(IocApplication.RequestHandler)
                    .WithIoC()
                    .WithTestData()
                    .WithExternalUserAccess();

                date = DateTime.UtcNow;
                fixture = new Fixture();
                handler = Container.Resolve<IRequestHandler<GetUserOrganisationTransaction, OrganisationTransactionData>>();

                organisationTransactionData = fixture.Create<OrganisationTransactionData>();

                return setup;
            }
        }
    }
}
