﻿namespace EA.Weee.Core.Organisations
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using EA.Weee.Core.DataStandards;
    using EA.Weee.Core.Organisations.Base;

    public class PartnershipDetailsViewModel : OrganisationViewModel
    {
        [Required(ErrorMessage = "Enter company name / business name")]
        [DisplayName("Company name / Business name")]
        [StringLength(CommonMaxFieldLengths.DefaultString)]
        public override string CompanyName { get; set; }

        [DisplayName("Trading name")]
        [StringLength(CommonMaxFieldLengths.DefaultString)]
        public override string BusinessTradingName { get; set; }
    }
}