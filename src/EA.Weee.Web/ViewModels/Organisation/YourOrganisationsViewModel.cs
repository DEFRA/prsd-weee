namespace EA.Weee.Web.ViewModels.Organisation
{
    using Core.Organisations;
    using EA.Weee.Web.ViewModels.Shared;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class YourOrganisationsViewModel
    {
        [Required]
        //[DisplayName("Which organisation would you like to perform activities for?")] --TODO: Update content to allow legend
        public StringGuidRadioButtons AccessibleOrganisations { get; private set; }

        public YourOrganisationsViewModel()
        {
            AccessibleOrganisations = new StringGuidRadioButtons();
        }
    }
}