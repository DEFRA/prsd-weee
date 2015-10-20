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

        public ChooseActivityViewModel() : base(new List<string>
            {
                PcsAction.ManagePcsMembers,
                PcsAction.ViewSubmissionHistory,
                PcsAction.ManageOrganisationUsers,
                PcsAction.ViewOrganisationDetails,
                PcsAction.ManageContactDetails
            })
        {
        }
    }
}