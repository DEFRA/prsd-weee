﻿namespace EA.Weee.Integration.Tests.Handlers.DirectRegistrant
{
    using Autofac;
    using AutoFixture;
    using Base;
    using EA.Weee.Core.Organisations;
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
    using System.Data.Entity;
    using System.Linq;
    using System.Security;
    using EA.Prsd.Core;

    public class EditRepresentedOrganisationDetailsRequestHandlerIntegrationTests : IntegrationTestBase
    {
        [Component]
        public class WhenIUpdateRepresentingOrganisationDetailsWithNoSubmissionDetails : EditRepresentedOrganisationDetailsRequestHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                authorisedRepresentative = AuthorisedRepDbSetup.Init().Create();

                var registeredProducer = new RegisteredProducer(SystemTime.UtcNow.Ticks.ToString(), SystemTime.UtcNow.Year);

                directRegistrant = DirectRegistrantDbSetup.Init()
                    .WithAuthorisedRep(authorisedRepresentative)
                    .Create();

                directProducerSubmission = DirectRegistrantSubmissionDbSetup.Init()
                    .WithDirectRegistrant(directRegistrant)
                    .WithComplianceYear(SystemTime.UtcNow.Year)
                    .WithRegisteredProducer(registeredProducer)
                    .Create();

                directProducerSubmissionHistory = DirectRegistrantSubmissionHistoryDbSetup.Init()
                    .WithDirectProducerSubmission(directProducerSubmission).Create();

                Query.UpdateCurrentProducerSubmission(directProducerSubmission.Id, directProducerSubmissionHistory.Id);
                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, directRegistrant.OrganisationId).Create();

                request = new RepresentedOrganisationDetailsRequest(
                    directRegistrant.Id,
                    "business trading name",
                    representingCompanyDetails);
            };

            private readonly Because of = () =>
            {
                result = AsyncHelper.RunSync(() => handler.HandleAsync(request));
            };

            private readonly It shouldUpdateTheData = () =>
            {
                var entity = Query.GetDirectProducerSubmissionById(directProducerSubmission.Id);

                entity.Should().NotBeNull();

                var authedRep = entity.CurrentSubmission.AuthorisedRepresentative;

                authedRep.OverseasProducerName.Should()
                    .Be(authorisedRepresentative.OverseasProducerName);
                authedRep.OverseasProducerTradingName.Should().Be(request.BusinessTradingName);
                authedRep.OverseasContact.Address.PrimaryName.Should().Be(request.Address.Address1);
                authedRep.OverseasContact.Address.SecondaryName.Should().BeEmpty();
                authedRep.OverseasContact.Address.Street.Should().Be(request.Address.Address2);
                authedRep.OverseasContact.Address.Town.Should().Be(request.Address.TownOrCity);
                authedRep.OverseasContact.Address.AdministrativeArea.Should().Be(request.Address.CountyOrRegion);
                authedRep.OverseasContact.Address.CountryId.Should().Be(request.Address.CountryId);
                authedRep.OverseasContact.Address.PostCode.Should().Be(request.Address.Postcode);
                authedRep.OverseasContact.Email.Should().Be(request.Address.Email);
                authedRep.OverseasContact.Telephone.Should().Be(request.Address.Telephone);
            };
        }

        [Component]
        public class WhenIUpdateRepresentingOrganisationDetailsWithExistingSubmissionDetails : EditRepresentedOrganisationDetailsRequestHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                authorisedRepresentative = AuthorisedRepDbSetup.Init().Create();

                var registeredProducer = new RegisteredProducer(SystemTime.UtcNow.Ticks.ToString(), SystemTime.UtcNow.Year);

                directRegistrant = DirectRegistrantDbSetup.Init()
                    .WithAuthorisedRep(authorisedRepresentative)
                    .Create();

                var submissionAuthorisedRep = AuthorisedRepDbSetup.Init().Create();

                directProducerSubmission = DirectRegistrantSubmissionDbSetup.Init()
                    .WithDirectRegistrant(directRegistrant)
                    .WithComplianceYear(SystemTime.UtcNow.Year)
                    .WithRegisteredProducer(registeredProducer)
                    .Create();

                directProducerSubmissionHistory = DirectRegistrantSubmissionHistoryDbSetup.Init()
                    .WithAuthorisedRep(submissionAuthorisedRep.Id)
                    .WithDirectProducerSubmission(directProducerSubmission).Create();

                Query.UpdateCurrentProducerSubmission(directProducerSubmission.Id, directProducerSubmissionHistory.Id);
                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, directRegistrant.OrganisationId).Create();

                request = new RepresentedOrganisationDetailsRequest(
                    directRegistrant.Id,
                    "business trading name",
                    representingCompanyDetails);
            };

            private readonly Because of = () =>
            {
                result = AsyncHelper.RunSync(() => handler.HandleAsync(request));
            };

            private readonly It shouldUpdateTheData = () =>
            {
                var entity = Query.GetDirectProducerSubmissionById(directProducerSubmission.Id);

                entity.Should().NotBeNull();

                var authedRep = entity.CurrentSubmission.AuthorisedRepresentative;

                authedRep.OverseasProducerName.Should()
                    .Be(authorisedRepresentative.OverseasProducerName);
                authedRep.OverseasProducerTradingName.Should().Be(request.BusinessTradingName);
                authedRep.OverseasContact.Address.PrimaryName.Should().Be(request.Address.Address1);
                authedRep.OverseasContact.Address.SecondaryName.Should().BeEmpty();
                authedRep.OverseasContact.Address.Street.Should().Be(request.Address.Address2);
                authedRep.OverseasContact.Address.Town.Should().Be(request.Address.TownOrCity);
                authedRep.OverseasContact.Address.AdministrativeArea.Should().Be(request.Address.CountyOrRegion);
                authedRep.OverseasContact.Address.CountryId.Should().Be(request.Address.CountryId);
                authedRep.OverseasContact.Address.PostCode.Should().Be(request.Address.Postcode);
                authedRep.OverseasContact.Email.Should().Be(request.Address.Email);
                authedRep.OverseasContact.Telephone.Should().Be(request.Address.Telephone);
            };
        }

        [Component]
        public class WhenUserIsNotAuthorised : EditRepresentedOrganisationDetailsRequestHandlerIntegrationTestBase
        {
            protected static IRequestHandler<RepresentedOrganisationDetailsRequest, bool> authHandler;

            private readonly Establish context = () =>
            {
                SetupTest(IocApplication.RequestHandler)
                    .WithDefaultSettings();

                directRegistrant = DirectRegistrantDbSetup.Init()
                    .WithAuthorisedRep(authorisedRepresentative)
                    .Create();

                authHandler = Container.Resolve<IRequestHandler<RepresentedOrganisationDetailsRequest, bool>>();
            };

            private readonly Because of = () =>
            {
                CatchExceptionAsync(() => authHandler.HandleAsync(new RepresentedOrganisationDetailsRequest(
                    directRegistrant.Id,
                    "business trading name",
                    representingCompanyDetails)));
            };

            private readonly It shouldHaveCaughtArgumentException = ShouldThrowException<SecurityException>;
        }

        public class EditRepresentedOrganisationDetailsRequestHandlerIntegrationTestBase : WeeeContextSpecification
        {
            protected static IRequestHandler<RepresentedOrganisationDetailsRequest, bool> handler;
            protected static Fixture fixture;
            protected static Domain.Producer.DirectRegistrant directRegistrant;
            protected static Domain.Producer.DirectProducerSubmission directProducerSubmission;
            protected static Domain.Producer.DirectProducerSubmissionHistory directProducerSubmissionHistory;
            protected static RepresentedOrganisationDetailsRequest request;
            protected static bool result;
            protected static Country country;
            protected static AuthorisedRepresentative authorisedRepresentative;
            protected static RepresentingCompanyAddressData representingCompanyDetails;

            public static IntegrationTestSetupBuilder LocalSetup()
            {
                var setup = SetupTest(IocApplication.RequestHandler)
                    .WithIoC()
                    .WithTestData()
                .WithExternalUserAccess();

                fixture = new Fixture();

                handler = Container.Resolve<IRequestHandler<RepresentedOrganisationDetailsRequest, bool>>();

                country = AsyncHelper.RunSync(() => Query.GetCountryByNameAsync("UK - England"));

                representingCompanyDetails = fixture.Build<RepresentingCompanyAddressData>()
                    .With(r => r.CountryId, country.Id).Create();

                country = AsyncHelper.RunSync(() => Query.GetCountryByNameAsync("UK - England"));

                return setup;
            }
        }
    }
}
