namespace EA.Weee.Core.Organisations.Base
{
    using EA.Weee.Core.Constants;
    using EA.Weee.Core.DataStandards;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Validation;
    using System;
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

        public ExternalAddressData Address { get; set; } = new ExternalAddressData();

        [CompaniesRegistrationNumberStringLength]
        [Display(Name = "Company registration number (CRN)")]
        [RequiredWhenUK("Company registration number")]
        public string CompaniesRegistrationNumber { get; set; }

        [Required(ErrorMessage = "EEE Brand names is required")]
        [StringLength(CommonMaxFieldLengths.DefaultString)]
        [DisplayName("If you are registering as an authorised representative of a non-UK established organisation, enter the brands they place on the market.")]
        public string EEEBrandNames { get; set; }

        [DisplayName("Organisation type")]
        public ExternalOrganisationType? OrganisationType { get; set; }

        public bool HasPaid { get; set; } = false;

        public SubmissionStatus Status { get; set; }
        public DateTime? RegistrationDate { get; set; }

        public DateTime? SubmittedDate { get; set; }

        public string PaymentReference { get; set; }

        public Guid DirectProducerSubmissionId { get; set; }

        public bool IsPreviousSchemeMember { get; set; }

        [StringLength(CommonMaxFieldLengths.ProducerRegistrationNumber)]
        [DisplayName("Producer registration number (PRN)")]
        public virtual string ProducerRegistrationNumber { get; set; }

        public bool HasAuthorisedRepresentitive { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            results.AddRange(ExternalAddressValidator.Validate(Address.CountryId, Address.Postcode, "Address.CountryId", "Address.Postcode"));

            var isUkCountry = UkCountry.ValidIds.Contains(Address.CountryId);
            if (isUkCountry == false && HasAuthorisedRepresentitive)
            {
                var validationsResult = new ValidationResult("Selected country must be a UK country", new[] { "Address.CountryId" });

                results.Add(validationsResult);
            }

            return results;
        }

        public static IEnumerable<string> ValidationMessageDisplayOrder => new List<string>
        {
            "Address.CountryId",
            nameof(CompaniesRegistrationNumber),
            nameof(ProducerRegistrationNumber),
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
