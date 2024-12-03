namespace EA.Weee.RequestHandlers.Organisations.DirectRegistrants
{
    using DataAccess;
    using Domain.Organisation;
    using Domain.Producer;
    using EA.Weee.Core.Organisations;
    using EA.Weee.DataAccess.DataAccess;
    using Mappings;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;

    internal abstract class CompleteOrganisationTransactionHandlerBase
    {
        private readonly IGenericDataAccess genericDataAccess;
        private readonly WeeeContext weeeContext;

        protected CompleteOrganisationTransactionHandlerBase(IGenericDataAccess genericDataAccess,
            WeeeContext weeeContext)
        {
            this.genericDataAccess = genericDataAccess;
            this.weeeContext = weeeContext;
        }
        protected async Task<AuthorisedRepresentative> CreateRepresentingCompany(OrganisationTransactionData organisationTransactionData)
        {
            AuthorisedRepresentative authorisedRepresentative = null;
            if (organisationTransactionData.AuthorisedRepresentative == YesNoType.Yes)
            {
                var country = await weeeContext.Countries.SingleAsync(c => c.Id == organisationTransactionData.RepresentingCompanyDetailsViewModel.Address.CountryId);

                authorisedRepresentative = RepresentingCompanyHelper.CreateRepresentingCompany(organisationTransactionData.RepresentingCompanyDetailsViewModel, country);
            }

            return authorisedRepresentative;
        }

        protected Contact CreateContact(OrganisationTransactionData organisationTransactionData)
        {
            var contactDetails = new Contact(organisationTransactionData.ContactDetailsViewModel.FirstName, organisationTransactionData.ContactDetailsViewModel.LastName,
                                                organisationTransactionData.ContactDetailsViewModel.Position ?? string.Empty);
            return contactDetails;
        }

        protected List<AdditionalCompanyDetails> CreateAdditionalCompanyDetails(OrganisationTransactionData organisationTransactionData)
        {
            var additionalCompanyDetails = new List<AdditionalCompanyDetails>();

            if (organisationTransactionData.PartnerModels != null)
            {
                additionalCompanyDetails.AddRange(organisationTransactionData.PartnerModels.OrderBy(p => p.Order).Select(CreatePartnerDetails));
            }

            if (organisationTransactionData.SoleTraderViewModel != null)
            {
                additionalCompanyDetails.Add(CreateSoleTraderDetails(organisationTransactionData.SoleTraderViewModel));
            }

            return additionalCompanyDetails;
        }

        protected AdditionalCompanyDetails CreatePartnerDetails(AdditionalContactModel partner)
        {
            return new AdditionalCompanyDetails
            {
                FirstName = partner.FirstName,
                LastName = partner.LastName,
                Type = OrganisationAdditionalDetailsType.Partner,
                Order = partner.Order
            };
        }

        protected AdditionalCompanyDetails CreateSoleTraderDetails(SoleTraderViewModel soleTrader)
        {
            return new AdditionalCompanyDetails
            {
                FirstName = soleTrader.FirstName,
                LastName = soleTrader.LastName,
                Type = OrganisationAdditionalDetailsType.SoleTrader
            };
        }
        protected async Task<Address> CreateContactAddress(OrganisationTransactionData organisationTransactionData)
        {
            var country = await weeeContext.Countries.SingleAsync(c => c.Id == organisationTransactionData.ContactDetailsViewModel.AddressData.CountryId);
            var contactAddress = new Address(organisationTransactionData.ContactDetailsViewModel.AddressData.Address1,
                                        organisationTransactionData.ContactDetailsViewModel.AddressData.Address2,
                                        organisationTransactionData.ContactDetailsViewModel.AddressData.TownOrCity,
                                        organisationTransactionData.ContactDetailsViewModel.AddressData.CountyOrRegion,
                                        organisationTransactionData.ContactDetailsViewModel.AddressData.Postcode,
                                        country,
                                        organisationTransactionData.ContactDetailsViewModel.AddressData.Telephone,
                                        organisationTransactionData.ContactDetailsViewModel.AddressData.Email);
            return contactAddress;
        }

        protected Organisation CreateOrganisation(OrganisationTransactionData organisationTransactionData)
        {
            switch (organisationTransactionData.OrganisationType)
            {
                case ExternalOrganisationType.Partnership:
                    return Organisation.CreateDirectRegistrantCompany(
                        Domain.Organisation.OrganisationType.DirectRegistrantPartnership,
                        organisationTransactionData.OrganisationViewModel.CompanyName,
                        organisationTransactionData.OrganisationViewModel.BusinessTradingName,
                        organisationTransactionData.OrganisationViewModel.CompaniesRegistrationNumber);
                case ExternalOrganisationType.RegisteredCompany:
                    return Organisation.CreateDirectRegistrantCompany(
                        Domain.Organisation.OrganisationType.RegisteredCompany,
                        organisationTransactionData.OrganisationViewModel.CompanyName,
                        organisationTransactionData.OrganisationViewModel.BusinessTradingName,
                        organisationTransactionData.OrganisationViewModel.CompaniesRegistrationNumber);
                case ExternalOrganisationType.SoleTrader:
                    return Organisation.CreateDirectRegistrantCompany(
                        Domain.Organisation.OrganisationType.SoleTraderOrIndividual,
                        organisationTransactionData.OrganisationViewModel.CompanyName,
                        organisationTransactionData.OrganisationViewModel.BusinessTradingName,
                        organisationTransactionData.OrganisationViewModel.CompaniesRegistrationNumber);

                case null:
                    throw new InvalidOperationException("Organisation type is null.");

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected async Task<Address> CreateAndAddAddress(OrganisationTransactionData organisationTransactionData, Organisation organisation)
        {
            var addressData = organisationTransactionData.OrganisationViewModel.Address;
            if (addressData == null)
            {
                throw new InvalidOperationException("Address data is null.");
            }

            var country = await weeeContext.Countries.SingleAsync(c => c.Id == addressData.CountryId);
            var address = ValueObjectInitializer.CreateAddress(addressData, country);

            var updatedAddress = await genericDataAccess.Add(address);

            return updatedAddress;
        }

        protected async Task<BrandName> CreateAndAddBrandName(OrganisationTransactionData organisationTransactionData)
        {
            var brandNames = organisationTransactionData.OrganisationViewModel.EEEBrandNames;
            if (string.IsNullOrWhiteSpace(brandNames))
            {
                return null;
            }

            var brandName = new BrandName(brandNames);
            return await genericDataAccess.Add(brandName);
        }
    }
}
