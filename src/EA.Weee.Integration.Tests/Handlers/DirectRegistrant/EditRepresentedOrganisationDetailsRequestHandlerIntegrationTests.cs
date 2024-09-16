namespace EA.Weee.Integration.Tests.Handlers.DirectRegistrant
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

    public class EditRepresentedOrganisationDetailsRequestHandlerIntegrationTests : IntegrationTestBase
    {
        [Component]
        public class WhenIUpdateRepresentingOrganisationDetails : EditRepresentedOrganisationDetailsRequestHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();
            };

            private readonly Because of = () =>
            {
                AsyncHelper.RunSync(() =>
                    createSubmissionHandler.HandleAsync(new AddSmallProducerSubmission(directRegistrant.Id)));

                result = AsyncHelper.RunSync(() => handler.HandleAsync(request));
            };

            private readonly It shouldUpdateTheData = () =>
            {
                var entity = Query.GetDirectRegistrantByOrganisationId(directRegistrant.OrganisationId);

                var submission = entity.DirectProducerSubmissions.ElementAt(0);

                var authedRep = submission.CurrentSubmission.AuthorisedRepresentative;

                authedRep.OverseasProducerName.Should()
                    .Be(authorisedRepresentative.OverseasProducerName);
                authedRep.OverseasProducerTradingName.Should().Be(request.BusinessTradingName);
                authedRep.OverseasContact.Address.PrimaryName.Should().Be(request.Address.Address1);
                authedRep.OverseasContact.Address.SecondaryName.Should().Be(request.Address.Address2);
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

                authHandler = Container.Resolve<IRequestHandler<RepresentedOrganisationDetailsRequest, bool>>();
            };

            private readonly Because of = () =>
            {
                CatchExceptionAsync(() => authHandler.HandleAsync(request));
            };

            private readonly It shouldHaveCaughtArgumentException = ShouldThrowException<SecurityException>;
        }

        public class EditRepresentedOrganisationDetailsRequestHandlerIntegrationTestBase : WeeeContextSpecification
        {
            protected static IRequestHandler<RepresentedOrganisationDetailsRequest, bool> handler;
            protected static IRequestHandler<AddSmallProducerSubmission, Guid> createSubmissionHandler;
            protected static Fixture fixture;
            protected static Domain.Producer.DirectRegistrant directRegistrant;
            protected static Domain.Producer.DirectProducerSubmission directProducerSubmission;
            protected static RepresentedOrganisationDetailsRequest request;
            protected static bool result;
            protected static Country country;
            protected static AuthorisedRepresentative authorisedRepresentative;

            public static IntegrationTestSetupBuilder LocalSetup()
            {
                var setup = SetupTest(IocApplication.RequestHandler)
                    .WithIoC()
                    .WithTestData()
                .WithExternalUserAccess();

                authorisedRepresentative = AuthorisedRepDbSetup.Init().Create();

                directRegistrant = DirectRegistrantDbSetup.Init()
                    .WithAuthorisedRep(authorisedRepresentative)
                    .Create();

                directProducerSubmission = DirectRegistrantSubmissionDbSetup.Init()
                    .WithDirectRegistrant(directRegistrant, authorisedRepresentative)
                    .Create();
                
                Query.UpdateCurrentProducerSubmission(directProducerSubmission.Id, directProducerSubmission.SubmissionHistory.ElementAt(0).Id);
                handler = Container.Resolve<IRequestHandler<RepresentedOrganisationDetailsRequest, bool>>();
                createSubmissionHandler = Container.Resolve<IRequestHandler<AddSmallProducerSubmission, Guid>>();

                fixture = new Fixture();

                country = AsyncHelper.RunSync(() => Query.GetCountryByNameAsync("UK - England"));

                var representingCompanyDetails = fixture.Build<RepresentingCompanyAddressData>()
                    .With(r => r.CountryId, country.Id).Create();

                request = new RepresentedOrganisationDetailsRequest(
                    directRegistrant.Id,
                    "business trading name",
                    representingCompanyDetails);

                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, directRegistrant.OrganisationId).Create();

                return setup;
            }
        }
    }
}
