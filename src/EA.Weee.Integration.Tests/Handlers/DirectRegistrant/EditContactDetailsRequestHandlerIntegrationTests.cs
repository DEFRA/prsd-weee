namespace EA.Weee.Integration.Tests.Handlers.DirectRegistrant
{
    using Autofac;
    using AutoFixture;
    using Base;
    using EA.Prsd.Core;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Shared;
    using EA.Weee.Domain;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.Integration.Tests.Builders;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using FluentAssertions;
    using NUnit.Specifications.Categories;
    using Prsd.Core.Autofac;
    using Prsd.Core.Mediator;
    using System.Security;

    public class EditContactDetailsRequestHandlerIntegrationTests : IntegrationTestBase
    {
        [Component]
        public class WhenIUpdateContactDetailsWithNoExistingDetails : EditContactDetailsRequestHandlerIntegrationTestBase
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

                entity.CurrentSubmission.Contact.FirstName.Should().Be(request.ContactData.FirstName);
                entity.CurrentSubmission.Contact.LastName.Should().Be(request.ContactData.LastName);
                entity.CurrentSubmission.Contact.Position.Should().Be(request.ContactData.Position);
                entity.CurrentSubmission.ContactAddress.Address1.Should().Be(request.AddressData.Address1);
                entity.CurrentSubmission.ContactAddress.Address2.Should().Be(request.AddressData.Address2);
                entity.CurrentSubmission.ContactAddress.TownOrCity.Should().Be(request.AddressData.TownOrCity);
                entity.CurrentSubmission.ContactAddress.CountyOrRegion.Should().Be(request.AddressData.CountyOrRegion);
                entity.CurrentSubmission.ContactAddress.CountryId.Should().Be(request.AddressData.CountryId);
                entity.CurrentSubmission.ContactAddress.Postcode.Should().Be(request.AddressData.Postcode);
                entity.CurrentSubmission.ContactAddress.Telephone.Should().Be(request.AddressData.Telephone);
                entity.CurrentSubmission.ContactAddress.Email.Should().Be(request.AddressData.Email);
            };
        }

        [Component]
        public class WhenIUpdateContactDetailsWithExistingDetails : EditContactDetailsRequestHandlerIntegrationTestBase
        {
            private static Contact contact;
            private static Address address;

            private readonly Establish context = () =>
            {
                LocalSetup();

                contact = ContactDbSetup.Init().Create();
                address = AddressDbSetup.Init().Create();

                var directProducerSubmissionHistory = DirectRegistrantSubmissionHistoryDbSetup
                    .Init()
                    .WithContact(contact)
                    .WithContactAddress(address)
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

                entity.CurrentSubmission.Contact.FirstName.Should().Be(request.ContactData.FirstName);
                entity.CurrentSubmission.Contact.LastName.Should().Be(request.ContactData.LastName);
                entity.CurrentSubmission.Contact.Position.Should().Be(request.ContactData.Position);
                entity.CurrentSubmission.ContactAddress.Address1.Should().Be(request.AddressData.Address1);
                entity.CurrentSubmission.ContactAddress.Address2.Should().Be(request.AddressData.Address2);
                entity.CurrentSubmission.ContactAddress.TownOrCity.Should().Be(request.AddressData.TownOrCity);
                entity.CurrentSubmission.ContactAddress.CountyOrRegion.Should().Be(request.AddressData.CountyOrRegion);
                entity.CurrentSubmission.ContactAddress.CountryId.Should().Be(request.AddressData.CountryId);
                entity.CurrentSubmission.ContactAddress.Postcode.Should().Be(request.AddressData.Postcode);
                entity.CurrentSubmission.ContactAddress.Telephone.Should().Be(request.AddressData.Telephone);
                entity.CurrentSubmission.ContactAddress.Email.Should().Be(request.AddressData.Email);
            };
        }

        [Component]
        public class WhenUserIsNotAuthorised : EditContactDetailsRequestHandlerIntegrationTestBase
        {
            protected static IRequestHandler<EditContactDetailsRequest, bool> authHandler;

            private readonly Establish context = () =>
            {
                SetupTest(IocApplication.RequestHandler)
                    .WithDefaultSettings();

                authHandler = Container.Resolve<IRequestHandler<EditContactDetailsRequest, bool>>();
            };

            private readonly Because of = () =>
            {
                CatchExceptionAsync(() => authHandler.HandleAsync(request));
            };

            private readonly It shouldHaveCaughtSecurityException = ShouldThrowException<SecurityException>;
        }

        public class EditContactDetailsRequestHandlerIntegrationTestBase : WeeeContextSpecification
        {
            protected static IRequestHandler<EditContactDetailsRequest, bool> handler;
            protected static Fixture fixture;
            protected static Domain.Producer.DirectRegistrant directRegistrant;
            protected static EditContactDetailsRequest request;
            protected static Domain.Producer.DirectProducerSubmission directProducerSubmission;
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

                directProducerSubmission = DirectRegistrantSubmissionDbSetup.Init()
                    .WithDefaultRegisteredProducer()
                    .WithComplianceYear(SystemTime.UtcNow.Year)
                    .WithDirectRegistrant(directRegistrant)
                    .Create();

                handler = Container.Resolve<IRequestHandler<EditContactDetailsRequest, bool>>();

                fixture = new Fixture();

                country = AsyncHelper.RunSync(() => Query.GetCountryByNameAsync("UK - England"));

                var addressData = fixture.Build<AddressData>().With(a => a.CountryId, country.Id).Create();
                var contactData = fixture.Build<ContactData>()
                    .With(a => a.FirstName, "First")
                    .With(a => a.LastName, "Last")
                    .With(a => a.Position, "Pos").Create();

                request = new EditContactDetailsRequest(directRegistrant.Id, addressData, contactData);

                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, directRegistrant.OrganisationId).Create();

                return setup;
            }
        }
    }
}
