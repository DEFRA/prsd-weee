namespace EA.Weee.Integration.Tests.Handlers.DirectRegistrant
{
    using Autofac;
    using AutoFixture;
    using Base;
    using EA.Weee.Core.Organisations;
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

                var registeredCompanyDetails = fixture.Build<RegisteredCompanyDetailsViewModel>()
                    .With(r => r.Address, addressData).Create();

                organisationTransactionData = fixture.Build<OrganisationTransactionData>()
                    .With(o => o.OrganisationType, ExternalOrganisationType.RegisteredCompany)
                    .With(o => o.RegisteredCompanyDetailsViewModel, registeredCompanyDetails)
                    .Create();

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
                var entity = Query.GetOrganisationTransactionForUser(UserId.ToString());
                organisation.Should().NotBeNull();
                organisation.BusinessAddress.Address1.Should().Be(addressData.Address1);
                organisation.BusinessAddress.Address2.Should().Be(addressData.Address2);
                organisation.BusinessAddress.CountryId.Should().Be(addressData.CountryId);
                organisation.BusinessAddress.Postcode.Should().Be(addressData.Postcode);
                organisation.BusinessAddress.TownOrCity.Should().Be(addressData.TownOrCity);
                entity.Organisation.Should().Be(organisation);
            };

            private readonly It shouldHaveReturnedOrganisationDetails = () =>
            {
                result.Should().NotBeEmpty();

                var organisation = Query.GetOrganisationById(result);
                organisation.Should().NotBeNull();
                organisation.OrganisationType.Should().Be(Domain.Organisation.OrganisationType.RegisteredCompany);
                organisation.OrganisationName.Should()
                    .Be(organisationTransactionData.RegisteredCompanyDetailsViewModel.CompanyName);
                organisation.TradingName.Should()
                    .Be(organisationTransactionData.RegisteredCompanyDetailsViewModel.BusinessTradingName);
                organisation.CompanyRegistrationNumber.Should()
                    .Be(organisationTransactionData.RegisteredCompanyDetailsViewModel.CompaniesRegistrationNumber);
                organisation.OrganisationStatus.Should().Be(OrganisationStatus.Complete);
            };

            private readonly It shouldHaveCreatedDirectRegistrant = () =>
            {
                var directRegistrant = Query.GetDirectRegistrantByOrganisationId(result);

                directRegistrant.Should().NotBeNull();
                directRegistrant.BrandName.Name.Should()
                    .Be(organisationTransactionData.RegisteredCompanyDetailsViewModel.EEEBrandNames);
                directRegistrant.RepresentingCompany.Should().BeNull();
            };
        }

        [Component]
        public class WhenICompleteAnSoleTraderOrganisationTransaction : CompleteOrganisationTransactionIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                var soleTraderDetails = fixture.Build<SoleTraderDetailsViewModel>()
                    .With(r => r.Address, addressData).Create();

                organisationTransactionData = fixture.Build<OrganisationTransactionData>()
                    .With(o => o.OrganisationType, ExternalOrganisationType.SoleTrader)
                    .With(o => o.AuthorisedRepresentative, YesNoType.No)
                    .With(o => o.SoleTraderDetailsViewModel, soleTraderDetails).Create();

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
                var entity = Query.GetOrganisationTransactionForUser(UserId.ToString());
                organisation.Should().NotBeNull();
                organisation.OrganisationType.Should().Be(Domain.Organisation.OrganisationType.SoleTraderOrIndividual);
                organisation.OrganisationName.Should()
                    .Be(organisationTransactionData.SoleTraderDetailsViewModel.CompanyName);
                organisation.TradingName.Should()
                    .Be(organisationTransactionData.SoleTraderDetailsViewModel.BusinessTradingName);
                organisation.CompanyRegistrationNumber.Should()
                    .Be(organisationTransactionData.SoleTraderDetailsViewModel.CompaniesRegistrationNumber);
                organisation.OrganisationStatus.Should().Be(OrganisationStatus.Complete);
                entity.Organisation.Should().Be(organisation);
            };

            private readonly It shouldHaveCreatedDirectRegistrant = () =>
            {
                var directRegistrant = Query.GetDirectRegistrantByOrganisationId(result);

                directRegistrant.Should().NotBeNull();
                directRegistrant.BrandName.Name.Should()
                    .Be(organisationTransactionData.SoleTraderDetailsViewModel.EEEBrandNames);
                directRegistrant.RepresentingCompany.Should().BeNull();
            };
        }

        [Component]
        public class WhenICompleteAPartnershipOrganisationTransaction : CompleteOrganisationTransactionIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                var partnershipDetails = fixture.Build<PartnershipDetailsViewModel>()
                    .With(r => r.Address, addressData).Create();

                organisationTransactionData = fixture.Build<OrganisationTransactionData>()
                    .With(o => o.OrganisationType, ExternalOrganisationType.Partnership)
                    .With(o => o.AuthorisedRepresentative, YesNoType.No)
                    .With(o => o.PartnershipDetailsViewModel, partnershipDetails).Create();

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
                var entity = Query.GetOrganisationTransactionForUser(UserId.ToString());

                organisation.Should().NotBeNull();
                organisation.OrganisationType.Should().Be(Domain.Organisation.OrganisationType.DirectRegistrantPartnership);
                organisation.OrganisationName.Should()
                    .Be(organisationTransactionData.PartnershipDetailsViewModel.CompanyName);
                organisation.TradingName.Should()
                    .Be(organisationTransactionData.PartnershipDetailsViewModel.BusinessTradingName);
                organisation.CompanyRegistrationNumber.Should()
                    .Be(organisationTransactionData.PartnershipDetailsViewModel.CompaniesRegistrationNumber);
                organisation.OrganisationStatus.Should().Be(OrganisationStatus.Complete);
                entity.Organisation.Should().Be(organisation);
            };

            private readonly It shouldHaveCreatedDirectRegistrant = () =>
            {
                var directRegistrant = Query.GetDirectRegistrantByOrganisationId(result);

                directRegistrant.Should().NotBeNull();
                directRegistrant.BrandName.Name.Should()
                    .Be(organisationTransactionData.PartnershipDetailsViewModel.EEEBrandNames);
                directRegistrant.RepresentingCompany.Should().BeNull();
            };
        }

        [Component]
        public class WhenICompleteAnOrganisationWithAuthorisedRepresentitiveTransaction : CompleteOrganisationTransactionIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                var registeredCompanyDetails = fixture.Build<RegisteredCompanyDetailsViewModel>()
                    .With(r => r.Address, addressData).Create();

                organisationTransactionData = fixture.Build<OrganisationTransactionData>()
                    .With(o => o.OrganisationType, ExternalOrganisationType.RegisteredCompany)
                    .With(r => r.AuthorisedRepresentative, YesNoType.Yes)
                    .With(o => o.RegisteredCompanyDetailsViewModel, registeredCompanyDetails)
                    .With(o => o.RepresentingCompanyDetailsViewModel, representingCompanyDetails)
                    .Create();

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

                var entity = Query.GetOrganisationTransactionForUser(UserId.ToString());
                var organisation = Query.GetOrganisationById(result);
                organisation.Should().NotBeNull();
                organisation.OrganisationType.Should().Be(Domain.Organisation.OrganisationType.RegisteredCompany);
                organisation.OrganisationName.Should()
                    .Be(organisationTransactionData.RegisteredCompanyDetailsViewModel.CompanyName);
                organisation.TradingName.Should()
                    .Be(organisationTransactionData.RegisteredCompanyDetailsViewModel.BusinessTradingName);
                organisation.CompanyRegistrationNumber.Should()
                    .Be(organisationTransactionData.RegisteredCompanyDetailsViewModel.CompaniesRegistrationNumber);
                organisation.OrganisationStatus.Should().Be(OrganisationStatus.Complete);
                entity.Organisation.Should().Be(organisation);
            };

            private readonly It shouldHaveCreatedDirectRegistrant = () =>
            {
                var directRegistrant = Query.GetDirectRegistrantByOrganisationId(result);

                directRegistrant.Should().NotBeNull();
                directRegistrant.BrandName.Name.Should()
                    .Be(organisationTransactionData.RegisteredCompanyDetailsViewModel.EEEBrandNames);
                directRegistrant.RepresentingCompany.Should().NotBeNull();
                directRegistrant.RepresentingCompany.CompanyName.Should().Be(representingCompanyDetails.CompanyName);
                directRegistrant.RepresentingCompany.TradingName.Should().Be(representingCompanyDetails.BusinessTradingName);
                directRegistrant.RepresentingCompany.Address1.Should().Be(representingCompanyDetails.Address.Address1);
                directRegistrant.RepresentingCompany.Address2.Should().Be(representingCompanyDetails.Address.Address2);
                directRegistrant.RepresentingCompany.TownOrCity.Should().Be(representingCompanyDetails.Address.TownOrCity);
                directRegistrant.RepresentingCompany.CountyOrRegion.Should().Be(representingCompanyDetails.Address.CountyOrRegion);
                directRegistrant.RepresentingCompany.CountryId.Should().Be(representingCompanyDetails.Address.CountryId);
                directRegistrant.RepresentingCompany.Postcode.Should().Be(representingCompanyDetails.Address.Postcode);
                directRegistrant.RepresentingCompany.Email.Should().Be(representingCompanyDetails.Address.Email);
                directRegistrant.RepresentingCompany.Telephone.Should().Be(representingCompanyDetails.Address.Telephone);
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
            protected static RepresentingCompanyDetailsViewModel representingCompanyDetails;
            protected static Country country;
            protected static Guid result;
            protected static DateTime date;

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

                var representingCompanyAddressDetails = fixture.Build<RepresentingCompanyAddressData>()
                    .With(r => r.CountryId, country.Id).Create();

                representingCompanyDetails = fixture.Build<RepresentingCompanyDetailsViewModel>()
                    .With(r => r.Address, representingCompanyAddressDetails).Create();

                return setup;
            }
        }
    }
}
