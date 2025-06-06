﻿namespace EA.Weee.Integration.Tests.Handlers.DirectRegistrant
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
    using System.Collections.Generic;
    using System.Linq;
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

                request = new CompleteOrganisationTransaction();
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

            private readonly It shouldHaveReturnedOrganisationAddress = () =>
            {
                result.Should().NotBeEmpty();

                var entity = Query.GetOrganisationTransactionForUser(UserId.ToString());
                organisation.Should().NotBeNull();
                organisation.BusinessAddress.Address1.Should().Be(addressData.Address1);
                organisation.BusinessAddress.Address2.Should().Be(addressData.Address2);
                organisation.BusinessAddress.CountryId.Should().Be(addressData.CountryId);
                organisation.BusinessAddress.Postcode.Should().Be(addressData.Postcode);
                organisation.BusinessAddress.TownOrCity.Should().Be(addressData.TownOrCity);
                entity.Organisation.Should().Be(organisation);
            };

            private readonly It shouldHaveReturnedNoPartners = () =>
            {
                var directRegistrant = Query.GetDirectRegistrantByOrganisationId(result);
                var additionalDetails = Query.GetAdditionalDetailsByRegistrantId(directRegistrant.Id, OrganisationAdditionalDetailsType.Partner);

                additionalDetails.Should().BeEmpty();
            };

            private readonly It shouldHaveReturnedNoSoleTraders = () =>
            {
                var directRegistrant = Query.GetDirectRegistrantByOrganisationId(result);
                var additionalDetails = Query.GetAdditionalDetailsByRegistrantId(directRegistrant.Id, OrganisationAdditionalDetailsType.SoleTrader);

                additionalDetails.Should().BeEmpty();
            };

            private readonly It shouldHaveReturnedOrganisationDetails = () =>
            {
                result.Should().NotBeEmpty();

                organisation.Should().NotBeNull();
                organisation.IsRepresentingCompany.Should().BeFalse();
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
                directRegistrant.AuthorisedRepresentative.Should().BeNull();
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

            private readonly It shouldHaveCreatedOrganisationUser = () =>
            {
                var users = Query.GetOrganisationForUser(UserId.ToString());

                users.Should().Contain(o => o.OrganisationId == organisation.Id);
            };
        }

        [Component]
        public class WhenICompleteAnSoleTraderOrganisationTransaction : CompleteOrganisationTransactionIntegrationTestBase
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

                request = new CompleteOrganisationTransaction();
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

            private readonly It shouldHaveReturnedOrganisationAddress = () =>
            {
                result.Should().NotBeEmpty();

                organisation.Should().NotBeNull();
                organisation.BusinessAddress.Address1.Should().Be(addressData.Address1);
                organisation.BusinessAddress.Address2.Should().Be(addressData.Address2);
                organisation.BusinessAddress.CountryId.Should().Be(addressData.CountryId);
                organisation.BusinessAddress.Postcode.Should().Be(addressData.Postcode);
                organisation.BusinessAddress.TownOrCity.Should().Be(addressData.TownOrCity);
            };

            private readonly It shouldHaveReturnedNoPartners = () =>
            {
                var directRegistrant = Query.GetDirectRegistrantByOrganisationId(result);
                var additionalDetails = Query.GetAdditionalDetailsByRegistrantId(directRegistrant.Id, OrganisationAdditionalDetailsType.Partner);

                additionalDetails.Should().BeEmpty();
            };

            private readonly It shouldHaveReturnedOrganisationDetails = () =>
            {
                result.Should().NotBeEmpty();

                var entity = Query.GetOrganisationTransactionForUser(UserId.ToString());
                organisation.Should().NotBeNull();
                organisation.IsRepresentingCompany.Should().BeFalse();
                organisation.OrganisationType.Should().Be(Domain.Organisation.OrganisationType.SoleTraderOrIndividual);
                organisation.OrganisationName.Should()
                    .Be(organisationTransactionData.OrganisationViewModel.CompanyName);
                organisation.TradingName.Should()
                    .Be(organisationTransactionData.OrganisationViewModel.BusinessTradingName);
                organisation.CompanyRegistrationNumber.Should()
                    .Be(organisationTransactionData.OrganisationViewModel.CompaniesRegistrationNumber);
                organisation.OrganisationStatus.Should().Be(Domain.Organisation.OrganisationStatus.Complete);
                entity.Organisation.Should().Be(organisation);
            };

            private readonly It shouldHaveCreatedDirectRegistrant = () =>
            {
                var directRegistrant = Query.GetDirectRegistrantByOrganisationId(result);

                directRegistrant.Should().NotBeNull();
                directRegistrant.BrandName.Name.Should()
                    .Be(organisationTransactionData.OrganisationViewModel.EEEBrandNames);
                directRegistrant.AuthorisedRepresentative.Should().BeNull();
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

            private readonly It shouldHaveReturnedSoleTraderDetails = () =>
            {
                var directRegistrant = Query.GetDirectRegistrantByOrganisationId(result);
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
        public class WhenICompleteAPartnershipOrganisationTransaction : CompleteOrganisationTransactionIntegrationTestBase
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

                request = new CompleteOrganisationTransaction();
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

            private readonly It shouldHaveReturnedOrganisationAddress = () =>
            {
                result.Should().NotBeEmpty();

                organisation.Should().NotBeNull();
                organisation.BusinessAddress.Address1.Should().Be(addressData.Address1);
                organisation.BusinessAddress.Address2.Should().Be(addressData.Address2);
                organisation.BusinessAddress.CountryId.Should().Be(addressData.CountryId);
                organisation.BusinessAddress.Postcode.Should().Be(addressData.Postcode);
                organisation.BusinessAddress.TownOrCity.Should().Be(addressData.TownOrCity);
            };

            private readonly It shouldHaveReturnedPartners = () =>
            {
                var directRegistrant = Query.GetDirectRegistrantByOrganisationId(result);
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
                var directRegistrant = Query.GetDirectRegistrantByOrganisationId(result);
                var additionalDetails = Query.GetAdditionalDetailsByRegistrantId(directRegistrant.Id, OrganisationAdditionalDetailsType.SoleTrader);

                additionalDetails.Should().BeEmpty();
            };

            private readonly It shouldHaveReturnedOrganisationDetails = () =>
            {
                result.Should().NotBeEmpty();

                var entity = Query.GetOrganisationTransactionForUser(UserId.ToString());

                organisation.Should().NotBeNull();
                organisation.IsRepresentingCompany.Should().BeFalse();
                organisation.OrganisationType.Should().Be(Domain.Organisation.OrganisationType.DirectRegistrantPartnership);
                organisation.OrganisationName.Should()
                    .Be(organisationTransactionData.OrganisationViewModel.CompanyName);
                organisation.TradingName.Should()
                    .Be(organisationTransactionData.OrganisationViewModel.BusinessTradingName);
                organisation.CompanyRegistrationNumber.Should()
                    .Be(organisationTransactionData.OrganisationViewModel.CompaniesRegistrationNumber);
                organisation.OrganisationStatus.Should().Be(Domain.Organisation.OrganisationStatus.Complete);
                entity.Organisation.Should().Be(organisation);
            };

            private readonly It shouldHaveCreatedDirectRegistrant = () =>
            {
                var directRegistrant = Query.GetDirectRegistrantByOrganisationId(result);

                directRegistrant.Should().NotBeNull();
                directRegistrant.BrandName.Name.Should()
                    .Be(organisationTransactionData.OrganisationViewModel.EEEBrandNames);
                directRegistrant.AuthorisedRepresentative.Should().BeNull();
            };

            private readonly It shouldHaveCreatedOrganisationUser = () =>
            {
                var users = Query.GetOrganisationForUser(UserId.ToString());

                users.Should().Contain(o => o.OrganisationId == organisation.Id);
            };
        }

        [Component]
        public class WhenICompleteAnOrganisationWithAuthorisedRepresentitiveTransaction : CompleteOrganisationTransactionIntegrationTestBase
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

                request = new CompleteOrganisationTransaction();
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

            private readonly It shouldHaveReturnedOrganisationAddress = () =>
            {
                result.Should().NotBeEmpty();

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
                organisation.Should().NotBeNull();
                organisation.IsRepresentingCompany.Should().BeTrue();
                organisation.OrganisationType.Should().Be(Domain.Organisation.OrganisationType.RegisteredCompany);
                organisation.OrganisationName.Should()
                    .Be(organisationTransactionData.OrganisationViewModel.CompanyName);
                organisation.TradingName.Should()
                    .Be(organisationTransactionData.OrganisationViewModel.BusinessTradingName);
                organisation.CompanyRegistrationNumber.Should()
                    .Be(organisationTransactionData.OrganisationViewModel.CompaniesRegistrationNumber);
                organisation.OrganisationStatus.Should().Be(Domain.Organisation.OrganisationStatus.Complete);
                entity.Organisation.Should().Be(organisation);
            };

            private readonly It shouldHaveCreatedDirectRegistrant = () =>
            {
                var directRegistrant = Query.GetDirectRegistrantByOrganisationId(result);

                directRegistrant.Should().NotBeNull();
                directRegistrant.BrandName.Name.Should()
                    .Be(organisationTransactionData.OrganisationViewModel.EEEBrandNames);
                directRegistrant.AuthorisedRepresentative.Should().NotBeNull();
                directRegistrant.AuthorisedRepresentative.OverseasProducerName.Should().Be(representingCompanyDetails.CompanyName);
                directRegistrant.AuthorisedRepresentative.OverseasProducerTradingName.Should().Be(representingCompanyDetails.BusinessTradingName);
                directRegistrant.AuthorisedRepresentative.OverseasContact.Address.PrimaryName.Should().Be(representingCompanyDetails.Address.Address1);
                directRegistrant.AuthorisedRepresentative.OverseasContact.Address.Street.Should().Be(representingCompanyDetails.Address.Address2);
                directRegistrant.AuthorisedRepresentative.OverseasContact.Address.Town.Should().Be(representingCompanyDetails.Address.TownOrCity);
                directRegistrant.AuthorisedRepresentative.OverseasContact.Address.AdministrativeArea.Should().Be(representingCompanyDetails.Address.CountyOrRegion);
                directRegistrant.AuthorisedRepresentative.OverseasContact.Address.CountryId.Should().Be(representingCompanyDetails.Address.CountryId);
                directRegistrant.AuthorisedRepresentative.OverseasContact.Address.PostCode.Should().Be(representingCompanyDetails.Address.Postcode);
                directRegistrant.AuthorisedRepresentative.OverseasContact.Email.Should().Be(representingCompanyDetails.Address.Email);
                directRegistrant.AuthorisedRepresentative.OverseasContact.Telephone.Should().Be(representingCompanyDetails.Address.Telephone);
                directRegistrant.AuthorisedRepresentative.OverseasContact.Address.SecondaryName.Should().BeEmpty();
                directRegistrant.AuthorisedRepresentative.OverseasContact.Address.Locality.Should().BeEmpty();
                directRegistrant.AuthorisedRepresentative.OverseasContact.SurName.Should().BeEmpty();
                directRegistrant.AuthorisedRepresentative.OverseasContact.Fax.Should().BeEmpty();
                directRegistrant.AuthorisedRepresentative.OverseasContact.ForeName.Should().BeEmpty();
                directRegistrant.AuthorisedRepresentative.OverseasContact.Mobile.Should().BeEmpty();
                directRegistrant.AuthorisedRepresentative.OverseasContact.Title.Should().BeEmpty();
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

            private readonly It shouldHaveCreatedOrganisationUser = () =>
            {
                var users = Query.GetOrganisationForUser(UserId.ToString());

                users.Should().Contain(o => o.OrganisationId == organisation.Id);
            };
        }

        [Component]
        public class WhenICompleteAnOrganisationWithAuthorisedRepresentitiveWithNullFieldsTransaction : CompleteOrganisationTransactionIntegrationTestBase
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
                organisation.BusinessAddress.Address2.Should().BeNull();
                organisation.BusinessAddress.CountryId.Should().Be(addressData.CountryId);
                organisation.BusinessAddress.Postcode.Should().Be(addressData.Postcode);
                organisation.BusinessAddress.TownOrCity.Should().Be(addressData.TownOrCity);
                organisation.BusinessAddress.CountyOrRegion.Should().BeNull();
            };

            private readonly It shouldHaveReturnedOrganisationDetails = () =>
            {
                result.Should().NotBeEmpty();

                var entity = Query.GetOrganisationTransactionForUser(UserId.ToString());
                var organisation = Query.GetOrganisationById(result);
                organisation.Should().NotBeNull();
                organisation.IsRepresentingCompany.Should().BeTrue();
                organisation.OrganisationType.Should().Be(Domain.Organisation.OrganisationType.RegisteredCompany);
                organisation.OrganisationName.Should()
                    .Be(organisationTransactionData.OrganisationViewModel.CompanyName);
                organisation.TradingName.Should()
                    .Be(organisationTransactionData.OrganisationViewModel.BusinessTradingName);
                organisation.CompanyRegistrationNumber.Should()
                    .Be(organisationTransactionData.OrganisationViewModel.CompaniesRegistrationNumber);
                organisation.OrganisationStatus.Should().Be(Domain.Organisation.OrganisationStatus.Complete);
                entity.Organisation.Should().Be(organisation);
            };

            private readonly It shouldHaveCreatedDirectRegistrant = () =>
            {
                var directRegistrant = Query.GetDirectRegistrantByOrganisationId(result);

                directRegistrant.Should().NotBeNull();
                directRegistrant.BrandName.Name.Should()
                    .Be(organisationTransactionData.OrganisationViewModel.EEEBrandNames);
                directRegistrant.AuthorisedRepresentative.Should().NotBeNull();
                directRegistrant.AuthorisedRepresentative.OverseasProducerName.Should().Be(representingCompanyDetails.CompanyName);
                directRegistrant.AuthorisedRepresentative.OverseasProducerTradingName.Should().Be(representingCompanyDetails.BusinessTradingName);
                directRegistrant.AuthorisedRepresentative.OverseasContact.Address.PrimaryName.Should().Be(representingCompanyDetails.Address.Address1);
                directRegistrant.AuthorisedRepresentative.OverseasContact.Address.Street.Should().BeEmpty();
                directRegistrant.AuthorisedRepresentative.OverseasContact.Address.Town.Should().Be(representingCompanyDetails.Address.TownOrCity);
                directRegistrant.AuthorisedRepresentative.OverseasContact.Address.AdministrativeArea.Should().BeEmpty();
                directRegistrant.AuthorisedRepresentative.OverseasContact.Address.CountryId.Should().Be(representingCompanyDetails.Address.CountryId);
                directRegistrant.AuthorisedRepresentative.OverseasContact.Address.PostCode.Should().Be(representingCompanyDetails.Address.Postcode);
                directRegistrant.AuthorisedRepresentative.OverseasContact.Email.Should().Be(representingCompanyDetails.Address.Email);
                directRegistrant.AuthorisedRepresentative.OverseasContact.Telephone.Should().Be(representingCompanyDetails.Address.Telephone);
                directRegistrant.AuthorisedRepresentative.OverseasContact.Address.SecondaryName.Should().BeEmpty();
                directRegistrant.AuthorisedRepresentative.OverseasContact.Address.Locality.Should().BeEmpty();
                directRegistrant.AuthorisedRepresentative.OverseasContact.SurName.Should().BeEmpty();
                directRegistrant.AuthorisedRepresentative.OverseasContact.Fax.Should().BeEmpty();
                directRegistrant.AuthorisedRepresentative.OverseasContact.ForeName.Should().BeEmpty();
                directRegistrant.AuthorisedRepresentative.OverseasContact.Mobile.Should().BeEmpty();
                directRegistrant.AuthorisedRepresentative.OverseasContact.Title.Should().BeEmpty();
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
            protected static RepresentingCompanyDetailsViewModel representingCompanyDetails;
            protected static Country country;
            protected static Guid result;
            protected static DateTime date;
            protected static ContactDetailsViewModel contactDetailsViewModel;
            protected static Organisation organisation;
            protected static List<AdditionalContactModel> partnerViewModel;
            
            protected static SoleTraderViewModel soleTraderModel;

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

                soleTraderModel = fixture.Create<SoleTraderViewModel>();

                partnerViewModel = fixture.CreateMany<AdditionalContactModel>().ToList();

                var representingCompanyAddressDetails = fixture.Build<RepresentingCompanyAddressData>()
                    .With(r => r.CountryId, country.Id).Create();

                representingCompanyDetails = fixture.Build<RepresentingCompanyDetailsViewModel>()
                    .With(r => r.Address, representingCompanyAddressDetails).Create();

                Query.DeleteAllAdditionalCompanyDetails();

                return setup;
            }
        }

        [Component]
        public class WhenICompleteAnOrganisationTransactionPrnIsSetOnDirectRegistrant : CompleteOrganisationTransactionIntegrationTestBase
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

                request = new CompleteOrganisationTransaction();
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

            private readonly It shouldHaveReturnedOrganisationAddress = () =>
            {
                result.Should().NotBeEmpty();

                var entity = Query.GetOrganisationTransactionForUser(UserId.ToString());
                organisation.Should().NotBeNull();
                organisation.BusinessAddress.Address1.Should().Be(addressData.Address1);
                organisation.BusinessAddress.Address2.Should().Be(addressData.Address2);
                organisation.BusinessAddress.CountryId.Should().Be(addressData.CountryId);
                organisation.BusinessAddress.Postcode.Should().Be(addressData.Postcode);
                organisation.BusinessAddress.TownOrCity.Should().Be(addressData.TownOrCity);
                entity.Organisation.Should().Be(organisation);
            };

            private readonly It shouldHaveReturnedNoPartners = () =>
            {
                var directRegistrant = Query.GetDirectRegistrantByOrganisationId(result);
                var additionalDetails = Query.GetAdditionalDetailsByRegistrantId(directRegistrant.Id, OrganisationAdditionalDetailsType.Partner);

                additionalDetails.Should().BeEmpty();
            };

            private readonly It shouldHaveReturnedNoSoleTraders = () =>
            {
                var directRegistrant = Query.GetDirectRegistrantByOrganisationId(result);
                var additionalDetails = Query.GetAdditionalDetailsByRegistrantId(directRegistrant.Id, OrganisationAdditionalDetailsType.SoleTrader);

                additionalDetails.Should().BeEmpty();
            };

            private readonly It shouldHaveReturnedOrganisationDetails = () =>
            {
                result.Should().NotBeEmpty();

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
                directRegistrant.ProducerRegistrationNumber.Should().Be(organisationTransactionData.OrganisationViewModel.ProducerRegistrationNumber);
                directRegistrant.AuthorisedRepresentative.Should().BeNull();
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

            private readonly It shouldHaveCreatedOrganisationUser = () =>
            {
                var users = Query.GetOrganisationForUser(UserId.ToString());

                users.Should().Contain(o => o.OrganisationId == organisation.Id);
            };
        }
    }
}
