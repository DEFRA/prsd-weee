namespace EA.Weee.Integration.Tests.Handlers.DirectRegistrant
{
    using Autofac;
    using AutoFixture;
    using Base;
    using EA.Weee.Core.Shared;
    using EA.Weee.Domain;
    using EA.Weee.Integration.Tests.Builders;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using FluentAssertions;
    using NUnit.Specifications.Categories;
    using Prsd.Core.Autofac;
    using Prsd.Core.Mediator;
    using System;
    using System.Linq;
    using System.Security;

    public class ServiceOfNoticeRequestHandlerIntegrationTests : IntegrationTestBase
    {
        [Component]
        public class WhenIUpdateServiceOfNotice : ServiceOfNoticeRequestHandlerIntegrationTestBase
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

                entity.DirectProducerSubmissions.Count().Should().Be(1);

                var submission = entity.DirectProducerSubmissions.ElementAt(0);
                submission.CurrentSubmission.ServiceOfNoticeAddress.Address1.Should().Be(request.Address.Address1);
                submission.CurrentSubmission.ServiceOfNoticeAddress.Address2.Should().Be(request.Address.Address2);
                submission.CurrentSubmission.ServiceOfNoticeAddress.TownOrCity.Should().Be(request.Address.TownOrCity);
                submission.CurrentSubmission.ServiceOfNoticeAddress.CountyOrRegion.Should().Be(request.Address.CountyOrRegion);
                submission.CurrentSubmission.ServiceOfNoticeAddress.CountryId.Should().Be(request.Address.CountryId);
                submission.CurrentSubmission.ServiceOfNoticeAddress.Postcode.Should().Be(request.Address.Postcode);
                submission.CurrentSubmission.ServiceOfNoticeAddress.Telephone.Should().Be(request.Address.Telephone);
            };
        }

        [Component]
        public class WhenUserIsNotAuthorised : ServiceOfNoticeRequestHandlerIntegrationTestBase
        {
            protected static IRequestHandler<ServiceOfNoticeRequest, bool> authHandler;

            private readonly Establish context = () =>
            {
                SetupTest(IocApplication.RequestHandler)
                    .WithDefaultSettings();

                authHandler = Container.Resolve<IRequestHandler<ServiceOfNoticeRequest, bool>>();
            };

            private readonly Because of = () =>
            {
                CatchExceptionAsync(() => authHandler.HandleAsync(request));
            };

            private readonly It shouldHaveCaughtArgumentException = ShouldThrowException<SecurityException>;
        }

        public class ServiceOfNoticeRequestHandlerIntegrationTestBase : WeeeContextSpecification
        {
            protected static IRequestHandler<ServiceOfNoticeRequest, bool> handler;
            protected static IRequestHandler<AddSmallProducerSubmission, Guid> createSubmissionHandler;
            protected static Fixture fixture;
            protected static Domain.Producer.DirectRegistrant directRegistrant;
            protected static ServiceOfNoticeRequest request;
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

                handler = Container.Resolve<IRequestHandler<ServiceOfNoticeRequest, bool>>();
                createSubmissionHandler = Container.Resolve<IRequestHandler<AddSmallProducerSubmission, Guid>>();

                fixture = new Fixture();

                country = AsyncHelper.RunSync(() => Query.GetCountryByNameAsync("UK - England"));

                var address = fixture.Build<AddressData>().With(a => a.CountryId, country.Id).Create();

                request = new ServiceOfNoticeRequest(directRegistrant.Id, address);

                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, directRegistrant.OrganisationId).Create();

                return setup;
            }
        }
    }
}
