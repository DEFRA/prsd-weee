namespace EA.Weee.Web.Areas.Admin.ViewModels.Aatf
{
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Shared;
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;

    public class AatfDetailsViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string ApprovalNumber { get; set; }

        public UKCompetentAuthorityData CompetentAuthority { get; set; }

        public AatfStatus AatfStatus { get; set; }

        public virtual AatfAddressData SiteAddress { get; set; }

        [AllowHtml]
        public string SiteAddressLong { get; set; }

        public AatfSize Size { get; set; }

        public OrganisationData Organisation { get; set; }
        
        [AllowHtml]
        public string OrganisationAddress { get; set; }

        public DateTime? ApprovalDate { get; set; }

        public AatfContactData ContactData { get; set; }

        public bool CanEdit { get; set; }

        public string ApprovalDateString
        {
            get
            {
                if (this.ApprovalDate == null)
                {
                    return "-";
                }
                return this.ApprovalDate.Value.ToShortDateString();
            }
        }

        public List<AatfDataList> AssociatedAatfs { get; set; }

        public List<AatfDataList> AssociatedAEs { get; set; }

        public List<Core.Scheme.SchemeData> AssociatedSchemes { get; set; }
    }
}