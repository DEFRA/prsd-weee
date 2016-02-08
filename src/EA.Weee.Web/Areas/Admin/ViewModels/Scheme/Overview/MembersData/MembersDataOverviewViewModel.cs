namespace EA.Weee.Web.Areas.Admin.ViewModels.Scheme.Overview.MembersData
{
    using System;
    using System.Collections.Generic;
    using Core.Scheme;

    public class MembersDataOverviewViewModel : OverviewViewModel
    {
        public Guid OrganisationId { get; set; }

        public string ApprovalNumber { get; set; }

        public SchemeDownloadsByYears DownloadsByYears { get; set; }

        public MembersDataOverviewViewModel() : base(OverviewDisplayOption.MembersData)
        {
        }
    }
}