namespace EA.Weee.Core.AatfReturn
{
    using Core.Shared;
    using EA.Weee.Core.Admin;
    using EA.Weee.Core.Organisations;
    using System;

    [Serializable]
    public class AatfData
    {
        public AatfData(Guid id, string name, string approvalNumber, Int16 complianceYear, UKCompetentAuthorityData competentAuthority = null, AatfStatus status = null, AatfAddressData siteAddress = null, AatfSize size = null, DateTime approvalDate = default(DateTime), PanAreaData panAreaData = null, LocalAreaData localAreaData = null)
        {
            this.Id = id;
            this.Name = name;
            this.ApprovalNumber = approvalNumber;
            this.CompetentAuthority = competentAuthority;
            this.AatfStatus = status;
            this.SiteAddress = siteAddress;
            this.Size = size;
            this.ApprovalDate = approvalDate;
            this.ComplianceYear = complianceYear;
            this.PanAreaData = panAreaData;
            this.LocalAreaData = localAreaData;
        }

        public AatfData()
        {
        }

        public Guid Id { get; set; }

        public virtual string Name { get; set; }

        public string ApprovalNumber { get; set; }

        public virtual UKCompetentAuthorityData CompetentAuthority { get; set; }

        public virtual PanAreaData PanAreaData { get; set; }

        public virtual LocalAreaData LocalAreaData { get; set; }

        public virtual AatfStatus AatfStatus { get; set; }

        public virtual AatfAddressData SiteAddress { get; set; }

        public AatfSize Size { get; set; }

        public DateTime? ApprovalDate { get; set; }

        public AatfContactData Contact { get; set; }

        public OrganisationData Organisation { get; set; }

        public FacilityType FacilityType { get; set; }

        public Int16 ComplianceYear { get; set; }

        public bool CanEdit { get; set; }

        public Guid AatfId { get; set; }

        public int AatfStatusValue { get; set; }

        public string AatfStatusDisplay { get; set; }

        public int AatfSizeValue { get; set; }

        public string AatfContactDetailsName { get; set; }

        public virtual Guid? PanAreaDataId { get; set; }

        public virtual Guid? LocalAreaDataId { get; set; }

        public bool HasEvidenceNotes { get; set; }

        public bool EvidenceSiteDisplay { get; set; }
    }
}