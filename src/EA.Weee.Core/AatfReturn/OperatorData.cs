namespace EA.Weee.Core.AatfReturn
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class OperatorData
    {
        public OperatorData(Guid id, string operatorName, Guid organisationid)
        {
            this.Id = id;
            this.OperatorName = operatorName;
            this.OrganisationId = organisationid;
        }
        public Guid Id { get; set; }

        public Guid OrganisationId { get; set; }

        public string OperatorName { get; set; }
    }
}
