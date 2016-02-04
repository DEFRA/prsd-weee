namespace EA.Weee.Web.Areas.Admin.ViewModels.Scheme.Overview.MembersData
{
    using System;
    using System.Collections.Generic;

    public class MembersDataOverviewViewModel : OverviewViewModel
    {
        public Guid OrganisationId { get; set; }

        public string ApprovalNumber { get; set; }

        public IList<YearlyDownloads> DownloadsByYear { get; set; }

        public MembersDataOverviewViewModel(Guid organisationId, string approvalNumber, Guid schemeId, string schemeName)
            : base(schemeId, schemeName, OverviewDisplayOption.MembersData)
        {
            DownloadsByYear = new List<YearlyDownloads>();
            OrganisationId = organisationId;
            ApprovalNumber = approvalNumber;
        }
    }
}