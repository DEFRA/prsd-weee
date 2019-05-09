namespace EA.Weee.Core.AatfReturn
{
    using EA.Weee.Core.Shared;
    using EA.Weee.Domain;
    using EA.Weee.Domain.AatfReturn;
    using System;

    public class AatfDataList
    {
        public Guid Id { get; set; }

        public virtual string Name { get; set; }

        public string ApprovalNumber { get; set; }

        public virtual UKCompetentAuthorityData CompetentAuthority { get; set; }

        public virtual AatfStatus AatfStatus { get; set; }

        public virtual string AatfStatusString { get; set; }

        public virtual Operator Operator { get; private set; }

        public AatfDataList(Guid id, string name, UKCompetentAuthorityData competentAuthority, string approvalNumber, AatfStatus aatfStatus, Operator @operator)
        {
            this.Id = id;
            this.Name = name;
            this.ApprovalNumber = approvalNumber;
            this.AatfStatus = aatfStatus;
            this.AatfStatusString = aatfStatus.DisplayName;
            this.CompetentAuthority = competentAuthority;
            this.Operator = @operator;
        }
    }
}
