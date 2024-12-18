namespace EA.Weee.Integration.Tests.Handlers.DirectRegistrant
{
    using Autofac;
    using AutoFixture;
    using Base;
    using EA.Prsd.Core;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Shared;
    using EA.Weee.Domain;
    using EA.Weee.Integration.Tests.Builders;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using FluentAssertions;
    using NUnit.Specifications.Categories;
    using Prsd.Core.Autofac;
    using Prsd.Core.Mediator;
    using System;
    using System.Security;

    public class EditOrganisationDetailsRequestHandlerIntegrationTests : IntegrationTestBase
    {
        [Component]
        public class WhenIUpdateOrganisationDetailsWhereNoExist : EditOrganisationDetailsRequestHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                var directProducerSubmissionHistory = DirectRegistrantSubmissionHistoryDbSetup.Init()
                    .WithDirectProducerSubmission(directProducerSubmission).Create();

                Query.UpdateCurrentProducerSubmission(directProducerSubmission.Id, directProducerSubmissionHistory.Id);
            };

            private readonly Because of = () =>
            {
               result = AsyncHelper.RunSync(() => handler.HandleAsync(request));
            };

            private readonly It shouldUpdateTheData = () =>
            {
                var entity = Query.GetDirectProducerSubmissionById(directProducerSubmission.Id);

                entity.Should().NotBeNull();

                entity.CurrentSubmission.CompanyName.Should().Be(request.CompanyName);
                entity.CurrentSubmission.TradingName.Should().Be(request.TradingName);
                entity.CurrentSubmission.BrandName.Name.Should().Be(request.EEEBrandNames);
                entity.CurrentSubmission.BusinessAddress.Address1.Should().Be(request.BusinessAddressData.Address1);
                entity.CurrentSubmission.BusinessAddress.Address2.Should().Be(request.BusinessAddressData.Address2);
                entity.CurrentSubmission.BusinessAddress.TownOrCity.Should().Be(request.BusinessAddressData.TownOrCity);
                entity.CurrentSubmission.BusinessAddress.CountyOrRegion.Should().Be(request.BusinessAddressData.CountyOrRegion);
                entity.CurrentSubmission.BusinessAddress.CountryId.Should().Be(request.BusinessAddressData.CountryId);
                entity.CurrentSubmission.BusinessAddress.Postcode.Should().Be(request.BusinessAddressData.Postcode);
                entity.CurrentSubmission.BusinessAddress.WebAddress.Should().Be(request.BusinessAddressData.WebAddress);
                entity.CurrentSubmission.BusinessAddress.Telephone.Should().Be(request.BusinessAddressData.Telephone);
                entity.CurrentSubmission.BusinessAddress.Email.Should().Be(request.BusinessAddressData.Email);
            };
        }

        [Component]
        public class WhenIUpdateOrganisationDetailsWithExistingDetails : EditOrganisationDetailsRequestHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                var address = AddressDbSetup.Init().Create();

                var directProducerSubmissionHistory = DirectRegistrantSubmissionHistoryDbSetup.Init()
                    .WithBusinessAddress(address)
                    .WithBrandName(fixture.Create<string>())
                    .WithDirectProducerSubmission(directProducerSubmission).Create();

                Query.UpdateCurrentProducerSubmission(directProducerSubmission.Id, directProducerSubmissionHistory.Id);
            };

            private readonly Because of = () =>
            {
                result = AsyncHelper.RunSync(() => handler.HandleAsync(request));
            };

            private readonly It shouldUpdateTheData = () =>
            {
                var entity = Query.GetDirectProducerSubmissionById(directProducerSubmission.Id);

                entity.Should().NotBeNull();
                entity.CurrentSubmission.CompanyName.Should().Be(request.CompanyName);
                entity.CurrentSubmission.TradingName.Should().Be(request.TradingName);
                entity.CurrentSubmission.BrandName.Name.Should().Be(request.EEEBrandNames);
                entity.CurrentSubmission.BusinessAddress.Address1.Should().Be(request.BusinessAddressData.Address1);
                entity.CurrentSubmission.BusinessAddress.Address2.Should().Be(request.BusinessAddressData.Address2);
                entity.CurrentSubmission.BusinessAddress.TownOrCity.Should().Be(request.BusinessAddressData.TownOrCity);
                entity.CurrentSubmission.BusinessAddress.CountyOrRegion.Should().Be(request.BusinessAddressData.CountyOrRegion);
                entity.CurrentSubmission.BusinessAddress.CountryId.Should().Be(request.BusinessAddressData.CountryId);
                entity.CurrentSubmission.BusinessAddress.Postcode.Should().Be(request.BusinessAddressData.Postcode);
                entity.CurrentSubmission.BusinessAddress.WebAddress.Should().Be(request.BusinessAddressData.WebAddress);
                entity.CurrentSubmission.BusinessAddress.Telephone.Should().Be(request.BusinessAddressData.Telephone);
                entity.CurrentSubmission.BusinessAddress.Email.Should().Be(request.BusinessAddressData.Email);
            };
        }

        [Component]
        public class WhenIUpdateOrganisationDetailsWithEmptyBrandName : EditOrganisationDetailsRequestHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                var address = AddressDbSetup.Init().Create();

                var directProducerSubmissionHistory = DirectRegistrantSubmissionHistoryDbSetup.Init()
                    .WithBusinessAddress(address)
                    .WithBrandName(fixture.Create<string>())
                    .WithDirectProducerSubmission(directProducerSubmission).Create();

                Query.UpdateCurrentProducerSubmission(directProducerSubmission.Id, directProducerSubmissionHistory.Id);

                request = new EditOrganisationDetailsRequest(directRegistrant.Id, "New company name",
                    "New trading name", requestAddress, string.Empty);
            };

            private readonly Because of = () =>
            {
                result = AsyncHelper.RunSync(() => handler.HandleAsync(request));
            };

            private readonly It shouldUpdateTheData = () =>
            {
                var entity = Query.GetDirectProducerSubmissionById(directProducerSubmission.Id);

                entity.Should().NotBeNull();
                entity.CurrentSubmission.CompanyName.Should().Be(request.CompanyName);
                entity.CurrentSubmission.TradingName.Should().Be(request.TradingName);
                entity.CurrentSubmission.BrandName.Should().BeNull();
                entity.CurrentSubmission.BusinessAddress.Address1.Should().Be(request.BusinessAddressData.Address1);
                entity.CurrentSubmission.BusinessAddress.Address2.Should().Be(request.BusinessAddressData.Address2);
                entity.CurrentSubmission.BusinessAddress.TownOrCity.Should().Be(request.BusinessAddressData.TownOrCity);
                entity.CurrentSubmission.BusinessAddress.CountyOrRegion.Should().Be(request.BusinessAddressData.CountyOrRegion);
                entity.CurrentSubmission.BusinessAddress.CountryId.Should().Be(request.BusinessAddressData.CountryId);
                entity.CurrentSubmission.BusinessAddress.Postcode.Should().Be(request.BusinessAddressData.Postcode);
                entity.CurrentSubmission.BusinessAddress.WebAddress.Should().Be(request.BusinessAddressData.WebAddress);
                entity.CurrentSubmission.BusinessAddress.Telephone.Should().Be(request.BusinessAddressData.Telephone);
                entity.CurrentSubmission.BusinessAddress.Email.Should().Be(request.BusinessAddressData.Email);
            };
        }

        [Component]
        public class WhenUserIsNotAuthorised : EditOrganisationDetailsRequestHandlerIntegrationTestBase
        {
            protected static IRequestHandler<EditOrganisationDetailsRequest, bool> authHandler;

            private readonly Establish context = () =>
            {
                SetupTest(IocApplication.RequestHandler)
                    .WithDefaultSettings();

                authHandler = Container.Resolve<IRequestHandler<EditOrganisationDetailsRequest, bool>>();
            };

            private readonly Because of = () =>
            {
                CatchExceptionAsync(() => authHandler.HandleAsync(request));
            };

            private readonly It shouldHaveCaughtSecurityException = ShouldThrowException<SecurityException>;
        }

        public class EditOrganisationDetailsRequestHandlerIntegrationTestBase : WeeeContextSpecification
        {
            protected static IRequestHandler<EditOrganisationDetailsRequest, bool> handler;
            protected static IRequestHandler<AddSmallProducerSubmission, AddSmallProducerSubmissionResult> createSubmissionHandler;
            protected static Fixture fixture;
            protected static Domain.Producer.DirectRegistrant directRegistrant;
            protected static Domain.Producer.DirectProducerSubmission directProducerSubmission;
            protected static EditOrganisationDetailsRequest request;
            protected static AddressData requestAddress;
            protected static bool result;
            protected static Country country;

            public static IntegrationTestSetupBuilder LocalSetup()
            {
                var setup = SetupTest(IocApplication.RequestHandler)
                    .WithIoC()
                    .WithTestData()
                .WithExternalUserAccess();

                directRegistrant = DirectRegistrantDbSetup.Init()
                    .Create();

                handler = Container.Resolve<IRequestHandler<EditOrganisationDetailsRequest, bool>>();
                createSubmissionHandler = Container.Resolve<IRequestHandler<AddSmallProducerSubmission, AddSmallProducerSubmissionResult>>();

                fixture = new Fixture();

                directProducerSubmission = DirectRegistrantSubmissionDbSetup.Init()
                    .WithDefaultRegisteredProducer()
                    .WithComplianceYear(SystemTime.UtcNow.Year)
                    .WithDirectRegistrant(directRegistrant)
                    .Create();

                country = AsyncHelper.RunSync(() => Query.GetCountryByNameAsync("UK - England"));

                requestAddress = fixture.Build<AddressData>().With(a => a.CountryId, country.Id).Create();

                request = new EditOrganisationDetailsRequest(directRegistrant.Id, "New company name",
                    "New trading name", requestAddress, "New brand names");

                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, directRegistrant.OrganisationId).Create();

                return setup;
            }
        }
    }
}
