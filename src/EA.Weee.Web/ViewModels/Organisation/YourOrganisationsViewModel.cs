namespace EA.Weee.Web.ViewModels.Organisation
{
    using System.Collections.Generic;
    using Core.Organisations;
    using System.ComponentModel.DataAnnotations;
    using EA.Weee.Web.ViewModels.Shared;

    public class YourOrganisationsViewModel
    {
        [Required]
        [Display(Name = "Organisations")]
        public StringGuidRadioButtons AccessibleOrganisations { get; private set; }

        public YourOrganisationsViewModel()
        {
            AccessibleOrganisations = new StringGuidRadioButtons();
        }
    }
}