namespace EA.Weee.Domain.AatfReturn
{
    using Lookup;
    using Organisation;
    using Prsd.Core;
    using Prsd.Core.Domain;
    using System;

    public class Aatf : Entity
    {
        public virtual string Name { get; private set; }

        public virtual UKCompetentAuthority CompetentAuthority { get; private set; }

        public virtual string ApprovalNumber { get; private set; }

        public virtual AatfStatus AatfStatus { get; private set; }

        public virtual Guid OrganisationId { get; private set; }

        public virtual Organisation Organisation { get; private set; }

        public virtual AatfAddress SiteAddress { get; private set; }

        public Guid? SiteAddressId { get; private set; }

        public virtual AatfSize Size { get; private set; }

        public virtual DateTime? ApprovalDate { get; private set; }

        public virtual AatfContact Contact { get; private set; }

        public virtual FacilityType FacilityType { get; private set; }

        public virtual Int16 ComplianceYear { get; private set; }

        public virtual PanArea PanArea { get; private set; }

        public virtual Guid? PanAreaId { get; private set; }

        public virtual LocalArea LocalArea { get; private set; }

        public virtual Guid? LocalAreaId { get; private set; }

        public virtual Guid AatfId { get; private set; }

        public Aatf()
        {
        }

        public virtual void UpdateDetails(string name, UKCompetentAuthority competentAuthority, string approvalNumber, AatfStatus aatfStatus, Organisation organisation, AatfSize aatfSize, DateTime? approvalDate, LocalArea localArea, PanArea panArea)
        {
            Name = name;
            CompetentAuthority = competentAuthority;
            ApprovalNumber = approvalNumber;
            AatfStatus = aatfStatus;
            Organisation = organisation;
            Size = aatfSize;
            ApprovalDate = approvalDate;
            LocalArea = localArea;
            if (localArea == null)
            {
                LocalAreaId = null;
            }
            PanArea = panArea;
            if (panArea == null)
            {
                PanAreaId = null;
            }
        }

        public Aatf(string name,
           UKCompetentAuthority competentAuthority,
           string approvalNumber,
           AatfStatus aatfStatus,
           Organisation organisation,
           AatfAddress aatfSiteAddress,
           AatfSize aatfSize,
           DateTime approvalDate,
           AatfContact contact,
           FacilityType facilityType,
           int complianceYear,
           LocalArea localArea,
           PanArea panArea,
           Guid? aatfId = null)
        {
            Guard.ArgumentNotNullOrEmpty(() => name, name);
            Guard.ArgumentNotNullOrEmpty(() => approvalNumber, approvalNumber);
            Guard.ArgumentNotNull(() => competentAuthority, competentAuthority);
            Guard.ArgumentNotNull(() => aatfStatus, aatfStatus);
            Guard.ArgumentNotNull(() => organisation, organisation);
            Guard.ArgumentNotNull(() => contact, contact);
            Guard.ArgumentNotNull(() => aatfSize, aatfSize);
            Guard.ArgumentNotNull(() => aatfSiteAddress, aatfSiteAddress);
            Guard.ArgumentNotNull(() => facilityType, facilityType);

            Name = name;
            CompetentAuthority = competentAuthority;
            ApprovalNumber = approvalNumber;
            AatfStatus = aatfStatus;
            Organisation = organisation;
            Size = aatfSize;
            SiteAddress = aatfSiteAddress;
            ApprovalDate = approvalDate;
            Contact = contact;
            FacilityType = facilityType;
            ComplianceYear = (short)complianceYear;
            LocalArea = localArea;
            PanArea = panArea;
            AatfId = aatfId == null ? Guid.NewGuid() : aatfId.Value;
        }

        /// <summary>
        /// Only use for integration tests
        /// </summary>
        /// <param name="organisationId"></param>
        public void UpdateOrganisation(Guid organisationId)
        {
            OrganisationId = organisationId;
            Organisation = null;
        }
    }
}
