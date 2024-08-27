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

    public class AddUpdateOrganisationTransactionHandlerTests : IntegrationTestBase
    {
        [Component]
        public class WhenICreateAUserOrganisationTransaction : CreateUpdateOrganisationTransactionIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                request = new AddUpdateOrganisationTransaction(organisationTransactionData);
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(request)).Result;
            };

            private readonly It shouldHaveCreatedTheTransaction = () =>
            {
                var entity = Query.GetOrganisationTransactionForUser(UserId.ToString());

                entity.OrganisationJson.Should().Be(JsonConvert.SerializeObject(result));
                entity.UserId.Should().Be(UserId.ToString());
                entity.CompletedDateTime.Should().BeNull();
                entity.CreatedDateTime.Should().BeAfter(date);
                entity.CompletionStatus.Should().Be(CompletionStatus.Incomplete);
            };

            private readonly It shouldHaveReturnedTheTransaction = () =>
            {
                result.Should().BeEquivalentTo(organisationTransactionData);
            };
        }

        [Component]
        public class WhenIUpdateAUserOrganisationTransaction : CreateUpdateOrganisationTransactionIntegrationTestBase
        {
            private static OrganisationTransaction organisationTransaction;
            private static OrganisationTransactionData newOrganisationTransactionData;

            private readonly Establish context = () =>
            {
                LocalSetup();

                newOrganisationTransactionData = fixture.Create<OrganisationTransactionData>();

                organisationTransaction = OrganisationTransactionDbSetup.Init().WithModel(newOrganisationTransactionData).Create();

                request = new AddUpdateOrganisationTransaction(newOrganisationTransactionData);
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(request)).Result;
            };

            private readonly It shouldHaveUpdatedTheTransaction = () =>
            {
                var entity = Query.GetOrganisationTransactionForUser(UserId.ToString());

                entity.OrganisationJson.Should().Be(JsonConvert.SerializeObject(result));
                entity.UserId.Should().Be(UserId.ToString());
                entity.CompletedDateTime.Should().BeNull();
                entity.CreatedDateTime.Should().BeAfter(date);
                entity.CompletionStatus.Should().Be(CompletionStatus.Incomplete);
            };

            private readonly It shouldHaveReturnedTheTransaction = () =>
            {
                result.Should().BeEquivalentTo(newOrganisationTransactionData);
            };

            private readonly It shouldHaveOnlyASingleTransactionForTheUser = () =>
            {
                Query.GetOrganisationTransactionCountForUser(UserId.ToString()).Should().Be(1);
            };
        }

        [Component]
        public class WhenUserIsNotAuthorised : CreateUpdateOrganisationTransactionIntegrationTestBase
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

        public class CreateUpdateOrganisationTransactionIntegrationTestBase : WeeeContextSpecification
        {
            protected static IRequestHandler<AddUpdateOrganisationTransaction, OrganisationTransactionData> handler;
            protected static Fixture fixture;
            protected static OrganisationTransactionData organisationTransactionData;
            protected static AddUpdateOrganisationTransaction request;
            protected static OrganisationTransactionData result;
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
                handler = Container.Resolve<IRequestHandler<AddUpdateOrganisationTransaction, OrganisationTransactionData>>();

                organisationTransactionData = fixture.Create<OrganisationTransactionData>();

                return setup;
            }
        }
    }
}
