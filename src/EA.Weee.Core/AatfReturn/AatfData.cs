namespace EA.Weee.Core.AatfReturn
{
    using System;
    using Core.Shared;

    public class AatfData
    {
        public AatfData(Guid id, string name, string approvalNumber, UKCompetentAuthorityData competentAuthority = null, AatfStatus status = null)
        {
            this.Id = id;
            this.Name = name;
            this.ApprovalNumber = approvalNumber;
            this.CompetentAuthority = competentAuthority;
            this.AatfStatus = status;
        }

        public Guid Id { get; set; }

        public virtual string Name { get; set; }

        public string ApprovalNumber { get; set; }
        public virtual UKCompetentAuthorityData CompetentAuthority { get; set; }

        public virtual AatfStatus AatfStatus { get; set; }
        public virtual AatfAddressData SiteAddress { get; set; }
        public int Size { get; set; }
        public DateTime ApprovalDate { get; set; }
    }
}
