namespace EA.Weee.Integration.Tests.Handlers.DirectRegistrant
{
    using Autofac;
    using AutoFixture;
    using Base;
    using EA.Weee.Core.Organisations;
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

    public class AddSignatoryAndCompleteRequestHandlerIntegrationTests : IntegrationTestBase
    {
        [Component]
        public class WhenIUpdateOrganisationDetails : AddSignatoryAndCompleteRequestHandlerIntegrationTestBase
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
                submission.CurrentSubmission.Contact.FirstName.Should().Be(request.ContactData.FirstName);
                submission.CurrentSubmission.Contact.LastName.Should().Be(request.ContactData.LastName);
                submission.CurrentSubmission.Contact.Position.Should().Be(request.ContactData.Position);

                submission.DirectRegistrant.Organisation.BusinessAddress.Should().Be(submission.CurrentSubmission.BusinessAddress);
                submission.DirectRegistrant.Organisation.Name.Should().Be(submission.CurrentSubmission.CompanyName);
                submission.DirectRegistrant.Organisation.TradingName.Should().Be(submission.CurrentSubmission.TradingName);

                submission.DirectRegistrant.Contact.Should().Be(submission.CurrentSubmission.Contact);
                submission.DirectRegistrant.Address.Should().Be(submission.CurrentSubmission.BusinessAddress);
                submission.DirectRegistrant.BrandName.Should().Be(submission.CurrentSubmission.BrandName);
                submission.DirectRegistrant.AuthorisedRepresentative.Should().Be(submission.CurrentSubmission.AuthorisedRepresentative);
            };
        }

        [Component]
        public class WhenUserIsNotAuthorised : AddSignatoryAndCompleteRequestHandlerIntegrationTestBase
        {
            protected static IRequestHandler<AddSignatoryAndCompleteRequest, bool> authHandler;

            private readonly Establish context = () =>
            {
                SetupTest(IocApplication.RequestHandler)
                    .WithDefaultSettings();

                authHandler = Container.Resolve<IRequestHandler<AddSignatoryAndCompleteRequest, bool>>();
            };

            private readonly Because of = () =>
            {
                CatchExceptionAsync(() => authHandler.HandleAsync(request));
            };

            private readonly It shouldHaveCaughtArgumentException = ShouldThrowException<SecurityException>;
        }

        public class AddSignatoryAndCompleteRequestHandlerIntegrationTestBase : WeeeContextSpecification
        {
            protected static IRequestHandler<AddSignatoryAndCompleteRequest, bool> handler;
            protected static IRequestHandler<AddSmallProducerSubmission, Guid> createSubmissionHandler;
            protected static Fixture fixture;
            protected static Domain.Producer.DirectRegistrant directRegistrant;
            protected static AddSignatoryAndCompleteRequest request;
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

                handler = Container.Resolve<IRequestHandler<AddSignatoryAndCompleteRequest, bool>>();
                createSubmissionHandler = Container.Resolve<IRequestHandler<AddSmallProducerSubmission, Guid>>();

                fixture = new Fixture();

                country = AsyncHelper.RunSync(() => Query.GetCountryByNameAsync("UK - England"));

                var addressData = fixture.Build<AddressData>().With(a => a.CountryId, country.Id).Create();

                var contactData = fixture.Build<ContactData>()
                    .With(a => a.FirstName, "First")
                    .With(a => a.LastName, "Last")
                    .With(a => a.Position, "Pos").Create();

                request = new AddSignatoryAndCompleteRequest(directRegistrant.Id, contactData);

                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, directRegistrant.OrganisationId).Create();

                return setup;
            }
        }
    }
}
