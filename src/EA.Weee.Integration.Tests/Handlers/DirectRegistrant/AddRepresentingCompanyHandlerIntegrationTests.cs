namespace EA.Weee.Integration.Tests.Handlers.DirectRegistrant
{
    using Autofac;
    using AutoFixture;
    using EA.Prsd.Core.Autofac;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Domain;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.Integration.Tests.Base;
    using EA.Weee.Integration.Tests.Builders;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using FluentAssertions;
    using NUnit.Specifications.Categories;
    using System;
    using System.Security;

    public class AddRepresentingCompanyHandlerIntegrationTests : IntegrationTestBase
    {
        [Component]
        public class WhenIAddARepresentingCompanyWithNoBrandNames : AddRepresentingCompanyHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                var representingCompanyAddressDetails = fixture.Build<RepresentingCompanyAddressData>()
                    .With(r => r.CountryId, country.Id)
                    .Create();

                representingCompanyDetails = fixture.Build<RepresentingCompanyDetailsViewModel>()
                    .With(r => r.Address, representingCompanyAddressDetails)
                    .Create();

                originalDirectRegistrant = DirectRegistrantDbSetup.Init()
                    .WithOrganisation(organisation.Id)
                    .WithAddress(originalAddress.Id)
                    .WithContact(originalContact.Id)
                    .Create();

                request = new AddRepresentingCompany(organisation.Id, representingCompanyDetails);
            };

            private readonly Because of = () =>
            {
                result = AsyncHelper.RunSync(() => handler.HandleAsync(request));
            };

            private readonly It shouldHaveRepresentingCompanyRegistrant = () =>
            {
                var newDirectRegistrant = Query.GetDirectRegistrantById(result);

                newDirectRegistrant.Should().NotBeNull();
                newDirectRegistrant.Address.Id.Should().Be(originalDirectRegistrant.AddressId.Value);
                newDirectRegistrant.Contact.Id.Should().Be(originalDirectRegistrant.ContactId.Value);

                var overseasAddress = newDirectRegistrant.AuthorisedRepresentative.OverseasContact.Address;
                overseasAddress.PrimaryName.Should().Be(representingCompanyDetails.Address.Address1);
                overseasAddress.SecondaryName.Should().BeNullOrWhiteSpace();
                overseasAddress.Street.Should().Be(representingCompanyDetails.Address.Address2);
                overseasAddress.Town.Should().Be(representingCompanyDetails.Address.TownOrCity);
                overseasAddress.Locality.Should().BeNullOrWhiteSpace();
                overseasAddress.PostCode.Should().Be(representingCompanyDetails.Address.Postcode);
                overseasAddress.AdministrativeArea.Should().Be(representingCompanyDetails.Address.CountyOrRegion);
                overseasAddress.CountryId.Should().Be(representingCompanyDetails.Address.CountryId);

                newDirectRegistrant.BrandName.Should().BeNull();
            };
        }

        [Component]
        public class WhenIAddARepresentingCompanyWithBrandNames : AddRepresentingCompanyHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                var representingCompanyAddressDetails = fixture.Build<RepresentingCompanyAddressData>()
                    .With(r => r.CountryId, country.Id)
                    .Create();

                representingCompanyDetails = fixture.Build<RepresentingCompanyDetailsViewModel>()
                    .With(r => r.Address, representingCompanyAddressDetails)
                    .Create();

                originalDirectRegistrant = DirectRegistrantDbSetup.Init()
                    .WithOrganisation(organisation.Id)
                    .WithAddress(originalAddress.Id)
                    .WithContact(originalContact.Id)
                    .WithBrandName("new brand name")
                    .Create();

                request = new AddRepresentingCompany(organisation.Id, representingCompanyDetails);
            };

            private readonly Because of = () =>
            {
                result = AsyncHelper.RunSync(() => handler.HandleAsync(request));
            };

            private readonly It shouldHaveRepresentingCompanyRegistrant = () =>
            {
                var newDirectRegistrant = Query.GetDirectRegistrantById(result);

                newDirectRegistrant.Should().NotBeNull();
                newDirectRegistrant.Address.Id.Should().Be(originalDirectRegistrant.AddressId.Value);
                newDirectRegistrant.Contact.Id.Should().Be(originalDirectRegistrant.ContactId.Value);

                var overseasAddress = newDirectRegistrant.AuthorisedRepresentative.OverseasContact.Address;
                overseasAddress.PrimaryName.Should().Be(representingCompanyDetails.Address.Address1);
                overseasAddress.SecondaryName.Should().BeNullOrWhiteSpace();
                overseasAddress.Street.Should().Be(representingCompanyDetails.Address.Address2);
                overseasAddress.Town.Should().Be(representingCompanyDetails.Address.TownOrCity);
                overseasAddress.Locality.Should().BeNullOrWhiteSpace();
                overseasAddress.PostCode.Should().Be(representingCompanyDetails.Address.Postcode);
                overseasAddress.AdministrativeArea.Should().Be(representingCompanyDetails.Address.CountyOrRegion);
                overseasAddress.CountryId.Should().Be(representingCompanyDetails.Address.CountryId);

                newDirectRegistrant.BrandName.Name.Should().Be("new brand name");
            };
        }

        [Component]
        public class WhenUserIsNotAuthorised : AddRepresentingCompanyHandlerIntegrationTestBase
        {
            protected static IRequestHandler<AddRepresentingCompany, Guid> authHandler;

            private readonly Establish context = () =>
            {
                SetupTest(IocApplication.RequestHandler)
                    .WithDefaultSettings();

                authHandler = Container.Resolve<IRequestHandler<AddRepresentingCompany, Guid>>();
            };

            private readonly Because of = () =>
            {
                CatchExceptionAsync(() => authHandler.HandleAsync(new AddRepresentingCompany(organisation.Id,
                    fixture.Build<RepresentingCompanyDetailsViewModel>().Create())));
            };

            private readonly It shouldHaveCaughtArgumentException = ShouldThrowException<SecurityException>;
        }

        public class AddRepresentingCompanyHandlerIntegrationTestBase : WeeeContextSpecification
        {
            protected static IRequestHandler<AddRepresentingCompany, Guid> handler;
            protected static Fixture fixture;
            protected static AddRepresentingCompany request;
            protected static RepresentingCompanyDetailsViewModel representingCompanyDetails;
            protected static Country country;
            protected static Guid result;
            protected static Organisation organisation;
            protected static Domain.Producer.DirectRegistrant originalDirectRegistrant;
            protected static Address originalAddress;
            protected static Contact originalContact;

            public static IntegrationTestSetupBuilder LocalSetup()
            {
                var setup = SetupTest(IocApplication.RequestHandler)
                    .WithIoC()
                    .WithTestData()
                    .WithExternalUserAccess();

                fixture = new Fixture();
                handler = Container.Resolve<IRequestHandler<AddRepresentingCompany, Guid>>();

                organisation = OrganisationDbSetup.Init().Create();

                originalAddress = AddressDbSetup.Init().Create();
                originalContact = ContactDbSetup.Init().Create();

                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, organisation.Id).Create();

                country = AsyncHelper.RunSync(() => Query.GetCountryByNameAsync("UK - England"));

                return setup;
            }
        }
    }
}