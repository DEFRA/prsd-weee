namespace EA.Weee.Domain.AatfReturn
{
    using System;
    using Lookup;
    using Organisation;
    using Prsd.Core;
    using Prsd.Core.Domain;

    public class Aatf : Entity
    {
        public virtual string Name { get; private set; }

        public virtual UKCompetentAuthority CompetentAuthority { get; private set; }

        public virtual string ApprovalNumber { get; private set; }

        public virtual AatfStatus AatfStatus { get; private set; }

        public virtual Organisation Organisation { get; private set; }

        public virtual AatfAddress SiteAddress { get; private set; }

        public Guid? SiteAddressId { get; private set; }

        public virtual AatfSize Size { get; private set; }

        public virtual DateTime? ApprovalDate { get; private set; }

        public virtual AatfContact Contact { get; private set; }

        public virtual FacilityType FacilityType { get; private set; }

        public virtual Int16 ComplianceYear { get; private set; }

        public virtual PanArea PanArea { get; private set; }

        public virtual LocalArea LocalArea { get; private set; }

        public Aatf()
        {
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
            Int16 complianceYear,
            LocalArea localArea,
            PanArea panArea)
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
            Guard.ArgumentNotNull(() => localArea, localArea);
            Guard.ArgumentNotNull(() => panArea, panArea);

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
            ComplianceYear = complianceYear;
            LocalArea = localArea;
            PanArea = panArea;
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
            PanArea = panArea;
        }
    }
}
