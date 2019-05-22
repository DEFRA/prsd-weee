namespace EA.Weee.Core.AatfReturn
{
    using EA.Weee.Core.Organisations;
    using System;

    public class OperatorData
    {
        public OperatorData(Guid id, string operatorName, OrganisationData organisation, Guid organisationid)
        {
            this.Id = id;
            this.OperatorName = operatorName;
            this.Organisation = organisation;
            this.OrganisationId = organisationid;
        }
        public virtual Guid Id { get; set; }

        public OrganisationData Organisation { get; set; }

        public virtual Guid OrganisationId { get; set; }

        public string OperatorName { get; set; }
    }
}
