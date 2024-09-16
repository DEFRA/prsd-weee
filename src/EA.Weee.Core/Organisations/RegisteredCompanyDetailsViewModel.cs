namespace EA.Weee.Core.Organisations
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using EA.Weee.Core.DataStandards;
    using EA.Weee.Core.Organisations.Base;
    using EA.Weee.Core.Validation;

    public class RegisteredCompanyDetailsViewModel : OrganisationViewModel
    {
        [Required(ErrorMessage = "Enter company name")]
        [DisplayName("Company name")]
        [StringLength(CommonMaxFieldLengths.DefaultString)]
        public override string CompanyName { get; set; }

        [DisplayName("Trading name")]
        [StringLength(CommonMaxFieldLengths.DefaultString)]
        public override string BusinessTradingName { get; set; }
    }
}