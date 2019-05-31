namespace EA.Weee.Web.Areas.Admin.ViewModels.Organisation
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using EA.Weee.Core.DataStandards;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Shared;

    public class EditSoleTraderOrIndividualOrganisationDetailsViewModel : EditOrganisationDetailsViewModelBase
    {       
        [Required]
        [DisplayName("Business trading name")]
        [StringLength(CommonMaxFieldLengths.DefaultString)]
        public override string BusinessTradingName { get; set; }
    }
}