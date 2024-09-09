namespace EA.Weee.Web.Areas.Scheme.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Web.ViewModels.Shared;

    public class ChooseActivityViewModel : RadioButtonStringCollectionViewModel
    {
        public Guid OrganisationId { get; set; }

        public Guid? DirectRegistrantId { get; set; }

        public Guid? SchemeId { get; set; }

        public bool ShowLinkToCreateOrJoinOrganisation { get; set; }

        [Required(ErrorMessage = "Select the activity you would like to do")]
        public override string SelectedValue { get; set; }

        public ChooseActivityViewModel()
        {
        }

        public ChooseActivityViewModel(List<string> activites) : base(activites)
        {
        }
    }
}