namespace EA.Weee.Web.Areas.Admin.ViewModels.Aatf
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Core.Admin.AatfReports;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Admin;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Shared;

    public class AatfDetailsViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string ApprovalNumber { get; set; }

        public UKCompetentAuthorityData CompetentAuthority { get; set; }

        public PanAreaData PanArea { get; set; }

        public LocalAreaData LocalArea { get; set; }

        public AatfStatus AatfStatus { get; set; }

        public virtual AatfAddressData SiteAddress { get; set; }

        [AllowHtml]
        public string SiteAddressLong { get; set; }

        public AatfSize Size { get; set; }

        public OrganisationData Organisation { get; set; }
        
        [AllowHtml]
        public string OrganisationAddress { get; set; }

        public DateTime? ApprovalDate { get; set; }

        public Int16 ComplianceYear { get; set; }

        public AatfContactData ContactData { get; set; }

        [AllowHtml]
        public string ContactAddressLong { get; set; }

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

        public string AddressHeadingName
        {
            get
            {
                if (this.FacilityType == FacilityType.Aatf)
                {
                    return "Site address";
                }

                return "AE address";
            }
        }

        public FacilityType FacilityType { get; set; }

        public List<AatfDataList> AssociatedAatfs { get; set; }

        public List<AatfDataList> AssociatedAes { get; set; }

        public List<Core.Scheme.SchemeData> AssociatedSchemes { get; set; }

        public bool HasAnyRelatedEntities => IsNotNullOrEmpty(AssociatedAatfs) || IsNotNullOrEmpty(AssociatedAes) || IsNotNullOrEmpty(AssociatedSchemes);

        private bool IsNotNullOrEmpty<T>(IList<T> entityList)
        {
            return entityList != null && entityList.Any();
        }

        public bool HasPatArea => PanArea != null;

        public bool HasLocalArea => LocalArea != null;

        public List<AatfSubmissionHistoryViewModel> SubmissionHistoryData { get; set; }
    }
}