﻿namespace EA.Weee.Core.AatfReturn
{
    using System;
    using Core.Shared;
    using EA.Weee.Core.Organisations;

    public class AatfData
    {
        public AatfData(Guid id, string name, string approvalNumber, UKCompetentAuthorityData competentAuthority = null, AatfStatus status = null, AatfAddressData siteAddress = null, AatfSize size = null, DateTime approvalDate = default(DateTime))
        {
            this.Id = id;
            this.Name = name;
            this.ApprovalNumber = approvalNumber;
            this.CompetentAuthority = competentAuthority;
            this.AatfStatus = status;
            this.SiteAddress = siteAddress;
            this.Size = size;
            this.ApprovalDate = approvalDate;
        }

        public Guid Id { get; set; }

        public virtual string Name { get; set; }

        public string ApprovalNumber { get; set; }

        public virtual UKCompetentAuthorityData CompetentAuthority { get; set; }

        public virtual AatfStatus AatfStatus { get; set; }
      
        public virtual AatfAddressData SiteAddress { get; set; }
      
        public AatfSize Size { get; set; }
      
        public DateTime? ApprovalDate { get; set; }

        public AatfContactData Contact { get; set; }

        public OrganisationData Organisation { get; set; }

        public FacilityType FacilityType { get; set; }

        public bool CanEdit { get; set; }
    }
}