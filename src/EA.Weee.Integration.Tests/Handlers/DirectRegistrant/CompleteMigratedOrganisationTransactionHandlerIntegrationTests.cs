namespace EA.Weee.Integration.Tests.Handlers.DirectRegistrant
{
    using Autofac;
    using AutoFixture;
    using Base;
    using EA.Prsd.Core.Domain;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Organisations.Base;
    using EA.Weee.Core.Shared;
    using EA.Weee.Domain;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.Domain.Producer;
    using EA.Weee.Integration.Tests.Builders;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using FluentAssertions;
    using Newtonsoft.Json;
    using NUnit.Specifications.Categories;
    using Prsd.Core.Autofac;
    using Prsd.Core.Mediator;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;

    public class CompleteMigratedOrganisationTransactionHandlerIntegrationTests : IntegrationTestBase
    {
        [Component]
        public class WhenICompleteARegisteredCompanyMigratedOrganisationTransaction : CompleteMigratedOrganisationTransactionIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                var registeredCompanyDetails = fixture.Build<OrganisationViewModel>()
                    .With(r => r.CompaniesRegistrationNumber, "12345678")
                    .With(r => r.OrganisationType, ExternalOrganisationType.RegisteredCompany)
                    .With(r => r.Address, addressData).Create();

                registeredCompanyDetails.ProducerRegistrationNumber = null;

                organisationTransactionData = fixture.Build<OrganisationTransactionData>()
                    .With(o => o.OrganisationType, ExternalOrganisationType.RegisteredCompany)
                    .With(o => o.OrganisationViewModel, registeredCompanyDetails)
                    .With(o => o.ContactDetailsViewModel, contactDetailsViewModel)
                    .With(o => o.PreviousRegistration, PreviouslyRegisteredProducerType.No)
                    .With(o => o.AuthorisedRepresentative, YesNoType.No)
                    .Without(o => o.PartnerModels)
                    .Without(o => o.SoleTraderViewModel)
                    .Create();

                OrganisationTransactionDbSetup.Init().WithModel(organisationTransactionData).Create();

                request = new CompleteMigratedOrganisationTransaction(directRegistrant.Id);
            };

            private readonly Because of = () =>
            {
                result = AsyncHelper.RunSync(() => handler.HandleAsync(request));

                organisation = Query.GetOrganisationById(result);
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

            private readonly It shouldHaveUpdatedOrganisationAddress = () =>
            {
                result.Should().NotBeEmpty();

                var entity = Query.GetOrganisationTransactionForUser(UserId.ToString());
                var updatedOrganisation = Query.GetOrganisationById(organisation.Id);

                updatedOrganisation.Should().NotBeNull();
                updatedOrganisation.BusinessAddress.Address1.Should().Be(addressData.Address1);
                updatedOrganisation.BusinessAddress.Address2.Should().Be(addressData.Address2);
                updatedOrganisation.BusinessAddress.CountryId.Should().Be(addressData.CountryId);
                updatedOrganisation.BusinessAddress.Postcode.Should().Be(addressData.Postcode);
                updatedOrganisation.BusinessAddress.TownOrCity.Should().Be(addressData.TownOrCity);
                entity.Organisation.Should().Be(updatedOrganisation);
            };

            private readonly It shouldHaveReturnedNoPartners = () =>
            {
                var additionalDetails = Query.GetAdditionalDetailsByRegistrantId(directRegistrant.Id, OrganisationAdditionalDetailsType.Partner);

                additionalDetails.Should().BeEmpty();
            };

            private readonly It shouldHaveReturnedNoSoleTraders = () =>
            {
                var additionalDetails = Query.GetAdditionalDetailsByRegistrantId(directRegistrant.Id, OrganisationAdditionalDetailsType.SoleTrader);

                additionalDetails.Should().BeEmpty();
            };

            private readonly It shouldHaveUpdatedOrganisationDetails = () =>
            {
                result.Should().NotBeEmpty();

                var updatedOrganisation = Query.GetOrganisationById(organisation.Id);

                updatedOrganisation.Should().NotBeNull();
                updatedOrganisation.NpwdMigrated.Should().BeTrue();
                updatedOrganisation.NpwdMigratedComplete.Should().BeTrue();
                updatedOrganisation.IsRepresentingCompany.Should().BeFalse();
                updatedOrganisation.OrganisationType.Should().Be(Domain.Organisation.OrganisationType.RegisteredCompany);
                updatedOrganisation.OrganisationName.Should()
                    .Be(organisationTransactionData.OrganisationViewModel.CompanyName);
                updatedOrganisation.TradingName.Should()
                    .Be(organisationTransactionData.OrganisationViewModel.BusinessTradingName);
                updatedOrganisation.CompanyRegistrationNumber.Should()
                    .Be(organisation.CompanyRegistrationNumber);
                updatedOrganisation.OrganisationStatus.Should().Be(Domain.Organisation.OrganisationStatus.Complete);
            };

            private readonly It shouldHaveUpdatedDirectRegistrant = () =>
            {
                var updateDirectRegistrant = Query.GetDirectRegistrantById(directRegistrant.Id);

                updateDirectRegistrant.Should().NotBeNull();
                updateDirectRegistrant.BrandName.Name.Should()
                    .Be(organisationTransactionData.OrganisationViewModel.EEEBrandNames);
                updateDirectRegistrant.AuthorisedRepresentative.Should().BeNull();
            };

            private readonly It shouldHaveUpdatedContactDetails = () =>
            {
                var updateDirectRegistrant = Query.GetDirectRegistrantById(directRegistrant.Id);
                updateDirectRegistrant.Contact.FirstName.Should().Be(contactDetailsViewModel.FirstName);
                updateDirectRegistrant.Contact.LastName.Should().Be(contactDetailsViewModel.LastName);
                updateDirectRegistrant.Contact.Position.Should().Be(contactDetailsViewModel.Position);
                updateDirectRegistrant.Address.Address1.Should().Be(contactDetailsViewModel.AddressData.Address1);
                updateDirectRegistrant.Address.Address2.Should().Be(contactDetailsViewModel.AddressData.Address2);
                updateDirectRegistrant.Address.TownOrCity.Should().Be(contactDetailsViewModel.AddressData.TownOrCity);
                updateDirectRegistrant.Address.CountyOrRegion.Should().Be(contactDetailsViewModel.AddressData.CountyOrRegion);
                updateDirectRegistrant.Address.Postcode.Should().Be(contactDetailsViewModel.AddressData.Postcode);
                updateDirectRegistrant.Address.CountryId.Should().Be(contactDetailsViewModel.AddressData.CountryId);
                updateDirectRegistrant.Address.Email.Should().Be(contactDetailsViewModel.AddressData.Email);
                updateDirectRegistrant.Address.Telephone.Should().Be(contactDetailsViewModel.AddressData.Telephone);
            };

            private readonly It shouldHaveCreatedOrganisationUser = () =>
            {
                var users = Query.GetOrganisationForUser(UserId.ToString());

                users.Should().Contain(o => o.OrganisationId == organisation.Id);
            };
        }

        [Component]
        public class WhenICompleteASoleTraderMigratedOrganisationTransaction : CompleteMigratedOrganisationTransactionIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                var soleTraderDetails = fixture.Build<OrganisationViewModel>()
                    .With(r => r.CompaniesRegistrationNumber, "1234567")
                    .With(r => r.OrganisationType, ExternalOrganisationType.SoleTrader)
                    .With(r => r.Address, addressData).Create();

                organisationTransactionData = fixture.Build<OrganisationTransactionData>()
                    .With(o => o.OrganisationType, ExternalOrganisationType.SoleTrader)
                    .With(o => o.AuthorisedRepresentative, YesNoType.No)
                    .With(o => o.OrganisationViewModel, soleTraderDetails)
                    .With(o => o.ContactDetailsViewModel, contactDetailsViewModel)
                    .With(o => o.SoleTraderViewModel, soleTraderModel)
                    .Without(o => o.PartnerModels)
                    .Create();

                OrganisationTransactionDbSetup.Init().WithModel(organisationTransactionData).Create();

                request = new CompleteMigratedOrganisationTransaction(directRegistrant.Id);
            };

            private readonly Because of = () =>
            {
                result = AsyncHelper.RunSync(() => handler.HandleAsync(request));

                organisation = Query.GetOrganisationById(result);
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

            private readonly It shouldHaveUpdatedOrganisationAddress = () =>
            {
                result.Should().NotBeEmpty();

                var entity = Query.GetOrganisationTransactionForUser(UserId.ToString());
                var updatedOrganisation = Query.GetOrganisationById(organisation.Id);

                updatedOrganisation.Should().NotBeNull();
                updatedOrganisation.BusinessAddress.Address1.Should().Be(addressData.Address1);
                updatedOrganisation.BusinessAddress.Address2.Should().Be(addressData.Address2);
                updatedOrganisation.BusinessAddress.CountryId.Should().Be(addressData.CountryId);
                updatedOrganisation.BusinessAddress.Postcode.Should().Be(addressData.Postcode);
                updatedOrganisation.BusinessAddress.TownOrCity.Should().Be(addressData.TownOrCity);
                entity.Organisation.Should().Be(updatedOrganisation);
            };

            private readonly It shouldHaveReturnedNoPartners = () =>
            {
                var additionalDetails = Query.GetAdditionalDetailsByRegistrantId(directRegistrant.Id, OrganisationAdditionalDetailsType.Partner);

                additionalDetails.Should().BeEmpty();
            };

            private readonly It shouldHaveUpdatedOrganisationDetails = () =>
            {
                result.Should().NotBeEmpty();

                var updatedOrganisation = Query.GetOrganisationById(organisation.Id);

                updatedOrganisation.Should().NotBeNull();
                updatedOrganisation.NpwdMigrated.Should().BeTrue();
                updatedOrganisation.NpwdMigratedComplete.Should().BeTrue();
                updatedOrganisation.IsRepresentingCompany.Should().BeFalse();
                updatedOrganisation.OrganisationType.Should().Be(Domain.Organisation.OrganisationType.SoleTraderOrIndividual);
                updatedOrganisation.OrganisationName.Should()
                    .Be(organisationTransactionData.OrganisationViewModel.CompanyName);
                updatedOrganisation.TradingName.Should()
                    .Be(organisationTransactionData.OrganisationViewModel.BusinessTradingName);
                updatedOrganisation.CompanyRegistrationNumber.Should()
                    .Be(organisation.CompanyRegistrationNumber);
                updatedOrganisation.OrganisationStatus.Should().Be(Domain.Organisation.OrganisationStatus.Complete);
            };

            private readonly It shouldHaveUpdatedDirectRegistrant = () =>
            {
                var updatedDirectRegistrant = Query.GetDirectRegistrantById(directRegistrant.Id);

                updatedDirectRegistrant.Should().NotBeNull();
                updatedDirectRegistrant.BrandName.Name.Should()
                    .Be(organisationTransactionData.OrganisationViewModel.EEEBrandNames);
                updatedDirectRegistrant.AuthorisedRepresentative.Should().BeNull();
            };

            private readonly It shouldHaveUpdatedContactDetails = () =>
            {
                var updatedDirectRegistrant = Query.GetDirectRegistrantById(directRegistrant.Id);
                updatedDirectRegistrant.Contact.FirstName.Should().Be(contactDetailsViewModel.FirstName);
                updatedDirectRegistrant.Contact.LastName.Should().Be(contactDetailsViewModel.LastName);
                updatedDirectRegistrant.Contact.Position.Should().Be(contactDetailsViewModel.Position);
                updatedDirectRegistrant.Address.Address1.Should().Be(contactDetailsViewModel.AddressData.Address1);
                updatedDirectRegistrant.Address.Address2.Should().Be(contactDetailsViewModel.AddressData.Address2);
                updatedDirectRegistrant.Address.TownOrCity.Should().Be(contactDetailsViewModel.AddressData.TownOrCity);
                updatedDirectRegistrant.Address.CountyOrRegion.Should().Be(contactDetailsViewModel.AddressData.CountyOrRegion);
                updatedDirectRegistrant.Address.Postcode.Should().Be(contactDetailsViewModel.AddressData.Postcode);
                updatedDirectRegistrant.Address.CountryId.Should().Be(contactDetailsViewModel.AddressData.CountryId);
                updatedDirectRegistrant.Address.Email.Should().Be(contactDetailsViewModel.AddressData.Email);
                updatedDirectRegistrant.Address.Telephone.Should().Be(contactDetailsViewModel.AddressData.Telephone);
            };

            private readonly It shouldHaveReturnedSoleTraderDetails = () =>
            {
                var additionalDetails = Query.GetAdditionalDetailsByRegistrantId(directRegistrant.Id, OrganisationAdditionalDetailsType.SoleTrader);

                additionalDetails.Should().NotBeEmpty();
                additionalDetails.Count.Should().Be(1);
                additionalDetails.ElementAt(0).FirstName.Should().Be(soleTraderModel.FirstName);
                additionalDetails.ElementAt(0).LastName.Should().Be(soleTraderModel.LastName);
                additionalDetails.ElementAt(0).Type.Should().Be(OrganisationAdditionalDetailsType.SoleTrader);
            };

            private readonly It shouldHaveCreatedOrganisationUser = () =>
            {
                var users = Query.GetOrganisationForUser(UserId.ToString());

                users.Should().Contain(o => o.OrganisationId == organisation.Id);
            };
        }

        [Component]
        public class WhenICompleteAPartnershipMigratedOrganisationTransaction : CompleteMigratedOrganisationTransactionIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                var partnershipDetails = fixture.Build<OrganisationViewModel>()
                    .With(r => r.CompaniesRegistrationNumber, "1234567")
                    .With(r => r.OrganisationType, ExternalOrganisationType.Partnership)
                    .With(r => r.Address, addressData).Create();

                organisationTransactionData = fixture.Build<OrganisationTransactionData>()
                    .With(o => o.OrganisationType, ExternalOrganisationType.Partnership)
                    .With(o => o.AuthorisedRepresentative, YesNoType.No)
                    .With(o => o.OrganisationViewModel, partnershipDetails)
                    .With(o => o.ContactDetailsViewModel, contactDetailsViewModel)
                    .With(o => o.PartnerModels, partnerViewModel)
                    .Without(o => o.SoleTraderViewModel)
                    .Create();

                OrganisationTransactionDbSetup.Init().WithModel(organisationTransactionData).Create();

                request = new CompleteMigratedOrganisationTransaction(directRegistrant.Id);
            };

            private readonly Because of = () =>
            {
                result = AsyncHelper.RunSync(() => handler.HandleAsync(request));

                organisation = Query.GetOrganisationById(result);
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

            private readonly It shouldHaveUpdatedOrganisationAddress = () =>
            {
                result.Should().NotBeEmpty();

                var updatedOrganisation = Query.GetOrganisationById(organisation.Id);

                updatedOrganisation.Should().NotBeNull();
                updatedOrganisation.BusinessAddress.Address1.Should().Be(addressData.Address1);
                updatedOrganisation.BusinessAddress.Address2.Should().Be(addressData.Address2);
                updatedOrganisation.BusinessAddress.CountryId.Should().Be(addressData.CountryId);
                updatedOrganisation.BusinessAddress.Postcode.Should().Be(addressData.Postcode);
                updatedOrganisation.BusinessAddress.TownOrCity.Should().Be(addressData.TownOrCity);
            };

            private readonly It shouldHaveReturnedPartners = () =>
            {
                var additionalDetails = Query.GetAdditionalDetailsByRegistrantId(directRegistrant.Id, OrganisationAdditionalDetailsType.Partner);

                additionalDetails.Should().NotBeEmpty();
                
                foreach (var partnerModel in partnerViewModel)
                {
                    additionalDetails.Should().Contain(c =>
                        c.FirstName == partnerModel.FirstName && c.LastName == partnerModel.LastName && c.Type == OrganisationAdditionalDetailsType.Partner);
                }
            };

            private readonly It shouldHaveReturnedNoSoleTraders = () =>
            {
                var additionalDetails = Query.GetAdditionalDetailsByRegistrantId(directRegistrant.Id, OrganisationAdditionalDetailsType.SoleTrader);

                additionalDetails.Should().BeEmpty();
            };

            private readonly It shouldHaveUpdatedOrganisationDetails = () =>
            {
                result.Should().NotBeEmpty();

                var updatedOrganisation = Query.GetOrganisationById(organisation.Id);

                updatedOrganisation.Should().NotBeNull();
                updatedOrganisation.NpwdMigrated.Should().BeTrue();
                updatedOrganisation.NpwdMigratedComplete.Should().BeTrue();
                updatedOrganisation.IsRepresentingCompany.Should().BeFalse();
                updatedOrganisation.OrganisationType.Should().Be(Domain.Organisation.OrganisationType.DirectRegistrantPartnership);
                updatedOrganisation.OrganisationName.Should()
                    .Be(organisationTransactionData.OrganisationViewModel.CompanyName);
                updatedOrganisation.TradingName.Should()
                    .Be(organisationTransactionData.OrganisationViewModel.BusinessTradingName);
                updatedOrganisation.CompanyRegistrationNumber.Should()
                    .Be(organisation.CompanyRegistrationNumber);
                updatedOrganisation.OrganisationStatus.Should().Be(Domain.Organisation.OrganisationStatus.Complete);
            };

            private readonly It shouldHaveUpdatedDirectRegistrant = () =>
            {
                var updatedDirectRegistrant = Query.GetDirectRegistrantById(directRegistrant.Id);

                updatedDirectRegistrant.Should().NotBeNull();
                updatedDirectRegistrant.BrandName.Name.Should()
                    .Be(organisationTransactionData.OrganisationViewModel.EEEBrandNames);
                updatedDirectRegistrant.AuthorisedRepresentative.Should().BeNull();
            };

            private readonly It shouldHaveCreatedOrganisationUser = () =>
            {
                var users = Query.GetOrganisationForUser(UserId.ToString());

                users.Should().Contain(o => o.OrganisationId == organisation.Id);
            };
        }

        [Component]
        public class WhenICompleteAMigratedOrganisationWithAuthorisedRepresentitiveTransaction : CompleteMigratedOrganisationTransactionIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                var registeredCompanyDetails = fixture.Build<OrganisationViewModel>()
                    .With(r => r.CompaniesRegistrationNumber, "12345678")
                    .With(r => r.OrganisationType, ExternalOrganisationType.RegisteredCompany)
                    .With(r => r.Address, addressData).Create();

                organisationTransactionData = fixture.Build<OrganisationTransactionData>()
                    .With(o => o.OrganisationType, ExternalOrganisationType.RegisteredCompany)
                    .With(r => r.AuthorisedRepresentative, YesNoType.Yes)
                    .With(o => o.ContactDetailsViewModel, contactDetailsViewModel)
                    .With(o => o.OrganisationViewModel, registeredCompanyDetails)
                    .With(o => o.RepresentingCompanyDetailsViewModel, representingCompanyDetails)
                    .Create();

                OrganisationTransactionDbSetup.Init().WithModel(organisationTransactionData).Create();

                request = new CompleteMigratedOrganisationTransaction(directRegistrant.Id);
            };

            private readonly Because of = () =>
            {
                result = AsyncHelper.RunSync(() => handler.HandleAsync(request));

                organisation = Query.GetOrganisationById(result);
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

            private readonly It shouldHaveUpdatedOrganisationAddress = () =>
            {
                result.Should().NotBeEmpty();

                var updatedOrganisation = Query.GetOrganisationById(organisation.Id);

                updatedOrganisation.Should().NotBeNull();
                updatedOrganisation.BusinessAddress.Address1.Should().Be(addressData.Address1);
                updatedOrganisation.BusinessAddress.Address2.Should().Be(addressData.Address2);
                updatedOrganisation.BusinessAddress.CountryId.Should().Be(addressData.CountryId);
                updatedOrganisation.BusinessAddress.Postcode.Should().Be(addressData.Postcode);
                updatedOrganisation.BusinessAddress.TownOrCity.Should().Be(addressData.TownOrCity);
            };

            private readonly It shouldHaveUpdatedOrganisationDetails = () =>
            {
                result.Should().NotBeEmpty();

                var updatedOrganisation = Query.GetOrganisationById(organisation.Id);

                updatedOrganisation.Should().NotBeNull();
                updatedOrganisation.NpwdMigrated.Should().BeTrue();
                updatedOrganisation.NpwdMigratedComplete.Should().BeTrue();
                updatedOrganisation.IsRepresentingCompany.Should().BeTrue();
                updatedOrganisation.OrganisationType.Should().Be(Domain.Organisation.OrganisationType.RegisteredCompany);
                updatedOrganisation.OrganisationName.Should()
                    .Be(organisationTransactionData.OrganisationViewModel.CompanyName);
                updatedOrganisation.TradingName.Should()
                    .Be(organisationTransactionData.OrganisationViewModel.BusinessTradingName);
                updatedOrganisation.CompanyRegistrationNumber.Should()
                    .Be(organisation.CompanyRegistrationNumber);
                updatedOrganisation.OrganisationStatus.Should().Be(Domain.Organisation.OrganisationStatus.Complete);
            };

            private readonly It shouldHaveUpdatedDirectRegistrant = () =>
            {
                var updatedDirectRegistrant = Query.GetDirectRegistrantById(directRegistrant.Id);

                updatedDirectRegistrant.Should().NotBeNull();
                updatedDirectRegistrant.BrandName.Name.Should()
                    .Be(organisationTransactionData.OrganisationViewModel.EEEBrandNames);
                updatedDirectRegistrant.AuthorisedRepresentative.Should().NotBeNull();
                updatedDirectRegistrant.AuthorisedRepresentative.OverseasProducerName.Should().Be(representingCompanyDetails.CompanyName);
                updatedDirectRegistrant.AuthorisedRepresentative.OverseasProducerTradingName.Should().Be(representingCompanyDetails.BusinessTradingName);
                updatedDirectRegistrant.AuthorisedRepresentative.OverseasContact.Address.PrimaryName.Should().Be(representingCompanyDetails.Address.Address1);
                updatedDirectRegistrant.AuthorisedRepresentative.OverseasContact.Address.Street.Should().Be(representingCompanyDetails.Address.Address2);
                updatedDirectRegistrant.AuthorisedRepresentative.OverseasContact.Address.Town.Should().Be(representingCompanyDetails.Address.TownOrCity);
                updatedDirectRegistrant.AuthorisedRepresentative.OverseasContact.Address.AdministrativeArea.Should().Be(representingCompanyDetails.Address.CountyOrRegion);
                updatedDirectRegistrant.AuthorisedRepresentative.OverseasContact.Address.CountryId.Should().Be(representingCompanyDetails.Address.CountryId);
                updatedDirectRegistrant.AuthorisedRepresentative.OverseasContact.Address.PostCode.Should().Be(representingCompanyDetails.Address.Postcode);
                updatedDirectRegistrant.AuthorisedRepresentative.OverseasContact.Email.Should().Be(representingCompanyDetails.Address.Email);
                updatedDirectRegistrant.AuthorisedRepresentative.OverseasContact.Telephone.Should().Be(representingCompanyDetails.Address.Telephone);
                updatedDirectRegistrant.AuthorisedRepresentative.OverseasContact.Address.SecondaryName.Should().BeEmpty();
                updatedDirectRegistrant.AuthorisedRepresentative.OverseasContact.Address.Locality.Should().BeEmpty();
                updatedDirectRegistrant.AuthorisedRepresentative.OverseasContact.SurName.Should().BeEmpty();
                updatedDirectRegistrant.AuthorisedRepresentative.OverseasContact.Fax.Should().BeEmpty();
                updatedDirectRegistrant.AuthorisedRepresentative.OverseasContact.ForeName.Should().BeEmpty();
                updatedDirectRegistrant.AuthorisedRepresentative.OverseasContact.Mobile.Should().BeEmpty();
                updatedDirectRegistrant.AuthorisedRepresentative.OverseasContact.Title.Should().BeEmpty();
            };

            private readonly It shouldHaveUpdatedContactDetails = () =>
            {
                var updatedDirectRegistrant = Query.GetDirectRegistrantById(directRegistrant.Id);
                updatedDirectRegistrant.Contact.FirstName.Should().Be(contactDetailsViewModel.FirstName);
                updatedDirectRegistrant.Contact.LastName.Should().Be(contactDetailsViewModel.LastName);
                updatedDirectRegistrant.Contact.Position.Should().Be(contactDetailsViewModel.Position);
                updatedDirectRegistrant.Address.Address1.Should().Be(contactDetailsViewModel.AddressData.Address1);
                updatedDirectRegistrant.Address.Address2.Should().Be(contactDetailsViewModel.AddressData.Address2);
                updatedDirectRegistrant.Address.TownOrCity.Should().Be(contactDetailsViewModel.AddressData.TownOrCity);
                updatedDirectRegistrant.Address.CountyOrRegion.Should().Be(contactDetailsViewModel.AddressData.CountyOrRegion);
                updatedDirectRegistrant.Address.Postcode.Should().Be(contactDetailsViewModel.AddressData.Postcode);
                updatedDirectRegistrant.Address.CountryId.Should().Be(contactDetailsViewModel.AddressData.CountryId);
                updatedDirectRegistrant.Address.Email.Should().Be(contactDetailsViewModel.AddressData.Email);
                updatedDirectRegistrant.Address.Telephone.Should().Be(contactDetailsViewModel.AddressData.Telephone);
            };

            private readonly It shouldHaveCreatedOrganisationUser = () =>
            {
                var users = Query.GetOrganisationForUser(UserId.ToString());

                users.Should().Contain(o => o.OrganisationId == organisation.Id);
            };
        }

        [Component]
        public class WhenICompleteAMigratedOrganisationWithAuthorisedRepresentitiveWithNullFieldsTransaction : CompleteMigratedOrganisationTransactionIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                var representingCompanyAddressDetails = fixture.Build<RepresentingCompanyAddressData>()
                    .Without(r => r.Address2)
                    .Without(r => r.CountyOrRegion)
                    .With(r => r.Email)
                    .With(r => r.Telephone)
                    .With(r => r.CountryId, country.Id).Create();

                representingCompanyDetails = fixture.Build<RepresentingCompanyDetailsViewModel>()
                    .With(r => r.Address, representingCompanyAddressDetails).Create();

                addressData = fixture.Build<ExternalAddressData>()
                    .Without(e => e.Address2)
                    .Without(e => e.CountyOrRegion)
                    .With(e => e.CountryId, country.Id).Create();

                var registeredCompanyDetails = fixture.Build<OrganisationViewModel>()
                    .With(r => r.CompaniesRegistrationNumber, "12345678")
                    .With(r => r.OrganisationType, ExternalOrganisationType.RegisteredCompany)
                    .With(r => r.Address, addressData).Create();

                organisationTransactionData = fixture.Build<OrganisationTransactionData>()
                    .With(o => o.OrganisationType, ExternalOrganisationType.RegisteredCompany)
                    .With(r => r.AuthorisedRepresentative, YesNoType.Yes)
                    .With(o => o.ContactDetailsViewModel, contactDetailsViewModel)
                    .With(o => o.OrganisationViewModel, registeredCompanyDetails)
                    .With(o => o.RepresentingCompanyDetailsViewModel, representingCompanyDetails)
                    .Create();

                OrganisationTransactionDbSetup.Init().WithModel(organisationTransactionData).Create();

                request = new CompleteMigratedOrganisationTransaction(directRegistrant.Id);
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

            private readonly It shouldHaveUpdatedOrganisationAddress = () =>
            {
                result.Should().NotBeEmpty();

                var updatedOrganisation = Query.GetOrganisationById(organisation.Id);

                updatedOrganisation.Should().NotBeNull();
                updatedOrganisation.BusinessAddress.Address1.Should().Be(addressData.Address1);
                updatedOrganisation.BusinessAddress.Address2.Should().BeNull();
                updatedOrganisation.BusinessAddress.CountryId.Should().Be(addressData.CountryId);
                updatedOrganisation.BusinessAddress.Postcode.Should().Be(addressData.Postcode);
                updatedOrganisation.BusinessAddress.TownOrCity.Should().Be(addressData.TownOrCity);
                updatedOrganisation.BusinessAddress.CountyOrRegion.Should().BeNull();
            };

            private readonly It shouldHaveUpdatedOrganisationDetails = () =>
            {
                result.Should().NotBeEmpty();

                var updatedOrganisation = Query.GetOrganisationById(organisation.Id);

                updatedOrganisation.Should().NotBeNull();
                updatedOrganisation.IsRepresentingCompany.Should().BeTrue();
                updatedOrganisation.OrganisationType.Should().Be(Domain.Organisation.OrganisationType.RegisteredCompany);
                updatedOrganisation.OrganisationName.Should()
                    .Be(organisationTransactionData.OrganisationViewModel.CompanyName);
                updatedOrganisation.TradingName.Should()
                    .Be(organisationTransactionData.OrganisationViewModel.BusinessTradingName);
                updatedOrganisation.CompanyRegistrationNumber.Should()
                    .Be(organisation.CompanyRegistrationNumber);
                updatedOrganisation.OrganisationStatus.Should().Be(Domain.Organisation.OrganisationStatus.Complete);
            };

            private readonly It shouldHaveUpdatedDirectRegistrant = () =>
            {
                var updatedDirectRegistrant = Query.GetDirectRegistrantById(directRegistrant.Id);

                updatedDirectRegistrant.Should().NotBeNull();
                updatedDirectRegistrant.BrandName.Name.Should()
                    .Be(organisationTransactionData.OrganisationViewModel.EEEBrandNames);
                updatedDirectRegistrant.AuthorisedRepresentative.Should().NotBeNull();
                updatedDirectRegistrant.AuthorisedRepresentative.OverseasProducerName.Should().Be(representingCompanyDetails.CompanyName);
                updatedDirectRegistrant.AuthorisedRepresentative.OverseasProducerTradingName.Should().Be(representingCompanyDetails.BusinessTradingName);
                updatedDirectRegistrant.AuthorisedRepresentative.OverseasContact.Address.PrimaryName.Should().Be(representingCompanyDetails.Address.Address1);
                updatedDirectRegistrant.AuthorisedRepresentative.OverseasContact.Address.Street.Should().BeEmpty();
                updatedDirectRegistrant.AuthorisedRepresentative.OverseasContact.Address.Town.Should().Be(representingCompanyDetails.Address.TownOrCity);
                updatedDirectRegistrant.AuthorisedRepresentative.OverseasContact.Address.AdministrativeArea.Should().BeEmpty();
                updatedDirectRegistrant.AuthorisedRepresentative.OverseasContact.Address.CountryId.Should().Be(representingCompanyDetails.Address.CountryId);
                updatedDirectRegistrant.AuthorisedRepresentative.OverseasContact.Address.PostCode.Should().Be(representingCompanyDetails.Address.Postcode);
                updatedDirectRegistrant.AuthorisedRepresentative.OverseasContact.Email.Should().Be(representingCompanyDetails.Address.Email);
                updatedDirectRegistrant.AuthorisedRepresentative.OverseasContact.Telephone.Should().Be(representingCompanyDetails.Address.Telephone);
                updatedDirectRegistrant.AuthorisedRepresentative.OverseasContact.Address.SecondaryName.Should().BeEmpty();
                updatedDirectRegistrant.AuthorisedRepresentative.OverseasContact.Address.Locality.Should().BeEmpty();
                updatedDirectRegistrant.AuthorisedRepresentative.OverseasContact.SurName.Should().BeEmpty();
                updatedDirectRegistrant.AuthorisedRepresentative.OverseasContact.Fax.Should().BeEmpty();
                updatedDirectRegistrant.AuthorisedRepresentative.OverseasContact.ForeName.Should().BeEmpty();
                updatedDirectRegistrant.AuthorisedRepresentative.OverseasContact.Mobile.Should().BeEmpty();
                updatedDirectRegistrant.AuthorisedRepresentative.OverseasContact.Title.Should().BeEmpty();
            };

            private readonly It shouldHaveUpdatedContactDetails = () =>
            {
                var updatedDirectRegistrant = Query.GetDirectRegistrantById(directRegistrant.Id);
                updatedDirectRegistrant.Contact.FirstName.Should().Be(contactDetailsViewModel.FirstName);
                updatedDirectRegistrant.Contact.LastName.Should().Be(contactDetailsViewModel.LastName);
                updatedDirectRegistrant.Contact.Position.Should().Be(contactDetailsViewModel.Position);
                updatedDirectRegistrant.Address.Address1.Should().Be(contactDetailsViewModel.AddressData.Address1);
                updatedDirectRegistrant.Address.Address2.Should().Be(contactDetailsViewModel.AddressData.Address2);
                updatedDirectRegistrant.Address.TownOrCity.Should().Be(contactDetailsViewModel.AddressData.TownOrCity);
                updatedDirectRegistrant.Address.CountyOrRegion.Should().Be(contactDetailsViewModel.AddressData.CountyOrRegion);
                updatedDirectRegistrant.Address.Postcode.Should().Be(contactDetailsViewModel.AddressData.Postcode);
                updatedDirectRegistrant.Address.CountryId.Should().Be(contactDetailsViewModel.AddressData.CountryId);
                updatedDirectRegistrant.Address.Email.Should().Be(contactDetailsViewModel.AddressData.Email);
                updatedDirectRegistrant.Address.Telephone.Should().Be(contactDetailsViewModel.AddressData.Telephone);
            };
        }

        [Component]
        public class WhenICompleteAMigratedOrganisationTransactionThatIsNotIncomplete : CompleteMigratedOrganisationTransactionIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                request = new CompleteMigratedOrganisationTransaction(directRegistrant.Id);
            };

            private readonly Because of = () =>
            {
                CatchExceptionAsync(() => handler.HandleAsync(request));
            };

            private readonly It shouldHaveCaughtArgumentException = ShouldThrowException<InvalidOperationException>;
        }

        [Component]
        public class WhenUserIsNotAuthorised : CompleteOrganisationTransactionHandlerIntegrationTests.CompleteOrganisationTransactionIntegrationTestBase
        {
            protected static IRequestHandler<CompleteMigratedOrganisationTransaction, Guid> authHandler;
            protected static DirectRegistrant directRegistrant;
            private readonly Establish context = () =>
            {
                SetupTest(IocApplication.RequestHandler)
                    .WithDefaultSettings();

                organisation = OrganisationDbSetup.Init()
                    .WithNpWdMigrated(true)
                    .WithNpWdMigratedComplete(false)
                    .Create();

                directRegistrant = DirectRegistrantDbSetup.Init()
                    .WithPrn("PRN123456")
                    .WithOrganisation(organisation.Id)
                    .Create();

                authHandler = Container.Resolve<IRequestHandler<CompleteMigratedOrganisationTransaction, Guid>>();
            };

            private readonly Because of = () =>
            {
                CatchExceptionAsync(() => authHandler.HandleAsync(new CompleteMigratedOrganisationTransaction(directRegistrant.Id)));
            };

            private readonly It shouldHaveCaughtArgumentException = ShouldThrowException<SecurityException>;
        }

        [Component]
        public class WhenICompleteAOrganisationTransactionPrnIsSetOnDirectRegistrant : CompleteMigratedOrganisationTransactionIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                var registeredCompanyDetails = fixture.Build<OrganisationViewModel>()
                    .With(r => r.CompaniesRegistrationNumber, "12345678")
                    .With(r => r.OrganisationType, ExternalOrganisationType.RegisteredCompany)
                    .With(r => r.Address, addressData).Create();

                registeredCompanyDetails.ProducerRegistrationNumber = "12345";

                organisationTransactionData = fixture.Build<OrganisationTransactionData>()
                    .With(o => o.OrganisationType, ExternalOrganisationType.RegisteredCompany)
                    .With(o => o.OrganisationViewModel, registeredCompanyDetails)
                    .With(o => o.ContactDetailsViewModel, contactDetailsViewModel)
                    .With(o => o.PreviousRegistration, PreviouslyRegisteredProducerType.YesPreviousSchemeMember)
                    .With(o => o.AuthorisedRepresentative, YesNoType.No)
                    .Without(o => o.PartnerModels)
                    .Without(o => o.SoleTraderViewModel)
                    .Create();

                OrganisationTransactionDbSetup.Init().WithModel(organisationTransactionData).Create();

                request = new CompleteMigratedOrganisationTransaction(directRegistrant.Id);
            };

            private readonly Because of = () =>
            {
                result = AsyncHelper.RunSync(() => handler.HandleAsync(request));

                organisation = Query.GetOrganisationById(result);
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

            private readonly It shouldHaveUpdatedOrganisationAddress = () =>
            {
                result.Should().NotBeEmpty();

                var updatedOrganisation = Query.GetOrganisationById(organisation.Id);

                updatedOrganisation.Should().NotBeNull();
                updatedOrganisation.BusinessAddress.Address1.Should().Be(addressData.Address1);
                updatedOrganisation.BusinessAddress.Address2.Should().Be(addressData.Address2);
                updatedOrganisation.BusinessAddress.CountryId.Should().Be(addressData.CountryId);
                updatedOrganisation.BusinessAddress.Postcode.Should().Be(addressData.Postcode);
                updatedOrganisation.BusinessAddress.TownOrCity.Should().Be(addressData.TownOrCity);
            };

            private readonly It shouldHaveReturnedNoPartners = () =>
            {
                var additionalDetails = Query.GetAdditionalDetailsByRegistrantId(directRegistrant.Id, OrganisationAdditionalDetailsType.Partner);

                additionalDetails.Should().BeEmpty();
            };

            private readonly It shouldHaveReturnedNoSoleTraders = () =>
            {
                var additionalDetails = Query.GetAdditionalDetailsByRegistrantId(directRegistrant.Id, OrganisationAdditionalDetailsType.SoleTrader);

                additionalDetails.Should().BeEmpty();
            };

            private readonly It shouldHaveUpdatedOrganisationDetails = () =>
            {
                result.Should().NotBeEmpty();

                var updatedOrganisation = Query.GetOrganisationById(organisation.Id);

                updatedOrganisation.Should().NotBeNull();
                updatedOrganisation.OrganisationType.Should().Be(Domain.Organisation.OrganisationType.RegisteredCompany);
                updatedOrganisation.OrganisationName.Should()
                    .Be(organisationTransactionData.OrganisationViewModel.CompanyName);
                updatedOrganisation.TradingName.Should()
                    .Be(organisationTransactionData.OrganisationViewModel.BusinessTradingName);
                updatedOrganisation.CompanyRegistrationNumber.Should()
                    .Be(organisation.CompanyRegistrationNumber);
                updatedOrganisation.OrganisationStatus.Should().Be(Domain.Organisation.OrganisationStatus.Complete);
            };

            private readonly It shouldHaveUpdatedDirectRegistrant = () =>
            {
                var updatedDirectRegistrant = Query.GetDirectRegistrantById(directRegistrant.Id);

                updatedDirectRegistrant.Should().NotBeNull();
                updatedDirectRegistrant.BrandName.Name.Should()
                    .Be(organisationTransactionData.OrganisationViewModel.EEEBrandNames);
                updatedDirectRegistrant.ProducerRegistrationNumber.Should().Be(updatedDirectRegistrant.ProducerRegistrationNumber);
                updatedDirectRegistrant.AuthorisedRepresentative.Should().BeNull();
            };

            private readonly It shouldHaveUpdatedContactDetails = () =>
            {
                var updatedDirectRegistrant = Query.GetDirectRegistrantById(directRegistrant.Id);
                updatedDirectRegistrant.Contact.FirstName.Should().Be(contactDetailsViewModel.FirstName);
                updatedDirectRegistrant.Contact.LastName.Should().Be(contactDetailsViewModel.LastName);
                updatedDirectRegistrant.Contact.Position.Should().Be(contactDetailsViewModel.Position);
                updatedDirectRegistrant.Address.Address1.Should().Be(contactDetailsViewModel.AddressData.Address1);
                updatedDirectRegistrant.Address.Address2.Should().Be(contactDetailsViewModel.AddressData.Address2);
                updatedDirectRegistrant.Address.TownOrCity.Should().Be(contactDetailsViewModel.AddressData.TownOrCity);
                updatedDirectRegistrant.Address.CountyOrRegion.Should().Be(contactDetailsViewModel.AddressData.CountyOrRegion);
                updatedDirectRegistrant.Address.Postcode.Should().Be(contactDetailsViewModel.AddressData.Postcode);
                updatedDirectRegistrant.Address.CountryId.Should().Be(contactDetailsViewModel.AddressData.CountryId);
                updatedDirectRegistrant.Address.Email.Should().Be(contactDetailsViewModel.AddressData.Email);
                updatedDirectRegistrant.Address.Telephone.Should().Be(contactDetailsViewModel.AddressData.Telephone);
            };

            private readonly It shouldHaveCreatedOrganisationUser = () =>
            {
                var users = Query.GetOrganisationForUser(UserId.ToString());

                users.Should().Contain(o => o.OrganisationId == organisation.Id);
            };
        }

        public class CompleteMigratedOrganisationTransactionIntegrationTestBase : WeeeContextSpecification
        {
            protected static IRequestHandler<CompleteMigratedOrganisationTransaction, Guid> handler;
            protected static Fixture fixture;
            protected static OrganisationTransactionData organisationTransactionData;
            protected static CompleteMigratedOrganisationTransaction request;
            protected static ExternalAddressData addressData;
            protected static RepresentingCompanyDetailsViewModel representingCompanyDetails;
            protected static Country country;
            protected static Guid result;
            protected static DateTime date;
            protected static ContactDetailsViewModel contactDetailsViewModel;
            protected static Organisation organisation;
            protected static List<AdditionalContactModel> partnerViewModel;
            protected static SoleTraderViewModel soleTraderModel;
            protected static Domain.Producer.DirectRegistrant directRegistrant;

            public static IntegrationTestSetupBuilder LocalSetup()
            {
                var setup = SetupTest(IocApplication.RequestHandler)
                    .WithIoC()
                    .WithTestData()
                    .WithExternalUserAccess();

                AsyncHelper.RunSync(() => Query.DeleteAllOrganisationTransactionsAsync());

                date = DateTime.UtcNow;
                fixture = new Fixture();
                handler = Container.Resolve<IRequestHandler<CompleteMigratedOrganisationTransaction, Guid>>();

                country = AsyncHelper.RunSync(() => Query.GetCountryByNameAsync("UK - England"));
                addressData = fixture.Build<ExternalAddressData>().With(e => e.CountryId, country.Id).Create();

                var organisationContactAddress = fixture.Build<AddressPostcodeRequiredData>()
                    .With(o => o.CountryId, country.Id).Create();

                contactDetailsViewModel = fixture.Build<ContactDetailsViewModel>()
                    .With(r => r.AddressData, organisationContactAddress).Create();

                soleTraderModel = fixture.Create<SoleTraderViewModel>();

                partnerViewModel = fixture.CreateMany<AdditionalContactModel>().ToList();

                var representingCompanyAddressDetails = fixture.Build<RepresentingCompanyAddressData>()
                    .With(r => r.CountryId, country.Id).Create();

                representingCompanyDetails = fixture.Build<RepresentingCompanyDetailsViewModel>()
                    .With(r => r.Address, representingCompanyAddressDetails).Create();

                organisation = OrganisationDbSetup.Init()
                    .WithNpWdMigrated(true)
                    .WithNpWdMigratedComplete(false)
                    .Create();

                directRegistrant = DirectRegistrantDbSetup.Init()
                    .WithPrn("PRN123456")
                    .WithOrganisation(organisation.Id)
                    .Create();
                
                Query.DeleteAllAdditionalCompanyDetails();

                return setup;
            }
        }
    }
}
