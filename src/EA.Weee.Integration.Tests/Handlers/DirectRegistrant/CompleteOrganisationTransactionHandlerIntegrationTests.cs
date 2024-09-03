namespace EA.Weee.Integration.Tests.Handlers.DirectRegistrant
{
    using Autofac;
    using AutoFixture;
    using Base;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Organisations.Base;
    using EA.Weee.Core.Shared;
    using EA.Weee.Domain;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.Integration.Tests.Builders;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using FluentAssertions;
    using Newtonsoft.Json;
    using NUnit.Specifications.Categories;
    using Prsd.Core.Autofac;
    using Prsd.Core.Mediator;
    using System;
    using System.Security;

    public class CompleteOrganisationTransactionHandlerIntegrationTests : IntegrationTestBase
    {
        [Component]
        public class WhenICompleteAnRegisteredCompanyOrganisationTransaction : CompleteOrganisationTransactionIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                var registeredCompanyDetails = fixture.Build<OrganisationViewModel>()
                    .With(r => r.Address, addressData).Create();

                organisationTransactionData = fixture.Build<OrganisationTransactionData>()
                    .With(o => o.OrganisationType, ExternalOrganisationType.RegisteredCompany)
                    .With(o => o.OrganisationViewModel, registeredCompanyDetails)
                    .With(o => o.ContactDetailsViewModel, contactDetailsViewModel).Create();

                OrganisationTransactionDbSetup.Init().WithModel(organisationTransactionData).Create();

                request = new CompleteOrganisationTransaction();
            };

            private readonly Because of = () =>
            {
                result = AsyncHelper.RunSync(() => handler.HandleAsync(request));
            };

            private readonly It shouldHaveCompletedTheTransaction = () =>
            {
                var entity = Query.GetOrganisationTransactionForUser(UserId.ToString());

                entity.OrganisationJson.Should().Be(JsonConvert.SerializeObject(organisationTransactionData));
                entity.UserId.Should().Be(UserId.ToString());
                entity.CreatedDateTime.Should().BeAfter(date);
                entity.CompletedDateTime.Should().BeAfter(date);
                entity.CompletionStatus.Should().Be(CompletionStatus.Complete);
            };

            private readonly It shouldHaveReturnedOrganisationAddress = () =>
            {
                result.Should().NotBeEmpty();

                var organisation = Query.GetOrganisationById(result);
                organisation.Should().NotBeNull();
                organisation.BusinessAddress.Address1.Should().Be(addressData.Address1);
                organisation.BusinessAddress.Address2.Should().Be(addressData.Address2);
                organisation.BusinessAddress.CountryId.Should().Be(addressData.CountryId);
                organisation.BusinessAddress.Postcode.Should().Be(addressData.Postcode);
                organisation.BusinessAddress.TownOrCity.Should().Be(addressData.TownOrCity);
            };

            private readonly It shouldHaveReturnedOrganisationDetails = () =>
            {
                result.Should().NotBeEmpty();

                var organisation = Query.GetOrganisationById(result);
                organisation.Should().NotBeNull();
                organisation.OrganisationType.Should().Be(Domain.Organisation.OrganisationType.RegisteredCompany);
                organisation.OrganisationName.Should()
                    .Be(organisationTransactionData.OrganisationViewModel.CompanyName);
                organisation.TradingName.Should()
                    .Be(organisationTransactionData.OrganisationViewModel.BusinessTradingName);
                organisation.CompanyRegistrationNumber.Should()
                    .Be(organisationTransactionData.OrganisationViewModel.CompaniesRegistrationNumber);
                organisation.OrganisationStatus.Should().Be(Domain.Organisation.OrganisationStatus.Complete);
            };

            private readonly It shouldHaveCreatedDirectRegistrant = () =>
            {
                var directRegistrant = Query.GetDirectRegistrantByOrganisationId(result);

                directRegistrant.Should().NotBeNull();
                directRegistrant.BrandName.Name.Should()
                    .Be(organisationTransactionData.OrganisationViewModel.EEEBrandNames);
            };

            private readonly It shouldHaveReturnedContactDetails = () =>
            {
                var directRegistrant = Query.GetDirectRegistrantByOrganisationId(result);
                directRegistrant.Contact.FirstName.Should().Be(contactDetailsViewModel.FirstName);
                directRegistrant.Contact.LastName.Should().Be(contactDetailsViewModel.LastName);
                directRegistrant.Contact.Position.Should().Be(contactDetailsViewModel.Position);
                directRegistrant.Address.Address1.Should().Be(contactDetailsViewModel.AddressData.Address1);
                directRegistrant.Address.Address2.Should().Be(contactDetailsViewModel.AddressData.Address2);
                directRegistrant.Address.TownOrCity.Should().Be(contactDetailsViewModel.AddressData.TownOrCity);
                directRegistrant.Address.CountyOrRegion.Should().Be(contactDetailsViewModel.AddressData.CountyOrRegion);
                directRegistrant.Address.Postcode.Should().Be(contactDetailsViewModel.AddressData.Postcode);
                directRegistrant.Address.CountryId.Should().Be(contactDetailsViewModel.AddressData.CountryId);
                directRegistrant.Address.Email.Should().Be(contactDetailsViewModel.AddressData.Email);
                directRegistrant.Address.Telephone.Should().Be(contactDetailsViewModel.AddressData.Telephone);
            };
        }

        [Component]
        public class WhenICompleteAnSoleTraderOrganisationTransaction : CompleteOrganisationTransactionIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                var soleTraderDetails = fixture.Build<OrganisationViewModel>()
                    .With(r => r.Address, addressData).Create();

                organisationTransactionData = fixture.Build<OrganisationTransactionData>()
                    .With(o => o.OrganisationType, ExternalOrganisationType.SoleTrader)
                    .With(o => o.OrganisationViewModel, soleTraderDetails)
                    .With(o => o.ContactDetailsViewModel, contactDetailsViewModel).Create();

                OrganisationTransactionDbSetup.Init().WithModel(organisationTransactionData).Create();

                request = new CompleteOrganisationTransaction();
            };

            private readonly Because of = () =>
            {
                result = AsyncHelper.RunSync(() => handler.HandleAsync(request));
            };

            private readonly It shouldHaveCompletedTheTransaction = () =>
            {
                var entity = Query.GetOrganisationTransactionForUser(UserId.ToString());

                entity.OrganisationJson.Should().Be(JsonConvert.SerializeObject(organisationTransactionData));
                entity.UserId.Should().Be(UserId.ToString());
                entity.CreatedDateTime.Should().BeAfter(date);
                entity.CompletedDateTime.Should().BeAfter(date);
                entity.CompletionStatus.Should().Be(CompletionStatus.Complete);
            };

            private readonly It shouldHaveReturnedOrganisationAddress = () =>
            {
                result.Should().NotBeEmpty();

                var organisation = Query.GetOrganisationById(result);
                organisation.Should().NotBeNull();
                organisation.BusinessAddress.Address1.Should().Be(addressData.Address1);
                organisation.BusinessAddress.Address2.Should().Be(addressData.Address2);
                organisation.BusinessAddress.CountryId.Should().Be(addressData.CountryId);
                organisation.BusinessAddress.Postcode.Should().Be(addressData.Postcode);
                organisation.BusinessAddress.TownOrCity.Should().Be(addressData.TownOrCity);
            };

            private readonly It shouldHaveReturnedOrganisationDetails = () =>
            {
                result.Should().NotBeEmpty();

                var organisation = Query.GetOrganisationById(result);
                organisation.Should().NotBeNull();
                organisation.OrganisationType.Should().Be(Domain.Organisation.OrganisationType.SoleTraderOrIndividual);
                organisation.OrganisationName.Should()
                    .Be(organisationTransactionData.OrganisationViewModel.CompanyName);
                organisation.TradingName.Should()
                    .Be(organisationTransactionData.OrganisationViewModel.BusinessTradingName);
                organisation.CompanyRegistrationNumber.Should()
                    .Be(organisationTransactionData.OrganisationViewModel.CompaniesRegistrationNumber);
                organisation.OrganisationStatus.Should().Be(Domain.Organisation.OrganisationStatus.Complete);
            };

            private readonly It shouldHaveCreatedDirectRegistrant = () =>
            {
                var directRegistrant = Query.GetDirectRegistrantByOrganisationId(result);

                directRegistrant.Should().NotBeNull();
                directRegistrant.BrandName.Name.Should()
                    .Be(organisationTransactionData.OrganisationViewModel.EEEBrandNames);
            };

            private readonly It shouldHaveReturnedContactDetails = () =>
            {
                var directRegistrant = Query.GetDirectRegistrantByOrganisationId(result);
                directRegistrant.Contact.FirstName.Should().Be(contactDetailsViewModel.FirstName);
                directRegistrant.Contact.LastName.Should().Be(contactDetailsViewModel.LastName);
                directRegistrant.Contact.Position.Should().Be(contactDetailsViewModel.Position);
                directRegistrant.Address.Address1.Should().Be(contactDetailsViewModel.AddressData.Address1);
                directRegistrant.Address.Address2.Should().Be(contactDetailsViewModel.AddressData.Address2);
                directRegistrant.Address.TownOrCity.Should().Be(contactDetailsViewModel.AddressData.TownOrCity);
                directRegistrant.Address.CountyOrRegion.Should().Be(contactDetailsViewModel.AddressData.CountyOrRegion);
                directRegistrant.Address.Postcode.Should().Be(contactDetailsViewModel.AddressData.Postcode);
                directRegistrant.Address.CountryId.Should().Be(contactDetailsViewModel.AddressData.CountryId);
                directRegistrant.Address.Email.Should().Be(contactDetailsViewModel.AddressData.Email);
                directRegistrant.Address.Telephone.Should().Be(contactDetailsViewModel.AddressData.Telephone);
            };
        }

        [Component]
        public class WhenICompleteAPartnershipOrganisationTransaction : CompleteOrganisationTransactionIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                var partnershipDetails = fixture.Build<OrganisationViewModel>()
                    .With(r => r.Address, addressData).Create();

                organisationTransactionData = fixture.Build<OrganisationTransactionData>()
                    .With(o => o.OrganisationType, ExternalOrganisationType.Partnership)
                    .With(o => o.OrganisationViewModel, partnershipDetails)
                    .With(o => o.ContactDetailsViewModel, contactDetailsViewModel).Create();

                OrganisationTransactionDbSetup.Init().WithModel(organisationTransactionData).Create();

                request = new CompleteOrganisationTransaction();
            };

            private readonly Because of = () =>
            {
                result = AsyncHelper.RunSync(() => handler.HandleAsync(request));
            };

            private readonly It shouldHaveCompletedTheTransaction = () =>
            {
                var entity = Query.GetOrganisationTransactionForUser(UserId.ToString());

                entity.OrganisationJson.Should().Be(JsonConvert.SerializeObject(organisationTransactionData));
                entity.UserId.Should().Be(UserId.ToString());
                entity.CreatedDateTime.Should().BeAfter(date);
                entity.CompletedDateTime.Should().BeAfter(date);
                entity.CompletionStatus.Should().Be(CompletionStatus.Complete);
            };

            private readonly It shouldHaveReturnedOrganisationAddress = () =>
            {
                result.Should().NotBeEmpty();

                var organisation = Query.GetOrganisationById(result);
                organisation.Should().NotBeNull();
                organisation.BusinessAddress.Address1.Should().Be(addressData.Address1);
                organisation.BusinessAddress.Address2.Should().Be(addressData.Address2);
                organisation.BusinessAddress.CountryId.Should().Be(addressData.CountryId);
                organisation.BusinessAddress.Postcode.Should().Be(addressData.Postcode);
                organisation.BusinessAddress.TownOrCity.Should().Be(addressData.TownOrCity);
            };

            private readonly It shouldHaveReturnedOrganisationDetails = () =>
            {
                result.Should().NotBeEmpty();

                var organisation = Query.GetOrganisationById(result);
                organisation.Should().NotBeNull();
                organisation.OrganisationType.Should().Be(Domain.Organisation.OrganisationType.DirectRegistrantPartnership);
                organisation.OrganisationName.Should()
                    .Be(organisationTransactionData.OrganisationViewModel.CompanyName);
                organisation.TradingName.Should()
                    .Be(organisationTransactionData.OrganisationViewModel.BusinessTradingName);
                organisation.CompanyRegistrationNumber.Should()
                    .Be(organisationTransactionData.OrganisationViewModel.CompaniesRegistrationNumber);
                organisation.OrganisationStatus.Should().Be(Domain.Organisation.OrganisationStatus.Complete);
            };

            private readonly It shouldHaveCreatedDirectRegistrant = () =>
            {
                var directRegistrant = Query.GetDirectRegistrantByOrganisationId(result);

                directRegistrant.Should().NotBeNull();
                directRegistrant.BrandName.Name.Should()
                    .Be(organisationTransactionData.OrganisationViewModel.EEEBrandNames);
            };

            private readonly It shouldHaveReturnedContactDetails = () =>
            {
                var directRegistrant = Query.GetDirectRegistrantByOrganisationId(result);
                directRegistrant.Contact.FirstName.Should().Be(contactDetailsViewModel.FirstName);
                directRegistrant.Contact.LastName.Should().Be(contactDetailsViewModel.LastName);
                directRegistrant.Contact.Position.Should().Be(contactDetailsViewModel.Position);
                directRegistrant.Address.Address1.Should().Be(contactDetailsViewModel.AddressData.Address1);
                directRegistrant.Address.Address2.Should().Be(contactDetailsViewModel.AddressData.Address2);
                directRegistrant.Address.TownOrCity.Should().Be(contactDetailsViewModel.AddressData.TownOrCity);
                directRegistrant.Address.CountyOrRegion.Should().Be(contactDetailsViewModel.AddressData.CountyOrRegion);
                directRegistrant.Address.Postcode.Should().Be(contactDetailsViewModel.AddressData.Postcode);
                directRegistrant.Address.CountryId.Should().Be(contactDetailsViewModel.AddressData.CountryId);
                directRegistrant.Address.Email.Should().Be(contactDetailsViewModel.AddressData.Email);
                directRegistrant.Address.Telephone.Should().Be(contactDetailsViewModel.AddressData.Telephone);
            };
        }

        [Component]
        public class WhenICompleteAnOrganisationTransactionThatIsNotIncomplete : CompleteOrganisationTransactionIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                request = new CompleteOrganisationTransaction();
            };

            private readonly Because of = () =>
            {
                CatchExceptionAsync(() => handler.HandleAsync(request));
            };

            private readonly It shouldHaveCaughtArgumentException = ShouldThrowException<InvalidOperationException>;
        }

        [Component]
        public class WhenUserIsNotAuthorised : CompleteOrganisationTransactionIntegrationTestBase
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

        public class CompleteOrganisationTransactionIntegrationTestBase : WeeeContextSpecification
        {
            protected static IRequestHandler<CompleteOrganisationTransaction, Guid> handler;
            protected static Fixture fixture;
            protected static OrganisationTransactionData organisationTransactionData;
            protected static CompleteOrganisationTransaction request;
            protected static ExternalAddressData addressData;
            protected static Country country;
            protected static Guid result;
            protected static DateTime date;
            protected static ContactDetailsViewModel contactDetailsViewModel;

            public static IntegrationTestSetupBuilder LocalSetup()
            {
                var setup = SetupTest(IocApplication.RequestHandler)
                    .WithIoC()
                    .WithTestData()
                    .WithExternalUserAccess();

                AsyncHelper.RunSync(() => Query.DeleteAllOrganisationTransactionsAsync());

                date = DateTime.UtcNow;
                fixture = new Fixture();
                handler = Container.Resolve<IRequestHandler<CompleteOrganisationTransaction, Guid>>();

                country = AsyncHelper.RunSync(() => Query.GetCountryByNameAsync("UK - England"));
                addressData = fixture.Build<ExternalAddressData>().With(e => e.CountryId, country.Id).Create();

                var organisationContactAddress = fixture.Build<AddressPostcodeRequiredData>()
                    .With(o => o.CountryId, country.Id).Create();

                contactDetailsViewModel = fixture.Build<ContactDetailsViewModel>()
                    .With(r => r.AddressData, organisationContactAddress).Create();

                return setup;
            }
        }
    }
}
