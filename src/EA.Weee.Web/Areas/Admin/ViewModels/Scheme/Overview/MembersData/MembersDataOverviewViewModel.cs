namespace EA.Weee.Web.Areas.Admin.ViewModels.Scheme.Overview.MembersData
{
    using Core.Scheme;
    using System;

    public class MembersDataOverviewViewModel : OverviewViewModel
    {
        public Guid OrganisationId { get; set; }

        public string ApprovalNumber { get; set; }

        public SchemeDataAvailability SchemeDataAvailability { get; set; }

        public MembersDataOverviewViewModel() : base(OverviewDisplayOption.MembersData)
        {
        }
    }
}