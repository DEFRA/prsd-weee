﻿namespace EA.Weee.Web.Areas.Admin.ViewModels.Aatf
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Web.Mvc;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Admin;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Shared;
    using EA.Weee.Web.Areas.Admin.Helper;

    public class AatfDetailsViewModel
    {
        public Guid Id { get; set; }

        public Guid AatfId { get; set; }

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

        public bool HasSubmissionData
        {
            get
            {
                if (SubmissionHistoryData != null && SubmissionHistoryData.Any())
                {
                    return true;
                }

                return false;
            }
        }

        [DisplayName("Compliance year")]
        public Int16 SelectedComplianceYear { get; set; }

        public IEnumerable<short> ComplianceYearList { get; set; }

        public DateTime CurrentDate { get; set; }

        public bool IsLatestComplianceYear
        {
            get
            {
                return ComplianceYearList != null && ComplianceYear == ComplianceYearList.First();
            }
        }

        public bool ShowCopyLink
        {
            get
            {
                if (IsLatestComplianceYear)
                {
                   var list = AatfHelper.FetchCurrentComplianceYears(CurrentDate).Except(ComplianceYearList.Select(x => (int)x));

                   return list.Count() > 0 ? true : false;
                }
                return false;
            }
        }
    }
}