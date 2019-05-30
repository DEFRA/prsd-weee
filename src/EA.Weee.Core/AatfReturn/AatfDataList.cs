namespace EA.Weee.Core.AatfReturn
{
    using EA.Weee.Core.Shared;
    using EA.Weee.Domain;
    using EA.Weee.Domain.AatfReturn;
    using System;
    using Domain.Organisation;
    using Organisations;

    public class AatfDataList
    {
        public Guid Id { get; set; }

        public virtual string Name { get; set; }

        public string ApprovalNumber { get; set; }

        public virtual UKCompetentAuthorityData CompetentAuthority { get; set; }

        public virtual AatfStatus AatfStatus { get; set; }

        public virtual string AatfStatusString { get; set; }

        public virtual OrganisationData Organisation { get; private set; }

        public virtual FacilityType FacilityType { get; private set; }

        public AatfDataList(Guid id, string name, UKCompetentAuthorityData competentAuthority, string approvalNumber, AatfStatus aatfStatus, OrganisationData organisation, FacilityType facilityType)
        {
            this.Id = id;
            this.Name = name;
            this.ApprovalNumber = approvalNumber;
            this.AatfStatus = aatfStatus;
            this.AatfStatusString = aatfStatus.DisplayName;
            this.CompetentAuthority = competentAuthority;
            this.Organisation = organisation;
            this.FacilityType = facilityType;
        }
    }
}
