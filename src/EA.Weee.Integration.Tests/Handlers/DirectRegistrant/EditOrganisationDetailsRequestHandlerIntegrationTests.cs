namespace EA.Weee.Integration.Tests.Handlers.DirectRegistrant
{
    using Autofac;
    using AutoFixture;
    using Base;
    using EA.Prsd.Core;
    using EA.Weee.Core.Shared;
    using EA.Weee.Domain;
    using EA.Weee.Domain.Producer;
    using EA.Weee.Integration.Tests.Builders;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using FluentAssertions;
    using NUnit.Specifications.Categories;
    using Prsd.Core.Autofac;
    using Prsd.Core.Mediator;
    using System;
    using System.Linq;
    using System.Security;

    public class EditOrganisationDetailsRequestHandlerIntegrationTests : IntegrationTestBase
    {
        [Component]
        public class WhenIUpdateOrganisationDetails : EditOrganisationDetailsRequestHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();
            };

            private readonly Because of = () =>
            {
               result = AsyncHelper.RunSync(() => handler.HandleAsync(request));
            };

            private readonly It shouldUpdateTheData = () =>
            {
                var entity = Query.GetDirectRegistrantByOrganisationId(directRegistrant.OrganisationId);

                entity.DirectProducerSubmissions.Count().Should().Be(1);

                var submission = entity.DirectProducerSubmissions.ElementAt(0);
                submission.CurrentSubmission.CompanyName.Should().Be(request.CompanyName);
                submission.CurrentSubmission.TradingName.Should().Be(request.TradingName);
                submission.CurrentSubmission.BrandName.Name.Should().Be(request.EEEBrandNames);
                submission.CurrentSubmission.BusinessAddress.Address1.Should().Be(request.BusinessAddressData.Address1);
                submission.CurrentSubmission.BusinessAddress.Address2.Should().Be(request.BusinessAddressData.Address2);
                submission.CurrentSubmission.BusinessAddress.TownOrCity.Should().Be(request.BusinessAddressData.TownOrCity);
                submission.CurrentSubmission.BusinessAddress.CountyOrRegion.Should().Be(request.BusinessAddressData.CountyOrRegion);
                submission.CurrentSubmission.BusinessAddress.CountryId.Should().Be(request.BusinessAddressData.CountryId);
                submission.CurrentSubmission.BusinessAddress.Postcode.Should().Be(request.BusinessAddressData.Postcode);
                submission.CurrentSubmission.BusinessAddress.WebAddress.Should().Be(request.BusinessAddressData.WebAddress);
                submission.CurrentSubmission.BusinessAddress.Telephone.Should().Be(request.BusinessAddressData.Telephone);
                submission.CurrentSubmission.BusinessAddress.Email.Should().Be(request.BusinessAddressData.Email);
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

            private readonly It shouldHaveCaughtArgumentException = ShouldThrowException<SecurityException>;
        }

        public class EditOrganisationDetailsRequestHandlerIntegrationTestBase : WeeeContextSpecification
        {
            protected static IRequestHandler<EditOrganisationDetailsRequest, bool> handler;
            protected static IRequestHandler<AddSmallProducerSubmission, Guid> createSubmissionHandler;
            protected static Fixture fixture;
            protected static Domain.Producer.DirectRegistrant directRegistrant;
            protected static Domain.Producer.DirectProducerSubmission directProducerSubmission;
            protected static EditOrganisationDetailsRequest request;
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
                createSubmissionHandler = Container.Resolve<IRequestHandler<AddSmallProducerSubmission, Guid>>();

                fixture = new Fixture();

                directProducerSubmission = DirectRegistrantSubmissionDbSetup.Init()
                    .WithDefaultRegisteredProducer()
                    .WithComplianceYear(SystemTime.UtcNow.Year)
                    .WithDirectRegistrant(directRegistrant)
                    .Create();

                var directProducerSubmissionHistory = DirectRegistrantSubmissionHistoryDbSetup.Init()
                    .WithDirectProducerSubmission(directProducerSubmission).Create();

                Query.UpdateCurrentProducerSubmission(directProducerSubmission.Id, directProducerSubmissionHistory.Id);

                country = AsyncHelper.RunSync(() => Query.GetCountryByNameAsync("UK - England"));

                var address = fixture.Build<AddressData>().With(a => a.CountryId, country.Id).Create();

                request = new EditOrganisationDetailsRequest(directRegistrant.Id, "New company name",
                    "New trading name", address, "New brand names");

                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, directRegistrant.OrganisationId).Create();

                return setup;
            }
        }
    }
}
