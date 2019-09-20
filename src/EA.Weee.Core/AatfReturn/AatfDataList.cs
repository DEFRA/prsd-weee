namespace EA.Weee.Core.AatfReturn
{
    using EA.Weee.Core.Shared;
    using Organisations;
    using System;

    public class AatfDataList
    {
        public Guid Id { get; set; }

        public virtual string Name { get; set; }

        public string ApprovalNumber { get; set; }

        public Int16 ComplianceYear { get; set; }

        public virtual UKCompetentAuthorityData CompetentAuthority { get; set; }

        public virtual AatfStatus AatfStatus { get; set; }

        public virtual string AatfStatusString { get; set; }

        public virtual OrganisationData Organisation { get; private set; }

        public virtual FacilityType FacilityType { get; private set; }

        public string NameWithComplianceYear => $"{Name} ({ComplianceYear})";

        public Guid AatfId { get; set; }

        public DateTime? ApprovalDate { get; set; }

        public AatfDataList(Guid id, string name, UKCompetentAuthorityData competentAuthority, string approvalNumber, AatfStatus aatfStatus, OrganisationData organisation, FacilityType facilityType, Int16 complianceYear, Guid aatfId, DateTime? approvalDate)
        {
            this.Id = id;
            this.Name = name;
            this.ApprovalNumber = approvalNumber;
            this.AatfStatus = aatfStatus;
            this.AatfStatusString = aatfStatus.DisplayName;
            this.CompetentAuthority = competentAuthority;
            this.Organisation = organisation;
            this.FacilityType = facilityType;
            this.ComplianceYear = complianceYear;
            this.AatfId = aatfId;
            this.ApprovalDate = approvalDate;
        }
    }
}
