using System;

namespace EA.Weee.Web.Areas.AatfReturn.ViewModels
{
    public class ReusedOffSiteViewModel
    {
        public bool IsReusedOffSite { get; set; }

        public Guid OrganisationId { get; set; }

        public Guid ReturnId { get; set; }
    }
}