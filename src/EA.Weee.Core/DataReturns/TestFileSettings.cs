namespace EA.Weee.Core.DataReturns
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class TestFileSettings
    {
        public Guid OrganisationID { get; private set; }

        public Quarter Quarter { get; private set; }

        public TestFileSettings(Guid organisationID, Quarter quarter)
        {
            OrganisationID = organisationID;
            Quarter = quarter;
        }
    }
}
