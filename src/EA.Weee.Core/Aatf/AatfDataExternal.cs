namespace EA.Weee.Core.Aatf
{
    using EA.Weee.Core.AatfReturn;
    using System;

    public class AatfDataExternal
    {
        public Guid Id { get; set; }

        public virtual string Name { get; set; }

        public AatfContactData Contact { get; set; }

        public string AatfContactDetailsName { get; set; }

        public string ApprovalNumber { get; set; }

        public string FacilityType { get; set; }

        public AatfDataExternal(Guid id, string name)
        {
            this.Id = id;
            this.Name = name;
        }
    }
}
