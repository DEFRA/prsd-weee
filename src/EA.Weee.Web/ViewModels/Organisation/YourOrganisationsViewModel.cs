namespace EA.Weee.Web.ViewModels.Organisation
{
    using System;
    using Core.Organisations;
    using EA.Weee.Web.ViewModels.Shared;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public class YourOrganisationsViewModel
    {
        [Required]
        //[DisplayName("Which organisation would you like to perform activities for?")] --TODO: Update content to allow legend
        public StringGuidRadioButtons AccessibleOrganisations { get; private set; }

        [Required(ErrorMessage = "Select an organisation to perform activities")]
        public RadioButtonPair<string, Guid> Selected
        {
            get
            {
                return AccessibleOrganisations.Selected;
            }
            set
            {
                AccessibleOrganisations.Selected = value;
            }
        }

        public YourOrganisationsViewModel()
        {
            AccessibleOrganisations = new StringGuidRadioButtons();
        }
    }
}