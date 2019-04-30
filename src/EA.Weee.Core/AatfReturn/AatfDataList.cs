namespace EA.Weee.Core.AatfReturn
{
    using EA.Weee.Domain;
    using EA.Weee.Domain.AatfReturn;
    using System;

    public class AatfDataList
    {
        public Guid Id { get; set; }

        public virtual string Name { get; set; }

        public string ApprovalNumber { get; set; }

        public virtual UKCompetentAuthority CompetentAuthority { get; set; }

        public virtual AatfStatus AatfStatus { get; set; }

        public virtual Operator Operator { get; private set; }

        public AatfDataList(Guid id, string name, UKCompetentAuthority competentAuthority, string approvalNumber, AatfStatus aatfStatus, Operator @operator)
        {
            this.Id = id;
            this.Name = name;
            this.ApprovalNumber = approvalNumber;
            this.AatfStatus = aatfStatus;
            this.CompetentAuthority = competentAuthority;
            this.Operator = @operator;
        }
    }
}
