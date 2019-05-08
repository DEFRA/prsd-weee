namespace EA.Weee.Core.AatfReturn
{
    using EA.Weee.Domain;
    using EA.Weee.Domain.AatfReturn;
    using System;
    using System.Collections.Generic;

    public class AatfData
    {
        public AatfData(Guid id, string name, string approvalNumber)
        {
            this.Id = id;
            this.Name = name;
            this.ApprovalNumber = approvalNumber;
        }

        public Guid Id { get; set; }

        public virtual string Name { get; set; }

        public string ApprovalNumber { get; set; }

        public virtual UKCompetentAuthority CompetentAuthority { get; set; }

        public virtual AatfStatus AatfStatus { get; set; }
    }
}