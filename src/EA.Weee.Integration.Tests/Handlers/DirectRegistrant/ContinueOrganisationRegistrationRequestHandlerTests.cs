namespace EA.Weee.Integration.Tests.Handlers.DirectRegistrant
{
    using Autofac;
    using Base;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.Integration.Tests.Builders;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using FluentAssertions;
    using NUnit.Specifications.Categories;
    using Prsd.Core.Autofac;
    using Prsd.Core.Mediator;
    using System;
    using System.Security;

    public class ContinueOrganisationRegistrationRequestHandlerIntegrationTests : IntegrationTestBase
    {
        [Component]
        public class WhenIUpdateContactDetailsWithNoExistingDetails : ContinueOrganisationRegistrationRequestHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();
            };

            private readonly Because of = () =>
            {
                result = AsyncHelper.RunSync(async () => await handler.HandleAsync(request));
            };

            private readonly It shouldDefaultOrganisationData = () =>
            {
                result.OrganisationViewModel.Should().NotBeNull();
                result.OrganisationViewModel.CompaniesRegistrationNumber.Should()
                    .Be(organisation.CompanyRegistrationNumber);
                result.OrganisationViewModel.CompanyName.Should().Be(organisation.OrganisationName);
                result.OrganisationViewModel.BusinessTradingName.Should().Be(organisation.TradingName);
                result.OrganisationViewModel.ProducerRegistrationNumber.Should()
                    .Be(directRegistrant.ProducerRegistrationNumber);
                result.NpwdMigrated.Should().BeTrue();
            };

            private readonly It shouldHaveCreatedOrganisationTransactionEntry = () =>
            {
                var organisationTransaction = Query.GetOrganisationTransactionForUser(UserId.ToString());
                organisationTransaction.OrganisationJson.Should().NotBeNull();
                organisationTransaction.UserId.Should().Be(UserId.ToString());
                organisationTransaction.CompletedDateTime.Should().BeNull();
                organisationTransaction.CreatedDateTime.Should().BeAfter(date);
                organisationTransaction.CompletionStatus.Should().Be(CompletionStatus.Incomplete);
            };
        }

        [Component]
        public class WhenUserIsNotAuthorised : ContinueOrganisationRegistrationRequestHandlerIntegrationTestBase
        {
            protected static IRequestHandler<ContinueOrganisationRegistrationRequest, OrganisationTransactionData> authHandler;

            private readonly Establish context = () =>
            {
                SetupTest(IocApplication.RequestHandler)
                    .WithDefaultSettings();

                authHandler = Container.Resolve<IRequestHandler<ContinueOrganisationRegistrationRequest, OrganisationTransactionData>>();
            };

            private readonly Because of = () =>
            {
                CatchExceptionAsync(() => authHandler.HandleAsync(request));
            };

            private readonly It shouldHaveCaughtSecurityException = ShouldThrowException<SecurityException>;
        }

        public class ContinueOrganisationRegistrationRequestHandlerIntegrationTestBase : WeeeContextSpecification
        {
            protected static IRequestHandler<ContinueOrganisationRegistrationRequest, OrganisationTransactionData> handler;
            protected static Organisation organisation;
            protected static Domain.Producer.DirectRegistrant directRegistrant;
            protected static ContinueOrganisationRegistrationRequest request;
            protected static OrganisationTransactionData result;
            protected static DateTime date;

            public static IntegrationTestSetupBuilder LocalSetup()
            {
                var setup = SetupTest(IocApplication.RequestHandler)
                    .WithIoC()
                    .WithTestData()
                .WithExternalUserAccess();

                organisation = OrganisationDbSetup.Init()
                    .WithNpWdMigrated(true)
                    .WithNpWdMigratedComplete(false)
                    .Create();

                directRegistrant = DirectRegistrantDbSetup.Init()
                    .WithPrn("PRN123456")
                    .WithOrganisation(organisation.Id)
                    .Create();

                date = DateTime.UtcNow;

                AsyncHelper.RunSync(() => Query.DeleteAllOrganisationTransactionsAsync());

                handler = Container.Resolve<IRequestHandler<ContinueOrganisationRegistrationRequest, OrganisationTransactionData>>();

                request = new ContinueOrganisationRegistrationRequest(organisation.Id);

                return setup;
            }
        }
    }
}
