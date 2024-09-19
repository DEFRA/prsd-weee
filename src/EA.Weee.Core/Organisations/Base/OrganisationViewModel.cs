namespace EA.Weee.Core.Organisations.Base
{
    using EA.Weee.Core.Constants;
    using EA.Weee.Core.DataStandards;
    using EA.Weee.Core.Validation;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Reflection;
    using System.Web.Mvc;

    public class OrganisationViewModel : IValidatableObject
    {
        public bool LookupFound { get; set; }

        public string Action { get; set; }

        [StringLength(CommonMaxFieldLengths.DefaultString)]
        public virtual string CompanyName { get; set; }

        [StringLength(CommonMaxFieldLengths.DefaultString)]
        [DisplayName("Business trading name")]
        public virtual string BusinessTradingName { get; set; }

        public ExternalAddressData Address { get; set; } = new ExternalAddressData() { CountryId = UkCountry.Ids.England };

        [StringLength(maximumLength: EnvironmentAgencyMaxFieldLengths.CompanyRegistrationNumber, MinimumLength = 7, ErrorMessage = "The company registration number should be 7 to 15 characters long")]
        [Display(Name = "Company registration number (CRN)")]
        public string CompaniesRegistrationNumber { get; set; }

        [StringLength(CommonMaxFieldLengths.DefaultString)]
        [DisplayName("If you are registering as an authorised representative of a non-UK established organisation, enter the brands they place on the market.")]
        public string EEEBrandNames { get; set; }

        [DisplayName("Organisation type")]
        public ExternalOrganisationType? OrganisationType { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return ExternalAddressValidator.Validate(Address.CountryId, Address.Postcode, "Address.CountryId", "Address.Postcode");
        }

        public static IEnumerable<string> ValidationMessageDisplayOrder => new List<string>
        {
            "Address.CountryId",
            nameof(CompaniesRegistrationNumber),
            nameof(CompanyName),
            nameof(BusinessTradingName),
            "Address.WebsiteAddress",
            "Address.Address1",
            "Address.Address2",
            "Address.TownOrCity",
            "Address.CountyOrRegion",
            "Address.Postcode"
        };

        public object CastToSpecificViewModel(OrganisationViewModel model)
        {
            switch (OrganisationType)
            {
                case ExternalOrganisationType.RegisteredCompany:
                    return model.MapToViewModel<RegisteredCompanyDetailsViewModel>(model);
                case ExternalOrganisationType.Partnership:
                    return model.MapToViewModel<PartnershipDetailsViewModel>(model);
                case ExternalOrganisationType.SoleTrader:
                    return model.MapToViewModel<SoleTraderDetailsViewModel>(model);
                default:
                    return model.MapToViewModel<RegisteredCompanyDetailsViewModel>(model);
            }
        }

        private T MapToViewModel<T>(OrganisationViewModel source) where T : OrganisationViewModel, new()
        {
            var target = new T();

            var propertiesToIgnore = new HashSet<string>
            {
                nameof(OrganisationViewModel.ValidationMessageDisplayOrder),
            };

            var properties = typeof(OrganisationViewModel)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => !propertiesToIgnore.Contains(p.Name));

            foreach (var prop in properties)
            {
                var value = prop.GetValue(source);
                prop.SetValue(target, value);
            }

            return target;
        }
    }
}