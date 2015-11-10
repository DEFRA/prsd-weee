namespace EA.Weee.Web.Areas.Scheme.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Web.ViewModels.Shared;

    public class ChooseActivityViewModel : RadioButtonStringCollectionViewModel
    {
        public Guid OrganisationId { get; set; }

        public bool ShowLinkToCreateOrJoinOrganisation { get; set; }

        [Required(ErrorMessage = "Select an activity")]
        public override string SelectedValue { get; set; }

        public ChooseActivityViewModel()
        {
        }

        public ChooseActivityViewModel(List<string> activites) : base(activites)
        {
        }
    }
}