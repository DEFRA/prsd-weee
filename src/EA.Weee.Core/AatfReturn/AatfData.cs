namespace EA.Weee.Core.AatfReturn
{
    using System;

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
    }
}
