namespace EA.Weee.Web.Areas.Scheme.ViewModels
{
    using System;
    using System.Collections.Generic;
    using Web.ViewModels.Shared;

    public class ChooseActivityViewModel : RadioButtonStringCollectionViewModel
    {
        public Guid OrganisationId { get; set; }

        public bool ShowLinkToCreateOrJoinOrganisation { get; set; }

        public ChooseActivityViewModel()
        {
        }

        public ChooseActivityViewModel(List<string> activites) : base(activites)
        {
        }
    }
}